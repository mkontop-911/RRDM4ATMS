using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
//using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public class RRDMHelp : Logger
    {
        public RRDMHelp() : base() { }

        public string Category;
        public string Name;
        public string Text;
        public string AttachmentPath;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        public void InsertHelpItem()
        {

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[Help]"
                    + "( [ID], [CATEGORY], [ITEM], "
                    + " [TEXT], [ATTACHMENTPATH])"
                    + " VALUES (NEWID() ,@Category, @Name, @Text, @AttachmentPath)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@Category", Category);
                        cmd.Parameters.AddWithValue("@Name", Name);

                        cmd.Parameters.AddWithValue("@Text", Text);
                        cmd.Parameters.AddWithValue("@AttachmentPath", AttachmentPath);



                       cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    string exception = ex.ToString();
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    conn.Close();
                }
        }

        public DataSet GetHelpItems()
        {
            SqlDataAdapter da;
            DataSet ds = new DataSet();

            string SqlString = "SELECT * FROM [ATMS].[dbo].[Help]"
                   + " ORDER BY [CATEGORY], [ITEM]";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        da = new SqlDataAdapter(SqlString,conn);
                    }

                    // Close conn
                    da.Fill(ds, "Help");
                    conn.Close();
                    da.Dispose();

                   
                    return ds;
                }
                catch (Exception ex)
                {
                    da = new SqlDataAdapter();
                    da.Fill(ds, "Help");
                    conn.Close();
                    da.Dispose();

                    string exception = ex.ToString();
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    return ds;
                }
        }

        // UPDATE HelpTEXT 
        // 
        public void UpdateHelpText(string InID, string InTEXT )
        {

            //ErrorFound = false;
            //ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[Help] SET "
                            + " TEXT = @TEXT "
                            + " WHERE ID =  @ID", conn))
                    {
                        cmd.Parameters.AddWithValue("ID", InID);
                        cmd.Parameters.AddWithValue("@TEXT", InTEXT);


                     cmd.ExecuteNonQuery();
                      

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    RRDMLog4Net Log = new RRDMLog4Net();

                    StringBuilder WParameters = new StringBuilder();

                    WParameters.Append("User : ");
                    WParameters.Append("NotAssignYet");
                    WParameters.Append(Environment.NewLine);

                    WParameters.Append("ATMNo : ");
                    WParameters.Append("NotDefinedYet");
                    WParameters.Append(Environment.NewLine);

                    string Logger = "RRDM4Atms";
                    string Parameters = WParameters.ToString();

                    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                    if (Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                                 + " . Application will be aborted! Call controller to take care. ");
                    }
                }
        }

        public void DeleteItem(string IdToDelete)
        {

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[Help] "
                            + " WHERE ID =  @ID ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", IdToDelete);

                        cmd.ExecuteNonQuery();
                      

                    }
                    // Close conn
                    conn.Close();
                }

                catch (Exception ex)
                {
                    string exception = ex.ToString();
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    conn.Close();

                }

        }

    }
}
