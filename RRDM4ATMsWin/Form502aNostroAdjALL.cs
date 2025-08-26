using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form502aNostroAdjALL : Form
    {
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
        RRDMMatchingCategoriesVsSourcesFiles Rcs = new RRDMMatchingCategoriesVsSourcesFiles();
        RRDMUniversalTableFieldsDefinition Utd = new RRDMUniversalTableFieldsDefinition();
        RRDMMatchingCategStageVsMatchingFields Rsm = new RRDMMatchingCategStageVsMatchingFields();

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();
        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        int WSeqNo;
        //int WSeqNo2;

        //int WRowGrid2;

        //string WCategoryNm;

        string WUserBankId;

        //int RunningJob;

        //string Ccy;
        //string InternalAcc;
        //string ExternalAcc;

        ////
        //string W4DigitMainCateg;

        string SelectionCriteria;

        int WSeqNo3;

        //string WComboboxText;

        //int WRowGrid1;
        //int WRowGrid3;

        //string WCategoryIdAndName;

        int WMode = 6;
        string WReference;

        string WOrderCriteria;

        DateTime WDateOfExternal = new DateTime(2015, 02, 15);

        public DataTable TableFieldsOrder = new DataTable();

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSubSystem;      

        public Form502aNostroAdjALL(string InSignedId, int SignRecordNo, string InOperator, string InSubSystem)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WSubSystem = InSubSystem; 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = InSignedId;

            Us.ReadUsersRecord(WSignedId);
            WUserBankId = Us.BankId;

            textBoxMsgBoard.Text = "Select Txns To Match. ";

            //Initialise 

            TableFieldsOrder = new DataTable();
            TableFieldsOrder.Clear();

            TableFieldsOrder.Columns.Add("SeqNo", typeof(int));
            TableFieldsOrder.Columns.Add("FieldName", typeof(string));
            TableFieldsOrder.Columns.Add("FieldDBName", typeof(string));

            //Home
            WMode = 57;
            WReference = "";
            WOrderCriteria = " Origin ASC ";

            textBoxMsgBoard.Text = "See Unmatched and Inform User ";
        }

        private void Form502a_Load(object sender, EventArgs e)
        {

            //WDateOfExternal

            Se.ReadNVStatements_LinesByMode(WOperator, WSignedId, WSubSystem, WMode,  0 , "", "", "", WDateOfExternal, WReference);

            ShowGrid1();

            // Matching Fields (THREE)

            ShowGridMatchingFields();

        }

        string WOrigin;
        //string WOrigin2;
        decimal LAmt;

        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;
            WOrigin = (string)rowSelected.Cells[4].Value;

            SelectionCriteria = " WHERE SeqNo =" + WSeqNo + " AND Origin ='" + WOrigin + "'";

            Se.ReadNVStatements_Lines_BySelectionCriteria(SelectionCriteria);

            LAmt = Se.StmtLineAmt;

        }
      

        // Row Enter For Matching fields 
        private void dataGridView3_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            WSeqNo3 = (int)rowSelected.Cells[0].Value;

            Utd.ReadUniversalTableFieldsDefinitionSeqNo(WSeqNo);

            WFieldName3 = Utd.FieldName;
            WFieldDBName3 = Utd.FieldDBName;
        }

       


        // FINISH
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
      
        //**********************************************************************
        // END NOTES 
        //********************************************************************** 
        // Show Grid 1 
       
        public void ShowGrid1()
        {
            dataGridView1.DataSource = Se.TableNVStatement_Lines_Both.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                //Form2 MessageForm = new Form2("Selection has ended");
                //MessageForm.ShowDialog();
               
                return;
            }

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = true;

            dataGridView1.Columns[1].Width = 40; // ColorNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 70; // MatchingNo
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[3].Width = 170; //MatchedType
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = false;

            dataGridView1.Columns[4].Width = 85;  // Origin
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[5].Width = 105;  // Acc No 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = true;

            dataGridView1.Columns[6].Width = 40;  // Done 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = false;

            dataGridView1.Columns[7].Width = 40;  // Code 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 90; //  ValueDate
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[9].Width = 90; // EntryDate
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[10].Width = 40; // DR/CR
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[11].Width = 100; // Amt
            dataGridView1.Columns[11].DefaultCellStyle = style;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[11].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[11].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[11].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[12].Width = 130; // OurRef
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[12].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[12].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[12].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[13].Width = 100; // TheirRef
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[14].Width = 95; // OtherDetails
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[15].Width = 95; // Ccy
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[15].Visible = true;

            dataGridView1.Columns[16].Width = 95; // CcyRate
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[16].Visible = true;

            dataGridView1.Columns[17].Width = 95; // GLAccount
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[17].Visible = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string WOrigin = row.Cells[4].Value.ToString();

                if (WOrigin == "EXTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (WOrigin == "INTERNAL")
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }

            //TotalExternal1 = 0;
            //TotalInternal1 = 0;
            //TotalAdjust1 = 0;
            //GTotal1 = 0;
            //WOurReference1 = "";

            //int K = 0;
            ////
            //// Totals and Analysis  
            ////
            //while (K <= (dataGridView1.Rows.Count - 1))
            //{
            //    RSeqNo = (int)dataGridView1.Rows[K].Cells["SeqNo"].Value;
            //    ROrigin = (string)dataGridView1.Rows[K].Cells["Origin"].Value;

            //    if (ROrigin == "EXTERNAL")
            //    {
            //        SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAcc + "' AND SeqNo =" + RSeqNo;

            //        Se.ReadNVStatement_Lines_ExternalBySelectionCriteria(SelectionCriteria);

            //        TotalExternal1 = TotalExternal1 + Se.StmtLineAmt;
            //    }

            //    if (ROrigin == "INTERNAL")
            //    {
            //        SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

            //        Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

            //        WOurReference1 = Se.StmtLineRefForAccountOwner;

            //        TotalInternal1 = TotalInternal1 + Se.StmtLineAmt;
            //    }

            //    if (ROrigin == "WAdjustment")
            //    {
            //        SelectionCriteria = " WHERE StmtAccountID ='" + InternalAcc + "' AND SeqNo =" + RSeqNo;

            //        Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);

            //        TotalAdjust1 = TotalAdjust1 + Se.StmtLineAmt;
            //    }

            //    K++; // Read Next entry of the table 
            //}
            //// 
            //GTotal1 = TotalExternal1 + TotalInternal1 + TotalAdjust1;
       

        }
       
        //******************
        // SHOW GRID dataGridView3
        //******************
        private void ShowGridMatchingFields()
        {
            // Keep Scroll position 
            //int scrollPosition = 0;
            //if (WRowGrid1 > 0)
            //{
            //    scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            //}

            // Matching Fields (THREE)

            //string WFilter3 = "Operator = '" + WOperator + "' AND Application = 'NOSTRO'";

            //Rmf.ReadReconcMatchingFieldsToFillDataTable(WFilter3);
            //dataGridView3.DataSource = Rmf.MatchingFieldsDataTable.DefaultView;

            
            string WTableStructureId = "NOSTRO";
            Utd.ReadUniversalTableMatchingFieldsToFillDataTable(WTableStructureId);

            if (dataGridView3.Rows.Count == 0)
            {
                return;
            }
            dataGridView3.Columns[0].Width = 70; // SeqNo
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView3.Columns[1].Width = 250; // Field Name
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView3.Columns[2].Width = 90; // Field Type
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView3.Rows[WRowGrid3].Selected = true;
            //dataGridView3_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowGrid3));

            //dataGridView3.FirstDisplayedScrollingRowIndex = scrollPosition;

            // DATA TABLE ROWS DEFINITION 
            //MatchingFieldsDataTable.Columns.Add("SeqNo", typeof(int));
            //MatchingFieldsDataTable.Columns.Add("Field Name", typeof(string));
            //MatchingFieldsDataTable.Columns.Add("Field Type", typeof(string));
        }

        //******************
        // SHOW GRID dataGridView4
        //******************
        private void ShowGrid4()
        {
            // Keep Scroll position 
            //int scrollPosition = 0;
            //if (WRowGrid1 > 0)
            //{
            //    scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
            //}

            dataGridView4.DataSource = TableFieldsOrder.DefaultView;

            if (dataGridView4.Rows.Count == 0)
            {
                return;
            }
            dataGridView4.Columns[0].Width = 70; // SeqNo
            dataGridView4.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView4.Columns[1].Width = 250; // Field Name
            dataGridView4.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            dataGridView4.Columns[2].Width = 100; // Field Type
            dataGridView4.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView3.FirstDisplayedScrollingRowIndex = scrollPosition;

        }
      
       
        // Add Field


        string WFieldName3;
        string WFieldDBName3;
        private void buttonAddField_Click(object sender, EventArgs e)
        {

            DataRow RowSelected = TableFieldsOrder.NewRow();

            RowSelected["SeqNo"] = WSeqNo3;
            RowSelected["FieldName"] = WFieldName3;
            RowSelected["FieldDBName"] = WFieldDBName3;

            // ADD ROW
            TableFieldsOrder.Rows.Add(RowSelected);


            ShowGrid4();

        }
        // Remove Field
        private void buttonRemoveField_Click(object sender, EventArgs e)
        {
            for (int i = TableFieldsOrder.Rows.Count - 1; i >= 0; i--)
            {
                int TempSeqNo = (int)TableFieldsOrder.Rows[i]["SeqNo"];
                if (TempSeqNo == WSeqNo4)
                {
                    TableFieldsOrder.Rows[i].Delete();
                    return;
                }
                //DataRow dr = TableFieldsOrder.Rows[i];
                //if (dr["FieldName"] == WFieldName4)
                //    dr.Delete();
            }

        }
        // ROW ENTER FOR Grid 4 
        int WSeqNo4;
        string WFieldName4;
        string WFieldDBName4;
        private void dataGridView4_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView4.Rows[e.RowIndex];

            WSeqNo4 = (int)rowSelected.Cells[0].Value;
            WFieldName4 = (string)rowSelected.Cells[1].Value;
            WFieldDBName4 = (string)rowSelected.Cells[2].Value;


        }
        // Order By 
        private void buttonOrderBy_Click(object sender, EventArgs e)
        {

            StringBuilder WOrderCriteriaBuild = new StringBuilder();

            if (dataGridView4.Rows.Count == 0)
            {
                return;
            }

            int K = 0;
            //
            //VALIDATION 
            //
            while (K <= (dataGridView4.Rows.Count - 1))
            {
                WFieldDBName4 = (string)dataGridView4.Rows[K].Cells["FieldDBName"].Value;

                WOrderCriteriaBuild.Append(WFieldDBName4);
                if (dataGridView4.Rows.Count - 1 != K) WOrderCriteriaBuild.Append(", ");
                if (dataGridView4.Rows.Count - 1 == K) WOrderCriteriaBuild.Append(" ASC");
                K++; // Read Next entry of the table 
            }

            WOrderCriteria = WOrderCriteriaBuild.ToString();

            Form502a_Load(this, new EventArgs()); // Load Grid    

        }
        
    }
}
