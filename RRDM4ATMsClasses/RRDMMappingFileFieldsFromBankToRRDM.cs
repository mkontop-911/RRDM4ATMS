using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace RRDM4ATMs
{
    public class RRDMMappingFileFieldsFromBankToRRDM : Logger
    {
        public RRDMMappingFileFieldsFromBankToRRDM() : base() { }

        public int SeqNo;

        public string SourceFileId;
        public string SourceFieldNm;

        public bool IsUniversal;

        public string TargetFieldType;

        public string SourceFieldValue; // Is a transformation routine that creates the target value

        public int SourceFieldPositionStart;
        public int SourceFieldPositionEnd;

        public string TargetFieldNm;
        public string TargetFieldValue;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();

        // Define the data table 
        public DataTable DataTableFileFields = new DataTable();

        //Alecos
        public struct SourceFileLayoutStruct
        {
            public bool Exists;
            public int SeqNo;
            public string SourceFileId;
            public string SourceFieldNm;
            public bool IsUniversal;
            public string SourceFieldValue;
            public int SourceFieldPositionStart;
            public int SourceFieldPositionEnd;
            public string TargetFieldNm;
            public string TargetFieldType;
            public string TargetFieldValue;

        };
        public DataTable SourceFilelayout = new DataTable();


        public int TotalSelected;
        string SqlString; // Do not delete 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;
        //
        // Methods 
        // READ fields  
        // FILL UP A TABLE
        //
        public void ReadFileFields(string InSourceFileId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableFileFields = new DataTable();
            DataTableFileFields.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            DataTableFileFields.Columns.Add("PositionStart", typeof(int));
            DataTableFileFields.Columns.Add("PositionEnd", typeof(int));
            //DataTableFileFields.Columns.Add("SourceFileId", typeof(string));
            DataTableFileFields.Columns.Add("SourceFieldNm", typeof(string));
            DataTableFileFields.Columns.Add("IsDone", typeof(bool));
            DataTableFileFields.Columns.Add("TargetFieldNm", typeof(string));

            DataTableFileFields.Columns.Add("SourceFieldValue", typeof(string));
            DataTableFileFields.Columns.Add("TargetFieldValue", typeof(string));
            DataTableFileFields.Columns.Add("SeqNo", typeof(int));

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                   + " WHERE SourceFileId = @SourceFileId"
                   + " ORDER BY SourceFieldPositionStart, SeqNo ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];

                            IsUniversal = (bool)rdr["IsUniversal"];

                            TargetFieldType = (string)rdr["TargetFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];
                            SourceFieldValue = (string)rdr["SourceFieldValue"];

                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];

                            DataRow RowSelected = DataTableFileFields.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            //RowSelected["SourceFileId"] = SourceFileId;
                            if (SourceFieldNm == "")
                            {
                                RowSelected["SourceFieldNm"] = "Empty";
                                
                            }
                            else
                            {
                                RowSelected["SourceFieldNm"] = SourceFieldNm;
                            }
                            if (SourceFieldNm == "Please Fill")
                            {
                                RowSelected["IsDone"] = false;
                            }
                            else
                            {
                                RowSelected["IsDone"] = true;
                            }

                            RowSelected["TargetFieldNm"] = TargetFieldNm;
                            RowSelected["PositionStart"] = SourceFieldPositionStart;
                            RowSelected["PositionEnd"] = SourceFieldPositionEnd;

                            if (SourceFieldValue == "")
                            {
                                RowSelected["SourceFieldValue"] = "Empty";
                            }
                            else
                            {
                                RowSelected["SourceFieldValue"] = SourceFieldValue;
                            }

                            if (TargetFieldValue == "")
                            {
                                RowSelected["TargetFieldValue"] = "Empty";
                            }
                            else
                            {
                                RowSelected["TargetFieldValue"] = TargetFieldValue;
                            }

                            // ADD ROW
                            DataTableFileFields.Rows.Add(RowSelected);

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

        // Alecos
        // Methods 
        // READ fields  
        // FILL UP A TABLE
        //
        public void ReadSourceFileLayout(string InSourceFileId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SourceFilelayout = new DataTable();
            SourceFilelayout.Clear();

            TotalSelected = 0;


            // DATA TABLE ROWS DEFINITION 
            SourceFilelayout.Columns.Add("SeqNo", typeof(int));
            SourceFilelayout.Columns.Add("SourceFileId", typeof(string));
            SourceFilelayout.Columns.Add("SourceFieldNm", typeof(string));
            SourceFilelayout.Columns.Add("IsUniversal", typeof(bool));
            SourceFilelayout.Columns.Add("SourceFieldValue", typeof(string));
            SourceFilelayout.Columns.Add("SourceFieldPositionStart", typeof(int));
            SourceFilelayout.Columns.Add("SourceFieldPositionEnd", typeof(int));
            SourceFilelayout.Columns.Add("TargetFieldNm", typeof(string));
            SourceFilelayout.Columns.Add("TargetFieldType", typeof(string));
            SourceFilelayout.Columns.Add("TargetFieldValue", typeof(string));

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                   + " WHERE SourceFileId = @SourceFileId"
                   + " ORDER BY SeqNo ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            SourceFileId = (string)rdr["SourceFileId"];
                            SourceFieldNm = (string)rdr["SourceFieldNm"];
                            IsUniversal = (bool)rdr["IsUniversal"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];
                            SourceFieldValue = (string)rdr["SourceFieldValue"];
                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldType = (string)rdr["TargetFieldType"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];

                            DataRow RowSelected = SourceFilelayout.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["SourceFileId"] = SourceFileId;
                            if (SourceFieldNm == "")
                            {
                                RowSelected["SourceFieldNm"] = "Empty";
                                RowSelected["IsUniversal"] = true;
                            }
                            else
                            {
                                RowSelected["SourceFieldNm"] = SourceFieldNm;
                                RowSelected["IsUniversal"] = IsUniversal;

                            }
                            RowSelected["SourceFieldValue"] = SourceFieldValue;
                            RowSelected["SourceFieldPositionStart"] = SourceFieldPositionStart;
                            RowSelected["SourceFieldPositionEnd"] = SourceFieldPositionEnd;
                            RowSelected["TargetFieldNm"] = TargetFieldNm;
                            RowSelected["TargetFieldType"] = TargetFieldType;
                            RowSelected["TargetFieldValue"] = TargetFieldValue;


                            // ADD ROW
                            SourceFilelayout.Rows.Add(RowSelected);

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
        // READ Mapping table and delete these not in Universal 
        //
        public void ReadTableAndDeleteNotInUniversal(string InSource_File_Id, string InTableStructureId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMUniversalTableFieldsDefinition Ufd = new RRDMUniversalTableFieldsDefinition(); 

            if (InSource_File_Id == "Atms_Journals_Txns")
            {
                return;
            }

            SqlString = "SELECT *"
                  + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                  + " WHERE SourceFileId = @SourceFileId  "
                  + " ORDER BY SeqNo ASC ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", InSource_File_Id);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];
                            IsUniversal = (bool)rdr["IsUniversal"];

                            TargetFieldType = (string)rdr["TargetFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];
                            SourceFieldValue = (string)rdr["SourceFieldValue"];

                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];
                            string WSelectCriteria = " WHERE FieldDBName='"+ TargetFieldNm+"'"
                                                    + " AND TableStructureId ='"+ InTableStructureId + "'";
                            Ufd.ReadUniversalTableFieldsDefinitionBySelectionCriteria(WSelectCriteria);
                            if (Ufd.RecordFound == true)
                            {
                                // OK
                            }
                            else
                            {
                                // Delete record
                                DeleteFileFieldRecord(SeqNo); 
                            }

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
        // Methods 
        // READ By Source Field 

        //
        public void ReadFileFieldsBySourceField(string InSourceFileId, string InSourceFieldNm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                   + " WHERE SourceFileId = @SourceFileId AND SourceFieldNm = @SourceFieldNm   "
                   + " ORDER BY SeqNo ASC ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);
                        cmd.Parameters.AddWithValue("@SourceFieldNm", InSourceFieldNm);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];
                            IsUniversal = (bool)rdr["IsUniversal"];

                            TargetFieldType = (string)rdr["TargetFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];
                            SourceFieldValue = (string)rdr["SourceFieldValue"];

                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];


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
        // Methods 
        // READ By Source Fileld 

        //
        public void ReadTableFieldsByTargetField(string InSourceFileId, string InTargetFieldNm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                   + " WHERE SourceFileId = @SourceFileId AND TargetFieldNm = @TargetFieldNm   "
                   + "  ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);
                        cmd.Parameters.AddWithValue("@TargetFieldNm", InTargetFieldNm);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];
                            IsUniversal = (bool)rdr["IsUniversal"];

                            TargetFieldType = (string)rdr["TargetFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];
                            SourceFieldValue = (string)rdr["SourceFieldValue"];

                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];


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



        // REFORMAT 
        public void ReadBULK_ALL_AND_REFORMAT_Fields(string InSourceFileId, string InSourceFieldNm, int InLoadedAtRMCycle)
        {

            string BulkTable_ALL = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InSourceFileId + "_ALL";
            string RTN_String = " , CAST(@SourceFieldNm As decimal(18,2))"; 
            //SqlString = " SELECT SeqNo ," + ",CAST(TotalNetValue As decimal(18,2))" + " as FormatedField"
            //        + " FROM " + BulkTable_ALL 
            //        + "  ";
            SqlString = " SELECT SeqNo ," + " , CAST(@SourceFieldNm As decimal(18,2))" + " as FormatedField"
                   + " FROM " + BulkTable_ALL
                   + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFieldId", InSourceFileId);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);
                        // cmd.Parameters.AddWithValue("@TargetFieldNm", InTargetFieldNm);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            string Reformated = (string)rdr["FormatedField"]; 

                        }

                        // Close Reader
                        rdr.Close();
                    }
                    // Close conn
                    conn.Close();

                    FullCreatedSqlCommand_ITMX =
                       SQL_HeaderWithParameterFields
                       + SQL_ToBody
                       + CloseParanthesis + Environment.NewLine
                       + SQL_SelectWithParameters
                       + SQL_SelectBody
                       + SQL_From;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }


        string SQL_HeaderWithParameterFields;
        string SQL_ToBody;
        string CloseParanthesis = " ) ";
        string SQL_SelectWithParameters;
        string SQL_SelectBody;
        string SQL_From;
        public string FullCreatedSqlCommand_ITMX;
       
        //
        public void ReadTableFieldsBySourceFile_Reformat_AND_CREATE_COMMAND(string InBulkFileId, string InSourceFileId, int InRMCycle)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SQL_HeaderWithParameterFields = "";
            SQL_ToBody = "";
            SQL_SelectWithParameters = "";
            SQL_SelectBody = "";
            SQL_From = "";
            FullCreatedSqlCommand_ITMX = "";

            SQL_HeaderWithParameterFields = " INSERT INTO [RRDM_Reconciliation_ITMX].[dbo]." + InSourceFileId + Environment.NewLine
                                      + "(" + Environment.NewLine
                                       + " OriginFileName " + Environment.NewLine
                                      + ",Origin " + Environment.NewLine
                                     // + " ,TerminalType " + Environment.NewLine
                                         + ",Operator " + Environment.NewLine
                                           + ",LoadedAtRMCycle " + Environment.NewLine
                                      ;
            SQL_SelectWithParameters = " SELECT " + Environment.NewLine
                                       // + "(" + Environment.NewLine
                                       + " @OriginFileName " + Environment.NewLine
                                      + ", @Origin " + Environment.NewLine
                                     // + " , @TerminalType " + Environment.NewLine
                                         + ", @Operator " + Environment.NewLine
                                           + ", @LoadedAtRMCycle " + Environment.NewLine
                                      ;
            SQL_From = " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InSourceFileId + Environment.NewLine;


            SqlString = " SELECT *"
                   + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                   + " WHERE SourceFileId = @SourceFileId "
                   + " AND SourceFieldNm <>'Please Fill'"
                   + " ORDER BY TargetFieldNm "
                   //+ " AND SourceFieldValue <>'Rtn_Default'"
                   + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);
                        // cmd.Parameters.AddWithValue("@TargetFieldNm", InTargetFieldNm);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];
                            IsUniversal = (bool)rdr["IsUniversal"];

                            TargetFieldType = (string)rdr["TargetFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];
                            SourceFieldValue = (string)rdr["SourceFieldValue"];
                            // Reformat field 
                            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
                            if (SourceFieldNm != "Not_Present_but_has_value")
                            {
                                if (SourceFieldValue.Substring(0, 3) == "Rtn")
                                {
                                    if (SourceFieldValue == "Rtn_NONE")
                                    {
                                        // do nothing
                                    }
                                    else
                                    {
                                        Bio.Read_SOURCE_Table_AND_Reformat_Field(SourceFileId,
                                     SourceFieldNm, SourceFieldValue, InRMCycle);
                                    }

                                }
                            }
                            
                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];

                            // Prepare fields
                            //
                            if (SourceFieldNm == "Not_Present_but_has_value")
                            {
                                // Do nothing
                            }
                            else
                            {
                                // Apply IsNULL
                                if (SourceFieldValue == "Rtn_NONE")
                                {
                                    if (TargetFieldType == "Character")
                                    {
                                        SourceFieldNm = "ISNULL(" + SourceFieldNm + ", '')";
                                    }
                                    if (TargetFieldType == "Decimal" || TargetFieldType == "Numeric")
                                    {
                                        SourceFieldNm = "ISNULL(" + SourceFieldNm + ", 0)";
                                    }
                                    if (TargetFieldType == "Date" || TargetFieldType == "DateTime")
                                    {
                                        SourceFieldNm = "ISNULL(" + SourceFieldNm + ", '1900-01-01')";
                                    }
                                }
                                else
                                {
                                    if (TargetFieldType == "Character")
                                    {
                                        SourceFieldValue = "ISNULL(" + SourceFieldValue + ", '')";
                                    }
                                    if (TargetFieldType == "Decimal" || TargetFieldType == "Numeric")
                                    {
                                        SourceFieldValue = "ISNULL(" + SourceFieldValue + ", 0)";
                                    }
                                    if (TargetFieldType == "Date" || TargetFieldType == "DateTime")
                                    {
                                        SourceFieldValue = "ISNULL(" + SourceFieldValue + ", '1900-01-01')";
                                    }
                                }
                                

                            }


                            // Make this based on field characteristics 
                            //
                            //textBoxRTN.Text.Substring(0, 3) != "Rtn"
                            if (SourceFieldNm == "Not_Present_but_has_value")
                            {
                                SQL_SelectBody = SQL_SelectBody + " , " + "'" + SourceFieldValue + "'" + Environment.NewLine;
                            }
                            else
                            {
                                if (SourceFieldValue.Substring(0, 3) == "Rtn")
                                {
                                    // Default or Routine 
                                    // Field
                                    SQL_SelectBody = SQL_SelectBody + " , " + SourceFieldNm + Environment.NewLine;
                                }
                                else
                                {
                                    // Value 
                                    SQL_SelectBody = SQL_SelectBody + " , " + SourceFieldValue + Environment.NewLine;
                                }
                            }

                            
                            SQL_ToBody = SQL_ToBody + " , " + TargetFieldNm + Environment.NewLine;

                        }

                        // Close Reader
                        rdr.Close();
                    }
                    // Close conn
                    conn.Close();

                    FullCreatedSqlCommand_ITMX =
                       SQL_HeaderWithParameterFields
                       + SQL_ToBody
                       + CloseParanthesis + Environment.NewLine
                       + SQL_SelectWithParameters
                       + SQL_SelectBody
                       + SQL_From;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    CatchDetails(ex);
                    return;
                }
        }


        //
        // Methods 
        // READ ReconcCategory  by Seq no  
        // 
        //
        public void ReadReadFileFieldsbySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                  + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                  + " WHERE SeqNo = @SeqNo ";

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

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];
                            IsUniversal = (bool)rdr["IsUniversal"];
                            TargetFieldType = (string)rdr["TargetFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];
                            SourceFieldValue = (string)rdr["SourceFieldValue"];

                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];


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
        // Methods 
        // READ ReconcCategory  by Seq no  
        // 
        //
        public string ReadCheckForBrokenSequence(string InSourceFileId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int OldEnd = 0;

            string BrokenSequence = "";

            SqlString = "SELECT *"
                  + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                  + " WHERE SourceFileId = @SourceFileId "
                  + " Order By SourceFieldPositionStart";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];
                            IsUniversal = (bool)rdr["IsUniversal"];
                            TargetFieldType = (string)rdr["TargetFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];

                            if (OldEnd > 0)
                            {
                                if (OldEnd + 1 != SourceFieldPositionStart)
                                {
                                    // Problem 

                                    BrokenSequence = "Broken Sequence Just Before Source Field Name " + SourceFieldNm;

                                }
                            }

                            OldEnd = SourceFieldPositionEnd;

                            SourceFieldValue = (string)rdr["SourceFieldValue"];

                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];


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
            return BrokenSequence;
        }

        //
        // Methods 
        // READ CheckWithinOtherRange
        // 
        //
        public string ReadCheckWithinOtherRange(string InSourceFileId, string InSourceFieldNm, int InFrom, int InTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string Illegal = "";

            SqlString = "SELECT *"
                  + " FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                  + " WHERE SourceFileId = @SourceFileId "
                  + " Order By SourceFieldPositionStart";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", InSourceFileId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            SourceFileId = (string)rdr["SourceFileId"];

                            SourceFieldNm = (string)rdr["SourceFieldNm"];
                            IsUniversal = (bool)rdr["IsUniversal"];
                            TargetFieldType = (string)rdr["TargetFieldType"];
                            SourceFieldPositionStart = (int)rdr["SourceFieldPositionStart"];
                            SourceFieldPositionEnd = (int)rdr["SourceFieldPositionEnd"];

                            if (InFrom >= SourceFieldPositionStart & InFrom <= SourceFieldPositionEnd & InSourceFieldNm != SourceFieldNm)
                            {
                                // Illegal 
                                Illegal = "The From Position falls within range of " + SourceFieldNm;
                                break;
                            }


                            if (InTo >= SourceFieldPositionStart & InTo <= SourceFieldPositionEnd & InSourceFieldNm != SourceFieldNm)
                            {
                                // Illegal 
                                Illegal = "The To Position falls within range of " + SourceFieldNm;
                                break;
                            }

                            SourceFieldValue = (string)rdr["SourceFieldValue"];

                            TargetFieldNm = (string)rdr["TargetFieldNm"];
                            TargetFieldValue = (string)rdr["TargetFieldValue"];

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
            return Illegal;
        }

        // Insert File Field Record 
        //
        public void InsertFileFieldRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[MatchingBankToRRDMFileFields]"
                    + "([SourceFileId], [SourceFieldNm],  [IsUniversal], "
                    + " [TargetFieldType], [SourceFieldPositionStart], [SourceFieldPositionEnd], "
                    + " [SourceFieldValue],  "
                    + " [TargetFieldNm], [TargetFieldValue]"
                    + "  )"
                    + " VALUES (@SourceFileId, @SourceFieldNm, @IsUniversal, "
                    + " @TargetFieldType, @SourceFieldPositionStart, @SourceFieldPositionEnd, "
                    + " @SourceFieldValue,"
                    + " @TargetFieldNm, @TargetFieldValue "
                    + " )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@SourceFileId", SourceFileId);

                        cmd.Parameters.AddWithValue("@SourceFieldNm", SourceFieldNm);
                        cmd.Parameters.AddWithValue("@IsUniversal", IsUniversal);

                        cmd.Parameters.AddWithValue("@TargetFieldType", TargetFieldType);
                        cmd.Parameters.AddWithValue("@SourceFieldPositionStart", SourceFieldPositionStart);
                        cmd.Parameters.AddWithValue("@SourceFieldPositionEnd", SourceFieldPositionEnd);

                        cmd.Parameters.AddWithValue("@SourceFieldValue", SourceFieldValue);

                        cmd.Parameters.AddWithValue("@TargetFieldNm", TargetFieldNm);
                        cmd.Parameters.AddWithValue("@TargetFieldValue", TargetFieldValue);

                        //rows number of record got updated

                        cmd.ExecuteNonQuery();


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

        // UPDATE Category
        // 
        public void UpdateFileFieldRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingBankToRRDMFileFields] SET "
                              + " SourceFileId = @SourceFileId, SourceFieldNm = @SourceFieldNm,  IsUniversal = @IsUniversal,"
                              + " TargetFieldType = @TargetFieldType, SourceFieldPositionStart = @SourceFieldPositionStart, "
                              + " SourceFieldPositionEnd = @SourceFieldPositionEnd, "
                              + " SourceFieldValue = @SourceFieldValue, "
                              + "TargetFieldNm = @TargetFieldNm, TargetFieldValue = @TargetFieldValue  "
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@SourceFileId", SourceFileId);

                        cmd.Parameters.AddWithValue("@SourceFieldNm", SourceFieldNm);

                        cmd.Parameters.AddWithValue("@IsUniversal", IsUniversal);

                        cmd.Parameters.AddWithValue("@TargetFieldType", TargetFieldType);
                        cmd.Parameters.AddWithValue("@SourceFieldPositionStart", SourceFieldPositionStart);
                        cmd.Parameters.AddWithValue("@SourceFieldPositionEnd", SourceFieldPositionEnd);

                        cmd.Parameters.AddWithValue("@SourceFieldValue", SourceFieldValue);

                        cmd.Parameters.AddWithValue("@TargetFieldNm", TargetFieldNm);
                        cmd.Parameters.AddWithValue("@TargetFieldValue", TargetFieldValue);

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
                    CatchDetails(ex);
                }
        }

        //
        // DELETE Category
        //
        public void DeleteFileFieldRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingBankToRRDMFileFields] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        //rows number of record got updated

                        cmd.ExecuteNonQuery();


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

       
    }
}
