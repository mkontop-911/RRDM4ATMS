using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDM_JccTransClass
    {
        // Fields for both InPool and Trans to be posted 

        // Fields for InPool Trans
        public int TranNo;
        public int EJournalTraceNo;
     
        public string Origin;

        public int ErrNo;
        public string AtmNo;
        public int SesNo;

        public string BankId;

        public int AtmTraceNo;

        public string BranchId;
        public DateTime AtmDtTime;

        public DateTime HostDtTime;

        public string CardNo;
        public int CardOrigin;

        public int SystemTarget;
        public int TransType; // 11 And 21 is related with customer withdrwals and reversals cassettes 
        // 12 And 22 are related with Cash In and Cash out during Replenishment 
        // 23 Cash customer deposits 24 Cheques customer deposits 
        public string TransDesc;
        public string AccNo;


        public string CurrDesc;
        public decimal TranAmount;
        public int AuthCode;
        public int RefNumb;
        public int RemNo;

        public string TransMsg;
        public string AtmMsg;

        public int StartTrxn;
        public int EndTrxn;

        public decimal DepCount;

        public int CommissionCode;
        public decimal CommissionAmount;

        public bool SuccTran;

        public string Operator;

        // Define the data table 
        public DataTable JCC_TransSelected = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //
        // READ JCC TRANS AND FILL TABLE
        // BASED ON selection creteria 
        //
        public void ReadJCC_TranFillTable(string InCardNo, int InTranNo, string InChosen)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            JCC_TransSelected = new DataTable();
            JCC_TransSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            if (InTranNo > 0 & InChosen == "Chosen")
            {
                JCC_TransSelected.Columns.Add("Chosen", typeof(bool));
            }
            JCC_TransSelected.Columns.Add("TranNo", typeof(int));
            JCC_TransSelected.Columns.Add("TransDesc", typeof(string));
            JCC_TransSelected.Columns.Add("CurrDesc", typeof(string));
            JCC_TransSelected.Columns.Add("TranAmount", typeof(decimal));
            if (InTranNo > 0 & InChosen == "Chosen")
            {
                JCC_TransSelected.Columns.Add("DispAmnt", typeof(decimal));
            }
            JCC_TransSelected.Columns.Add("HostDtTime", typeof(DateTime));
            
            JCC_TransSelected.Columns.Add("RRN", typeof(int));
            JCC_TransSelected.Columns.Add("BankId", typeof(string));   
            JCC_TransSelected.Columns.Add("TransType", typeof(int));
          
            JCC_TransSelected.Columns.Add("Origin", typeof(string));


            if (InTranNo == 0)
            {
                SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[InPoolTransJCC] "
                      + " WHERE CardNo = @CardNo";
            }
            

            if (InTranNo > 0)
            {
                SqlString = "Select * FROM [ATMS].[dbo].[InPoolTransJCC]"
                          + " WHERE TranNo = @TranNo ";
            }
            

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InTranNo == 0)
                        {
                            cmd.Parameters.AddWithValue("@CardNo", InCardNo);
                        }
                        if (InTranNo > 0)
                        {
                            cmd.Parameters.AddWithValue("@TranNo", InTranNo);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            DataRow RowSelected = JCC_TransSelected.NewRow();

                            if (InTranNo > 0 & InChosen == "Chosen")
                            {
                                RowSelected["Chosen"] = false;
                            } 

                            RowSelected["TranNo"] = (int)rdr["TranNo"];
                            RowSelected["RRN"] = (int)rdr["RefNumb"];
                            RowSelected["BankId"] = (string)rdr["BankId"];
                            RowSelected["HostDtTime"] = (DateTime)rdr["HostDtTime"];
                            RowSelected["TransType"] = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            RowSelected["TransDesc"] = (string)rdr["TransDesc"];
                            RowSelected["CurrDesc"] = (string)rdr["CurrDesc"];
                            RowSelected["TranAmount"] = (decimal)rdr["TranAmount"];

                            if (InTranNo > 0 & InChosen == "Chosen")
                            {
                                RowSelected["DispAmnt"] = (decimal)rdr["TranAmount"];
                            }

                            RowSelected["Origin"] = "ProcessorATMs";

                            // ADD ROW
                            JCC_TransSelected.Rows.Add(RowSelected);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //  log4net.Config.XmlConfigurator.Configure();
                    //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "ReadTranSpecific");
                }
        }

        //
        // READ SPECIFIC TRANSACTION FROM IN POOL based on Number 
        //
        public void ReadJCC_TranSpecific(int InTranNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTransJCC] "
          + " WHERE TranNo = @TranNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            TranNo = (int)rdr["TranNo"];
                            Origin = (string)rdr["Origin"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            EJournalTraceNo = (int)rdr["EJournalTraceNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            BankId = (string)rdr["BankId"];
                            //         Prive = (bool)rdr["Prive"];
                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            HostDtTime = (DateTime)rdr["HostDtTime"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                            CardNo = (string)rdr["CardNo"];

                            CardOrigin = (int)rdr["CardOrigin"];
                            AccNo = (string)rdr["AccNo"];

                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNumb = (int)rdr["RefNumb"];
                            RemNo = (int)rdr["RemNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            AtmMsg = (string)rdr["AtmMsg"];

                            ErrNo = (int)rdr["ErrNo"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DepCount = (decimal)rdr["DepCount"];

                            CommissionCode = (int)rdr["CommissionCode"];
                            CommissionAmount = (decimal)rdr["CommissionAmount"];

                            SuccTran = (bool)rdr["SuccTran"];

                            Operator = (string)rdr["Operator"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //  log4net.Config.XmlConfigurator.Configure();
                    //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "ReadTranSpecific");
                }
        }
    }
}
