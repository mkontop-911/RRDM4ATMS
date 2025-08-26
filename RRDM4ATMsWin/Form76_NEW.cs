using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMsClasses; 

namespace RRDM4ATMsWin
{
    public partial class Form76_NEW : Form
    {
        RRDM_AuditTrailClass_NEW At = new RRDM_AuditTrailClass_NEW();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime FromDt;
        DateTime ToDt;

        string WSelectionCriteria; 

        string WSignedId;
       // int WSignRecordNo;
        string WOperator;
        string WInUserId; 
  

        public Form76_NEW(string InSignedId, string InOperator, string InUserId)
        {
            WSignedId = InSignedId;
           // WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WInUserId = InUserId; 

            InitializeComponent();

            pictureBox1.BackgroundImage = appResImg.logo2;

            textBoxMsgBoard.Text = "Make choices in order to view the audit trails of specific screens.";
            if (WInUserId != "")
            {
                labelStep1.Text = "Audit Trail for.."+ WInUserId;
            }
            else
            {
                labelStep1.Text = "Audit Trail";
            }
            

            FromDt = ToDt = NullPastDate; 

            WSelectionCriteria = " WHERE Operator='" + WOperator + "'";

            UpdateComboBoxes();
        }

        private void UpdateComboBoxes()
        {
            //AuditTrailClass ATclass = new AuditTrailClass();

            comboBoxCategory.DataSource = At.GetCategory();
            comboBoxCategory.DisplayMember = "DisplayValue";
            //comboBoxCategory.ValueMember = "ItemValue";

            comboBoxSubCategory.DataSource = At.GetSubCategory();
            comboBoxSubCategory.DisplayMember = "DisplayValue";
            //comboBoxSubCategory.ValueMember = "ItemValue";

            comboBoxType.DataSource = At.GetAction();
            comboBoxType.DisplayMember = "DisplayValue";
            //comboBoxType.ValueMember = "ItemValue";
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Form76_Load(object sender, EventArgs e)
        {
            
            At.ReadBranchesAtmAndFillTable(WSelectionCriteria, FromDt, ToDt);
            
            dataGridView2.DataSource = At.AuditTrailDataTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //buttonAdd.Hide();
                //buttonUpdate.Hide();
                //buttonDelete.Hide();
                return;
            }

            //AuditTrailDataTable.Columns.Add("UniqueId", typeof(int)); //1

            //AuditTrailDataTable.Columns.Add("Category", typeof(string)); // 2
            //AuditTrailDataTable.Columns.Add("Sub Category", typeof(string)); //3
            //AuditTrailDataTable.Columns.Add("Type Of Change", typeof(string)); //4
            //AuditTrailDataTable.Columns.Add("User Id", typeof(string)); // 5
            //AuditTrailDataTable.Columns.Add("DateTime", typeof(DateTime)); //6
            //AuditTrailDataTable.Columns.Add("Screenshot", typeof(byte[])); // B Screen shot //7
            //AuditTrailDataTable.Columns.Add("Action", typeof(string)); //8
            //AuditTrailDataTable.Columns.Add("ScreenshotPriorChange", typeof(byte[])); // A Screen shot //9
            //AuditTrailDataTable.Columns.Add("Message", typeof(string)); // 10

            dataGridView2.Columns[0].Width = 60; // UniqueId
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ////dataGridView2.Columns[0].Visible = false;

            ////dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);

            dataGridView2.Columns[1].Width = 90; // Category
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 90; // Sub Category
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 90; // Type Of Change
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 60; // User Id
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Width = 110; // DateTime
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[6].Width = 120; // Screenshot
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[6].Visible = false;

            dataGridView2.Columns[7].Width = 100; // Action
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[8].Width = 120; // ScreenshotPriorChange
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[8].Visible = false;

            dataGridView2.Columns[9].Width = 250; // Message
            dataGridView2.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


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

                if (Atmfilter !="")
                {
                    WSelectionCriteria = "WHERE "+Atmfilter;
                }
                
                if (checkBoxDateRange.Checked)
                {
                    DateTime start = new DateTime(dateTimePickerStart.Value.Year, dateTimePickerStart.Value.Month, dateTimePickerStart.Value.Day, 0, 0, 0);
                    DateTime end = new DateTime(dateTimePickerEnd.Value.Year, dateTimePickerEnd.Value.Month, dateTimePickerEnd.Value.Day, 23, 59, 59);

                    if (end>start)
                    {
                        // OK 
                        FromDt = start;
                        ToDt = end; 
                    }
                    else
                    {
                        MessageBox.Show("Invalid Dates");
                        return; 
                    }

                    //if (Atmfilter.Equals(""))
                    //{

                    //    //Atmfilter = "DateTime BETWEEN '" + start + "' AND '" + end + "'";
                    //    Atmfilter = "DateTime > '" + start + "' AND DateTime < '" + end + "'";
                    //}
                    //else
                    //{
                    //    //Atmfilter = "AND DateTime BETWEEN '" + start + "' AND '" + end + "'";
                    //    Atmfilter = "AND DateTime > '" + start + "' AND DateTime < '" + end + "'";
                    //}
                }

                Form76_Load(this, new EventArgs());

                //auditTrailBindingSource.Filter = Atmfilter;

                //this.auditTrailTableAdapter.Fill(this.aTMSDataSet17.AuditTrail);
            }
            catch { }
        }


        // ROW ENTER
        int WSeqNo;
        int WrowSelected; 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WrowSelected = e.RowIndex; 
            WSeqNo = (int)rowSelected.Cells[0].Value;
        }

        //private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.ColumnIndex == 7)
        //    {
        //        Form77 NForm77 = new Form77(dataGridView2,e.RowIndex);
        //        NForm77.ShowDialog();
        //    }
        //}

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
               // this.auditTrailTableAdapter.FillBy(this.aTMSDataSet17.AuditTrail);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }
// Double Click
        private void dataGridView2_DoubleClick(object sender, EventArgs e)
        {

            Form77_NEW NForm77_NEW = new Form77_NEW(dataGridView2, WrowSelected);
            NForm77_NEW.ShowDialog();
        }
// Show
        private void buttonShow_Click(object sender, EventArgs e)
        {
            Form77_NEW NForm77_NEW = new Form77_NEW(dataGridView2, WrowSelected);
            NForm77_NEW.ShowDialog();
        }

        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Form77_NEW NForm77_NEW = new Form77_NEW(dataGridView2, e.RowIndex);
            NForm77_NEW.ShowDialog();
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Form77_NEW NForm77_NEW = new Form77_NEW(dataGridView2, e.RowIndex);
            NForm77_NEW.ShowDialog();
        }
    }
}
