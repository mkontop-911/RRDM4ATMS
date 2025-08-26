using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;
using System.Xml.Linq;
using RRDM4ATMs;
using System.Xml.Linq;
using System.Data;
using System.IO;


namespace RRDM4ATMsWin
{
    public partial class Form78d_Discre : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMMatchingDiscrepancies Md = new RRDMMatchingDiscrepancies();
        // FIND CUTOFF CYCLE
        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        DateTime WCut_Off_Date; 

       // int WSeqNo;

        string WOperator;
        string WSignedId;
        string WMatchingCateg;
        int WRMCycle;
        int WMode;
        string WInAtmNo; 
       
        public Form78d_Discre(string InOpertor , string InSignedId, string InMatchingCateg, int InRMCycle, int InMode, string InAtmNo)
        {
            WOperator = InOpertor;
            WSignedId = InSignedId;
            WMatchingCateg = InMatchingCateg;
            WRMCycle = InRMCycle;
            WMode = InMode;
            WInAtmNo = InAtmNo;
            // Mode = 1 => show discrepancies per category
            // Mode = 2 => show discrepacincies for all ATMs
            // Mode = 3 => show discrepacincies by category from Master
            // Mode = 5 => show in Journal but not in IST
            // Mode = 6 => show up to last 20 Journals 

            // Mode = 21 => show Records In IST by Matching category and NET_DATE 




            InitializeComponent();

            string WJobCategory = "ATMs";
            int WReconcCycleNo;

            // Date of the file in string
            //var resultLastThreeDigits = InFullPath.Substring(InFullPath.Length - 3);
            //string result1 = InFullPath.Substring(InFullPath.Length - 12);
            //string result2 = result1.Substring(0, 8);

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            if (WReconcCycleNo == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    return;
                }
            }
            else
            {
                WCut_Off_Date = Rjc.Cut_Off_Date;

               // ReversedCut_Off_Date = WCut_Off_Date.ToString("yyyyMMdd");

            }
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            //if (WDataTable.Rows.Count == 0)
            //{
            //    MessageBox.Show("No Entries to show."); 
            //    return; 
            //}
            if (WMode == 1)
            {
                Md.ReadMatchingDiscrepanciesFillTableByATM_RMCycle(WMatchingCateg, WRMCycle);
                dataGridView1.DataSource = Md.TableMatchingDiscrepancies.DefaultView;

                labelWhatGrid.Text = "DETAIL DISCREPANCIES BY CATEGORY : " + WMatchingCateg +"..FOR..CYCLE.."+ WRMCycle.ToString();
                dataGridView1.Show();
                dataGridView2.Hide();
                labelAtmNo.Hide();
                textBoxAtmNo.Hide();
                buttonShow.Hide();

                textBoxTotalRec.Text = Md.TableMatchingDiscrepancies.Rows.Count.ToString();

            }
            if (WMode == 2)
            {
               
                Mpa.ReadMatchingDiscrepanciesFillTableByATM_RMCycle(WMatchingCateg, WRMCycle, WMode,1);

                labelWhatGrid.Text = "DISCREPANCIES BY ATM" + "..FOR..CYCLE.." + WRMCycle.ToString();
                
                dataGridView2.DataSource = Mpa.TableFullFromMaster.DefaultView;

                dataGridView2.Show();
                dataGridView1.Hide();
                labelAtmNo.Show();
                textBoxAtmNo.Show();
                buttonShow.Show();

                textBoxTotalRec.Text = Mpa.TableFullFromMaster.Rows.Count.ToString();

            }
            if (WMode == 3)
            {
               
                Mpa.ReadMatchingDiscrepanciesFillTableByATM_RMCycle(WMatchingCateg, WRMCycle, WMode,1);

                labelWhatGrid.Text = "DISCREPANCIES FROM MASTER..FOR CATEGORY.."+ WMatchingCateg + "..FOR..CYCLE.." + WRMCycle.ToString();
               
                dataGridView1.DataSource = Mpa.TableFullFromMaster.DefaultView;

                dataGridView1.Show();
                dataGridView2.Hide();
                labelAtmNo.Hide();
                textBoxAtmNo.Hide();
                buttonShow.Hide();

                textBoxTotalRec.Text = Mpa.TableFullFromMaster.Rows.Count.ToString();

            }

            if (WMode == 5)
            {
              
                Mpa.ReadMatchingDiscrepanciesFillTableByATM_CutoffDate(WRMCycle, WCut_Off_Date);

                labelWhatGrid.Text = "In Journals But not in IST.." + "..FOR..Cut_off.." + WCut_Off_Date.ToString();

                dataGridView1.DataSource = Mpa.TableFullFromMaster.DefaultView;

                dataGridView1.Show();
                dataGridView2.Hide();
                labelAtmNo.Hide();
                textBoxAtmNo.Hide();
                buttonShow.Hide();

                textBoxTotalRec.Text = Mpa.TableFullFromMaster.Rows.Count.ToString();

            }

            if (WMode == 6)
            {

                RRDMReconcFileMonitorLog Fl = new RRDMReconcFileMonitorLog(); 

                labelWhatGrid.Text = "LIST OF last 20 Journals for ATM.." + WInAtmNo;

                Fl.ReadDataTableLast_20_Journals(WInAtmNo); 

                dataGridView1.DataSource = Fl.DataTableLast_20_Journals.DefaultView;

                dataGridView1.Show();
                dataGridView2.Hide();
                labelAtmNo.Hide();
                textBoxAtmNo.Hide();
                buttonShow.Hide();

                textBoxTotalRec.Text = Fl.DataTableLast_20_Journals.Rows.Count.ToString();

            }

            if (WMode == 21)
            {
                Md.Read_AND_SHOW_IST_OUTSTANDING_RECORDS(); 
                dataGridView1.DataSource = Md.TableMatchingDiscrepancies.DefaultView;

                labelWhatGrid.Text = "DETAIL IST RECORDS : " ;
                dataGridView1.Show();
                dataGridView2.Hide();
                labelAtmNo.Hide();
                textBoxAtmNo.Hide();
                buttonShow.Hide();

                textBoxTotalRec.Text = Md.TableMatchingDiscrepancies.Rows.Count.ToString();

            }

            // SHOW GRID

        }

        // On ROW ENTER 
       
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //WSeqNo = (int)dataGridView1.Rows[e.RowIndex].Cells["SeqNo"].Value;

        }
        // ROW ENTER FOR 2
        string WAtmNo; 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
          
            WAtmNo = (string)rowSelected.Cells[0].Value;
            textBoxAtmNo.Text = WAtmNo;
        }

        // Show Grid 03 
        public void ShowGrid03()
        {

            //dataGridView1.Columns[0].Width = 70; // WhatFile
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            //dataGridView1.Columns[1].Width = 60; // SeqNo
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

     

// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// Show Discrepancies for ATM No
        private void buttonShow_Click(object sender, EventArgs e)
        {
            Form78d_FileRecords NForm78d_FileRecords;
            // textBoxFileId.Text
            int WMode = 3; //
                           // InMode = 1 : Not processed yet 
                           // InMode = 2 : Processed this Cycle
                           // InMode = 3 : Errors for this ATM
            NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, "Atms_Journals_Txns", WAtmNo, WRMCycle,"", WMode,true);
            NForm78d_FileRecords.ShowDialog();
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Print
            string P1 = "";



            if (WMode == 3)
            {
                P1 = "Discrepancies for Category.." + WMatchingCateg + "..At Cycle.." + WRMCycle.ToString();
            }
            else
            {
                MessageBox.Show("No Printing for this"); 
            }

            string P2 = "";  // Category 
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;

                Form56R68ATMS_W_Pool_Table ReportATMS_Pool_Table = new Form56R68ATMS_W_Pool_Table(P1, P2, P3, P4, P5);
                ReportATMS_Pool_Table.Show();
           
        }

        private void buttonXML_Click(object sender, EventArgs e)
        {
            string XMLDoc;

            //XMLDoc = ToXml(Ac.ATMsDetailsDataTable, 0);
            string sFileName = "C:\\RRDM\\Working\\TEXT.XML";
            StreamWriter outputFile = new StreamWriter(@sFileName);

            DataSet dS = new DataSet();
            dS.DataSetName = "RecordSet";
            dS.Tables.Add(Md.TableMatchingDiscrepancies);
            //StringWriter sw = new StringWriter();
            dS.WriteXml(outputFile, XmlWriteMode.IgnoreSchema);

            MessageBox.Show("An XML File is created in RRDM working directory");
        }
    }
}
