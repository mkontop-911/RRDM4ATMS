using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
//using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMSessionsPhysicalInspection : Logger
    {
        public RRDMSessionsPhysicalInspection() : base() { }

        public int SeqNo;

        public string AtmNo;
        public int SesNo;
        public bool Selection;
        public string InspectionId; 
        public string PhysicalInspectionNm;
        public DateTime DateTimeUpdated; 

        // Define the data table 
        public DataTable PhysicalInspectionDataTable = new DataTable();
        public int TotalSelected;

        public bool InspectionAlert; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;
        readonly string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //
        // READ Selection Physical Inspection records and fill Table 
        //
        public void ReadPhysicalInspectionRecordsToFillDataTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            InspectionAlert = false; 


            PhysicalInspectionDataTable = new DataTable();
            PhysicalInspectionDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION
            PhysicalInspectionDataTable.Columns.Add("Selection", typeof(bool));
            PhysicalInspectionDataTable.Columns.Add("PhysicalInspectionNm", typeof(string));
            PhysicalInspectionDataTable.Columns.Add("InspectionId", typeof(string));
            PhysicalInspectionDataTable.Columns.Add("AtmNo", typeof(string));
            PhysicalInspectionDataTable.Columns.Add("SesNo", typeof(int));
            PhysicalInspectionDataTable.Columns.Add("SeqNo", typeof(int));      

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[SessionsPhysicalInspection]"
                + " WHERE " + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@Ope*/rator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            Selection = (bool)rdr["Selection"];
                            InspectionId = (string)rdr["InspectionId"];
                            PhysicalInspectionNm = (string)rdr["PhysicalInspectionNm"];
                            DateTimeUpdated = (DateTime)rdr["DateTimeUpdated"];

                            if (Selection == false) InspectionAlert = true;

                            //FILL TABLE 
                            DataRow RowSelected = PhysicalInspectionDataTable.NewRow();
                        
                            RowSelected["Selection"] = Selection;
                            RowSelected["PhysicalInspectionNm"] = PhysicalInspectionNm;
                            RowSelected["InspectionId"] = InspectionId;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["SesNo"] = SesNo;
                            RowSelected["SeqNo"] = SeqNo;

                            // ADD ROW
                            PhysicalInspectionDataTable.Rows.Add(RowSelected);

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
        // READ to find out if Inspection Alert 
        //
        public void ReadPhysicalInspectionRecordsToSeeIfAlert(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            InspectionAlert = false;

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[SessionsPhysicalInspection]"
                + " WHERE " + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@Ope*/rator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            Selection = (bool)rdr["Selection"];
                            InspectionId = (string)rdr["InspectionId"];
                            PhysicalInspectionNm = (string)rdr["PhysicalInspectionNm"];
                            DateTimeUpdated = (DateTime)rdr["DateTimeUpdated"];

                            if (Selection == false) InspectionAlert = true;

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
        // READ Physical Inspection Record by Inspection Id 
        //
        public void ReadSessionsPhysicalInspectionByInspectionId(string InAtmNo, 
                                                         int InSesNo, string InInspectionId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[SessionsPhysicalInspection]"
               + " WHERE AtmNo = @AtmNo AND SesNo = @SesNo AND InspectionId = @InspectionId";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                        cmd.Parameters.AddWithValue("@InspectionId", InInspectionId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            Selection = (bool)rdr["Selection"];
                            InspectionId = (string)rdr["InspectionId"];
                            PhysicalInspectionNm = (string)rdr["PhysicalInspectionNm"];
                            DateTimeUpdated = (DateTime)rdr["DateTimeUpdated"];

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

        public void ReadSessionsPhysicalInspectionBySelection(string InAtmNo,
                                                 int InSesNo, bool InSelection)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[SessionsPhysicalInspection] "
               + " WHERE AtmNo = @AtmNo AND SesNo = @SesNo AND Selection = @Selection ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);
                        cmd.Parameters.AddWithValue("@Selection", InSelection);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            Selection = (bool)rdr["Selection"];
                            InspectionId = (string)rdr["InspectionId"];
                            PhysicalInspectionNm = (string)rdr["PhysicalInspectionNm"];
                            DateTimeUpdated = (DateTime)rdr["DateTimeUpdated"];

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
        // INSERT Physical Inspection Records For the ATM and Ses No
        // 
        public void InsertPhysicalInspectionRecords(string InAtmNo, int InSessionNo, int InLoadedAtRMCycle)
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            RRDMGasParameters Gp = new RRDMGasParameters(); 
            Ac.ReadAtm(InAtmNo);
            string ParamId = "212"; // Physical Inspection Data 
            Gp.ReadAllOccurancesNmsForSpecificParDataTable(Ac.BankId, ParamId);
            //Gp.DataTableOccurancesIds
            int I = 0;

            while (I <= (Gp.DataTableOccurancesIds.Rows.Count - 1))
            {

                RecordFound = true;

                // GET Table fields - Line by Line
                //

                AtmNo = InAtmNo;
                SesNo = InSessionNo;
                Selection = false;

                InspectionId = (string)Gp.DataTableOccurancesIds.Rows[I]["OccuranceId"];
                PhysicalInspectionNm = (string)Gp.DataTableOccurancesIds.Rows[I]["OccuranceNm"];

                InsertSessionsPhysicalInspectionRecord(InLoadedAtRMCycle);

                I++; // Read Next entry of the table 
            }
        }



        // Insert File Field Record 
        //
        public void InsertSessionsPhysicalInspectionRecord(int InLoadedAtRMCycle)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[SessionsPhysicalInspection] "
                    + "([AtmNo], [SesNo],[Selection], [InspectionId], "
                    + " [PhysicalInspectionNm],[LoadedAtRMCycle] ,[DateTimeUpdated] )"
                    + " VALUES (@AtmNo, @SesNo, @Selection, @InspectionId, "
                    + " @PhysicalInspectionNm, @LoadedAtRMCycle, @DateTimeUpdated )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);
                        cmd.Parameters.AddWithValue("@Selection", Selection);
                        cmd.Parameters.AddWithValue("@InspectionId", InspectionId);
                        cmd.Parameters.AddWithValue("@PhysicalInspectionNm", PhysicalInspectionNm);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);
                        cmd.Parameters.AddWithValue("@DateTimeUpdated", DateTime.Now);
                        
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

                    CatchDetails(ex);
                }
        }

        // UPDATE Physical Inspection record 
        // 
        public void UpdateSessionsPhysicalInspectionRecord(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[SessionsPhysicalInspection] SET "
                              + " Selection = @Selection, DateTimeUpdated = @DateTimeUpdated  "
                              + " WHERE SeqNo = @SeqNo", conn))

                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@Selection", Selection);
                        cmd.Parameters.AddWithValue("@DateTimeUpdated", DateTime.Now);

                        int rows = cmd.ExecuteNonQuery();
                    

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

        // UPDATE Physical Inspection record 
        // 
        public void UpdateSessionsPhysicalInspectionRecord(string InAtmNo, int InSesNo, bool InSelection )
        {
            ErrorFound = false;
            ErrorOutput = "";
            int rows; 
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[SessionsPhysicalInspection] SET "
                                       + " Selection = @Selection, DateTimeUpdated = @DateTimeUpdated  "
                                       + " WHERE AtmNo = @AtmNo AND SesNo = @SesNo ", conn))

                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@Selection", InSelection);
                        cmd.Parameters.AddWithValue("@DateTimeUpdated", DateTime.Now);

                        rows = cmd.ExecuteNonQuery();


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

