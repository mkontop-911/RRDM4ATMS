using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form291NVCcyRatesDefinition : Form
    {
        /// <summary>

        //bool ReconciliationAuthor;
        //string StageDescr;
        //int WAuthorSeqNumber;

        //// Working Fields 
        //bool WViewFunction;
        //bool WAuthoriser;
        //bool WRequestor;

        //bool NormalProcess;

        //bool ViewWorkFlow;

        string WModeNotes;

        //int WMode;

        //string WSelectionCriteria;
        //string WSortCriteria;

        int WRow; 

        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        int WRowIndexGrid1;

        RRDMNVCurrentCcyRates Ccr = new RRDMNVCurrentCcyRates(); 
       
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime WDateOfExternal = new DateTime(2015, 02, 15);
        //
        //string WCcy;
        
        
        string WJobCategory ;
        string WOrigin;
        int WRMCycleNo; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSubSystem;

        public Form291NVCcyRatesDefinition(string InSignedId, int SignRecordNo, string InOperator, string InSubSystem)
        {
           
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WJobCategory = WSubSystem = InSubSystem;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;
            buttonAdd.Hide();

            if (WSubSystem == "CardsSettlement")
            {
                WJobCategory = "CardsSettlement";
                WOrigin = "Nostro - Vostro"; // 
            }
            if (WSubSystem == "NostroReconciliation")
            {
                WJobCategory = "NostroReconciliation";
                WOrigin = "Nostro - Vostro"; // 
            }

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            Rjc.ReadLastReconcJobCycleATMsAndNostro(WOperator, WJobCategory);
            if (Rjc.RecordFound == true)
            {
                WRMCycleNo = Rjc.JobCycle; 
            }
            else
            {
                WRMCycleNo = 0 ;
            }

            }

        // LOAD SCREEN   
        private void Form291_Load(object sender, EventArgs e)
        {
            // Read and Fill Table only the default 
            //public void ReadNVStatement_LinesForMatched(string InOperator, string InSignedId,
            //                      int InRunningJobNo, string InExternalAccno, string InternalAccNo)

            Ccr.ReadNVCurrentCcyRatesFillTable(WOperator);
            if (Ccr.TableCurrentCcyRates.Rows.Count == 0)
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

            Ccr.ReadNVCurrentCcyRatesBySeqNo(WOperator, WSeqNo);

            textBox1.Text = Ccr.Ccy;
            textBox2.Text = Ccr.CcyName;
            textBox3.Text = Ccr.CcyRate.ToString();
            textBox4.Text = Ccr.UpdatedDateTime.ToString();

        }

        //UNDO Default 
        //int WRowIndex;
      
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


            dataGridView1.DataSource = Ccr.TableCurrentCcyRates.DefaultView;

            //DataGridViewCellStyle style = new DataGridViewCellStyle();
            //style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 60; // Ccy
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[2].Width = 100; // Ccy NAME
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 90;  // Rate 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[4].Width = 120;  // Date
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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
           // Form56R64_RATES NForm56R64_RATES;

            Us.ReadUsersRecord(WSignedId); // Get the Bank for Bank Logo

            string P1 = "CURRENT CURRENCY RATES";
            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Us.BankId;
            string P5 = WSignedId;

            Form56R64_RATES ReportRATES = new Form56R64_RATES(P1, P2, P3, P4, P5);
            ReportRATES.Show();
        }
// DELETE 
       

// Add
        private void buttonAdd_Click(object sender, EventArgs e)
        {
          
            // Validation 

            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Please fill all fields ");
                return; 
            };

            if (decimal.TryParse(textBox3.Text, out Ccr.CcyRate))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid no for rate");
                return;
            }

            Ccr.ReadNVCurrentCcyRatesById(WOperator, textBox1.Text);
            if (Ccr.RecordFound == true)
            {
                MessageBox.Show(textBox1.Text, "Currency Already Exist");
                return;
            }
            Ccr.RMCycleNo = WRMCycleNo;
            Ccr.Ccy = textBox1.Text;
            Ccr.CcyName = textBox2.Text;
            Ccr.UpdatedDateTime = DateTime.Now; 
            Ccr.Operator = WOperator;

            Ccr.RMCycleNo = WRMCycleNo; 

            int ReturnNo = Ccr.InsertNewCcyRate();

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

            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Please fill all fields ");
                return;
            };

            if (decimal.TryParse(textBox3.Text, out Ccr.CcyRate))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid no for rate");
                return;
            }

            Ccr.Ccy = textBox1.Text;
            Ccr.CcyName = textBox2.Text;
            Ccr.UpdatedDateTime = DateTime.Now; 
            Ccr.Operator = WOperator;

            Ccr.UpdateCcyRecord(WSeqNo); 
         
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
                Ccr.DeleteCcyEntry(WSeqNo); 
                Form291_Load(this, new EventArgs());
            }
            else
            {
                return;
            }
            if (WRowIndexGrid1 - 1 > 0)
            {
                dataGridView1.Rows[WRowIndexGrid1].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexGrid1));
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
                textBox1.ReadOnly = true;
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = false;
                textBox4.ReadOnly = true;
                buttonAdd.Hide();
                buttonUpdate.Show();
                buttonDelete.Show();
                // Load Grid 
                Form291_Load(this, new EventArgs());
            }

        }
    }
    }

