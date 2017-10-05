using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace Evolution
{
    public partial class ExportForm : Form
    {
        private Controller Controller;

        public ExportForm(Controller controller)
        {
            this.Controller = controller;
            InitializeComponent();
        }

        private void SearchClick(object sender, EventArgs e)
        {
            var fileDialog = new SaveFileDialog()
            {
                Filter = "Json files (*.json)|*.json",
                Title = "Choose a file"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = fileDialog.FileName;
            }
        }

        private void SaveClick(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(this.textBox1.Text, out id))
            {
                MessageBox.Show($"Invalid id: {this.textBox1.Text}");
            }

            var skeleton = this.Controller.GetBlobSkeletonById(id);
            
            if (skeleton == null)
            {
                MessageBox.Show($"Blob {id} does not exist");
            }

            var json = JsonConvert.SerializeObject(skeleton);
            File.WriteAllText(this.textBox2.Text, json);

            this.Close();
        }
    }
}
