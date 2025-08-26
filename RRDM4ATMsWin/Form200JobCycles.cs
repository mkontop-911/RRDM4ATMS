using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;
using System.IO.Compression;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
//using System.Data.OleDb;
using RRDM4ATMs;
using RRDMAgent_Classes;
using System.Globalization;


namespace RRDM4ATMsWin
{
    public partial class Form200JobCycles : Form
    {
        // Variables

        Form54 NForm54;
        Form55 NForm55;
        
        public bool Prive;

        //int WAction;

        int WJobCycleNo;

        string WCategoryId;

        DateTime WCut_Off_Date; 

        string MsgFilter;

        RRDMBanks Ba = new RRDMBanks();

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        //RRDMMatchingCategoriesSessions Mcs = new RRDMMatchingCategoriesSessions();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMReconcCategories Rc = new RRDMReconcCategories(); 

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMMatchingCategoriesVsSourcesFiles Msf = new RRDMMatchingCategoriesVsSourcesFiles();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        RRDMHolidays Ho = new RRDMHolidays();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        string ProcessName;
        string Message;
        int Mode;

        DateTime SavedStartDt;

        string sourceDir;
        string zipFilePath;
        string ReversedCut_Off_Date;


        //string WSelectionCriteria;
        int WReconcCycleNo;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
        
        string WJobCategory;

        // Methods 
        // READ ATMs Main
        // 
        public Form200JobCycles(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, string InJobCategory)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            WJobCategory = InJobCategory; 
            InitializeComponent();

            // Set Working Date 
          
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            //*****************************************
            //
            //*****************************************

            dateTimePickerNewCutOff.Value = DateTime.Now.Date; 

            textBoxMsgBoard.Text = "Job Cycles ";

            // ....

            MsgFilter =
                  "(ReadMsg = 0 AND ToAllAtms = 1)"
              + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;

            }
            else
            {
                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }

            toolTipController.SetToolTip(buttonCommController, "Communicate with today's controller.");

            // Define Directory 
            ReversedCut_Off_Date = "";

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (Rjc.RecordFound == true)
            {
                ReversedCut_Off_Date = Rjc.Cut_Off_Date.Date.ToString("yyyyMMdd");

                sourceDir = @"C:\RRDM\FilesArchives\" + ReversedCut_Off_Date + "_" + WReconcCycleNo.ToString(); // directory to zip
                zipFilePath = @"C:\RRDM\FilesArchives\" + ReversedCut_Off_Date + "_" + WReconcCycleNo.ToString() + ".zip"; // output file
                //sourceDir = @"C:\RRDM\FilesArchives\20250306_200"; // directory to zip
                //zipFilePath = @"C:\RRDM\FilesArchives\20250306_200.zip"; // output file                                                                                                        //sourceDir = @"C:\RRDM\FilesArchives\20250306_200"; // directory to zip
                                                                                                                           //zipFilePath = @"C:\RRDM\FilesArchives\20250306_200.zip"; // output file
            }

            WJobCycleNo = 0;
            //WJobCategory = WMatchingRunningGroup;
            //WJobCategory = "ATMs"; 

        }
        
       
        // Load
        private void Form200JobCycles_Load(object sender, EventArgs e)
        {   
           
            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            if (Rjc.RecordFound == true)
            {
                label4.Show();
                
                dateTimePickerCurrentCutOff.Show(); 
                dateTimePickerCurrentCutOff.Value = Rjc.Cut_Off_Date.Date;
            }
            else
            {
                label4.Hide();
                dateTimePickerCurrentCutOff.Hide();
            }

            string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND JobCategory ='"+ WJobCategory+"'";
            Rjc.ReadReconcJobCyclesFillTable(SelectionCriteria);

            ShowGrid1(); 
          
        }

        // ROW ENTER FOR JOB CYCLE 
        string WLatestStatus; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WJobCycleNo = (int)rowSelected.Cells[0].Value;

            textBoxReconcCycle.Text = WJobCycleNo.ToString(); 

            //Flog.ReadDataTableFileMonitorLogByCycleNo_For_Not_Loaded(WOperator, WSignedId, WJobCycleNo);

            //if (Flog.DataTableFileMonitorLog.Rows.Count > 0)
            //{
            //    // There is error
            //    buttonERRORS.Show();

            //}
            //else
            //{
            //    // No Error
            //    buttonERRORS.Hide();
            //}


            labelOtherCycleInfo.Text = "OTHER INFO FOR CYCLE.."+ WJobCycleNo.ToString();


            Rjc.ReadReconcJobCyclesById(WOperator, WJobCycleNo);

            WCut_Off_Date = Rjc.Cut_Off_Date;

            panelCycleMATRIX.Hide();
            labelMatrix.Hide();

            //Rc.ReadReconcCategoriesForMatrix(WOperator, WJobCycleNo);

            //WLatestStatus = Rc.StatusCycle1; 

            //ShowGrid2();

        }

        // Show Matrix 
        private void buttonShowMatrix_Click(object sender, EventArgs e)
        {
            panelCycleMATRIX.Show();
            labelMatrix.Show();

            Rc.ReadReconcCategoriesForMatrix(WOperator, WJobCycleNo, 2);

            WLatestStatus = Rc.StatusCycle1;

            ShowGrid2();
        }

        // On ROW ENTER 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WCategoryId = (string)rowSelected.Cells[0].Value;
       
            //Mc.ReadMatchingCategorybyCategId(WOperator, WCategoryId);
            //if (Mc.RecordFound == true)
            //{
            //    textBoxReconcCateg.Text = WCategoryId + "\r\n" + Mc.CategoryName;
            //}
            //else
            //{
                Rc.ReadReconcCategorybyCategId(WOperator, WCategoryId); 
                textBoxReconcCateg.Text = WCategoryId + "\r\n" + Rc.CategoryName;
            //}
            
            panel3.Hide();
            buttonOutstandings.Hide(); 
        }
       

        // Show Grid1
        private void ShowGrid1()
        {

            dataGridView1.DataSource = Rjc.TableReconcJobCycles.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                buttonViewFiles.Hide();
                buttonERRORS.Hide(); 
                return;
            }
            else
            {
                //dataGridView1.Show(); 
                buttonViewFiles.Show();

                bool MoveToHistory = false;
                string ParamId = "853";
                string OccuranceId = "5"; // HST
                int AgingDays_HST = 0; 

                Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

                if (Gp.RecordFound == true)
                {
                 AgingDays_HST = (int)Gp.Amount; // 
                }
                
               textBox1.Text = dataGridView1.Rows.Count.ToString();
               textBox2.Text = AgingDays_HST.ToString(); 

            }
           
            dataGridView1.Columns[0].Width = 60; // JobCycle;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[1].Width = 200; // JobCategory;
            dataGridView1.Columns[1].Visible = false;
            
            dataGridView1.Columns[2].Width = 120; // StartDateTm.ToString();
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 120; // FinishDateTm
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 120; //  Description
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 120; // "Status"
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //return; 

        }

        // Show Grid RCS
        int CountNotDone; 
        private void ShowGrid2()
        {  
        
            dataGridView2.DataSource = Rc.TableReconcCateg.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                return;
            }
            else
            {
                //dataGridView2.Show(); 
            }

            CountNotDone = 0;

            dataGridView2.Columns[0].Width = 110; // CategoryId
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[1].Width = 80; // This Cycle Unmatched 
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[2].Width = 80; // ALL UnMatched
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[3].Width = 80; // Cycle1
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[3].HeaderText = "Cycle " + Rc.Cycle1; 

            dataGridView2.Columns[4].Width = 80; // Cycle2
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[4].HeaderText = "Cycle " + Rc.Cycle2;

            dataGridView2.Columns[5].Width = 80; // Cycle3
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[5].HeaderText = "Cycle " + Rc.Cycle3;

            dataGridView2.Columns[6].Width = 80; // Cycle4
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[6].HeaderText = "Cycle " + Rc.Cycle4;

            dataGridView2.Columns[7].Width = 80; // Cycle5
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[7].HeaderText = "Cycle " + Rc.Cycle5;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                //WSeqNo = (int)rowSelected.Cells[0].Value;
                string WCycle1 = (string)row.Cells[3].Value;

                if (WCycle1 == "Not Done")
                {
                    row.DefaultCellStyle.BackColor = Color.Gainsboro;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    CountNotDone = CountNotDone + 1; 
                }
                if (WCycle1 == "Fully Done")
                {
                    row.DefaultCellStyle.BackColor = Color.Green;
                    row.DefaultCellStyle.ForeColor = Color.White;

                }
                if (WCycle1 == "Partially")
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    CountNotDone = CountNotDone + 1;
                }
            }

            if (dataGridView2.Rows.Count == (CountNotDone+Rc.Inop)) buttonDelete.Show();
            else buttonDelete.Hide(); 

        }

        // ADD A NEW CYCLE 

        int OldReconcJobCycle;

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Validation 
            // STEP A
            // Find Current Cycle (process mode = -1) and set to Suspense 
            // so NO new matching can start 
            // Current On Going Matching will work with already taken ReconcCycle
            //
            // Turn process mode of old one to 0 = ready to reconcile 
            // 
            // Create New Cycle with process mode = -1
            //
            //
            // Create new Reconc Categories Sessions with the new Reconc Job Cycle 
            // 
            // NB: For Matching
            // Start Matching when all files are present. 
            // If Matching is completed then 
            // if Matching Category is Master then go to reconciliation session 
            // and and turn process mode from -1 to 0 
            // And create new Reconciliation Cycle with -1 
            // if Reconciliation Category has  Slaves check that all Slaves
            // have finished for this Reconc Cycle turn from -1 to 1 and create a new one with -1 
            // 
            // Turn the expected files to be loaded with the new date of Cycle start 
            //

            //DateTime STOP_UAT_DATE = new DateTime(2020, 09, 05);
            //if (dateTimePickerNewCutOff.Value.Date> STOP_UAT_DATE)
            //{
            //    MessageBox.Show("You cannot go above the stop UAT Date which is 05_09_2020"); 
            //    return; 
            //}

            int TempReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            // Cut_Off_Date
            if (TempReconcCycleNo != 0)
            {
                if (dateTimePickerNewCutOff.Value.Date > Rjc.Cut_Off_Date.Date.AddDays(500))
                {
                    MessageBox.Show("The New Date is Greater By the previous CutOff by many days");
                    DateTime Temp = Rjc.Cut_Off_Date.Date.AddDays(15);
                    return;
                }
            }
            //DateTime LimitDate = new DateTime(2021, 03, 05);
            //if (dateTimePickerNewCutOff.Value.Date > LimitDate)
            //{
            //    MessageBox.Show("You are above the limit date"+Environment.NewLine
            //        + "You are not allowed to proceed."
            //        + "Ensure that all Dates in UAT are PERFECT"
            //        );
               
            //    return;
            //}


            if (Rjc.RecordFound == true)
            {
                // Check that date dateTimePickerNewCutOff.Value is as expected
                DateTime NextCutOffCalc;
                DateTime TempWorking = Rjc.Cut_Off_Date;
                if (WOperator == "ETHNCY2N")
                {
                    bool Possible = false;

                    // Make Check for Ethniki 
                    // Find characteristics of the next to cut off
                    

                    NextCutOffCalc = Ho.GetNextSecondWorkingDt_NBG(WOperator, TempWorking, "Hol Version01-Standard");

                    if (dateTimePickerNewCutOff.Value != NextCutOffCalc)
                    {
                        
                        MessageBox.Show("NOT CORRECT NEW CUT-OFF" + Environment.NewLine
                                  + "RRDM says correct one is.. " + NextCutOffCalc.Date.ToShortDateString() + Environment.NewLine
                                  + "Check that holidays are correctly defined in RRDM and repeat" + Environment.NewLine
                                 , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; 
                    }
                }
                string ParId = "102";
                string OccurId = "1" ; // Check if Cycle Date against Hollydays 
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                if (Gp.RecordFound & Gp.OccuranceNm == "YES")
                {
                
                    // GET DATE AFTER THE Current 
                    DateTime NextWorkingDate = Ho.GetNextSecondWorkingDt(WOperator,TempWorking, "Hol Version01-Standard");

                    if (dateTimePickerNewCutOff.Value.Date == NextWorkingDate)
                    {
                        // OK 
                    }
                    else
                    {
                        // 
                        if (MessageBox.Show("Warning: Input Cycle is not next Working date." + Environment.NewLine
                           + "Next working date is.." + NextWorkingDate.ToShortDateString()
                           , "Do you want to proceed?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                        {
                            // YES Proceed
                        }
                        else
                        {
                            // Stop 
                            return;
                        }

                    }
                }
            }

            //Rc.ReadReconcCategoriesForMatrix(WOperator, TempReconcCycleNo);

            //WLatestStatus = Rc.StatusCycle1;

            //if (WLatestStatus == "Not Done")
            //{
            //    MessageBox.Show("Latest Cycle Not Matched yet! " + Environment.NewLine
            //             + " ...........................");
            //   // return; 
            //}
           
            // Verification Message 
            if (MessageBox.Show("Do you want to Start a New Cycle?" + Environment.NewLine
                               + "For " + dateTimePickerNewCutOff.Value.DayOfWeek + " Date : .. .." + dateTimePickerNewCutOff.Value.ToShortDateString()
                               , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                // YES Proceed
            }
            else
            {
                // Stop 
                return;
            }
            //
            // NEW CYCLE
            //
            int NewReconcCycleNo = 0;
            if (WJobCategory == "ATMs")
            {
                NewReconcCycleNo = Rjc.Create_A_New_ReconcJobCycle(WOperator, WSignedId, dateTimePickerNewCutOff.Value);
            }
            else
            {
                NewReconcCycleNo = Rjc.Create_A_New_ReconcJobCycle_MOBILE(WJobCategory, WOperator, WSignedId, dateTimePickerNewCutOff.Value);
            }

            OldReconcJobCycle = Rjc.OldReconcJobCycle;

            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "Create_New_Cycle";
            Message = "New Cycle Created with number.. " + NewReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, NewReconcCycleNo);
            //******************************
            //******************************
            //
            // DO THIS AFTER THE NEW CYCLE IS CREATED 
            //
            bool ZipDirectory = false;
            string ParamId = "951";
            string OccuranceId = "1"; // ZIP DIRECTORY 

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    ZipDirectory = true;
                }
            }

            if (ZipDirectory == true & ReversedCut_Off_Date != "")
            {
                try
                {
                    //string sourceDir = @"C:\RRDM\FilesArchives\20250306_200"; // directory to zip
                    //string zipFilePath = @"C:\RRDM\FilesArchives\20250306_200.zip"; // output file

                    // Overwrite if exists (optional)
                    if (File.Exists(zipFilePath))
                    {
                        File.Delete(zipFilePath);
                    }

                    ZipFile.CreateFromDirectory(sourceDir, zipFilePath, CompressionLevel.Fastest, includeBaseDirectory: false);

                    MessageBox.Show("ZIP DONE for " + sourceDir);
                    // Small delay to let OS release handles (optional but sometimes helps)
                    Thread.Sleep(100);

                    // Delete directory
                    try
                    {
                        Directory.Delete(sourceDir, recursive: true);
                        // Console.WriteLine("Folder deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"Failed to delete folder: {ex.Message}");
                        MessageBox.Show($"Failed to delete folder: {ex.Message}");
                    }

                    // File.Delete(sourceDir); 
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Cancel at creating Zip: {ex.Message}");

                }
            }

            // UNZIPPED 
          

            Form200JobCycles_Load(this, new EventArgs());
            
        }
        // Message from Controller 
        private void buttonMsgs_Click_1(object sender, EventArgs e)
        {
            NForm55 = new Form55(MsgFilter, WSignedId);
            NForm55.ShowDialog();

            MsgFilter =
                 "(ReadMsg = 0 AND ToAllAtms = 1)"
             + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";


            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(buttonMsgs, messagesStatus);
                toolTipMessages.ShowAlways = true;
            }
            else
            {
                toolTipMessages.SetToolTip(buttonMsgs, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }
        }
        // Todays Controller 
        private void buttonCommController_Click_1(object sender, EventArgs e)
        {
            NForm54 = new Form54(WSignedId, WSignRecordNo, WOperator);
            NForm54.ShowDialog();
        }
        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
       
        bool linkLabel1Alert, linkLabel2Alert, linkLabel3Alert, linkLabel4Alert
            , linkLabel5Alert, linkLabel6Alert, linkLabel7Alert, linkLabel8Alert;

        // SHOW DETAILS
        private void buttonShowDetails_Click(object sender, EventArgs e)
        {
            
            Color Red = Color.Red;

            int TempCycle = 0;
            // VALIDATION 
          
            if (int.TryParse(textBoxReconcCycle.Text, out TempCycle))
            {
                Rjc.ReadReconcJobCyclesById(WOperator, TempCycle);
                if (Rjc.RecordFound == false)
                {
                    MessageBox.Show("Reconc Cycle not Found! Please enter a valid one!");
                    panel3.Hide();
                    buttonOutstandings.Hide();
                    return;
                }         
            }
            else
            {
                MessageBox.Show("Please enter a valid Reconc Cycle!");
                panel3.Hide();
                buttonOutstandings.Hide();
                return;
            }

            panel3.Show();
            //buttonOutstandings.Show(); 

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, TempCycle);
            if (Rcs.RecordFound == true)
            {
                // Initialised
                labelCat1.Hide();
                labelNm1.Hide();
                linkLabel1.Hide();

                labelCat2.Hide();
                labelNm2.Hide();
                linkLabel2.Hide();

                labelCat3.Hide();
                labelNm3.Hide();
                linkLabel3.Hide();

                labelCat4.Hide();
                labelNm4.Hide();
                linkLabel4.Hide();

                labelCat5.Hide();
                labelNm5.Hide();
                linkLabel5.Hide();

                labelCat6.Hide();
                labelNm6.Hide();
                linkLabel6.Hide();

                labelCat7.Hide();
                labelNm7.Hide();
                linkLabel7.Hide();

                labelCat8.Hide();
                labelNm8.Hide();
                linkLabel8.Hide();

                if (Rcs.MatchingCat01 != "")
                {
                    if (Rcs.MatchingCat01Updated == false)
                    {
                        labelCat1.ForeColor = Red;
                        labelNm1.ForeColor = Red;
                        linkLabel1Alert = true;
                    }
                    else
                    {
                        labelCat1.ForeColor = Color.Black;
                        labelNm1.ForeColor = Color.Black;
                        linkLabel1Alert = false;
                    }
                    labelCat1.Show();
                    labelNm1.Show();
                    linkLabel1.Show();
                    labelNm1.Text = Rcs.MatchingCat01;
                }
                if (Rcs.MatchingCat02 != "")
                {
                    if (Rcs.MatchingCat02Updated == false)
                    {
                        labelCat2.ForeColor = Red;
                        labelNm2.ForeColor = Red;
                        linkLabel2Alert = true;
                    }
                    else
                    {
                        labelCat2.ForeColor = Color.Black;
                        labelNm2.ForeColor = Color.Black;
                        linkLabel2Alert = false;
                    }
                    labelCat2.Show();
                    labelNm2.Show();
                    linkLabel2.Show();
                    labelNm2.Text = Rcs.MatchingCat02;
                }

                if (Rcs.MatchingCat03 != "")
                {
                    if (Rcs.MatchingCat03Updated == false)
                    {
                        labelCat3.ForeColor = Red;
                        labelNm3.ForeColor = Red;
                        linkLabel3Alert = true;
                    }
                    else
                    {
                        labelCat3.ForeColor = Color.Black;
                        labelNm3.ForeColor = Color.Black;
                        linkLabel3Alert = false;
                    }

                    labelCat3.Show();
                    labelNm3.Show();
                    linkLabel3.Show();
                    labelNm3.Text = Rcs.MatchingCat03;
                }
                if (Rcs.MatchingCat04 != "")
                {
                    if (Rcs.MatchingCat04Updated == false)
                    {
                        labelCat4.ForeColor = Red;
                        labelNm4.ForeColor = Red;
                        linkLabel4Alert = true;
                    }
                    else
                    {
                        labelCat4.ForeColor = Color.Black;
                        labelNm4.ForeColor = Color.Black;
                        linkLabel4Alert = false;
                    }

                    labelCat4.Show();
                    labelNm4.Show();
                    linkLabel4.Show();
                    labelNm4.Text = Rcs.MatchingCat04;
                }
                if (Rcs.MatchingCat05 != "")
                {
                    if (Rcs.MatchingCat05Updated == false)
                    {
                        labelCat5.ForeColor = Red;
                        labelNm5.ForeColor = Red;
                        linkLabel5Alert = true;
                    }
                    else
                    {
                        labelCat5.ForeColor = Color.Black;
                        labelNm5.ForeColor = Color.Black;
                        linkLabel5Alert = false;
                    }

                    labelCat5.Show();
                    labelNm5.Show();
                    linkLabel5.Show();
                    labelNm5.Text = Rcs.MatchingCat05;
                }
                if (Rcs.MatchingCat06 != "")
                {
                    if (Rcs.MatchingCat06Updated == false)
                    {
                        labelCat6.ForeColor = Red;
                        labelNm6.ForeColor = Red;
                        linkLabel6Alert = true;
                    }
                    else
                    {
                        labelCat6.ForeColor = Color.Black;
                        labelNm6.ForeColor = Color.Black;
                        linkLabel6Alert = false;
                    }

                    labelCat6.Show();
                    labelNm6.Show();
                    linkLabel6.Show();
                    labelNm6.Text = Rcs.MatchingCat06;
                }

                if (Rcs.MatchingCat07 != "")
                {
                    if (Rcs.MatchingCat07Updated == false)
                    {
                        labelCat7.ForeColor = Red;
                        labelNm7.ForeColor = Red;
                        linkLabel7Alert = true;
                    }
                    else
                    {
                        labelCat7.ForeColor = Color.Black;
                        labelNm7.ForeColor = Color.Black;
                        linkLabel7Alert = false;
                    }

                    labelCat7.Show();
                    labelNm7.Show();
                    linkLabel7.Show();
                    labelNm7.Text = Rcs.MatchingCat07;
                }

                if (Rcs.MatchingCat08 != "")
                {
                    if (Rcs.MatchingCat08Updated == false)
                    {
                        labelCat8.ForeColor = Red;
                        labelNm8.ForeColor = Red;
                        linkLabel8Alert = true;
                    }
                    else
                    {
                        labelCat8.ForeColor = Color.Black;
                        labelNm8.ForeColor = Color.Black;
                        linkLabel8Alert = false;
                    }

                    labelCat8.Show();
                    labelNm8.Show();
                    linkLabel8.Show();
                    labelNm8.Text = Rcs.MatchingCat08;
                }
            }
            else
            {
                MessageBox.Show("There is no information for this Reconc Category / Cycle "); 
            }
        }

        RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();
        // Show Matching Files 

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel1Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }

            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm1.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm1.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonViewFiles_Click(object sender, EventArgs e)
        {
            if (WJobCycleNo == 0)
            {
                MessageBox.Show("RM Cycle No is zero!");
                return; 
            }

            Form18_LoadedFilesStatus NForm18_LoadedFilesStatus;

            int Mode = 12; // ALL
            
            NForm18_LoadedFilesStatus = new Form18_LoadedFilesStatus(WSignedId, WSignRecordNo, WOperator, WJobCycleNo, WCut_Off_Date, Mode);
            NForm18_LoadedFilesStatus.ShowDialog();
        }
// Delete Cycle 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // THE BELOW IS TRUE 
            //if (dataGridView2.Rows.Count == CountNotDone) buttonDelete.Show();
            //else buttonDelete.Hide();
            // Make Validation if loaded files for this cycle
            Flog.ReadLoadedFilesByCycleNumber_All(WJobCycleNo);
            if (Flog.RecordFound == true)
            {
                MessageBox.Show("Files Or Journals were loaded with this Cycle." + Environment.NewLine
                                + "You cannot delete this cycle unless you UNDO the loading" + Environment.NewLine
                               );
                return;
            }

            if (MessageBox.Show("Warning: Do you want to Delete this New Cycle?" + Environment.NewLine
                             + "For " + Rjc.Cut_Off_Date.DayOfWeek + " Date : .. .." + Rjc.Cut_Off_Date.ToShortDateString()
                             , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                      == DialogResult.Yes)
            {
                // YES Proceed
            }
            else
            {
                // Stop 
                return;
            }

            
            if (WJobCategory == "ATMs")
            {
                // RRDMJournalReadTxns_Text_Class Ed = new RRDMJournalReadTxns_Text_Class();
                RRDM_EXCEL_AND_Directories Ed = new RRDM_EXCEL_AND_Directories();

                RRDMGasParameters Gp = new RRDMGasParameters();

                // DELETE JOURNALS

                string ParId = "920";
                string OccurId = "12"; // C:\RRDM\FilePool\Atms_Journals_Txns\

                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                string WJournalDirectory = Gp.OccuranceNm;

                if (Directory.Exists(WJournalDirectory))
                {

                    string[] allFiles_2 = Directory.GetFiles(WJournalDirectory, "*.*");
                    if (allFiles_2.Length >= 0)
                    {
                        // bool InWithCopy, string InCopyDestination)
                        // MessageBox.Show("Number of files in directory " + allFiles_2.Length);

                        bool WithCopy = false; // Copy back to the origin 

                        Ed.DeleteFilesWithinDirectory(WJournalDirectory);

                        //  ProgressText += DateTime.Now + "_" + "Archive Directory Deleted with File Number..." + allFiles_2.Length.ToString() + "\r\n";
                    }
                }
                else
                {
                    MessageBox.Show("This Directory does not exist." + Environment.NewLine
                                   + WJournalDirectory
                                    );
                }

            }

            Rjc.DeleteCycleNo(WOperator, WJobCycleNo, WJobCategory);

            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "Delete Cycle";
            Message = "Cycle was Deleted with number.. " + WJobCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WJobCycleNo);
            //******************************

            Form200JobCycles_Load(this, new EventArgs());

        }

        private void buttonERRORS_Click(object sender, EventArgs e)
        {
            if (WJobCycleNo == 0)
            {
                MessageBox.Show("RM Cycle No is zero!");
                return;
            }

            Form18_LoadedFilesStatus NForm18_LoadedFilesStatus;

            int Mode = 0; // only the errors

            NForm18_LoadedFilesStatus = new Form18_LoadedFilesStatus(WSignedId, WSignRecordNo, WOperator, WJobCycleNo, WCut_Off_Date, Mode);
            NForm18_LoadedFilesStatus.ShowDialog();
        }

        // View Pending For Matching
        private void buttonOutstandings_Click_1(object sender, EventArgs e)
        {
            string WMatchingCateg ="";

            Form78d_AllFiles_Pending NForm78d_AllFiles_Pending;
            // textBoxFileId.Text
            int WMode = 2; //
                           // InMode = 1 : Not processed yet 
                           // InMode = 2 : Processed this Cycle

            NForm78d_AllFiles_Pending = new Form78d_AllFiles_Pending(WOperator, WSignedId, WMatchingCateg );
            NForm78d_AllFiles_Pending.Show();
           
        }
// Create file
        private void buttonCreateFile_Click(object sender, EventArgs e)
        {
            // Based on conditions read next 1 and fill a table line
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            
            // read records of output fie and fill up table
            // Loop and create a line
            // Insert line in file  
        }
        // Not Settled transactions 
        private void buttonNotSettled_Click(object sender, EventArgs e)
        {
          
            Form78d_FileRecords NForm78d_FileRecords;
            // textBoxFileId.Text
            int WMode = 5; //
                           // Not Action Taken by Maker
                           // 
            bool WCategoryOnly = false;

            string WTableId = "Atms_Journals_Txns";

            NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, WTableId, "", WJobCycleNo, "", WMode, WCategoryOnly);
            NForm78d_FileRecords.Show();
        }
// 
       

        private void buttonCUT_OFF_SUMMARY_Click(object sender, EventArgs e)
        {
            // SHOW CUT OFF GL SUMMARY
            //
            // Mode 6 are 
            Form78d_CUT_OFF_GL NForm78d_CUT_OFF_GL;

            int WMode = 6; // CUT OFF GL Summary for Categories
            string WMatchingCateg = "";
            string WAtmNo = ""; 
            NForm78d_CUT_OFF_GL = new Form78d_CUT_OFF_GL(WOperator, WSignedId, WCut_Off_Date, WMatchingCateg, WAtmNo
                           , WJobCycleNo, WMode);
            NForm78d_CUT_OFF_GL.ShowDialog();
        }


        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel2Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm2.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm2.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel3Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm3.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm3.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel4Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm4.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm4.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel5Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm5.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm5.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel6Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm6.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm6.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel7Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm7.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm7.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel8Alert == true)
            {
                MessageBox.Show("Please note that the matching of this Matching Category was not Done.");
            }
            Form2Grid NForm2Grid;
            string Header = "Matching Files For: " + labelNm8.Text;

            string SelectionCriteria = " WHERE CategoryId ='" + labelNm8.Text + "'";
            Mcf.ReadReconcCategoryVsSourcesANDFillTableofFiles(SelectionCriteria);

            NForm2Grid = new Form2Grid(Header, Mcf.RMCategoryFilesDataFiles);
            NForm2Grid.Show();
        }
        // Show Details
    }
}
