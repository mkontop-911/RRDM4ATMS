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
    public class RRDMPerformanceTraceClass
    {

// For critical proceses a record is inputed to see performance 

        public int RecordNo;
        public string BankId;
 
        public string ProcessNm;
        public string AtmNo;
        public DateTime StartDT;
        public DateTime EndDT;
        public int Duration;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ records for a particular ATM 

        public void ReadPerformanceTrace(string InBankId, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[PerformanceTrace] "
          + " WHERE BankId = @BankId and AtmNo = @AtmNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Details
                            RecordNo = (int)rdr["RecordNo"];

                            BankId = (string)rdr["BankId"];
                  

                            ProcessNm = (string)rdr["ProcessNm"];
                            AtmNo = (string)rdr["AtmNo"];

                            StartDT = (DateTime)rdr["StartDT"];
                            EndDT = (DateTime)rdr["EndDT"];

                            Duration = (int)rdr["Duration"];

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
                    ErrorOutput = "An error occured in Performance Trace Class............. " + ex.Message;

                }
        }

        // READ record for a particular Record No

        public void ReadPerformanceTraceRecNo(int InRecordNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[PerformanceTrace] "
          + " WHERE RecordNo = @RecordNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RecordNo", InRecordNo);
                       
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Details
                            RecordNo = (int)rdr["RecordNo"];

                            BankId = (string)rdr["BankId"];

                            ProcessNm = (string)rdr["ProcessNm"];
                            AtmNo = (string)rdr["AtmNo"];

                            StartDT = (DateTime)rdr["StartDT"];
                            EndDT = (DateTime)rdr["EndDT"];

                            Duration = (int)rdr["Duration"];

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
                    ErrorOutput = "An error occured in Performance Trace Class............. " + ex.Message;

                }
        }
        // READ to find Max Record Number conditional on Bank Id and AtmNo  

        public void ReadMaxRecordNo(string InBankId, string InAtmNo, string InProcessNm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT RecordNo = MAX(RecordNo)"
                   + " FROM [dbo].[PerformanceTrace]"
                   + " WHERE BankId = @BankId AND AtmNo = @AtmNo AND ProcessNm LIKE @ProcessNm"; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        InProcessNm = "%" + InProcessNm + "%"; 
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ProcessNm", InProcessNm);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            RecordNo = (int)rdr["RecordNo"];
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
                    ErrorOutput = "An error occured in Performance Trace Class............. " + ex.Message;

                }
        }
        //
        // Insert NEW Performance Trace 
        //
        public void InsertPerformanceTrace(string InBankId,string InOperator,  string InProcessNm, string InAtmNo, DateTime InStartDT)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [dbo].[PerformanceTrace]"   
             + " ([BankId], "
             + " [ProcessNm],[AtmNo] ,[StartDT], [Operator]) " 
             +  "  VALUES "
             + " (@BankId,  "
             + " @ProcessNm, @AtmNo , @StartDT, @Operator) "; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                 //       cmd.Parameters.AddWithValue("@RecordNo", RecordNo);
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                 //       cmd.Parameters.AddWithValue("@Prive", InPrive);

                        cmd.Parameters.AddWithValue("@ProcessNm", InProcessNm);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@StartDT", InStartDT);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Performance Trace Class............. " + ex.Message;

                }
        }


        // UPDATE Performance Trace upon completion 
        // 
        public void UpdatePerformanceTrace(int InRecordNo, int InCounter)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 
            // Calculate Duration in seconds 
            TimeSpan DurationTemp = EndDT - StartDT;
        //    string DurationString = (DurationTemp.TotalMinutes*60 + DurationTemp.TotalSeconds).ToString();

            Duration = Convert.ToInt32(DurationTemp.TotalSeconds);

            if (Duration == 0 )
            {
                using (SqlConnection conn =
               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("DELETE FROM [dbo].[PerformanceTrace] "
                                + " WHERE RecordNo = @RecordNo ", conn))
                        {
                            cmd.Parameters.AddWithValue("@RecordNo", InRecordNo);

                            //rows number of record got updated

                            int rows = cmd.ExecuteNonQuery();
                            //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                            //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        ErrorFound = true;
                        ErrorOutput = "An error occured in Performance Trace Class............. " + ex.Message;

                    }
            }
            else
            {
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE [dbo].[PerformanceTrace] SET "
                                + " EndDT = @EndDT, Duration = @Duration, Counter = @Counter  "
                                + " WHERE RecordNo = @RecordNo ", conn))
                        {
                            cmd.Parameters.AddWithValue("@RecordNo", InRecordNo);

                            cmd.Parameters.AddWithValue("@EndDT", EndDT);
                            cmd.Parameters.AddWithValue("@Duration", Duration);
                            cmd.Parameters.AddWithValue("@Counter", InCounter);

                            //rows number of record got updated

                            int rows = cmd.ExecuteNonQuery();
                            //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                            //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        ErrorFound = true;
                        ErrorOutput = "An error occured in Performance Trace Class............. " + ex.Message;

                    }
            }

           
        }
       
    }
}
