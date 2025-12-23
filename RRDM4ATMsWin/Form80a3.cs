using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

using System.Data;
using System.Text;
//using System.Windows.Forms;
//using System.Globalization;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80a3 : Form
    {
       
        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        //string WMainCateg;
        int WReconcCycleNo;
        string W_Application; 

        int WRowIndexLeft;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;
        string WRMCategory;
        string WhatBankId;
        public Form80a3(string InSignedId, int SignRecordNo, string InOperator, 
                                                      string InFunction, string InRMCategory, string InWhatBankId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // Reconc , View

            WRMCategory = InRMCategory; // Specific or "ALL"
            WhatBankId = InWhatBankId;

            InitializeComponent();

            // Set Working Date 
         
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            label1UserId.Text = WSignedId;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.RecordFound == true)
            {
                W_Application = Usi.SignInApplication;

                if (W_Application == "e_MOBILE")
                {
                    if (Usi.ProcessNo == 1)
                    {
                        W_Application = "ETISALAT";
                    }
                    if (Usi.ProcessNo == 2)
                    {
                        W_Application = "QAHERA";
                    }
                    if (Usi.ProcessNo == 3)
                    {
                        W_Application = "IPN";
                    }
                    if (Usi.WFieldNumeric11 == 15)
                    {
                        W_Application = "EGATE";
                    }
                    labelStep1.Text = "Category Menu_" + W_Application;
                }
                else
                {
                    W_Application = "ATMs";
                    labelStep1.Text = "Category Menu-_" + W_Application;
                }
            }

            }

        // ON LOAD
        private void Form80a2_Load(object sender, EventArgs e)
        {
            try
            {
                //Rc.ReadReconcCategoriesAndFillTableWithDiscrepancies(WOperator, WSignedId);
               
                Mc.ReadMatchingCategoriesAndFillTableInDetail(WOperator, W_Application);
                dataGridView1.DataSource = Mc.TableMatchingCateg.DefaultView;

                if (dataGridView1.Rows.Count == 0)
                {
                    Form2 MessageForm = new Form2("No Entries available");
                    MessageForm.ShowDialog();

                    this.Dispose();
                    return;
                }

                dataGridView1.Columns[0].Width = 60; // SeqNo
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[0].Visible = false;

                dataGridView1.Columns[1].Visible = false; // Select 
                
                dataGridView1.Columns[2].Width = 60; // CategoryId
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[3].Width = 240; // Category-Name
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[4].Visible = false; // Is POS_Type
                dataGridView1.Columns[5].Visible = false; // Days W
                dataGridView1.Columns[6].Visible = false; // Days C
                dataGridView1.Columns[7].Visible = false; // Is TWIN

                dataGridView1.Columns[8].Width = 120; // File_A
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[9].Width = 120; // File_B
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[10].Width = 120; // File_C
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[11].Visible = false; // MathingDone

                dataGridView1.Columns[12].Visible = false; // Assigned-To
            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }
                
        }
        // Row Enter for Datagridview1
        string WCategoryId;
        string WOrigin;
        //RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WCategoryId = rowSelected.Cells[2].Value.ToString();

            //string SelectionCriteria = " WHERE Operator='" + WOperator + "' AND JobCategory ='ATMs'";
            //string SelectionCriteria = " WHERE Operator='" + WOperator + "'";
            Rjc.ReadReconcJobCyclesFillTable_2(WCategoryId, W_Application);

            dataGridView2.DataSource = Rjc.TableReconcJobCycles.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //buttonViewFiles.Hide();
                //buttonERRORS.Hide();
                return;
            }
            else
            {
                //dataGridView1.Show(); 
                //buttonViewFiles.Show();

            }

            ShowGrid1();         
        }

        private void ShowGrid1()
        {
            
            dataGridView2.Columns[0].Width = 60; // JobCycle;
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[1].Width = 200; // JobCategory;
            dataGridView2.Columns[1].Visible = false;

            dataGridView2.Columns[2].Width = 120; // Cut_Off_Date
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 130; // StartDateTm
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[4].Width = 130; //  FinishDateTm
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[5].Width = 90; // "MathcNo"
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        // Row enter for Datagridview2
         
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            WReconcCycleNo = (int)rowSelected.Cells[0].Value;
        }
        // NEXT 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

            //if (Tuc.TotalATMsReady == 0)
            //{
            //    MessageBox.Show("No Atms ready to reconcile");
            //    return;
            //} 

            if (WFunction == "View" & WOperator != "ITMX")
            {
                WRowIndexLeft = dataGridView1.SelectedRows[0].Index;

                Form80b3 NForm80b3;

                WFunction = "View";

                int Type = 16; 

                if (W_Application == "ETISALAT" || W_Application == "QAHERA" || W_Application == "EGATE")
                {
                    // e_MOBILE
                    Form80b3_MOBILE NForm80b3_MOBILE; 
                    NForm80b3_MOBILE = new Form80b3_MOBILE(WSignedId, WSignRecordNo, WOperator,W_Application ,NullPastDate, NullPastDate, "", WCategoryId, WReconcCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
                    NForm80b3_MOBILE.FormClosed += NForm80b3_FormClosed;
                    NForm80b3_MOBILE.ShowDialog();
                }
                else
                {
                    // ATMS AND CARDS
                    NForm80b3 = new Form80b3(WSignedId, WSignRecordNo, WOperator, NullPastDate, NullPastDate, "", WCategoryId, WReconcCycleNo, "", 0, Type, WFunction, "", 0, NullPastDate, "", "");
                    NForm80b3.FormClosed += NForm80b3_FormClosed;
                    NForm80b3.ShowDialog();
                }

                
            }

            

        }
        private void NForm80b3_FormClosed(object sender, FormClosedEventArgs e)
        {
            WFunction = "View";

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form80a2_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

      
        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Dispose();
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

            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                         + " . Application will be aborted! Call controller to take care. ");
            }
        }

      
    }
}
