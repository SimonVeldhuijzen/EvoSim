using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Evolution.Helpers;
using Genetics;
using NeuralNetwork;
using Evolution.Core;
using System;
using NeuralNetwork.ActivationFunctions;

namespace Evolution.Models
{
    public class Blob : Entity
    {
        public int Health { get; private set; } = Parameters.Health / 2;
        public int EatenCount { get; private set; }
        public int LookingDirection { get; private set; }
        public bool IsChild { get; }
        public double EyeSight { get; }

        private Genome Genome;
        private Network Brain;
        private List<double> ThinkingResults = new List<double>();
        private RandomHelper RandomHelper;
        private TwoDHelper TwoDHelper;

        private int PulsatingRadius = -1;
        private long? Parent1Id;
        private long? Parent2Id;

        public bool NoMoreHealth { get { return this.Health < 0; } }

        public Blob(RandomHelper randomHelper, TwoDHelper twoDHelper, List<int> brainLayerSizes, Point location)
            : base(location)
        {
            this.RandomHelper = randomHelper ?? throw new ArgumentNullException(nameof(randomHelper));
            this.TwoDHelper = twoDHelper ?? throw new ArgumentNullException(nameof(twoDHelper));
            this.LookingDirection = this.RandomHelper.Next(0, 360);

            var genomeSize = Network.CalculateAmountOfNeededValues(brainLayerSizes);
            this.Genome = new Genome(this.RandomHelper, genomeSize + 1);
            this.Brain = new Network(brainLayerSizes, this.Genome.Genes.Take(genomeSize).ToList(), new Sigmoid());
            this.EyeSight = this.Genome.Genes.Last();
        }

        public Blob(RandomHelper randomHelper, TwoDHelper twoDHelper, BlobSkeleton skeleton, Point location, long? parent1Id, long? parent2Id)
            : base(location)
        {
            this.RandomHelper = randomHelper ?? throw new ArgumentNullException(nameof(randomHelper));
            this.TwoDHelper = twoDHelper ?? throw new ArgumentNullException(nameof(twoDHelper));
            this.LookingDirection = this.RandomHelper.Next(0, 360);

            this.Genome = new Genome(this.RandomHelper, skeleton.Genes);
            this.Brain = new Network(skeleton.BrainLayerSizes, this.Genome.Genes.Take(this.Genome.Genes.Count - 1).ToList(), new Sigmoid());
            this.EyeSight = this.Genome.Genes.Last();

            this.Parent1Id = parent1Id;
            this.Parent2Id = parent2Id;
            this.IsChild = true;
        }

        public void Pulsate()
        {
            if (this.PulsatingRadius <= 0)
            {
                this.PulsatingRadius = 20;
            }
        }

        public void Think(List<Blob> blobs, List<Danger> dangers, List<Food> foods)
        {
            var blobParams = blobs.Select(b => TwoDHelper.GetInputParams(this, b)).OrderBy(b => b.Distance).ToList();
            var dangerParams = dangers.Select(b => TwoDHelper.GetInputParams(this, b)).OrderBy(d => d.Distance).ToList();
            var foodParams = foods.Select(b => TwoDHelper.GetInputParams(this, b)).OrderBy(f => f.Distance).ToList();

            var inputParams = new List<double>()
            {
                this.Health / (Parameters.Health / 2.0) - 1
            };

            inputParams.AddRange(InputParams.ConvertToDoubles(blobParams));
            inputParams.AddRange(InputParams.ConvertToDoubles(dangerParams));
            inputParams.AddRange(InputParams.ConvertToDoubles(foodParams));

            this.ThinkingResults = this.Brain.Calculate(inputParams);
        }

        public void ProcessThought()
        {
            this.LookingDirection = TwoDHelper.GetAdjustedLookingDirection(this.LookingDirection, this.ThinkingResults[0]);
            this.Location = TwoDHelper.GetNewLocation(this, this.ThinkingResults[1], this.ThinkingResults[2]);
            this.Health -= Parameters.StepDamage;
        }

        public List<BlobSkeleton> Mate(Blob other)
        {
            var result = new List<BlobSkeleton>();
            if (this.Health < Parameters.Health * Parameters.MateFrom || other.Health < Parameters.Health * Parameters.MateFrom)
            {
                return result;
            }

            if (this.EatenCount < Parameters.EatenCountForMate || other.EatenCount < Parameters.EatenCountForMate)
            {
                return result;
            }

            var newGenomes = this.Genome.PairWith(other.Genome);

            result.Add(new BlobSkeleton(this.Brain.LayerSizes, newGenomes.Item1));
            result.Add(new BlobSkeleton(this.Brain.LayerSizes, newGenomes.Item2));

            this.Health -= (int)(Parameters.MateDamage * Parameters.Health);
            other.Health -= (int)(Parameters.MateDamage * Parameters.Health);

            this.EatenCount = 0;
            other.EatenCount = 0;

            this.Pulsate();
            other.Pulsate();

            return result;
        }

        public BlobSkeleton Split()
        {
            var genome = this.Genome.Clone();
            this.EatenCount = 0;
            this.Pulsate();
            return new BlobSkeleton(this.Brain.LayerSizes, genome);
        }

        public BlobSkeleton CreateSkeleton()
        {
            return new BlobSkeleton(this.Brain.LayerSizes, this.Genome);
        }

        public void Eat()
        {
            this.EatenCount++;
            this.Health += Parameters.FoodValue;
            if (this.Health > Parameters.Health)
            {
                this.Health = Parameters.Health;
            }
        }

        public void TouchDanger()
        {
            this.Health -= Parameters.DangerValue;
        }

        public void AdjustHealth(int previousHealth)
        {
            var fraction = (this.Health * 1.0) / previousHealth;
            this.Health = (int)(Parameters.Health * fraction);
        }

        public override void Draw(Graphics g)
        {
            var brush = new SolidBrush(this.GetColor(0, Parameters.Health, this.Health));
            var pen = new Pen(brush, 1)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
            };

            this.DrawFieldOfView(g, pen);
            this.DrawName(g);
            this.DrawBlob(g, brush);
            this.DrawPulsation(g, pen);
        }

        private void DrawFieldOfView(Graphics g, Pen pen)
        {
            var eyeSightLength = TwoDHelper.GetEyeSightLength(this.EyeSight);
            var eyeSightWidth = TwoDHelper.GetEyeSightWidth(this.EyeSight);

            var rect = new Rectangle(this.Location.X - eyeSightLength, this.Location.Y - eyeSightLength, 2 * eyeSightLength, 2 * eyeSightLength);
            var points = TwoDHelper.GetEdgeOfView(this);
            g.DrawArc(pen, rect, this.LookingDirection - eyeSightWidth / 2, eyeSightWidth);
            g.DrawLine(pen, this.Location, points[0]);
            g.DrawLine(pen, this.Location, points[1]);
        }

        private void DrawName(Graphics g)
        {
            var stringToDraw = this.EntityId.ToString();
            if (this.Parent1Id != null)
            {
                stringToDraw += " (" + this.Parent1Id.Value.ToString();
                if (this.Parent2Id != null)
                {
                    stringToDraw += ", " + this.Parent2Id.Value.ToString();
                }

                stringToDraw += ")";
            }

            g.DrawString(stringToDraw, SystemFonts.DefaultFont, Brushes.White, this.Location.X + Parameters.EntityRadius - 2, this.Location.Y + Parameters.EntityRadius - 2);
        }

        private void DrawBlob(Graphics g, Brush brush)
        {
            g.FillEllipse(brush, this.Location.X - Parameters.EntityRadius, this.Location.Y - Parameters.EntityRadius, Parameters.EntityRadius * 2, Parameters.EntityRadius * 2);
        }

        private void DrawPulsation(Graphics g, Pen pen)
        {
            if (this.PulsatingRadius > 0)
            {
                g.DrawEllipse(pen,
                    this.Location.X - Parameters.EntityRadius - this.PulsatingRadius,
                    this.Location.Y - Parameters.EntityRadius - this.PulsatingRadius,
                    Parameters.EntityRadius * 2 + this.PulsatingRadius * 2,
                    Parameters.EntityRadius * 2 + this.PulsatingRadius * 2);

                this.PulsatingRadius--;
            }
        }

        private Color GetColor(int rangeStart, int rangeEnd, int actualValue)
        {
            if (rangeStart >= rangeEnd || actualValue < 0)
            {
                return Color.Black;
            }

            var max = rangeEnd - rangeStart;
            var value = actualValue - rangeStart;
            
            var green = (255 * value) / max;
            var red = 255 - green;

            return Color.FromArgb(red, green, 0);
        }
    }
}
