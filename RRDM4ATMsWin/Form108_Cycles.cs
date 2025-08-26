using System;
using System.ComponentModel;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Xml.Linq;
using System.Data;
using System.IO;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form108_Cycles : Form
    {

        Form65 NForm65;

        int WRow;
        string WAtmNo;
        int scrollPosition;
        //string WSelectionCriteria; 

        RRDMAtmsClass Ac = new RRDMAtmsClass();
      
        RRDMComboClass Cc = new RRDMComboClass();

        //     FormMainScreen NFormMainScreen;
        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        public Form108_Cycles(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            // Set Working Date 
          
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            textBoxMsgBoard.Text = "Make your ATM and proceed with requested action";

            // ATM Reconc Group
            comboBoxGroups.DataSource = Cc.GetAtmsReconcGroups(WOperator);
            comboBoxGroups.DisplayMember = "DisplayValue";

            // CIT  
            //  
            comboBoxCIT.DataSource = Cc.GetCitIds_2(WOperator);
            comboBoxCIT.DisplayMember = "DisplayValue";

        }

        private void Form108_Load(object sender, EventArgs e)
        {      
            try
            {
                Ac.ReadAtmAndFillTableByOperator(WSignedId, WOperator);
                ShowGridAtms(); 

                if (comboBoxGroups.Text == "0")
                {
                    textBox13.Text = "ALL ATMS"; 
                }

            }
            catch (Exception ex)
            {
          
                RRDMLog4Net Log = new RRDMLog4Net();

                string WLogger = "RRDM4Atms";

                string WParameters = "";

                Log.CreateAndInsertRRDMLog4NetMessage(ex, WLogger, WParameters);

                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");

                //Environment.Exit(0);

            }  

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
          
            WAtmNo = rowSelected.Cells[0].Value.ToString();

            textBox1.Text = WAtmNo;
            Ac.ReadAtm(WAtmNo);
         
            textBox3.Text = Ac.AtmName;
            textBox4.Text = Ac.Branch;
            textBox5.Text = Ac.BranchName;
            textBox6.Text = Ac.Model;
            textBox7.Text = Ac.TypeOfRepl;
            textBox9.Text = Ac.Street;

            RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms();
            RRDMUsersRecords Ur = new RRDMUsersRecords();

            if (Ac.CitId != "1000")
            {
                
                Ur.ReadUsersRecord(Ac.CitId);
                textBoxReplOwner.Text = "CIT:."+ Ac.CitId + ".:."+Ur.UserName;

                Uaa.ReadUsersAccessAtmTableSpecificForAtmNo(WAtmNo);
                if (Uaa.RecordFound == true)
                {
                    Ur.ReadUsersRecord(Uaa.UserId);
                    textBoxReconcOwner.Text = Ur.UserId + ".:." + Ur.UserName;
                }
                else
                {
                    textBoxReconcOwner.Text = "Not Specified";
                }

            }
            else
            {
                
                Uaa.ReadUsersAccessAtmTableSpecificForAtmNo(WAtmNo);
                if (Uaa.RecordFound == true)
                {
                    
                    Ur.ReadUsersRecord(Uaa.UserId); 
                    textBoxReplOwner.Text = Ur.UserId + ".:." + Ur.UserName;
                    textBoxReconcOwner.Text = Ur.UserId + ".:." + Ur.UserName;
                }
                else
                {
                    textBoxReplOwner.Text = "Not Specified";
                    textBoxReconcOwner.Text = "Not Specified";
                }
                
            }
            
        }


        // Go to Form7 to open new ATM like chosen 
        private void button3_Click(object sender, EventArgs e)
        {
          
           
        }

        // Go to Form7 and Show Data of Chosen ATM 
        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        void NForm65_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (dataGridView1.Rows.Count != 0)
            {
                WRow = dataGridView1.SelectedRows[0].Index;
                scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
                // Load Grid 
                Form108_Load(this, new EventArgs());

                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
            }
            else
            {
                // Load Grid 
                Form108_Load(this, new EventArgs());
            }          
        }

        // Go to Form 7 and Update Chosen 
        private void button2_Click(object sender, EventArgs e)
        {
          
        }
        // Go to Form7 and open New ATM 
       
        private void button4_Click_1(object sender, EventArgs e)
        {
           

        }
// Finish 
        private void button5_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

// Finish event 
        private void Form108_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        // DELETE ATM
        private void button6_Click(object sender, EventArgs e)
        {
           
        }
        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGridAtms()
        {

            dataGridView1.DataSource = Ac.ATMsDetailsDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No ATMs Available!");
                return;
            }
            else
            {
                textBoxTotal.Text = dataGridView1.Rows.Count.ToString(); 
            }

            dataGridView1.Columns[0].Width = 90; // AtmNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 110; //Atms Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 60; //  Branch
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 60; // Cit
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 70; // CashInType
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


            //RowSelected["AtmNo"] = AtmNo;
            //RowSelected["AtmName"] = AtmName;
            //RowSelected["Branch"] = Branch;
            //RowSelected["CitId"] = CitId;
            //RowSelected["CashInType"] = CashInType;
            //RowSelected["BranchName"] = BranchName;
            //RowSelected["Street"] = Street;
        }
//Enter An ATM
        private void button7_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox10.Text))

            {
                MessageBox.Show("Enter An ATM number Please");
                return;
            }
            else
            {
                WAtmNo = textBox10.Text;

            }        

            Ac.ReadAtmAndFillTableByAtmNo(WSignedId, WOperator, WAtmNo);

            ShowGridAtms();

            //Form108_Load(this, new EventArgs());
        }
//Refresh - show all ATMs 
        private void button8_Click(object sender, EventArgs e)
        {

        //    Ac.ReadAtmAndFillTableByOperator(WSignedId, WOperator);

            Form108_Load(this, new EventArgs());
        }
// Print 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Matching is done but not Settled 
            //SelectionCriteria = " WHERE Operator ='" + WOperator + "' AND RMCateg ='" + WCategoryId + "'"
            //          + "  AND RMCycle =" + WRMCycle
            //          + " AND IsMatchingDone = 1 AND Matched = 0  "
            //          //+ " AND IsMatchingDone = 1 AND Matched = 0 AND SettledRecord = 0 "
            //          + " AND ActionType != '7' ";

            //Mpa.ReadMatchingTxnsMasterPoolAndFillTableFastTrack(WOperator, WSignedId, SelectionCriteria,
            //                                                                         WSortCriteria);

            string P1 = "ATMs DETAILS " ;

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = Ac.BankId;
            string P5 = WSignedId;

            Form56R56ATMS ReportATMS56 = new Form56R56ATMS(P1, P2, P3, P4, P5);
            ReportATMS56.Show();
        }
        int AtmsReconcGroup;
        private void comboBoxGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            RRDMGroups Gr = new RRDMGroups();
            
            if (int.TryParse(comboBoxGroups.Text, out AtmsReconcGroup))
            {
            }
            else
            {

            }

            Gr.ReadGroup(AtmsReconcGroup);

            textBox13.Text = Gr.Description;
         
        }
// Group
        private void buttonGroup_Click(object sender, EventArgs e)
        {
            //WSelectionCriteria = "Operator = '" + WOperator + "' AND AtmsReconcGroup =" + AtmsReconcGroup;

            Ac.ReadAtmAndFillTableByAtmsReconcGroup(WSignedId, WOperator, AtmsReconcGroup);

            ShowGridAtms(); 

            //Form108_Load(this, new EventArgs());
        }
// Export to Excel
        private void buttonExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();
           
           

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelDATE = DateTime.Now.Year.ToString()
                       + DateTime.Now.Month.ToString()
                       + DateTime.Now.Day.ToString()
                       + "_"
                       + DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString()
                        + DateTime.Now.Second.ToString()
                        ;

            string ExcelPath;
            string WorkingDir;
           
            ExcelPath = "C:\\RRDM\\Working\\ATMs_Details_" + ExcelDATE + ".xls";
            WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Ac.ATMsDetailsDataTable, WorkingDir, ExcelPath);
           
        }

        //public static string ToXml(this DataTable table, int metaIndex = 0)
        //{
        //    //XDocument xdoc = new XDocument(
        //    //    new XElement(table.TableName,
        //    //        from column in table.Columns.Cast<DataColumn>()
        //    //        where column != table.Columns[metaIndex]
        //    //        select new XElement(column.ColumnName,
        //    //            from row in table.AsEnumerable()
        //    //            select new XElement(row.Field<string>(metaIndex), row[column])
        //    //            )
        //    //        )
        //    //    );

        //    //return xdoc.ToString();
        //}
        // CIT Has Changed
        private void comboBoxCIT_SelectedIndexChanged(object sender, EventArgs e)
        {
            string WCitId = comboBoxCIT.Text;
            if (WCitId =="Select")
            {
                // No CIT was selected
                textBoxCitNm.Text = "No Name";
            }
            else
            {
                RRDMUsersRecords Us = new RRDMUsersRecords();
                Us.ReadUsersRecord(WCitId);
                textBoxCitNm.Text = Us.UserName;
            }
           
        }
// Show Replenished By 
        private void buttonShowCIT_Click(object sender, EventArgs e)
        {
            if (comboBoxCIT.Text == "Select")
            {
                MessageBox.Show("Please make selection");
                return; 
            }
            else
            {
                Ac.ReadAtmAndFillTableByAtmsCitId(WSignedId, WOperator, comboBoxCIT.Text);

                ShowGridAtms();
            }
            
        }
// EXPORT TO XML 
        private void buttonXML_Click(object sender, EventArgs e)
        {
            string XMLDoc;

            //XMLDoc = ToXml(Ac.ATMsDetailsDataTable, 0);
            string sFileName = "C:\\RRDM\\Working\\TEXT.XML";
            StreamWriter outputFile = new StreamWriter(@sFileName);

            DataSet dS = new DataSet();
            dS.DataSetName = "RecordSet";
            dS.Tables.Add(Ac.ATMsDetailsDataTable);
            //StringWriter sw = new StringWriter();
            dS.WriteXml(outputFile, XmlWriteMode.IgnoreSchema);

            MessageBox.Show("An XML File is created in RRDM working directory"); 

        }
        // Show all ATMs to be replenished by dates range  
        private void buttonShowAll_Click(object sender, EventArgs e)
        {
           
           
        }

        private void linkLabelRepl_For_this_ATM_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           
            DateTime FromDt = dateTimePicker1.Value;
            DateTime ToDt = dateTimePicker2.Value;

            if (ToDt < FromDt)
            {
                MessageBox.Show("Second date Less than First. Not allowed! ");
                return; 
            }

            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            Form67_BDC NForm67_BDC;

            int Mode = 35; // All Repl Cycles for all ATMS OUTSTANDING
            int WSesNo = 0;
            int WReconcCycleNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                                , WReconcCycleNo, FromDt, ToDt, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
        // Show all ATMs to be replenished by dates range  
        private void linkLabelOutstandingForRepl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime FromDt = dateTimePicker1.Value;
            DateTime ToDt = dateTimePicker2.Value;

            if (ToDt < FromDt)
            {
                MessageBox.Show("Second date Less than First. Not allowed! ");
                return;
            }

            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            Form67_BDC NForm67_BDC;

            int Mode = 6; // All Repl Cycles for all ATMS OUTSTANDING
            int WSesNo = 0;
            int WReconcCycleNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                                , WReconcCycleNo, FromDt, ToDt, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
// Manage Shortage OR Excess
        private void buttonManageShortage_Click(object sender, EventArgs e)
        {
            DateTime FromDt = dateTimePicker1.Value;
            DateTime ToDt = dateTimePicker2.Value;

            if (ToDt < FromDt)
            {
                MessageBox.Show("Second date Less than First. Not allowed! ");
                return;
            }

            Form67_Cycle_Rich_Picture NForm67_Cycle_Rich_Picture;

            Form67_BDC NForm67_BDC;

            int Mode = 1; // All Repl Cycles for all ATMS OUTSTANDING
            int WSesNo = 0;
            int WReconcCycleNo = 0;
            NForm67_Cycle_Rich_Picture = new Form67_Cycle_Rich_Picture(WSignedId, WOperator, WAtmNo, WSesNo
                                , WReconcCycleNo, FromDt, ToDt, Mode);
            NForm67_Cycle_Rich_Picture.ShowDialog();
        }
    }
}
