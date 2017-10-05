using Evolution.Core;
using Evolution.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Timers;
using System.Windows.Forms;

namespace Evolution
{
    public partial class Form1 : Form
    {
        private const int DrawingInterval = 1000 / 30;

        private Controller Controller;
        private System.Timers.Timer Timer = new System.Timers.Timer(DrawingInterval);

        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.InitializeComponent();

            this.Load += Form1_Load;
            this.Paint += this.PaintForm;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Controller = new Controller(this.Size, new RandomHelper());
            this.Controller.Start();

            this.Timer.Elapsed += Timer_Elapsed;
            this.Timer.Enabled = true;
            this.Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Timer.Enabled = false;
            this.Controller.DrawWorld();
            this.Invalidate();
            this.Timer.Enabled = true;
        }

        private void PaintForm(object sender, PaintEventArgs e)
        {
            this.Controller.Redraw(e.Graphics);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            this.Controller.Start();
            this.Timer.Start();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            this.Controller.Stop();
            this.Timer.Stop();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            using (var exportDialog = new ExportForm(this.Controller))
            {
                exportDialog.ShowDialog();
            }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "Json files (*.json)|*.json",
                Title = "Choose a file",
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var json = File.ReadAllText(fileDialog.FileName);
                var skeleton = JsonConvert.DeserializeObject<BlobSkeleton>(json);
                this.Controller.AddCustomSkeleton(skeleton);
            }
        }

        private void ParametersButton_Click(object sender, EventArgs e)
        {
            using (var parametersForm = new ParametersForm())
            {
                parametersForm.ShowDialog();
                if (parametersForm.ParametersHaveChanged)
                {
                    this.Controller.ParametersHaveChanged();
                }
            }
        }
    }
}
