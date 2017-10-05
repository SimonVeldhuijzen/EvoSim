using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using Evolution.Drawers;
using Evolution.Helpers;
using Evolution.Models;
using System;
using Evolution.Core;
using System.Text;

namespace Evolution
{
    public class Controller
    {
        private EntityList<Blob> Blobs = new EntityList<Blob>();
        private EntityList<Danger> Dangers = new EntityList<Danger>();
        private EntityList<Food> Foods = new EntityList<Food>();
        private RandomHelper RandomHelper;
        private TwoDHelper TwoDHelper;

        private EntityList<Blob> CustomBlobsToAdd = new EntityList<Blob>();

        private bool ParametersChanged = false;
        private int CurrentHealth = Parameters.Health;
        private int Iteration = 0;
        private int TotalMates;
        private int TotalSplits;

        private Timer Timer = new Timer(Parameters.TimerStep);
        private Stopwatch StopWatch = new Stopwatch();

        private DateTime LastDrawn = DateTime.Now.AddDays(-1);
        private BoardDrawer Drawer;

        private Size BoardSize;
        private List<int> BrainLayerSizes;

        public Controller(Size boardSize, RandomHelper randomHelper)
        {
            this.RandomHelper = randomHelper ?? throw new ArgumentNullException(nameof(randomHelper));
            this.BoardSize = boardSize;
            this.Drawer = new BoardDrawer(BoardSize);
            this.TwoDHelper = new TwoDHelper(this.BoardSize);

            this.SetBrainLayerSizes();
            this.CreateNewEntities();

            this.StopWatch.Start();
            this.DrawWorld();

            this.Timer.Elapsed += Timer_Elapsed;
            this.Timer.Enabled = true;
        }

        public void ParametersHaveChanged()
        {
            this.ParametersChanged = true;
        }

        public void Start()
        {
            this.Timer.Start();
        }

        public void Stop()
        {
            this.Timer.Stop();
        }

        public void DrawWorld()
        {
            var statusMessage = this.CreateStatusMessage(this.StopWatch.Elapsed);

            var entities = new List<Entity>();
            entities.AddRange(this.Blobs);
            entities.AddRange(this.Dangers);
            entities.AddRange(this.Foods);
            this.Drawer.DrawEntities(entities, statusMessage);
            LastDrawn = DateTime.Now;
        }

        public void Redraw(Graphics g)
        {
            this.Drawer.DrawBitmap(g);
        }

        public BlobSkeleton GetBlobSkeletonById(int id)
        {
            var blob = this.Blobs.GetById(id);
            if (blob != null)
            {
                return blob.CreateSkeleton();
            }
            else
            {
                return null;
            }
        }

        public void AddCustomSkeleton(BlobSkeleton skeleton)
        {
            lock (this.CustomBlobsToAdd)
            {
                this.CustomBlobsToAdd.Add(new Blob(this.RandomHelper, this.TwoDHelper, skeleton, this.GenerateStartingPoint(), null, null));
            }
        }

        private void Tick()
        {
            Iteration++;

            Parallel.ForEach(this.Blobs, (blob) => blob.Think(
                TwoDHelper.Sees(blob, this.Blobs),
                TwoDHelper.Sees(blob, this.Dangers),
                TwoDHelper.Sees(blob, this.Foods)
            ));

            Parallel.ForEach(this.Blobs, (blob) => blob.ProcessThought());

            this.BlobTicks();

            this.CreateNewEntities();

            var oldest = this.Blobs.GetOldest();
            oldest.Pulsate();
        }

        private void BlobTicks()
        {
            var newBlobs = new List<Blob>();
            var removedBlobs = new List<Blob>();
            var removedFoods = new List<Food>();

            foreach (var blob in this.Blobs)
            {
                foreach (var danger in TwoDHelper.FindCollisions(this.Dangers, blob))
                {
                    blob.TouchDanger();
                }

                foreach (var food in TwoDHelper.FindCollisions(this.Foods, blob))
                {
                    blob.Eat();
                    removedFoods.Add((Food)food);
                }

                foreach (var otherBlob in TwoDHelper.FindCollisions(this.Blobs, blob))
                {
                    var bornBlobs = blob.Mate((Blob)otherBlob);
                    var children = bornBlobs.Select(bornBlob =>
                        new Blob(this.RandomHelper, this.TwoDHelper, bornBlob, this.GenerateStartingPoint(), blob.EntityId, otherBlob.EntityId));
                    if (children.Any())
                    {
                        newBlobs.AddRange(children);
                        this.TotalMates++;
                    }
                }

                if (blob.EatenCount >= Parameters.EatenCountForSplit)
                {
                    var skeleton = blob.Split();
                    this.TotalSplits++;
                    newBlobs.Add(new Blob(this.RandomHelper, this.TwoDHelper, skeleton, this.GenerateStartingPoint(), blob.EntityId, null));
                }

                if (blob.NoMoreHealth)
                {
                    removedBlobs.Add(blob);
                }
            }

            removedBlobs.ForEach(blob => this.Blobs.Remove(blob));
            removedFoods.ForEach(food => this.Foods.Remove(food));
            foreach (var blob in newBlobs)
            {
                this.Blobs.AddEntity(blob);
            }

            foreach (var blob in this.Blobs)
            {
                if (this.Blobs.Count(b => b.Location.X == blob.Location.X && b.Location.Y == blob.Location.Y) > 1)
                    blob.Location = this.GenerateStartingPoint();
            }
        }

        private void CreateNewEntities()
        {
            for (int i = this.Foods.Count; i < Parameters.FoodCount; i++)
            {
                this.Foods.AddEntity(new Food(this.GenerateStartingPoint()));
            }

            if (this.CustomBlobsToAdd.Any())
            {
                lock(this.CustomBlobsToAdd)
                {
                    foreach (var blob in this.CustomBlobsToAdd.ToList())
                    {
                        this.Blobs.AddEntity(blob);
                    }

                    AutoClosingMessageBox.Show("Id:" + string.Join(", ", this.CustomBlobsToAdd.Select(b => b.EntityId)), "Added custom blob!", 15000);

                    this.CustomBlobsToAdd.Clear();
                }
            }

            for (int i = this.Blobs.Count; i < Parameters.BlobCount; i++)
            {
                this.Blobs.AddEntity(new Blob(this.RandomHelper, this.TwoDHelper, this.BrainLayerSizes, this.GenerateStartingPoint()));
            }

            if (this.Blobs.Count > Parameters.MaxBlobs)
            {
                var weakest = this.Blobs.OrderByDescending(b => b.Health).Skip(Parameters.MaxBlobs).ToList();
                weakest.ForEach(b => this.Blobs.Remove(b));
            }

            if (this.Dangers.Count > Parameters.DangerCount)
            {
                for (int i = Parameters.DangerCount; i < this.Dangers.Count; i++)
                {
                    this.Dangers.RemoveAt(0);
                }
            }

            if (this.Dangers.Count < Parameters.DangerCount)
            {
                for (int i = this.Dangers.Count; i < Parameters.DangerCount; i++)
                {
                    this.Dangers.AddEntity(new Danger(this.GenerateStartingPoint()));
                }
            }
        }

        private void SetBrainLayerSizes()
        {
            this.BrainLayerSizes = new List<int>()
            {
                Parameters.EntitiesToPass * 3 * 2 + 1,
                Parameters.EntitiesToPass * 3 * 2,
                (Parameters.EntitiesToPass / 2) * 3 * 2,
                3
            };

        }

        private Point GenerateStartingPoint()
        {
            var location = RandomHelper.GeneratePointWithin(this.BoardSize);
            while (this.Blobs.Any(blob => blob.Location.X == location.X && blob.Location.Y == location.Y))
            {
                location = RandomHelper.GeneratePointWithin(this.BoardSize);
            }

            return location;
        }

        private string CreateStatusMessage(TimeSpan elapsed)
        {
            var elapsedString = elapsed.ToString(@"hh\:mm\:ss");

            var oldestBlob = this.Blobs.GetOldest();
            var ageOfOldestblob = (int)Math.Round(DateTime.Now.Subtract(oldestBlob.CreatedOn).TotalSeconds, 0, MidpointRounding.AwayFromZero);

            var newestChild = this.Blobs.LastOrDefault(blob => blob.IsChild);
            var oldestChild = this.Blobs.FirstOrDefault(blob => blob.IsChild);

            var builder = new StringBuilder();
            builder.AppendLine($"Iteration {this.Iteration}");
            builder.AppendLine($"Running time: {elapsedString}");
            builder.AppendLine($"Oldest: {oldestBlob.EntityId} ({ageOfOldestblob} seconds)");
            builder.AppendLine($"Mates: {this.TotalMates}");
            builder.AppendLine($"Splits: {this.TotalSplits}");
            builder.AppendLine($"Oldest child: {(oldestChild?.EntityId.ToString() ?? "-")}");
            builder.AppendLine($"Newest child: {(newestChild?.EntityId.ToString() ?? "-")}");

            return builder.ToString();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Timer.Enabled = false;
            if (this.ParametersChanged)
            {
                this.ResetParameters();
            }

            this.Tick();
            this.Timer.Enabled = true;
        }

        private void ResetParameters()
        {
            if (Parameters.Health != this.CurrentHealth)
            {
                this.Blobs.ForEach(blob => blob.AdjustHealth(this.CurrentHealth));
                this.CurrentHealth = Parameters.Health;
            }

            this.Timer.Interval = Parameters.TimerStep;
            this.SetBrainLayerSizes();
            this.ParametersChanged = false;
        }
    }
}
