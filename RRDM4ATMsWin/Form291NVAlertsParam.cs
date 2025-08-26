using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form291NVAlertsParam : Form
    {
        /// <summary>

        //bool ViewWorkFlow;

        string WModeNotes;
        string WCcy; 

        int WRow; 

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        int WRowIndexGrid1;


        RRDMNVRulesForMatchingClass Ru = new RRDMNVRulesForMatchingClass();

        RRDMNVAlerts Ale = new RRDMNVAlerts();

     //   RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //
        //string WCcy;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WInternalAccNo; 
        string WExternalBankID;
        string WExternalAccNo;
        int WMode;

        public Form291NVAlertsParam(string InSignedId, int SignRecordNo, string InOperator, string InInternalAccNo,
                                          string InExternalBankID, string InExternalAccNo, int InMode)
        {

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WInternalAccNo = InInternalAccNo; 
            WExternalBankID = InExternalBankID;
            WExternalAccNo = InExternalAccNo;
            WMode = InMode;
            // 1 Updating 
            // 2 View Only 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;


            comboBoxLike.DataSource = Ru.GetRulesListGroups(WOperator);
            comboBoxLike.DisplayMember = "DisplayValue";

            Acc.ReadAndFindAccountSpecificForNostroVostro(WExternalAccNo, WExternalBankID);
            WCcy = Acc.CurrNm;

            label6.Text = "EXTERNAL BANK : " + WExternalBankID;
            label21.Text = "EXTERNAL ACC : " + WExternalAccNo;
            label22.Text = "Ccy : " + WCcy;

            if (WMode == 2)
            {
                labelStep1.Text = "View Alerts For This Pair";
                buttonDelete.Hide();
                buttonAdd.Hide();
                buttonUpdate.Hide();
                buttonSetAlerts.Hide();
                comboBoxLike.Hide();
                label7.Hide();
            }

        }

        // LOAD SCREEN 
   
        private void Form291_Load(object sender, EventArgs e)
        {
            // Read and Fill Table only the default 
            //public void ReadNVStatement_LinesForMatched(string InOperator, string InSignedId,
            //                      int InRunningJobNo, string InExternalAccno, string InternalAccNo)

            Ale.ReadNVAlertsFillTable(WOperator, WExternalBankID, WExternalAccNo); 
            if (Ale.TableAlertsLayers.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("No Records For Grid ");
                MessageForm.ShowDialog();

                return;
            }

            //Show Table
            ShowGrid();

        }

        int WSeqNo;
        //string SelectionCriteria;
 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
         
            WSeqNo = (int)rowSelected.Cells[0].Value;

            Ale.ReadNVAlertBySeqNo(WOperator, WSeqNo);

            textBox1.Text = Ale.RangeAmtFrom.ToString("#,##0.00");
            textBox2.Text = Ale.RangeAmtTo.ToString("#,##0.00");
            textBox3.Text = Ale.LimitDays.ToString();
            textBox4.Text = Ale.DateTimeCreated.ToString();

        }
      
        // Show Grid 
        public void ShowGrid()
        {
            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Currency Rates on: " + WSeqNo.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";


            dataGridView1.DataSource = Ale.TableAlertsLayers.DefaultView;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 80; // BankID
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dataGridView1.Columns[2].Width = 90; // ExternalAccNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 60;  // Ccy
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 120;  //RangeAmtFrom
            dataGridView1.Columns[4].DefaultCellStyle = style;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[5].Width = 120;  //RangeAmtTo
            dataGridView1.Columns[5].DefaultCellStyle = style;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[6].Width = 120;  //Limit Days
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }
        // NOTES 
        private void buttonNotes2_Click_1(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            WParameter4 = "Currency Rates on: " + WSeqNo.ToString();
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WModeNotes = "Read";
            //else 
            WModeNotes = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WModeNotes, SearchP4);
            NForm197.ShowDialog();
          
        }
    
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
// Print 
        private void button1_Click(object sender, EventArgs e)
        {
            Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

            string P1 = "REPORT OF MATCHED TRANSACTIONS";
            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Us.BankId;
            string P5 = WSignedId;

            Form56R64NOSTRO ReportNOSTRO64 = new Form56R64NOSTRO(P1, P2, P3, P4, P5, "", "");
            ReportNOSTRO64.Show();
        }
       

// Add
        private void buttonAdd_Click(object sender, EventArgs e)
        {
          
            // Validation 

            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Please fill all fields ");
                return; 
            };

            if (decimal.TryParse(textBox1.Text, out Ale.RangeAmtFrom))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid no ");
                return;
            }
            if (decimal.TryParse(textBox2.Text, out Ale.RangeAmtTo))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid no ");
                return;
            }
            if (int.TryParse(textBox3.Text, out Ale.LimitDays))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid no ");
                return;
            }

            Ale.ReadNVAlertsById(WOperator, Ale.RangeAmtFrom, Ale.RangeAmtTo);
            if (Ale.RecordFound == true)
            {
                MessageBox.Show(textBox1.Text, "Range Already Exist");
                return;
            }
            Ale.InternalAccNo = WInternalAccNo; 
            Ale.ExternalBankID = WExternalBankID;
            Ale.ExternalAccNo = WExternalAccNo; 
            Ale.Ccy = WCcy; 
            Ale.Operator = WOperator;

            int ReturnNo = Ale.InsertNewAlert();

            // Load Grid 
            Form291_Load(this, new EventArgs());

            checkBox1.Checked = false;

            //int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            //// Load Grid 
            //Form291_Load(this, new EventArgs());

            //WRow = dataGridView1.Rows.Count - 1;  
            //dataGridView1.Rows[WRow].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            //dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
// Update
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            // Read Record
            // Assign updated fields
            // Make updating 
            // Validation 
            // Validation 

            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Please fill all fields ");
                return;
            };

            if (decimal.TryParse(textBox1.Text, out Ale.RangeAmtFrom))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid no ");
                return;
            }
            if (decimal.TryParse(textBox2.Text, out Ale.RangeAmtTo))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid no ");
                return;
            }
            if (int.TryParse(textBox3.Text, out Ale.LimitDays))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid no ");
                return;
            }

            Ale.UpdateAlertRecord(WSeqNo); 
         
            WRow = dataGridView1.SelectedRows[0].Index;
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            // Load Grid 
            Form291_Load(this, new EventArgs());

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
          
        }
// Delete 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            WRowIndexGrid1 = dataGridView1.SelectedRows[0].Index;

            if (MessageBox.Show("Warning: Do you want to delete this entry ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {        
                Ale.DeleteAlertEntry(WSeqNo); 
                Form291_Load(this, new EventArgs());
            }
            else
            {
                return;
            }
            if (WRowIndexGrid1 - 1 > 0)
            {
                dataGridView1.Rows[WRowIndexGrid1].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexGrid1 - 1));
            }
        }
 
// Add New checkbox
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox1.Text = "";
                textBox1.ReadOnly = false;
                textBox2.Text = "";
                textBox2.ReadOnly = false;
                textBox3.Text = "";
                textBox3.ReadOnly = false;
                textBox4.Text = "";
                textBox4.ReadOnly = true;
                buttonAdd.Show();
                buttonUpdate.Hide();
                buttonDelete.Hide(); 
            }
            if (checkBox1.Checked == false)
            {
                textBox1.ReadOnly = false;
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = false;
                textBox4.ReadOnly = false;
                buttonAdd.Hide();
                buttonUpdate.Show();
                buttonDelete.Show();
               
            }

        }
        //
// Set Like 
//
        private void buttonSetAlerts_Click(object sender, EventArgs e)
        {
            // GET RULES FROM MODEL RULES 
            string s = comboBoxLike.Text;
            string[] values = s.Split(',');

            string ModelBank = values[1];
            string ModelAcc = values[2];

            // Read if any other previous rules for Working Bank And Account 

            string WSelectionCriteria =
                " WHERE ExternalBankID='" + WExternalBankID + "' AND ExternalAccNo ='" + WExternalAccNo + "'";
          
            Ale.ReadNVAlertBySelectionCriteria(WSelectionCriteria);

            if (Ale.TotalSelected > 0)
            {

                if (MessageBox.Show("Warning: There are previous : " + Ale.TotalSelected.ToString() + " Rules  Do you want to proceed ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                {
                    // YES 
                    Ale.DeleteALertsEntriesforBankAcc(WExternalBankID, WExternalAccNo);
                }
                else
                {
                    // NO 
                    return;
                }
            }

            // Copy From Model to WExternalBank And WExternalAcc
            WSelectionCriteria =
               " WHERE ExternalBankID='" + ModelBank + "' AND ExternalAccNo ='" + ModelAcc + "'";
            Ale.ReadRuleBySelectionCriteriaAndInsert(WSelectionCriteria, WInternalAccNo , WExternalBankID, WExternalAccNo, WCcy);

            MessageBox.Show("Number of records inserted :" + Ale.TotalSelected);

            Form291_Load(this, new EventArgs());
        }
    }
    }

