using System;
using System.Linq;
using System.Windows.Forms;

namespace Evolution
{
    public partial class ParametersForm : Form
    {
        public bool ParametersHaveChanged { get; private set; }

        public ParametersForm()
        {
            this.Load += ParametersForm_Load;
            InitializeComponent();
        }

        private void ParametersForm_Load(object sender, EventArgs e)
        {
            this.dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;

            foreach (var parameter in typeof(Parameters).GetProperties())
            {
                this.dataGridView1.Rows.Add(parameter.Name, parameter.GetValue(typeof(Parameters), null).ToString());
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var property = typeof(Parameters).GetProperty(this.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
            var typeOfProperty = property.PropertyType;
            if (typeOfProperty == typeof(double))
            {
                property.SetValue(typeof(Parameters), Convert.ToDouble(this.dataGridView1.Rows[e.RowIndex].Cells[1].Value));
            }
            else if (typeOfProperty == typeof(int))
            {
                property.SetValue(typeof(Parameters), Convert.ToInt32(this.dataGridView1.Rows[e.RowIndex].Cells[1].Value));
            }

            var changesValid = Parameters.CheckParameters();
            if (!changesValid)
            {
                var properties = typeof(Parameters).GetProperties();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    var value = properties.FirstOrDefault(p => p.Name == row.Cells[0].Value.ToString()).GetValue(typeof(Parameters));
                    row.Cells[1].Value = value;
                }
            }

            this.ParametersHaveChanged = true;
        }
    }
}
