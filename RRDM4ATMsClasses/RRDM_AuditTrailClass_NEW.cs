using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Data;
using System.Drawing;

namespace RRDM4ATMsClasses
{
    public class RRDM_AuditTrailClass_NEW
    {

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public DataTable AuditTrailDataTable = new DataTable();

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public int SeqNo;
        public DateTime DateTime;
        public string Category;
        public string SubCategory;
        public string TypeOfChange;
        public string UserId;
        public byte[] Screenshot;
        public Bitmap ScreenshotPriorChange;
        //  Screenshot]
        //,[ScreenshotPriorChange]
        public string Message;


        public void ReadBranchesAtmAndFillTable(string InSelectionCriteria, DateTime InFromDt, DateTime InToDt)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            
            AuditTrailDataTable = new DataTable();
            AuditTrailDataTable.Clear();

            AuditTrailDataTable.Columns.Add("UniqueId", typeof(int)); //1
            
            AuditTrailDataTable.Columns.Add("Category", typeof(string)); // 2
            AuditTrailDataTable.Columns.Add("Sub Category", typeof(string)); //3
            AuditTrailDataTable.Columns.Add("Type Of Change", typeof(string)); //4
            AuditTrailDataTable.Columns.Add("User Id", typeof(string)); // 5
            AuditTrailDataTable.Columns.Add("DateTime", typeof(DateTime)); //6
            AuditTrailDataTable.Columns.Add("Screenshot", typeof(byte[])); // B Screen shot //7
            AuditTrailDataTable.Columns.Add("Action", typeof(string)); //8
            AuditTrailDataTable.Columns.Add("ScreenshotPriorChange", typeof(byte[])); // A Screen shot //9
            AuditTrailDataTable.Columns.Add("Message", typeof(string)); // 10

            string SqlString = "";
            if (InFromDt != NullPastDate & InToDt != NullPastDate)
            {
                if (InSelectionCriteria != "")
                {
                    SqlString = " SELECT * "
            + " FROM [ATMS].[dbo].[AuditTrail_NEW] "
              + InSelectionCriteria
              + " AND (DateTime BETWEEN @FromDt AND @ToDt) ";
                }
                else
                {
                    SqlString = " SELECT * "
            + " FROM [ATMS].[dbo].[AuditTrail_NEW] "
              + " WHERE (DateTime BETWEEN @FromDt AND @ToDt) ";
                }

            }
            else
            {
                SqlString = " SELECT * "
              + " FROM [ATMS].[dbo].[AuditTrail_NEW] "
                + InSelectionCriteria
               + " ORDER BY SeqNo "
                ;
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                        cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
      
                            SeqNo = (int)rdr["SeqNo"];
                            
                            Category = (string)rdr["Category"];
                            SubCategory = (string)rdr["SubCategory"];
                            TypeOfChange = (string)rdr["TypeOfChange"];
                            UserId = (string)rdr["UserId"];
                           
                            DateTime = (DateTime)rdr["DateTime"];
                            Screenshot = (byte[])rdr["Screenshot"];
                            byte[] ScreenshotPriorChange2 = (byte[])rdr["ScreenshotPriorChange"];
                            Message = (string)rdr["Message"];

                            // SELECT ROW
                            DataRow RowSelected = AuditTrailDataTable.NewRow();


                            RowSelected["UniqueId"] = SeqNo;
                            RowSelected["Category"] = Category;
                            RowSelected["Sub Category"] = SubCategory;
                            RowSelected["Type Of Change"] = TypeOfChange;
                            RowSelected["User Id"] = UserId;
                            
                            RowSelected["DateTime"] = DateTime;
                            RowSelected["Screenshot"] = Screenshot;
                            RowSelected["Action"] = "View";
                            RowSelected["ScreenshotPriorChange"] = ScreenshotPriorChange2;
                            RowSelected["Message"] = Message;

                            // ADD ROW
                            AuditTrailDataTable.Rows.Add(RowSelected);

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

                    string exception = ex.ToString();

                    //throw new Exception(exception);

                }
        }


        public int InsertRecord(string category, string subcategory, string action, string userId,
            System.Drawing.Bitmap Screenshot, System.Drawing.Bitmap ScreenshotPriorChange, string message)
        {
            byte[] ScreenshotBinary = ImageToByte2(Screenshot);
            byte[] ScreenshotPriorChangeBinary = ImageToByte2(ScreenshotPriorChange);

            int SeqNo = 0;

            //  string UniqueId = Guid.NewGuid().ToString();

            string cmdinsert = "INSERT INTO [dbo].[AuditTrail_NEW] ( [DateTime], [Category], [SubCategory], [TypeOfChange], [UserID], [Screenshot],[ScreenshotPriorChange],[Message]) "
                + "VALUES ( @DateTime, @Category, @SubCategory, @TypeOfChange, @UserID, @Screenshot,@ScreenshotPriorChange,@Message)"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";
            ;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        //cmd.Parameters.AddWithValue("@UniqueID", UniqueId);

                        cmd.Parameters.AddWithValue("@DateTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Category", category);
                        cmd.Parameters.AddWithValue("@SubCategory", subcategory);
                        cmd.Parameters.AddWithValue("@TypeOfChange", action);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Screenshot", ScreenshotBinary);
                        cmd.Parameters.AddWithValue("@ScreenshotPriorChange", ScreenshotPriorChangeBinary);
                        cmd.Parameters.AddWithValue("@Message", message);
                        //sqlParam.DbType = DbType.Binary;

                        //rows number of record got updated

                        SeqNo = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();

                    return SeqNo;
                }
                catch (Exception ex)
                {
                    string exception = ex.ToString();

                    throw new Exception(exception);
                }

        }

        public void UpdateRecord(int UniqueId, string category, string subcategory, string action, string userId, System.Drawing.Bitmap Screenshot, System.Drawing.Bitmap ScreenshotPriorChange, string message)
        {
            byte[] ScreenshotBinary = ImageToByte2(Screenshot);
            byte[] ScreenshotPriorChangeBinary = ImageToByte2(ScreenshotPriorChange);

            string cmddelete = "DELETE FROM [dbo].[AuditTrail_NEW] WHERE [SeqNo]='" + UniqueId + "'";
            string cmdinsert = "INSERT INTO [dbo].[AuditTrail_NEW] ([DateTime], [Category], [SubCategory], [TypeOfChange], [UserID], [Screenshot],[ScreenshotPriorChange],[Message]) "
                + " VALUES (@DateTime, @Category, @SubCategory, @TypeOfChange, @UserID, @Screenshot,@ScreenshotPriorChange,@Message)"
                 + " SELECT CAST(SCOPE_IDENTITY() AS int)";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    using (SqlCommand cmd =

                      new SqlCommand(cmddelete, conn))
                    {
                        int rows = cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        // cmd.Parameters.AddWithValue("@UniqueID", UniqueId);

                        cmd.Parameters.AddWithValue("@DateTime", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Category", category);
                        cmd.Parameters.AddWithValue("@SubCategory", subcategory);
                        cmd.Parameters.AddWithValue("@TypeOfChange", action);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Screenshot", ScreenshotBinary);
                        cmd.Parameters.AddWithValue("@ScreenshotPriorChange", ScreenshotPriorChangeBinary);
                        cmd.Parameters.AddWithValue("@Message", message);
                        //sqlParam.DbType = DbType.Binary;

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    string exception = ex.ToString();

                    throw new Exception(exception);
                }

        }

        private static byte[] ImageToByte2(System.Drawing.Bitmap img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        public ArrayList GetCategory()
        {
            ArrayList categoryList = new ArrayList();
            categoryList.Add("");

            string SqlString = "SELECT DISTINCT [Category] FROM [dbo].[AuditTrail_NEW] ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            categoryList.Add(rdr[0].ToString().TrimEnd());
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    string exception = ex.ToString();

                }

            return categoryList;
        }

        public ArrayList GetSubCategory()
        {
            ArrayList SubCategoryList = new ArrayList();
            SubCategoryList.Add("");

            string SqlString = "SELECT DISTINCT [SubCategory] FROM [dbo].[AuditTrail_NEW] ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            SubCategoryList.Add(rdr[0].ToString().TrimEnd());
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    string exception = ex.ToString();

                }

            return SubCategoryList;
        }

        public ArrayList GetAction()
        {
            ArrayList actionList = new ArrayList();
            actionList.Add("");

            string SqlString = "SELECT DISTINCT [TypeOfChange] FROM [dbo].[AuditTrail_NEW] ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            actionList.Add(rdr[0].ToString().TrimEnd());
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    string exception = ex.ToString();

                }

            return actionList;
        }
    }
}
