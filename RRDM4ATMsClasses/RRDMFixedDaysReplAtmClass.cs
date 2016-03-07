using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
//using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMFixedDaysReplAtmClass
    {
        // DECLARE FIELDS
     
        public string BankId;
        public string AtmNo; 
       
        public DateTime NextDate;
      
        public string Day;
        public string Type;

        public DateTime SameAs;
      
        public string SDay;
        public string SType;

        public decimal Suggested;
        public string Correction; 
        public decimal Final; 

        public DateTime DateInsert;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ 

        public void ReadFixedDaysReplAtm(string InOperator, string InAtmNo, DateTime InDate)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[FixedDaysReplAtm] "
           + " WHERE (AtmNo = @AtmNo) AND (Operator = @Operator) AND (NextDate = @NextDate)";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@NextDate", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            BankId = (string)rdr["BankId"];
                            AtmNo = (string)rdr["AtmNo"];

                            NextDate = (DateTime)rdr["NextDate"];

                   //         Prive = (bool)rdr["Prive"];
                         
                            Day = (string)rdr["Day"];
                            Type = (string)rdr["Type"];

                            SameAs = (DateTime)rdr["SameAs"];
                     
                            SDay = (string)rdr["SDay"];
                            SType = (string)rdr["SType"];

                            Suggested = (decimal)rdr["Suggested"];
                            Correction = (string)rdr["Correction"];
                            Final = (decimal)rdr["Final"];

                            DateInsert = (DateTime)rdr["DateInsert"];

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
                    ErrorOutput = "An error occured in FixedDaysReplAtmClass............. " + ex.Message;

                }
        }

        // Insert 
        //
        public void InsertFixedDaysReplAtm(string InOperator, string InAtmNo, DateTime InDate)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 
           
       string cmdinsert = "INSERT INTO [dbo].[FixedDaysReplAtm] ([BankId], [AtmNo], [NextDate], [Day], [Type],"
           + " [SameAs], [SDay], [SType], [Suggested], [Correction], [Final], [DateInsert],[Operator] )"
           +" VALUES (@BankId, @AtmNo, @NextDate, @Day, @Type,"
           + " @SameAs, @SDay, @SType, @Suggested, @Correction, @Final, @DateInsert, @Operator)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@NextDate", InDate);

             
                        cmd.Parameters.AddWithValue("@Day", Day);
                        cmd.Parameters.AddWithValue("@Type", Type);

                        cmd.Parameters.AddWithValue("@SameAs", SameAs);
                        
                        cmd.Parameters.AddWithValue("@SDay", SDay);
                        cmd.Parameters.AddWithValue("@SType", SType);

                        cmd.Parameters.AddWithValue("@Suggested", Suggested);

                        cmd.Parameters.AddWithValue("@Correction", Correction);
                        cmd.Parameters.AddWithValue("@Final", Final);

                        cmd.Parameters.AddWithValue("@DateInsert", DateInsert);

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                       

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
                    ErrorOutput = "An error occured in FixedDaysReplAtmClass............. " + ex.Message;

                }
        }


        // UPDATE wit values
        // 
        public void UpdateFixedDaysReplAtm(string InOperator, string InAtmNo, DateTime InDate)
        {
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE [dbo].[FixedDaysReplAtm] SET "
                            + "[BankId] = @BankId, [AtmNo] = @AtmNo, [NextDate] = @NextDate,"
                             + "[Day] = @Day, [Type] = @Type, [SameAs] = @SameAs, [SDay] = @SDay, [SType] = @SType,"
                             + "[Suggested] = @Suggested, [Correction] = @Correction, [Final] = @Final, [DateInsert] = @DateInsert"
                             + "  WHERE AtmNo = @AtmNo AND Operator = @Operator AND NextDate = @NextDate", conn)) 
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@NextDate", InDate);

               
                        cmd.Parameters.AddWithValue("@Day", Day);
                        cmd.Parameters.AddWithValue("@Type", Type);

                        cmd.Parameters.AddWithValue("@SameAs", SameAs);

                        cmd.Parameters.AddWithValue("@SDay", SDay);
                        cmd.Parameters.AddWithValue("@SType", SType);

                        cmd.Parameters.AddWithValue("@Suggested", Suggested);

                        cmd.Parameters.AddWithValue("@Correction", Correction);
                        cmd.Parameters.AddWithValue("@Final", Final);

                        cmd.Parameters.AddWithValue("@DateInsert", DateInsert);


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
                    ErrorOutput = "An error occured in FixedDaysReplAtmClass............. " + ex.Message;

                }
        }

        // DELETE Fixed day    
        //
        public void DeleteFixedDaysReplAtm(string InOperator, string InAtmNo, DateTime InDate)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[FixedDaysReplAtm] "
                            + "   WHERE AtmNo = @AtmNo AND Operator = @Operator AND NextDate = @NextDate ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@NextDate", InDate);

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
                    ErrorOutput = "An error occured in FixedDaysReplAtmClass............. " + ex.Message;

                }
        }
    }
}
