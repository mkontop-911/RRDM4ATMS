using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;

namespace RRDM4ATMsWin
{
    public class AuditTrailClass
    {
        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        public string InsertRecord(string category, string subcategory, string action, string userId, 
            System.Drawing.Bitmap Screenshot, System.Drawing.Bitmap ScreenshotPriorChange, string message)
        {
            byte[] ScreenshotBinary = ImageToByte2(Screenshot);
            byte[] ScreenshotPriorChangeBinary = ImageToByte2(ScreenshotPriorChange);

            string UniqueId = Guid.NewGuid().ToString();

            string cmdinsert = "INSERT INTO [dbo].[AuditTrail] ([UniqueID], [DateTime], [Category], [SubCategory], [TypeOfChange], [UserID], [Screenshot],[ScreenshotPriorChange],[Message]) "
                + "VALUES (@UniqueID, @DateTime, @Category, @SubCategory, @TypeOfChange, @UserID, @Screenshot,@ScreenshotPriorChange,@Message)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UniqueID", UniqueId);

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

                    return UniqueId;
                }
                catch (Exception ex)
                {
                    string exception = ex.ToString();

                    throw new Exception(exception);
                }

        }

        public void UpdateRecord(string UniqueId, string category, string subcategory, string action, string userId, System.Drawing.Bitmap Screenshot, System.Drawing.Bitmap ScreenshotPriorChange, string message)
        {
            byte[] ScreenshotBinary = ImageToByte2(Screenshot);
            byte[] ScreenshotPriorChangeBinary = ImageToByte2(ScreenshotPriorChange);

            string cmddelete = "DELETE FROM [dbo].[AuditTrail] WHERE [UniqueID]='" + UniqueId + "'";
            string cmdinsert = "INSERT INTO [dbo].[AuditTrail] ([UniqueID], [DateTime], [Category], [SubCategory], [TypeOfChange], [UserID], [Screenshot],[ScreenshotPriorChange],[Message]) VALUES (@UniqueID, @DateTime, @Category, @SubCategory, @TypeOfChange, @UserID, @Screenshot,@ScreenshotPriorChange,@Message)";

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

                        cmd.Parameters.AddWithValue("@UniqueID", UniqueId);

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
       
            string SqlString = "SELECT DISTINCT [Category] FROM [dbo].[AuditTrail] ";
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

            string SqlString = "SELECT DISTINCT [SubCategory] FROM [dbo].[AuditTrail] ";
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

            string SqlString = "SELECT DISTINCT [TypeOfChange] FROM [dbo].[AuditTrail] ";
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
