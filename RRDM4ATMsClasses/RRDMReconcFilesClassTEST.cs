using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;

namespace RRDM4ATMs
{
    public class RRDMReconcFilesClassTEST
    {
        public int SeqNo;

        public string RMCateg;
        public int RMCycle; 
        public bool Matched; 
        public string MatchMask;
        //public string UnMatchedType; 

        public string TerminalId;
        public int TransType;

        public string TransDescr;
        public string CardNumber;
        public string AccNumber;

        public string TransCurr;
        public decimal TransAmount;

        public DateTime TransDate;

        public int AtmTraceNo;
        public int RRNumber;
        public int ResponseCode;
        public int T24RefNumber;

        public bool OpenRecord; // Currently only for UnMatched 

        public string Operator;


        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();

        // Define the data table 
        public DataTable RMRecords = new DataTable();

        public DataTable RMRecordsRRNa = new DataTable();

        public DataTable RMRecordsRRNb = new DataTable();

        public DataTable RMRecordsRRNc = new DataTable();

        public DataTable RMUnmathed = new DataTable();

        public DataTable RMDataTableLeft = new DataTable();

        public DataTable RMDataTableRight = new DataTable();

        public int TotalSelected;
        string SqlString; // Do not delete 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        //
        // Methods 
        // READ Specific by SeqNo
        // FILL UP A TABLE
        //
        public void ReadRMFileSpecificBySeqNo(string InSourceFileName, string InRMCateg, int InRMCycle,
                                                                                         int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                   + " WHERE RMCateg = @RMCateg AND RMCycle = @RMCycle AND SeqNo = @SeqNo";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;


                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }

        // Methods 
        // READ Specific by SeqNo
        // FILL UP A TABLE
        //
        public void ReadRMFileSpecificRecordBySeqNo(string InSourceFileName, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                   + " WHERE SeqNo = @SeqNo";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;


                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }
        //
        // Methods 
        // READ File Records 
        // FILL UP A TABLE
        //
        public void ReadRMFileSpecificByName(string InSourceFileName, string InRMCateg, int InRMCycle,
                                                                                         bool InMatched)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMRecords = new DataTable();
            RMRecords.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            RMRecords.Columns.Add("SeqNo", typeof(int));
            RMRecords.Columns.Add("TransDescr", typeof(string));      
            RMRecords.Columns.Add("CardNumber", typeof(string));
            RMRecords.Columns.Add("AccNumber", typeof(string));
            RMRecords.Columns.Add("TransCurr", typeof(string));
            RMRecords.Columns.Add("TransAmount", typeof(decimal));
            RMRecords.Columns.Add("TransDate", typeof(DateTime));
            RMRecords.Columns.Add("RRNumber", typeof(int));

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                   + " WHERE RMCateg = @RMCateg AND RMCycle = @RMCycle AND Matched = @Matched";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@Matched", InMatched);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;


                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

                            Operator = (string)rdr["Operator"];


                            DataRow RowSelected = RMRecords.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["TransDescr"] = TransDescr;
                            
                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNumber"] = AccNumber;

                            RowSelected["TransCurr"] = TransCurr;
                            RowSelected["TransAmount"] = TransAmount;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["RRNumber"] = RRNumber;

                            // ADD ROW
                            RMRecords.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }
        //
        // Methods 
        // READ Records For RRN
        // FILL UP A TABLE
        //
        public void ReadRMRecordsByRRNFileA(string InSourceFileName, string InRMCateg, int InRMCycle, int InRRN, int InT24RefNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMRecordsRRNa = new DataTable();
            RMRecordsRRNa.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            RMRecordsRRNa.Columns.Add("SeqNo", typeof(int));
            RMRecordsRRNa.Columns.Add("TransDescr", typeof(string));
            
            RMRecordsRRNa.Columns.Add("CardNumber", typeof(string));
            RMRecordsRRNa.Columns.Add("AccNumber", typeof(string));
            RMRecordsRRNa.Columns.Add("TransCurr", typeof(string));
            RMRecordsRRNa.Columns.Add("TransAmount", typeof(decimal));
            RMRecordsRRNa.Columns.Add("TransDate", typeof(DateTime));
            RMRecordsRRNa.Columns.Add("RRNumber", typeof(int));

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                   + " WHERE RMCateg = @RMCateg AND RMCycle = @RMCycle AND RRNumber = @RRNumber AND T24RefNumber = @T24RefNumber";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@RRNumber", InRRN);
                        cmd.Parameters.AddWithValue("@T24RefNumber", T24RefNumber);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;


                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

                            Operator = (string)rdr["Operator"];


                            DataRow RowSelected = RMRecordsRRNa.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["TransDescr"] = TransDescr;
                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNumber"] = AccNumber;

                            RowSelected["TransCurr"] = TransCurr;
                            RowSelected["TransAmount"] = TransAmount;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["RRNumber"] = RRNumber;

                            // ADD ROW
                            RMRecordsRRNa.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ Records For RRN - FILE B
        // FILL UP A TABLE
        //
        public void ReadRMRecordsByRRNFileB(string InSourceFileName, string InRMCateg, int InRMCycle, int InRRN, int InT24RefNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMRecordsRRNb = new DataTable();
            RMRecordsRRNb.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            RMRecordsRRNb.Columns.Add("SeqNo", typeof(int));
            RMRecordsRRNb.Columns.Add("TransDescr", typeof(string));
            RMRecordsRRNb.Columns.Add("CardNumber", typeof(string));
            RMRecordsRRNb.Columns.Add("AccNumber", typeof(string));
            RMRecordsRRNb.Columns.Add("TransCurr", typeof(string));
            RMRecordsRRNb.Columns.Add("TransAmount", typeof(decimal));
            RMRecordsRRNb.Columns.Add("TransDate", typeof(DateTime));
            RMRecordsRRNb.Columns.Add("RRNumber", typeof(int));

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                   + " WHERE RMCateg = @RMCateg AND RMCycle = @RMCycle AND RRNumber = @RRNumber AND T24RefNumber = @T24RefNumber";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@RRNumber", InRRN);
                        cmd.Parameters.AddWithValue("@T24RefNumber", T24RefNumber);



                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;


                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

                            Operator = (string)rdr["Operator"];


                            DataRow RowSelected = RMRecordsRRNb.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["TransDescr"] = TransDescr;
                            
                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNumber"] = AccNumber;

                            RowSelected["TransCurr"] = TransCurr;
                            RowSelected["TransAmount"] = TransAmount;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["RRNumber"] = RRNumber;

                            // ADD ROW
                            RMRecordsRRNb.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ Records For RRN - FILE C
        // FILL UP A TABLE
        //
        public void ReadRMRecordsByRRNFileC(string InSourceFileName, string InRMCateg, int InRMCycle, int InRRN, int InT24RefNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMRecordsRRNc = new DataTable();
            RMRecordsRRNc.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            RMRecordsRRNc.Columns.Add("SeqNo", typeof(int));
            RMRecordsRRNc.Columns.Add("TransDescr", typeof(string));
            RMRecordsRRNc.Columns.Add("CardNumber", typeof(string));
            RMRecordsRRNc.Columns.Add("AccNumber", typeof(string));
            RMRecordsRRNc.Columns.Add("TransCurr", typeof(string));
            RMRecordsRRNc.Columns.Add("TransAmount", typeof(decimal));
            RMRecordsRRNc.Columns.Add("TransDate", typeof(DateTime));
            RMRecordsRRNc.Columns.Add("RRNumber", typeof(int));

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                    + " WHERE RMCateg = @RMCateg AND RMCycle = @RMCycle AND RRNumber = @RRNumber AND T24RefNumber = @T24RefNumber";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@RRNumber", InRRN);
                        cmd.Parameters.AddWithValue("@T24RefNumber", T24RefNumber);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

                            Operator = (string)rdr["Operator"];


                            DataRow RowSelected = RMRecordsRRNc.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["TransDescr"] = TransDescr;
                          
                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNumber"] = AccNumber;

                            RowSelected["TransCurr"] = TransCurr;
                            RowSelected["TransAmount"] = TransAmount;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["RRNumber"] = RRNumber;

                            // ADD ROW
                            RMRecordsRRNc.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ UnMatched Records
        // FILL UP A TABLE
        //
        public void ReadRMUnMatched(string InSourceFileName, string InRMCateg, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
     
                RMUnmathed = new DataTable();
                RMUnmathed.Clear();
                TotalSelected = 0;
           
            // DATA TABLE ROWS DEFINITION 

                RMUnmathed.Columns.Add("SeqNo", typeof(int));
                RMUnmathed.Columns.Add("TransDescr", typeof(string));        
                RMUnmathed.Columns.Add("CardNumber", typeof(string));
                RMUnmathed.Columns.Add("AccNumber", typeof(string));
                RMUnmathed.Columns.Add("TransCurr", typeof(string));
                RMUnmathed.Columns.Add("TransAmount", typeof(decimal));
                RMUnmathed.Columns.Add("TransDate", typeof(DateTime));
                RMUnmathed.Columns.Add("RRNumber", typeof(int));

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                   + " WHERE RMCateg = @RMCateg AND RMCycle = @RMCycle";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

                            Operator = (string)rdr["Operator"];
                         
                                DataRow RowSelected = RMUnmathed.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["TransDescr"] = TransDescr;
                               
                                RowSelected["CardNumber"] = CardNumber;
                                RowSelected["AccNumber"] = AccNumber;

                                RowSelected["TransCurr"] = TransCurr;
                                RowSelected["TransAmount"] = TransAmount;
                                RowSelected["TransDate"] = TransDate;
                                RowSelected["RRNumber"] = RRNumber;


                                // ADD ROW
                                RMUnmathed.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ UNMatched records for LEFT Grid 
        // FILL UP A TABLE
        //
        public void ReadRMUnMatchedLeft(string InSourceFileName, string InRMCateg, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMDataTableLeft = new DataTable();
            RMDataTableLeft.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            RMDataTableLeft.Columns.Add("SeqNo", typeof(int));
            RMDataTableLeft.Columns.Add("Select", typeof(bool));
            RMDataTableLeft.Columns.Add("TransDescr", typeof(string));
            RMDataTableLeft.Columns.Add("CardNumber", typeof(string));
            RMDataTableLeft.Columns.Add("AccNumber", typeof(string));
            RMDataTableLeft.Columns.Add("TransCurr", typeof(string));
            RMDataTableLeft.Columns.Add("TransAmount", typeof(decimal));
            RMDataTableLeft.Columns.Add("TransDate", typeof(DateTime));
            RMDataTableLeft.Columns.Add("RRNumber", typeof(int));

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                   + " WHERE RMCateg = @RMCateg AND OpenRecord = 1 ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        //cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = RMDataTableLeft.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Select"] = false;

                            RowSelected["TransDescr"] = TransDescr;
                          
                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNumber"] = AccNumber;

                            RowSelected["TransCurr"] = TransCurr;
                            RowSelected["TransAmount"] = TransAmount;
                            RowSelected["TransDate"] = TransDate;

                            RowSelected["RRNumber"] = RRNumber;


                            // ADD ROW
                            RMDataTableLeft.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ UNMatched records for Right Grid 
        // FILL UP A TABLE
        //
        public void ReadRMUnMatchedRight(string InSourceFileName, string InRMCateg, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMDataTableRight = new DataTable();
            RMDataTableRight.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            RMDataTableRight.Columns.Add("SeqNo", typeof(int));
            RMDataTableRight.Columns.Add("Select", typeof(bool));
            RMDataTableRight.Columns.Add("TransDescr", typeof(string));
            RMDataTableRight.Columns.Add("CardNumber", typeof(string));
            RMDataTableRight.Columns.Add("AccNumber", typeof(string));
            RMDataTableRight.Columns.Add("TransCurr", typeof(string));
            RMDataTableRight.Columns.Add("TransAmount", typeof(decimal));
            RMDataTableRight.Columns.Add("TransDate", typeof(DateTime));
            RMDataTableRight.Columns.Add("RRNumber", typeof(int));

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                   + " WHERE RMCateg = @RMCateg AND OpenRecord = 1 ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        //cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = RMDataTableRight.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Select"] = false;

                            RowSelected["TransDescr"] = TransDescr;
                           
                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNumber"] = AccNumber;

                            RowSelected["TransCurr"] = TransCurr;
                            RowSelected["TransAmount"] = TransAmount;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["RRNumber"] = RRNumber;


                            // ADD ROW
                            RMDataTableRight.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ UNMatched records for Right Grid - Visa Authorization 
        // FILL UP A TABLE
        //
        public void ReadRMUnMatchedVisaAuthorRight(string InSourceFileName, string InRMCateg, bool InMatched)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMDataTableRight = new DataTable();
            RMDataTableRight.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            RMDataTableRight.Columns.Add("SeqNo", typeof(int));
            RMDataTableRight.Columns.Add("Select", typeof(bool));
            RMDataTableRight.Columns.Add("TransDescr", typeof(string));   
            RMDataTableRight.Columns.Add("CardNumber", typeof(string));
            RMDataTableRight.Columns.Add("AccNumber", typeof(string));
            RMDataTableRight.Columns.Add("TransCurr", typeof(string));
            RMDataTableRight.Columns.Add("TransAmount", typeof(decimal));
            RMDataTableRight.Columns.Add("TransDate", typeof(DateTime));
            RMDataTableRight.Columns.Add("RRNumber", typeof(int));

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                   + " WHERE RMCateg = @RMCateg AND Matched = @Matched ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        cmd.Parameters.AddWithValue("@Matched", InMatched);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = RMDataTableRight.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Select"] = false;

                            RowSelected["TransDescr"] = TransDescr;          
                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNumber"] = AccNumber;

                            RowSelected["TransCurr"] = TransCurr;
                            RowSelected["TransAmount"] = TransAmount;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["RRNumber"] = RRNumber;


                            // ADD ROW
                            RMDataTableRight.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ UNMatched records for Right Grid - Visa Authorization WITH MATCHING STRING 
        // FILL UP A TABLE
        //
        public void ReadRMUnMatchedVisaAuthorMatchingStringRight(string InSourceFileName, string InRMCateg, bool InMatched, string InMatchingString)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMDataTableRight = new DataTable();
            RMDataTableRight.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            RMDataTableRight.Columns.Add("SeqNo", typeof(int));
            RMDataTableRight.Columns.Add("Select", typeof(bool));
            RMDataTableRight.Columns.Add("TransDescr", typeof(string));
            RMDataTableRight.Columns.Add("CardNumber", typeof(string));
            RMDataTableRight.Columns.Add("AccNumber", typeof(string));
            RMDataTableRight.Columns.Add("TransCurr", typeof(string));
            RMDataTableRight.Columns.Add("TransAmount", typeof(decimal));
            RMDataTableRight.Columns.Add("TransDate", typeof(DateTime));
            RMDataTableRight.Columns.Add("RRNumber", typeof(int));

            SqlString = "SELECT *"
                   + " FROM " + InSourceFileName
                   + " WHERE RMCateg = @RMCateg AND Matched = @Matched AND " + InMatchingString;


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        cmd.Parameters.AddWithValue("@Matched", InMatched);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];
                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];
                            //UnMatchedType = (string)rdr["UnMatchedType"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (int)rdr["T24RefNumber"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = RMDataTableRight.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Select"] = false;
                            RowSelected["TransDescr"] = TransDescr;
                            RowSelected["CardNumber"] = CardNumber;
                            RowSelected["AccNumber"] = AccNumber;
                            RowSelected["TransCurr"] = TransCurr;
                            RowSelected["TransAmount"] = TransAmount;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["RRNumber"] = RRNumber;


                            // ADD ROW
                            RMDataTableRight.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }   
    }
}


