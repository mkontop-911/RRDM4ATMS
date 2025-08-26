using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMsWin
{
    public partial class Form502ArchiveCycles : Form
    {

        // The process of Archiving Goes as follows 
        // A new SQL SERVER INSTANCE is created
        // Production Data Bases are restored in the new instance => this is the Archive Server Data Bases
        // RRDM Executable is renamed and kept => This is the executable encapsulated with the Data Bases
        // Configuration is changed to have connected strings directed to new instance
        // A new Archiving cycle is created with defined from to dates boundaries 
        // Delele all data not within the date Boundaries
        // .... Check that eveything is working nicely
        // After check you that is well
        // Back production - save it for one year
        // Delete Archived records from production   

        RRDMArchiveCycles Ar = new RRDMArchiveCycles();

        RRDMGasParameters Gp = new RRDMGasParameters();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        //DateTime WDateTime;

        //string WSelectionCriteria;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WMode;

        public Form502ArchiveCycles(string InSignedId, int InSignRecordNo, string InOperator, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WMode = InMode; // 1 = view, 2 = Update

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;



            if (WMode == 1)
            {
                label2.Hide();
                labelHeadingOfSteps.Hide();
                panel3.Hide();
                panel4.Hide();
            }
            if (WMode == 2)
            {
                label2.Show();
                labelHeadingOfSteps.Show();
                panel3.Show();
                panel4.Show();
            }

        }
        // Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 
            Ar.ReadLastReconcArchiveCycle(WOperator);

            if (Ar.RecordFound == true)
            {
                dateTimePicker1.Enabled = false;
                if (Ar.Status == 4)
                {
                    dateTimePicker1.Value = Ar.ToDateTm.AddDays(1).Date;
                    dateTimePicker2.Value = DateTime.Now.Date;
                }
                else
                {
                    dateTimePicker1.Value = Ar.FromDateTm;
                    dateTimePicker2.Value = Ar.ToDateTm;
                    textBoxExec.Text = Ar.Executable;

                    textBoxSQLServer.Text = Ar.SQLInstance;

                    int temp = Ar.ReadNumberOfRecordsInPoolLessThan(WOperator, Ar.ToDateTm, 1, 0);

                    textBoxProdToArchive.Text = temp.ToString();

                    temp = Ar.ReadNumberOfRecordsInPoolGreaterThan(WOperator, Ar.ToDateTm, 1, 0);

                    textBoxReaminInProd.Text = temp.ToString();
                }
            }
            else
            {
                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }

            textBoxProdFromDate.Text = dateTimePicker1.Value.ToShortDateString();
            textBoxProdToDate.Text = DateTime.Now.ToShortDateString();

            TimeSpan difference = DateTime.Now - dateTimePicker1.Value;
            textBox5.Text = difference.TotalDays.ToString();

            int TotalTxns = Ar.ReadNumberOfRecordsInPoolLessThan(WOperator, DateTime.Now.Date, 1, 0);

            textBoxTotalRecords.Text = TotalTxns.ToString();

            if (Ar.Status == 4) // Complete Cycle - All updatings done
            {
                labelHeadingOfSteps.Text = "PROCESS TO ADD NEW CYCLE";
                buttonAddNewArchive.ForeColor = Color.Red;
                buttonAddNewArchive.Enabled = true;
                buttonDeleteInArchive.Enabled = false;
                buttonDeleteInProduction.Enabled = false;
                buttonDeleteCycle.Hide();
                buttonCheckArchive.Hide();
               
            }
            if (Ar.Status == 1) // Cycle in process - just started
            {
                labelHeadingOfSteps.Text = "NEW CYCLE PROCESS NOT COMPLETED";
                buttonAddNewArchive.Enabled = false;
                buttonDeleteInArchive.ForeColor = Color.Red;
                buttonDeleteInProduction.Enabled = false;
                buttonDeleteCycle.Show();
                buttonCheckArchive.Show();
               
            }
            if (Ar.Status == 2) // Cycle in process - Data Bases created 
            {
                labelHeadingOfSteps.Text = "NEW CYCLE PROCESS NOT COMPLETED";
                buttonAddNewArchive.Enabled = false;
                buttonDeleteInArchive.ForeColor = Color.Red;
                buttonDeleteInProduction.Enabled = false;
                buttonDeleteCycle.Show();
                buttonCheckArchive.Show();         
            }
            if (Ar.Status == 3) // Cycle in process - Records Deleted from Archiving 
            {
                labelHeadingOfSteps.Text = "NEW CYCLE PROCESS NOT COMPLETED";
                buttonAddNewArchive.Enabled = false;
                buttonDeleteInArchive.Enabled = false;
                buttonDeleteInProduction.ForeColor = Color.Red;
                buttonDeleteCycle.Hide();
                buttonCheckArchive.Hide();              
            }

            Ar.ReadArchiveCyclesFillTable(WOperator);

            ShowGrid1();

        }


        // Row Enter 
        int WSeqNo;

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Ar.ReadLastReconcArchiveCycle(WOperator);
            if (Ar.Status < 4 )
            {
                panel6.Show(); 
            }

            labelCycle.Text = "Archiving CycleNo : " + WSeqNo.ToString();

            Ar.ReadArchiveCyclesById(WOperator, WSeqNo);

            labelID.Text = "Cycle Id............." + Ar.SeqNo.ToString();
            labelDesc.Text = "Description.........." + Ar.Description;
            labelFrom.Text = "From Date............" + Ar.FromDateTm.ToShortDateString();
            labelTo.Text = "To Date.............." + Ar.FromDateTm.ToShortDateString();
            labelExeDir.Text = "Exec Directory......." + Ar.Executable;
            labelSQLServer.Text = "SQL Server Instance.." + Ar.SQLInstance;
            if (Ar.Status == 4)
            {
                labelAlertStatus.Hide();
                buttonViewArchive.Show();
            }
            else
            {
                labelAlertStatus.Show();
                buttonViewArchive.Hide();
            }

        }

        // NEW ARCHIVE
        private void buttonAddNewArchive_Click(object sender, EventArgs e)
        {
            // Read last and warn

            // Create new 
            DateTime WDateFrom = dateTimePicker1.Value.Date;
            DateTime WDateTo = dateTimePicker2.Value.Date;

            if (WDateTo <= WDateFrom)
            {
                MessageBox.Show("To Date less of Equal to From Date");
                return;
            }

            Ar.FromDateTm = WDateFrom;
            Ar.ToDateTm = WDateTo;

            Ar.Description = "Archiving Cycle from.." + WDateFrom.ToLongDateString() + "..TO.." + WDateTo.ToLongDateString();

            Ar.DateCreated = DateTime.Now;
            Ar.RecordsInMaster = 0;

            Ar.Executable = "";

            Ar.SQLInstance = "";

            Ar.Status = 1;

            Ar.UserId = WSignedId;

            Ar.Operator = WOperator;

            int NewCycle = Ar.InsertNewArchiveCycle();

            string DatestringFrom = DateConversion(WDateFrom);
            string DatestringTo = DateConversion(WDateTo);

            string TrailerNm = "Cycle_" + NewCycle.ToString()
                + "_Dates_" + DatestringFrom + "To" + DatestringTo;
            // Directory for Archiving 
            string ParId = "920";
            string OccurId = "8";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            string ArchiveDir = Gp.OccuranceNm + TrailerNm;

            Ar.Executable = ArchiveDir + "RRDM4ATMsWin.exe";


            // SQL Server exact name
            ParId = "920";
            OccurId = "9";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            Ar.SQLInstance = Gp.OccuranceNm + TrailerNm;
            //
            // Read items in production directory
            //
            ParId = "920";
            OccurId = "10";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            string ProductionDir = Gp.OccuranceNm;

            // Create directory and copy files 
            try
            {
                //Directory.CreateDirectory(Ar.Executable);

                string sourcePath = ProductionDir;

                string targetPath = ArchiveDir;


                if (!Directory.Exists(targetPath))

                {
                    Directory.CreateDirectory(targetPath);

                }

                string[] sourcefiles = Directory.GetFiles(sourcePath);

                foreach (string sourcefile in sourcefiles)

                {
                    string fileName = Path.GetFileName(sourcefile);

                    if (fileName == "RRDM4ATMsWin.exe.config")
                    {
                        // change contents of sourcefile to target the correct SQL Server
                        // Which name is in "Ar.SQLInstance"
                        MessageBox.Show("Change configuration file to target the archive SQL instance");
                    }

                    string destFile = Path.Combine(targetPath, fileName);

                    File.Move(sourcefile, destFile);

                }


            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

            Ar.Status = 1;

            Ar.UserId = WSignedId;

            Ar.UpdateArchiveCycle_1(NewCycle);

            // Move from exec from production to new directory

            if (NewCycle > 0)
            {
                MessageBox.Show("New Archive Cycle Created" + Environment.NewLine
                                + "NEXT PLEASE DO : " + Environment.NewLine
                                + "1) Create SQL Server Instance and Restore Production DataBases" + Environment.NewLine
                                + "2) Press Delete Records In Archive " + Environment.NewLine
                                + "3) Press Delete Records In Production " + Environment.NewLine
                                + " ... Ready to go ..." + Environment.NewLine
                                );

                panel6.Show();
            }

          

            //int WRowGrid1 = dataGridView1.SelectedRows[0].Index;

            Form502_Load(this, new EventArgs());

            //dataGridView1.Rows[WRowGrid1].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid1));


        }

        //
        // print cycle migrated 
        //
        private void buttonPrintCycle_Click(object sender, EventArgs e)
        {

            string P1 = "Migrated ATMs Results ";

            string P2 = WSeqNo.ToString();
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R58ATMS ReportATMS58 = new Form56R58ATMS(P1, P2, P3, P4, P5);
            ReportATMS58.Show();

        }
        // Date 
        private string DateConversion(DateTime WDate)
        {
            string WDay = "";
            string WMonth = "";
            if (WDate.Day < 10)
            {
                WDay = "0" + WDate.Day;
            }
            else WDay = WDate.Day.ToString();

            if (WDate.Month < 10)
            {
                WMonth = "0" + WDate.Month;
            }
            else WMonth = WDate.Month.ToString();


            string DateString = WDay + "." + WMonth + "." + WDate.Year; // eg ("17.11.2017")
            return DateString;
        }

        // FINISH
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        // Delete Cycle
        private void buttonDeleteCycle_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this UnComplete Cycle ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                Ar.DeleteArchiveCycle(WSeqNo);
                Form502_Load(this, new EventArgs());
                panel6.Hide();
            }
            else
            {
                return;
            }
        }
        // Delete Extra In Archive
        private void buttonDeleteInArchive_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete the Extra Archive Records ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                      == DialogResult.Yes)
            {
                // Yes
                int DeletedRecords = Ar.DeleteArchiveCycleExtraRecords(WSeqNo, Ar.ToDateTm.Date);
                MessageBox.Show(DeletedRecords.ToString() + " Records Are Deleted");
                // Update Cycle


                Ar.DateRecordsDeleted = DateTime.Now;
                Ar.DeletedMasterRecords = DeletedRecords;
                Ar.Status = 3;
                Ar.UserId = WSignedId;

                Ar.UpdateArchiveCycle_2(WSeqNo);
            }
            else
            {
                // No 
                return;
            }

        }
        // Delete Extra in Production
        private void buttonDeleteInProduction_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Warning: Do you want to delete the Extra Production Records ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                     == DialogResult.Yes)
            {
                // Yes
                int DeletedRecords = Ar.DeleteProductionExtraRecords(WOperator, Ar.ToDateTm.Date);
                MessageBox.Show(DeletedRecords.ToString() + " Records Are Deleted In Production");
                // Update Cycle

                Ar.DateRecordsDeleted = DateTime.Now;
                Ar.DeletedMasterRecords = DeletedRecords;
                Ar.Status = 4;
                Ar.UserId = WSignedId;

                Ar.UpdateArchiveCycle_2(WSeqNo);
            }
            else
            {
                // No 
                return;
            }

        }
        // Check Archive
        private void buttonCheckArchive_Click(object sender, EventArgs e)
        {
            //var proc = new Process();
            //proc.StartInfo.FileName = "something.exe";
            //proc.StartInfo.Arguments = "-v -s -a";
            //proc.Start();
            //proc.WaitForExit();
            //var exitCode = proc.ExitCode;
            //proc.Close();
        }
        // View Archive
        private void buttonViewArchive_Click(object sender, EventArgs e)
        {
            //var proc = new Process();
            //proc.StartInfo.FileName = "something.exe";
            //proc.StartInfo.Arguments = "-v -s -a";
            //proc.Start();
            //proc.WaitForExit();
            //var exitCode = proc.ExitCode;
            //proc.Close();
            MessageBox.Show("TESTING DATA NOT AVAILABLE YET!"); 
        }

        // Show Grid 
        private void ShowGrid1()
        {

            dataGridView1.DataSource = Ar.TableArchiveCycles.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Cycles Available!");
                return;
            }

            dataGridView1.Columns[0].Width = 60; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = true;

            dataGridView1.Columns[1].Width = 250; // Descr
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = true;

            dataGridView1.Columns[2].Width = 100; // Status
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].Visible = true;

            dataGridView1.Columns[3].Width = 90; // From date 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = true;

            dataGridView1.Columns[4].Width = 90; // To date
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].Visible = true;

            dataGridView1.Columns[5].Width = 90; // 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].Visible = true;

            dataGridView1.Columns[6].Width = 90; // 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].Visible = true;

            dataGridView1.Columns[7].Width = 90; // 
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[8].Width = 90; // 
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[8].Visible = true;

            dataGridView1.Columns[9].Width = 100; // 
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[9].Visible = true;

        }


        // Catch Details
        private static void CatchDetails(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");

            // Environment.Exit(0);
        }
    }
}
