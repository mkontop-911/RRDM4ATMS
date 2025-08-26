using System;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMAtmsMinMax : Logger
    {
        public RRDMAtmsMinMax() : base() { }
        // Variables
      
        public int RMCycle;
        public string AtmNo;
      
        public DateTime MinMaxDtTwoTables;
        public DateTime MinMaxDtThreeTables;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 


        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        // Reader Fields 
        private void MinMaxFields(SqlDataReader rdr)
        {
            RMCycle = (int)rdr["RMCycle"];
            AtmNo = (string)rdr["AtmNo"];

            MinMaxDtTwoTables = (DateTime)rdr["MinMaxDtTwoTables"];
            MinMaxDtThreeTables = (DateTime)rdr["MinMaxDtThreeTables"];
       
        }

        // Methods 
        // READ TableATMsPhys
        // 
        public void ReadRRDMAtmsMinMaxSpecific(int InRMCycle, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * "
          + " FROM [ATMS].[dbo].[AtmsMinMaxWorking] "
          + " WHERE RMCycle=@RMCycle AND AtmNo=@AtmNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            MinMaxFields(rdr);

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
        // UPDATE MINMAXTABLE 
        //
        public void DeleteTableATMsMinMax()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string SQLCmd = "DELETE FROM [ATMS].[dbo].[AtmsMinMaxWorking] "
               + "    "
               + " "
               ;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //  cmd.Parameters.AddWithValue("@TransDate", InDate);

                        cmd.CommandTimeout = 350;  // seconds

                         cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                    // CatchDetails(ex);
                }


            //using (SqlConnection conn =
            //    new SqlConnection(connectionString))
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd = 
            //            new SqlCommand("UPDATE [ATMS].[dbo].[AtmsMinMaxWorking] SET "
            //        + " [RMCycle]=@RMCycle, "
            //        + " [AtmNo]=@AtmNo,"
            //        + " [MinMaxDtTwoTables]=@MinMaxDtTwoTables,"
            //        + " [MinMaxDtThreeTables]=@MinMaxDtThreeTables "
            //        + " WHERE RMCycle= @RMCycle AND AtmNo = @AtmNo ", conn))
            //        {
            //            cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
            //            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                      
            //            cmd.Parameters.AddWithValue("@MinMaxDtTwoTables", MinMaxDtTwoTables);
            //            cmd.Parameters.AddWithValue("@MinMaxDtThreeTables", MinMaxDtThreeTables);

            //            // Execute and check success 
            //            int rows = cmd.ExecuteNonQuery();
            //            if (rows > 0)
            //            {
            //                // outcome = " ATMs Table UPDATED ";
            //            }

            //        }
            //        // Close conn
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn.Close();

            //        CatchDetails(ex);
            //    }

            ////  return outcome;

        }
        // INSERT New Record MinMax
        public void InsertToMinMax(int InRMCycle, string InAtmNo, DateTime InMinMaxDtTwoTables
            , DateTime InMinMaxDtThreeTables)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert =
            "INSERT INTO [ATMS].[dbo].[AtmsMinMaxWorking] "
                + " ([RMCycle],  "
                + " [AtmNo], "
                + " [MinMaxDtTwoTables], "
                + "[MinMaxDtThreeTables])"
                + " VALUES (@RMCycle, "
                + " @AtmNo,"
                + " @MinMaxDtTwoTables, "
                + "@MinMaxDtThreeTables)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@MinMaxDtTwoTables", InMinMaxDtTwoTables);
                        cmd.Parameters.AddWithValue("@MinMaxDtThreeTables", InMinMaxDtThreeTables);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                         //   outcome = " Record Inserted ";
                        }

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

    }
}
