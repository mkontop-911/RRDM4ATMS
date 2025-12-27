using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMMask_Reversals : Logger
    {
        public RRDMMask_Reversals() : base() { }

        public int SeqNo;

        public string MatchingFieldName;
       
        public string FieldDBName;
        public string FieldType;

        public string Application;

        public string Operator;

        // Define the data table 
        public DataTable TableReversals = new DataTable();
        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMJournalAudi_BDC Mgt = new RRDMJournalAudi_BDC(); 

        RRDMMappingFileFieldsFromBankToRRDM Rff = new RRDMMappingFileFieldsFromBankToRRDM();
  //
  // Reader fields 
  //
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            MatchingFieldName = (string)rdr["MatchingFieldName"];

            FieldDBName = (string)rdr["FieldDBName"];
            FieldType = (string)rdr["FieldType"];
            Application = (string)rdr["Application"];

            Operator = (string)rdr["Operator"];
        }

        //
        // READ MatchingFields to fill table 
        //
        public void ReadReconcMatchingFieldsToFillDataTable(string InSelectionCriteria, string InMask)
        { 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // FOR REVERSALS TABLE
            TableReversals = new DataTable();
            TableReversals.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableReversals.Columns.Add("SeqNo", typeof(int));
            TableReversals.Columns.Add("Field Name", typeof(string));
            TableReversals.Columns.Add("DB Field Name", typeof(string));
            TableReversals.Columns.Add("Field Type", typeof(string));
            TableReversals.Columns.Add("Application", typeof(string));

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingFields]"
                + " WHERE " + InSelectionCriteria;

            using (SqlConnection conn =

                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

                            DataRow RowSelected = TableReversals.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Field Name"] = MatchingFieldName;
                            RowSelected["DB Field Name"] = FieldDBName;
                            RowSelected["Field Type"] = FieldType;
                            RowSelected["Application"] = Application;

                            // ADD ROW
                            TableReversals.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }
        //
        // Find if reversals. 
        //
        bool WRecordFoundInUniversal; // leave it here
        public bool Reversals_In_One;
        public bool Reversals_In_Two;
        public bool Reversals_In_Three;
        public void ReadAndFind_Reversals(string InOperator, string InSignedId, int InUniqueRecordId)
        {

            string WOperator;
            string WSignedId;
            int WUniqueRecordId;

            WSignedId = InSignedId;
            WOperator = InOperator;
            WUniqueRecordId = InUniqueRecordId;

            RRDMJournalAudi_BDC Msr = new RRDMJournalAudi_BDC();
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
       
            DateTime NullPastDate = new DateTime(1900, 01, 01);

            //int MaskLength;
            int WTraceNo = 0;
            string WRRNumber = "";

            string WSelectionCriteria;

            bool RRNBased = false;

            bool IsOurAtm = false;

            Reversals_In_One = false;
            Reversals_In_Two = false;
            Reversals_In_Three = false;

            string FileA;
            string FileB;
            string FileC;

            // read record
            WSelectionCriteria = " WHERE UniqueRecordId = " + WUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(WSelectionCriteria,1);

            WRRNumber = Mpa.RRNumber;
            WTraceNo = Mpa.TraceNoWithNoEndZero;

            string TerminalId = Mpa.TerminalId;
            DateTime Date = Mpa.TransDate.Date;


            if (Mpa.Origin == "Our Atms")
            {      
                RRNBased = false;
            }
            else
            {
                RRNBased = true;
            }

            //
            // INITIALISE FILES
            //
            FileA = Mpa.FileId01;
            FileB = Mpa.FileId02;
            FileC = Mpa.FileId03;

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            Ac.ReadAtm(Mpa.TerminalId);

            if (Ac.RecordFound == true)
            {
                IsOurAtm = true;
            }
            else
            {
                IsOurAtm = false;
            }

            if (IsOurAtm == true)
            {
                Msr.FillTablesProcessForJournal(WOperator, WSignedId,
                                     Mpa.TerminalId,
                                     WTraceNo * 10, Mpa.TransDate.Date, Mpa.TransAmount);

                if (Msr.TableJournalDetails.Rows.Count > 0)
                {
                    // No REVERASALS HERE
                }
                else
                {
                   // label1.Text = "RECORD DETAILS NOT FOUND FOR Journal - Try journal";
                 
                }
            }
            else
            {
                if (RRNBased == true)
                {
                    // RRN BASED
                    WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileA, Mpa.MatchingCateg
                        , Mpa.TransDate.Date, TerminalId, 0, WRRNumber, Mpa.TransAmount, Mpa.Card_Encrypted, 1, 2);

                }
                else
                {
                    // TRACE BASED
                    WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileA, Mpa.MatchingCateg
                         , Mpa.TransDate.Date, TerminalId, WTraceNo, "", Mpa.TransAmount, Mpa.Card_Encrypted, 1, 2);
                }


                if (Msr.TableDetails_RAW.Rows.Count > 0)
                {
                   
                    int CountOfReversals = 0 ; 
                    int I = 0;

                    while (I <= (Msr.TableDetails_RAW.Rows.Count - 1))
                    {

                        RecordFound = true;

                        string Commnet = (string)Msr.TableDetails_RAW.Rows[I]["Comment"];

                        if (Commnet == "Reversals")
                        {
                            CountOfReversals = CountOfReversals + 1; 
                        }

                        I++; // Read Next entry of the table 

                    }

                    if (CountOfReversals == 2) Reversals_In_One = true;

                    //     dataGridView1.DataSource = Msr.TableDetails_RAW.DefaultView;
                    if (WRecordFoundInUniversal)
                    {
                      //  label1.Text = "RECORD DETAILS FOR :" + FileA;
                    }
                    else
                    {
                       // label1.Text = "UNMATCHED RECORD DETAILS FOR :" + FileA;
                    }

                }
                else
                {
                  //  label1.Text = "RECORD DETAILS NOT FOUND FOR :" + FileA;
                }

            }

          
            //
            // DO FILE B 
            //
            if (RRNBased == true)
            {
                // RRN BASED
                WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileB, Mpa.MatchingCateg
                    , Mpa.TransDate.Date, TerminalId, 0, WRRNumber, Mpa.TransAmount, Mpa.Card_Encrypted, 1,2);

            }
            else
            {
                // TRACE BASED
                WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileB, Mpa.MatchingCateg
                     , Mpa.TransDate.Date, TerminalId, WTraceNo, "", Mpa.TransAmount, Mpa.Card_Encrypted, 1, 2);
            }

            if (Msr.TableDetails_RAW.Rows.Count > 0)
            {
              
                int CountOfReversals = 0;
                int I = 0;

                while (I <= (Msr.TableDetails_RAW.Rows.Count - 1))
                {

                    RecordFound = true;

                    string Commnet = (string)Msr.TableDetails_RAW.Rows[I]["Comment"];

                    if (Commnet == "Reversals")
                    {
                        CountOfReversals = CountOfReversals + 1;
                    }

                    I++; // Read Next entry of the table 

                }

                if (CountOfReversals == 2) Reversals_In_Two = true;

                //  dataGridView2.DataSource = Msr.TableDetails_RAW.DefaultView;
              

            }
            else
            {
              //  label2.Text = "RECORD DETAILS NOT FOUND FOR :" + FileB;
            }
            //
            // DO FILE C 
            //

            if (FileC != "")
            {
              //  dataGridView3.Show();
                if (RRNBased == true)
                {
                    // RRN BASED
                    WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileC, Mpa.MatchingCateg
                        , Mpa.TransDate.Date, TerminalId, 0, WRRNumber, Mpa.TransAmount, Mpa.Card_Encrypted, 1, 2);

                }
                else
                {
                    // TRACE BASED
                    WRecordFoundInUniversal = Mgt.FindRecordsFromMasterRecord(WOperator, FileC, Mpa.MatchingCateg
                         , Mpa.TransDate.Date, TerminalId, WTraceNo, "", Mpa.TransAmount, Mpa.Card_Encrypted, 1, 2);
                }
                if (Msr.TableDetails_RAW.Rows.Count > 0)
                {
                  
                    int CountOfReversals = 0;
                    int I = 0;

                    while (I <= (Msr.TableDetails_RAW.Rows.Count - 1))
                    {

                        RecordFound = true;

                        string Comment = (string)Msr.TableDetails_RAW.Rows[I]["Comment"];

                        if (Comment == "Reversals")
                        {
                            CountOfReversals = CountOfReversals + 1;
                        }

                        I++; // Read Next entry of the table 

                    }

                    if (CountOfReversals == 2) Reversals_In_Three = true;
                    //   dataGridView3.DataSource = Msr.TableDetails_RAW.DefaultView;
                   

                }
                else
                {
                  //  label3.Text = "RECORD DETAILS NOT FOUND FOR :" + FileC;
                }
            }
            else
            {
               // label3.Text = "NO OTHER FILE ";
               // dataGridView3.Hide();
            }


        }

  

    }
}


