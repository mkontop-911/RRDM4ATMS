using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_DepositedNotes : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMDepositsClass Da = new RRDMDepositsClass();
        RRDMRepl_SupervisorMode_Details SM = new RRDMRepl_SupervisorMode_Details();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); 

        //int WSeqNo;

        DateTime WFromDt;
        DateTime WToDt;

        string WOperator;
        string WSignedId;
        string WAtmNo;
        int WReplCycle;
        int WMode; 
       
        public Form78d_DepositedNotes(string InOperator , string InSignedId, string InAtmNo, int InReplCycle, int InMode)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WAtmNo = InAtmNo;
            WReplCycle = InReplCycle;
            WMode = InMode; // 1 : File : Deposit_Txns_Analysis
                            // 2 : Same Analysis but from SM
                            // 3 : View Forex Child 
                            // 4 : Same as 2 but for recycle ATMs 

            InitializeComponent();

            Ta.ReadSessionsStatusTraces(WAtmNo, WReplCycle);

            WFromDt = Ta.SesDtTimeStart;
            WToDt = Ta.SesDtTimeEnd;
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            if (WMode == 1)
            {
                Da.ReadDepositsAndGet_Table_With_Notes(WAtmNo, WReplCycle);
                dataGridView1.DataSource = Da.DataTableNotes.DefaultView;
                // SHOW GRID
                ShowGrid01();

                Da.ReadDepositsAndGet_Table_With_TotalsByCurrency(WAtmNo, WReplCycle);
                dataGridView2.DataSource = Da.DataTableTotals.DefaultView;

                ShowGrid02();
            }

            if (WMode == 2)
            {
                // Get the totals from SM and not from Mpa            
                // GET TABLE
                SM.Read_SM_AND_FillTable_Deposits_2(WAtmNo, WReplCycle);
                dataGridView1.DataSource = SM.DataTable_SM_Deposits_2.DefaultView;

                if (SM.DataTable_SM_Deposits_2.Rows.Count >  0)
                {
                    // SHOW GRID
                    ShowGrid01();
                }
                

                //Da.ReadDepositsAndGet_Table_With_TotalsByCurrency(WAtmNo, WReplCycle);
                //dataGridView2.DataSource = Da.DataTableTotals.DefaultView;

                //ShowGrid02();
            }

            if (WMode == 4)
            {
                // Get the totals from SM and not from Mpa            
                // GET TABLE
                RRDMRepl_SupervisorMode_Details_Recycle SM = new RRDMRepl_SupervisorMode_Details_Recycle();
                SM.Read_SM_AND_FillTable_Deposits_2(WAtmNo, WReplCycle);
                dataGridView1.DataSource = SM.DataTable_SM_Deposits_2.DefaultView;

                if (SM.DataTable_SM_Deposits_2.Rows.Count > 0)
                {
                    // SHOW GRID
                    ShowGrid01_Recycling();
                }


                //Da.ReadDepositsAndGet_Table_With_TotalsByCurrency(WAtmNo, WReplCycle);
                //dataGridView2.DataSource = Da.DataTableTotals.DefaultView;

                //ShowGrid02();
            }

            if (WMode == 3)
            {
                // Get the totals from SM and not from Mpa            
                // GET TABLE
                SM.Read_ForexChildALL_Get_Txns(WAtmNo, WFromDt, WToDt); 
               
                dataGridView1.DataSource = SM.DataTable_Forex.DefaultView;

                if (SM.DataTable_Forex.Rows.Count > 0)
                {
                    labelHeader.Text = "FOREX ANALYSIS"; 
                    // SHOW GRID
                    ShowGrid03();
                }


                //Da.ReadDepositsAndGet_Table_With_TotalsByCurrency(WAtmNo, WReplCycle);
                //dataGridView2.DataSource = Da.DataTableTotals.DefaultView;

                //ShowGrid02();
            }


        }

        // On ROW ENTER 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

        }
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
            //WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

        }


        // Show Grid 01 
        public void ShowGrid01()
        {

            //"  SELECT  "
            //            + "  [AtmNo],[ReplCycle]  "
            //            + "  ,[Currency] ,[FaceValue]  "
            //            + "  ,[CASSETTE] ,[RETRACT] ,[RECYCLED]  "
            //            + "  , FaceValue* CASSETTE As CASSETTE_Amt "
            //            + "  , FaceValue * RETRACT As RETRACT_Amt  "
            //            + "  , FaceValue* RECYCLED As RECYCLED_Amt  "
            //            + " FROM [ATM_MT_Journals_AUDI].[dbo].[SM_Deposit_analysis] "
            //            + " WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle "
            //            + " ORDER BY Currency, FaceValue ";
            dataGridView1.Columns[0].Width = 70; //ATM No 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[0].Visible = false; 

            dataGridView1.Columns[1].Width = 70; //ReplCycle
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 60; // Currency
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 60; // FaceValue
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 90; // CASSETTE
            dataGridView1.Columns[4].DefaultCellStyle.Format = "#,##0";
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[5].Width = 90; // RETRACT
            dataGridView1.Columns[5].DefaultCellStyle.Format = "#,##0";
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[6].Width = 90; // RECYCLED
            dataGridView1.Columns[6].DefaultCellStyle.Format = "#,##0";
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[7].Width = 90; // CASSETTE_Amt
            dataGridView1.Columns[7].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            
        }

        public void ShowGrid01_Recycling()
        {
            //+"  [AtmNo],[ReplCycle]  "
            //         + "  ,[Currency] ,[FaceValue]  "
            //         + "  ,[CASSETTE] ,[RETRACT] ,[RECYCLED],NCR_DepositsDispensed As NCRNotes  "
            //         + "  ,(([CASSETTE] + [RECYCLED])-NCR_DepositsDispensed) As CIT_Notes  "
            //         + "  , FaceValue* CASSETTE As CASSETTE_Amt "
            //         + "  , FaceValue * RETRACT As RETRACT_Amt  "
            //         + "  , FaceValue* RECYCLED As RECYCLED_Amt  "
            //         + "  , FaceValue* NCR_DepositsDispensed As NCR_Amt  "
            //          + "  , ((FaceValue* CASSETTE+FaceValue* RECYCLED)-FaceValue* NCR_DepositsDispensed)  As CIT_Amt  "


            dataGridView1.Columns[0].Width = 70; //ATM No 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[0].Visible = false; 

            dataGridView1.Columns[1].Width = 70; //ReplCycle
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 60; // Currency
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[3].Width = 60; // FaceValue
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 90; // CASSETTE
            dataGridView1.Columns[4].DefaultCellStyle.Format = "#,##0";
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[5].Width = 90; // RETRACT
            dataGridView1.Columns[5].DefaultCellStyle.Format = "#,##0";
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[6].Width = 90; // RECYCLED
            dataGridView1.Columns[6].DefaultCellStyle.Format = "#,##0";
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[7].Width = 90; // NCRNotes 
            dataGridView1.Columns[7].DefaultCellStyle.Format = "#,##0";
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[8].Width = 90; // CIT_Notes 
            dataGridView1.Columns[8].DefaultCellStyle.Format = "#,##0";
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[9].Width = 90; // CASSETTE_Amt
            dataGridView1.Columns[9].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[10].Width = 90; // RETRACT_Amt
            dataGridView1.Columns[10].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[11].Width = 90; // RECYCLE  Amt
            dataGridView1.Columns[11].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[12].Width = 90; // NCR_Amt
            dataGridView1.Columns[12].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[13].Width = 90; // CIT_Amt
            dataGridView1.Columns[13].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }


        // Show Grid 02 
        public void ShowGrid02()
        {

            dataGridView2.Columns[0].Width = 70; //Currency
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Columns[0].Visible = false;

            dataGridView2.Columns[1].Width = 90; //Total 
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
           // dataGridView2.Columns[1].Visible = false;

          
        }

        // Show Grid 02 
        public void ShowGrid03()
        {

            dataGridView1.Columns[0].Width = 70; //
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Columns[0].Visible = false;

            //dataGridView2.Columns[1].Width = 90; //
            //dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            // dataGridView2.Columns[1].Visible = false;


        }



        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

    }
}
