using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RRDM4ATMsWin
{
    public partial class Form76 : Form
    {

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
  

        public Form76(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            pictureBox1.BackgroundImage = appResImg.logo2;

            textBoxMsgBoard.Text = "Make choices in order to view the audit trails of specific screens.";

            UpdateComboBoxes();
        }

        private void UpdateComboBoxes()
        {
            AuditTrailClass ATclass = new AuditTrailClass();

            comboBoxCategory.DataSource = ATclass.GetCategory();
            comboBoxCategory.DisplayMember = "DisplayValue";
            //comboBoxCategory.ValueMember = "ItemValue";

            comboBoxSubCategory.DataSource = ATclass.GetSubCategory();
            comboBoxSubCategory.DisplayMember = "DisplayValue";
            //comboBoxSubCategory.ValueMember = "ItemValue";

            comboBoxType.DataSource = ATclass.GetAction();
            comboBoxType.DisplayMember = "DisplayValue";
            //comboBoxType.ValueMember = "ItemValue";
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void panelFilters_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form76_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet17.AuditTrail' table. You can move, or remove it, as needed.
            dataGridView1.Sort(dataGridView1.Columns[5], ListSortDirection.Descending);
            this.auditTrailTableAdapter.Fill(this.aTMSDataSet17.AuditTrail);

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void auditTrailBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void buttonFilter_Click(object sender, EventArgs e)
        {
            try
            {
                string category = comboBoxCategory.SelectedItem.ToString().TrimEnd();
                string subcategory = comboBoxSubCategory.SelectedItem.ToString().TrimEnd();
                string type = comboBoxType.SelectedItem.ToString().TrimEnd();
                string user = textBoxUserId.Text.TrimEnd();

                string Atmfilter = "";

                if (!category.Equals("")) { Atmfilter = Atmfilter + "Category = '" + category + "' "; }
                if (!subcategory.Equals("")) { if (!Atmfilter.Equals("")) { Atmfilter = Atmfilter + "AND SubCategory='" + subcategory + "' "; } else { Atmfilter = Atmfilter + "SubCategory='" + subcategory + "' "; } }
                if (!type.Equals("")) { if (!Atmfilter.Equals("")) { Atmfilter = Atmfilter + "AND TypeOfChange='" + type + "' "; } else { Atmfilter = Atmfilter + "TypeOfChange='" + type + "' "; } }
                if (!user.Equals("")) { if (!Atmfilter.Equals("")) { Atmfilter = Atmfilter + "AND UserID='" + user + "' "; } else { Atmfilter = Atmfilter + "UserID='" + user + "'"; } }


                if (checkBoxDateRange.Checked)
                {
                    DateTime start = new DateTime(dateTimePickerStart.Value.Year, dateTimePickerStart.Value.Month, dateTimePickerStart.Value.Day, 0, 0, 0);
                    DateTime end = new DateTime(dateTimePickerEnd.Value.Year, dateTimePickerEnd.Value.Month, dateTimePickerEnd.Value.Day, 23, 59, 59);

                    if (Atmfilter.Equals(""))
                    {

                        //Atmfilter = "DateTime BETWEEN '" + start + "' AND '" + end + "'";
                        Atmfilter = "DateTime > '" + start + "' AND DateTime < '" + end + "'";
                    }
                    else
                    {
                        //Atmfilter = "AND DateTime BETWEEN '" + start + "' AND '" + end + "'";
                        Atmfilter = "AND DateTime > '" + start + "' AND DateTime < '" + end + "'";
                    }
                }
            
                
                
                auditTrailBindingSource.Filter = Atmfilter;

                this.auditTrailTableAdapter.Fill(this.aTMSDataSet17.AuditTrail);
            }
            catch { }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Form77 NForm77 = new Form77(dataGridView1, e.RowIndex);
            NForm77.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 7)
            {
                Form77 NForm77 = new Form77(dataGridView1,e.RowIndex);
                NForm77.ShowDialog();
            }
        }

        private void checkBoxDateRange_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDateRange.Checked)
            {
                dateTimePickerStart.Enabled = true;
                dateTimePickerEnd.Enabled = true;
                labelTo.Enabled = true;
            }
            else
            {
                dateTimePickerStart.Enabled = false;
                dateTimePickerEnd.Enabled = false;
                labelTo.Enabled = false;
            }

        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.auditTrailTableAdapter.FillBy(this.aTMSDataSet17.AuditTrail);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }
      
     
    }
}
