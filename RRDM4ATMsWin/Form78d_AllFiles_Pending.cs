using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.ComponentModel;

namespace RRDM4ATMsWin
{
    public partial class Form78d_AllFiles_Pending : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
      
        RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();
        RRDMMatchingOfTxnsFindOriginRAW Msr = new RRDMMatchingOfTxnsFindOriginRAW();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        //int MaskLength;

        bool UnMatchedFound; 

        string WOperator;
        string WSignedId;

        string WCategoryId;

        public Form78d_AllFiles_Pending(string InOperator, string InSignedId, string InCategoryId)

        {
            WSignedId = InSignedId;

            WOperator = InOperator;

            WCategoryId = InCategoryId;

            InitializeComponent();

        }

        //bool WRecordFoundInUniversal;
        //string WSelectionCriteria;
      
        string PhysicalName;
        private void Form78b_Load(object sender, EventArgs e)
        {
            UnMatchedFound = false; 

            Mcs.ReadReconcCategoriesVsSourcesAll(WCategoryId);

            string FileA = Mcs.SourceFileNameA;
            string FileB = Mcs.SourceFileNameB;
            string FileC = Mcs.SourceFileNameC;
            string FileD = Mcs.SourceFileNameD;

            Msf.ReadReconcSourceFilesByFileId(FileA); 
            
            //
            // DO FILE A 
            //
            string PhysicalNameA = "";
            string PhysicalNameB = "";
            string PhysicalNameC = "";

            if (Msf.SystemOfOrigin == "ATMs")
            {
             
                string WSelectionCriteria = " WHERE Operator ='"+ WOperator + "' AND MatchingCateg ='" + WCategoryId + "' AND IsMatchingDone = 0 ";
                Mpa.ReadMatchingTxnsMasterPoolByCategoryAndCycleAndFillTable( WSelectionCriteria,1);

                if (Mpa.DataTableActionsTaken.Rows.Count > 0)
                {
                    dataGridView1.DataSource = Mpa.DataTableActionsTaken.DefaultView;
                    label1.Text = "UNMATCHED RECORDS FOR:.." + FileA;
                    labelTotalA.Text = "TOTAL =.."+Mpa.DataTableActionsTaken.Rows.Count.ToString();
                    UnMatchedFound = true;
                }
                else
                {
                    label1.Text = "UNMATCHED RECORDS FOR:.." + FileA;
                    labelTotalA.Text = "TOTAL =.." + "0";
                }

            }
            else
            {

                Msf.ReadReconcSourceFilesByFileId(FileA);

                PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "]";

                Msr.FillTableOfPending(WOperator,  WCategoryId, PhysicalName);

                if (Msr.TableDetails_Physical.Rows.Count > 0)
                {
                    dataGridView1.DataSource = Msr.TableDetails_Physical.DefaultView;

                    label1.Text = "UNMATCHED RECORDS FOR :.." + FileA;
                    labelTotalB.Text = "TOTAL =.." + Msr.TableDetails_Physical.Rows.Count.ToString();
                    UnMatchedFound = true;
                }
                else
                {
                    label1.Text = "UNMATCHED RECORDS FOR :.." + FileA;
                    labelTotalB.Text = "TOTAL =.." + Msr.TableDetails_Physical.Rows.Count.ToString();
                }
            }

            //
            // DO FILE B 
            //

            Msf.ReadReconcSourceFilesByFileId(FileB);

            PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "]";

            Msr.FillTableOfPending(WOperator, WCategoryId  ,PhysicalName);

            if (Msr.TableDetails_Physical.Rows.Count > 0)
            {
                dataGridView2.DataSource = Msr.TableDetails_Physical.DefaultView;

                label2.Text = "UNMATCHED RECORDS FOR :.." + FileB;
                labelTotalB.Text = "TOTAL =.." + Msr.TableDetails_Physical.Rows.Count.ToString();
                UnMatchedFound = true;
            }
            else
            {
                label2.Text = "UNMATCHED RECORDS FOR :.." + FileB;
                labelTotalB.Text = "TOTAL =.." + Msr.TableDetails_Physical.Rows.Count.ToString();
            }
            //
            // DO FILE C 
            //

            if (FileC != "")
                {
                    dataGridView3.Show();
                Msf.ReadReconcSourceFilesByFileId(FileC);

                PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + "[" + Msf.InportTableName + "]";

                Msr.FillTableOfPending(WOperator, WCategoryId, PhysicalName);

                if (Msr.TableDetails_Physical.Rows.Count > 0)
                {
                    dataGridView3.DataSource = Msr.TableDetails_Physical.DefaultView;

                    label3.Text = "UNMATCHED RECORDS FOR :.." + FileC;
                    labelTotalC.Text = "TOTAL =.." + Msr.TableDetails_Physical.Rows.Count.ToString();
                    UnMatchedFound = true;
                }
                else
                {
                    label3.Text = "UNMATCHED RECORDS FOR :.." + FileC;
                    labelTotalC.Text = "TOTAL =.." + Msr.TableDetails_Physical.Rows.Count.ToString();
                }
            }
                else
                {
                    label3.Text = "NO OTHER FILE ";
                    dataGridView3.Hide();
                }

            if (UnMatchedFound == false)
            {
                Form2 MessageForm = new Form2("No UnMatched Found");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }

            }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

        }

        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
      
    }
}
