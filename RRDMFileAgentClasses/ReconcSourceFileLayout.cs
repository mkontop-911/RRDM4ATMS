using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;



namespace RRDMFileAgentClasses
{
    public class ReconcSourceFileLayout
    {
        public int SeqNo;
        public string LayoutID;
        public string FieldID;
        public string ColumnName;
        public string FieldType;
        public int StartPos;
        public int Length;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public DataTable LayoutTable = new DataTable();
        public int LayoutTableRows = 0;

        // Uses AgentConnection String
        string connectionString = ConfigurationManager.ConnectionStrings["AgentConnectionString"].ConnectionString;


        public void GetLayoutTable(string LayID)
        {
            RecordFound = false;
            ErrorFound = true;
            ErrorOutput = "An error occured while READING from RRDMSoureFile... ";

            LayoutTable = new DataTable();
            LayoutTable.Clear();
            LayoutTableRows = 0;

            // DATA TABLE ROWS DEFINITION 
            LayoutTable.Columns.Add("SeqNo", typeof(int));
            LayoutTable.Columns.Add("LayoutID", typeof(string));
            LayoutTable.Columns.Add("FieldID", typeof(string));
            LayoutTable.Columns.Add("ColumnName", typeof(string));
            LayoutTable.Columns.Add("FieldType", typeof(string));
            LayoutTable.Columns.Add("StartPos", typeof(int));
            LayoutTable.Columns.Add("Length", typeof(int));

            string SqlString = "SELECT * FROM [dbo].[ReconcSourceFileLayout] WHERE LayoutID = @LayoutID ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@LayoutID", LayID);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            SeqNo = (int)rdr["SeqNo"];
                            LayoutID = (string)rdr["LayoutID"];
                            FieldID = (string)rdr["FieldID"];
                            ColumnName = (string)rdr["ColumnName"];
                            FieldType = (string)rdr["FieldType"];
                            StartPos = (int)rdr["StartPos"];
                            Length = (int)rdr["Length"];

                            DataRow NewRow = LayoutTable.NewRow();

                            NewRow["SeqNo"] = SeqNo;
                            NewRow["LayoutID"] = LayoutID;
                            NewRow["FieldID"] = FieldID;
                            NewRow["ColumnName"] = ColumnName;
                            NewRow["FieldType"] = FieldType;
                            NewRow["StartPos"] = StartPos;
                            NewRow["Length"] = Length;

                            // ADD ROW
                            LayoutTable.Rows.Add(NewRow);
                        }
                        RecordFound = true;
                        ErrorFound = false;
                        ErrorOutput = "";

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    RecordFound = false;
                    ErrorFound = true;
                    ErrorOutput = "An error occured while READING Table in ReconcSourceFileLayout... " + ex.Message;
                }
            }
        }


    
    }
}

