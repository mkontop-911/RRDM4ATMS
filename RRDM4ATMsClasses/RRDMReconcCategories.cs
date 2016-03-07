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
using System.Collections;
using System.IO;


namespace RRDM4ATMs
{
    public class RRDMReconcCategories
    {

        public int SeqNo;

        public int SortId;

        public string CategoryId;
        public string CategoryName ;

        public string Origin ;
        public string TransTypeAtOrigin ;
        public string Product ;
        public string CostCentre ;

        public string GroupIdInFiles ;
        public string FieldName ;
        public int PosStart;
        public int PosEnd;

        public string Currency; 
        public string GlAccount;

        public string VostroBank; 
        public string VostroCurr; 
        public string VostroAcc; 

        public DateTime MatchingDtTm ;
        public int ProcessMode ; 
        public string Periodicity ;
        public string MatchingStatus ;
        public DateTime ReconcDtTm ;
        public string ReconcStatus ;
        public int OutstandingUnMatched; 
        public bool HasOwner; 
        public string OwnerId;
        public bool Active; 
        public string Operator ;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();

        // Define the data table 
        public DataTable ReconcCateg = new DataTable();

        public int TotalSelected;
        public int TotalMatchingDone;
        public int TotalMatchingNotDone; 
        public int TotalReconc ;
        public int TotalNotReconc ;
        public int  TotalUnMatchedRecords ; 

        string SqlString; // Do not delete 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        //
        // Methods 
        // READ ReconcCategories  
        // FILL UP A TABLE
        //
        public void ReadReconcCategories(string InOperator, string InOrigin, string InCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool AllOrigins;
            bool AllRMCategories;

            AllOrigins = false;
            AllRMCategories = false; 

            if (InOrigin == "ALL")
            {
                AllOrigins = true; 
            }
            if (InCategory == "ALL")
            {
                AllRMCategories = true;
            }


            ReconcCateg = new DataTable();
            ReconcCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ReconcCateg.Columns.Add("SeqNo", typeof(int));
            ReconcCateg.Columns.Add("Identity", typeof(string));
            ReconcCateg.Columns.Add("Category-Name", typeof(string));
            ReconcCateg.Columns.Add("Origin", typeof(string));
            ReconcCateg.Columns.Add("TransAtOrigin", typeof(string));
            ReconcCateg.Columns.Add("Product", typeof(string));

            if (AllOrigins == true)
            {
                SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[ReconcCategories] "
                   + " WHERE Operator = @Operator AND Active = 1"
                   + " ORDER BY CategoryId ASC ";
            }
            else
            {
                SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[ReconcCategories] "
                   + " WHERE Operator = @Operator AND Origin = @Origin AND Active = 1"
                   + " ORDER BY CategoryId ASC ";
            }

            if (AllRMCategories == false)
            {
                SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[ReconcCategories] "
                   + " WHERE Operator = @Operator AND Active = 1 AND CategoryId ='" + InCategory + "'"; 
            }
           
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        if (AllOrigins == true)
                        {
                            // Do nothing 
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Origin", InOrigin);
                        }
                         
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SortId = (int)rdr["SortId"];

                            CategoryId = (string)rdr["CategoryId"];

                            CategoryName = (string)rdr["CategoryName"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            GroupIdInFiles = (string)rdr["GroupIdInFiles"];
                            FieldName = (string)rdr["FieldName"]; 

                            PosStart = (int)rdr["PosStart"];
                            PosEnd = (int)rdr["PosEnd"];

                            Currency = (string)rdr["Currency"]; 
                            GlAccount = (string)rdr["GlAccount"];

                            VostroBank = (string)rdr["VostroBank"];
                            VostroCurr = (string)rdr["VostroCurr"];
                            VostroAcc = (string)rdr["VostroAcc"];

                            MatchingDtTm = (DateTime)rdr["MatchingDtTm"];

                            ProcessMode = (int)rdr["ProcessMode"];

                            Periodicity = (string)rdr["Periodicity"];

                            MatchingStatus = (string)rdr["MatchingStatus"];

                            ReconcDtTm = (DateTime)rdr["ReconcDtTm"];

                            ReconcStatus = (string)rdr["ReconcStatus"];

                            OutstandingUnMatched = (int)rdr["OutstandingUnMatched"];

                            HasOwner = (bool)rdr["HasOwner"];

                            OwnerId = (string)rdr["OwnerId"];

                            Active = (bool)rdr["Active"];

                            Operator = (string)rdr["Operator"];

                            if (InCategory == "ALL" & CategoryId != "EWB110")
                            {
                                DataRow RowSelected = ReconcCateg.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["Identity"] = CategoryId;
                                RowSelected["Category-Name"] = CategoryName;
                                RowSelected["Origin"] = Origin;
                                RowSelected["TransAtOrigin"] = TransTypeAtOrigin;
                                RowSelected["Product"] = Product;

                                // ADD ROW
                                ReconcCateg.Rows.Add(RowSelected);
                            }

                            if (InCategory == "EWB110")
                            {
                                DataRow RowSelected = ReconcCateg.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["Identity"] = CategoryId;
                                RowSelected["Category-Name"] = CategoryName;
                                RowSelected["Origin"] = Origin;
                                RowSelected["TransAtOrigin"] = TransTypeAtOrigin;
                                RowSelected["Product"] = Product;

                                // ADD ROW
                                ReconcCateg.Rows.Add(RowSelected);
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }
        //
        // Methods 
        // READ ReconcCategories with exceptions for Allocation of work   
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesForAllocation(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ReconcCateg = new DataTable();
            ReconcCateg.Clear();

            TotalSelected = 0;
            TotalMatchingDone = 0;
            TotalMatchingNotDone = 0; 
            TotalReconc = 0;
            TotalNotReconc = 0;
            TotalUnMatchedRecords = 0; 

            // DATA TABLE ROWS DEFINITION 
            ReconcCateg.Columns.Add("Identity", typeof(string));
            ReconcCateg.Columns.Add("Category_Name", typeof(string));
            ReconcCateg.Columns.Add("OutStanding", typeof(int));     
            ReconcCateg.Columns.Add("HasOwner", typeof(string));
            ReconcCateg.Columns.Add("OwnerId", typeof(string));
            ReconcCateg.Columns.Add("Matching_Dt", typeof(DateTime));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND Active = 1"
                    + " ORDER BY CategoryId ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            SortId = (int)rdr["SortId"];

                            CategoryId = (string)rdr["CategoryId"];

                            CategoryName = (string)rdr["CategoryName"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            GroupIdInFiles = (string)rdr["GroupIdInFiles"];
                            FieldName = (string)rdr["FieldName"];

                            PosStart = (int)rdr["PosStart"];
                            PosEnd = (int)rdr["PosEnd"];

                            Currency = (string)rdr["Currency"];
                            GlAccount = (string)rdr["GlAccount"];

                            VostroBank = (string)rdr["VostroBank"];
                            VostroCurr = (string)rdr["VostroCurr"];
                            VostroAcc = (string)rdr["VostroAcc"];

                            MatchingDtTm = (DateTime)rdr["MatchingDtTm"];

                            ProcessMode = (int)rdr["ProcessMode"];

                            Periodicity = (string)rdr["Periodicity"];

                            MatchingStatus = (string)rdr["MatchingStatus"];

                            ReconcDtTm = (DateTime)rdr["ReconcDtTm"];

                            ReconcStatus = (string)rdr["ReconcStatus"];

                            OutstandingUnMatched = (int)rdr["OutstandingUnMatched"];

                            HasOwner = (bool)rdr["HasOwner"];

                            OwnerId = (string)rdr["OwnerId"];

                            Active = (bool)rdr["Active"];

                            Operator = (string)rdr["Operator"];

                            if (MatchingDtTm.Date == DateTime.Today)
                            {
                                TotalMatchingDone = TotalMatchingDone + 1 ; 
                            }
                            else
                            {
                                TotalMatchingNotDone = TotalMatchingNotDone + 1 ; 
                            }
                            // Read ALL Cycles for this category that has differences and reconciliation didnt start. 
                            Rms.ReadReconcCategoriesMatchingSessionsSpecificCatForExceptions(CategoryId);

                            if (Rms.RecordFound == true)
                            {

                                TotalSelected = TotalSelected + 1;

                                TotalNotReconc = TotalNotReconc + 1 ;

                                TotalUnMatchedRecords = TotalUnMatchedRecords + Rms.TotalUnMatchedRecs; 

                                DataRow RowSelected = ReconcCateg.NewRow();

                                RowSelected["Identity"] = CategoryId;
                                RowSelected["Category_Name"] = CategoryName;

                                RowSelected["OutStanding"] = Rms.TotalUnMatchedRecs;
                                RowSelected["HasOwner"] = HasOwner;
                                RowSelected["OwnerId"] = OwnerId;

                                RowSelected["Matching_Dt"] = MatchingDtTm;

                                // ADD ROW
                                ReconcCateg.Rows.Add(RowSelected);

                            }
                            else
                            {
                                TotalReconc = TotalReconc + 1; 
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Reconc Categories......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ ReconcCategories to report Matching Status  
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesForMatchingStatus(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ReconcCateg = new DataTable();
            ReconcCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ReconcCateg.Columns.Add("Identity", typeof(string));
            ReconcCateg.Columns.Add("Category-Name", typeof(string));
            ReconcCateg.Columns.Add("Matching-Dt", typeof(DateTime));
            ReconcCateg.Columns.Add("Matching-Status", typeof(string));
            ReconcCateg.Columns.Add("OutStanding", typeof(int)); 

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND Active = 1"
                    + " ORDER BY CategoryId ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            SortId = (int)rdr["SortId"];

                            CategoryId = (string)rdr["CategoryId"];

                            CategoryName = (string)rdr["CategoryName"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            GroupIdInFiles = (string)rdr["GroupIdInFiles"];
                            FieldName = (string)rdr["FieldName"];

                            PosStart = (int)rdr["PosStart"];
                            PosEnd = (int)rdr["PosEnd"];

                            Currency = (string)rdr["Currency"];
                            GlAccount = (string)rdr["GlAccount"];

                            VostroBank = (string)rdr["VostroBank"];
                            VostroCurr = (string)rdr["VostroCurr"];
                            VostroAcc = (string)rdr["VostroAcc"];

                            MatchingDtTm = (DateTime)rdr["MatchingDtTm"];

                            ProcessMode = (int)rdr["ProcessMode"]; 

                            Periodicity = (string)rdr["Periodicity"];

                            MatchingStatus = (string)rdr["MatchingStatus"];

                            ReconcDtTm = (DateTime)rdr["ReconcDtTm"];

                            ReconcStatus = (string)rdr["ReconcStatus"];

                            OutstandingUnMatched = (int)rdr["OutstandingUnMatched"];

                            HasOwner = (bool)rdr["HasOwner"];

                            OwnerId = (string)rdr["OwnerId"];

                            Active = (bool)rdr["Active"];

                            Operator = (string)rdr["Operator"];

                            Rms.ReadReconcCategoriesMatchingSessionsSpecificCatForExceptions(CategoryId);

                                TotalSelected = TotalSelected + 1;

                                DataRow RowSelected = ReconcCateg.NewRow();

                                RowSelected["Identity"] = CategoryId;
                                RowSelected["Category-Name"] = CategoryName;
                                RowSelected["Matching-Dt"] = MatchingDtTm;
                                RowSelected["Matching-Status"] = MatchingStatus;
                                RowSelected["OutStanding"] = Rms.TotalUnMatchedRecs;

                                // ADD ROW
                                ReconcCateg.Rows.Add(RowSelected);

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
        // READ ReconcCategory  by Seq no  
        // 
        //
        public void ReadReconcCategorybySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND SeqNo = @SeqNo AND Active = 1"; 
                   
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SortId = (int)rdr["SortId"];

                            CategoryId = (string)rdr["CategoryId"];

                            CategoryName = (string)rdr["CategoryName"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            GroupIdInFiles = (string)rdr["GroupIdInFiles"];
                            FieldName = (string)rdr["FieldName"];

                            PosStart = (int)rdr["PosStart"];
                            PosEnd = (int)rdr["PosEnd"];

                            Currency = (string)rdr["Currency"];
                            GlAccount = (string)rdr["GlAccount"];

                            VostroBank = (string)rdr["VostroBank"];
                            VostroCurr = (string)rdr["VostroCurr"];
                            VostroAcc = (string)rdr["VostroAcc"];

                            MatchingDtTm = (DateTime)rdr["MatchingDtTm"];

                            ProcessMode = (int)rdr["ProcessMode"];

                            Periodicity = (string)rdr["Periodicity"];

                            MatchingStatus = (string)rdr["MatchingStatus"];

                            ReconcDtTm = (DateTime)rdr["ReconcDtTm"];

                            ReconcStatus = (string)rdr["ReconcStatus"];

                            OutstandingUnMatched = (int)rdr["OutstandingUnMatched"];

                            HasOwner = (bool)rdr["HasOwner"];

                            OwnerId = (string)rdr["OwnerId"];

                            Active = (bool)rdr["Active"];

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
        // READ ReconcCategory  by Origin, TransType, Product 
        // 
        //
        public void ReadReconcCategoryToFindCategoryId(string InOrigin, string InTransTypeAtOrigin, string InProduct)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Origin = @Origin AND TransTypeAtOrigin = @TransTypeAtOrigin AND Product = @Product AND Active = 1"; 


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Origin", InOrigin);
                        cmd.Parameters.AddWithValue("@TransTypeAtOrigin", InTransTypeAtOrigin);
                        cmd.Parameters.AddWithValue("@Product", InProduct);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SortId = (int)rdr["SortId"];

                            CategoryId = (string)rdr["CategoryId"];

                            CategoryName = (string)rdr["CategoryName"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            GroupIdInFiles = (string)rdr["GroupIdInFiles"];
                            FieldName = (string)rdr["FieldName"];

                            PosStart = (int)rdr["PosStart"];
                            PosEnd = (int)rdr["PosEnd"];

                            Currency = (string)rdr["Currency"];
                            GlAccount = (string)rdr["GlAccount"];

                            VostroBank = (string)rdr["VostroBank"];
                            VostroCurr = (string)rdr["VostroCurr"];
                            VostroAcc = (string)rdr["VostroAcc"];

                            MatchingDtTm = (DateTime)rdr["MatchingDtTm"];

                            ProcessMode = (int)rdr["ProcessMode"];

                            Periodicity = (string)rdr["Periodicity"];

                            MatchingStatus = (string)rdr["MatchingStatus"];

                            ReconcDtTm = (DateTime)rdr["ReconcDtTm"];

                            ReconcStatus = (string)rdr["ReconcStatus"];

                            OutstandingUnMatched = (int)rdr["OutstandingUnMatched"];

                            HasOwner = (bool)rdr["HasOwner"];

                            OwnerId = (string)rdr["OwnerId"];

                            Active = (bool)rdr["Active"];

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
        // READ ReconcCategories by Cat Id   
        // 
        //
        public void ReadReconcCategorybyCategId(string InOperator, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND CategoryId = @CategoryId AND Active = 1"; 


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SortId = (int)rdr["SortId"];

                            CategoryId = (string)rdr["CategoryId"];

                            CategoryName = (string)rdr["CategoryName"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            GroupIdInFiles = (string)rdr["GroupIdInFiles"];
                            FieldName = (string)rdr["FieldName"];

                            PosStart = (int)rdr["PosStart"];
                            PosEnd = (int)rdr["PosEnd"];

                            Currency = (string)rdr["Currency"];
                            GlAccount = (string)rdr["GlAccount"];

                            VostroBank = (string)rdr["VostroBank"];
                            VostroCurr = (string)rdr["VostroCurr"];
                            VostroAcc = (string)rdr["VostroAcc"];

                            MatchingDtTm = (DateTime)rdr["MatchingDtTm"];

                            ProcessMode = (int)rdr["ProcessMode"];

                            Periodicity = (string)rdr["Periodicity"];

                            MatchingStatus = (string)rdr["MatchingStatus"];

                            ReconcDtTm = (DateTime)rdr["ReconcDtTm"];

                            ReconcStatus = (string)rdr["ReconcStatus"];

                            OutstandingUnMatched = (int)rdr["OutstandingUnMatched"];

                            HasOwner = (bool)rdr["HasOwner"];

                            OwnerId = (string)rdr["OwnerId"];

                            Active = (bool)rdr["Active"];

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
        // READ ReconcCategories by Cat Name 
        // 
        //
        public void ReadReconcCategorybyCategName(string InOperator, string InCategoryName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND CategoryName = @CategoryName AND Active = 1"; 


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryName", InCategoryName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            SortId = (int)rdr["SortId"];

                            CategoryId = (string)rdr["CategoryId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            CategoryName = (string)rdr["CategoryName"];

                            GroupIdInFiles = (string)rdr["GroupIdInFiles"];
                            FieldName = (string)rdr["FieldName"];

                            PosStart = (int)rdr["PosStart"];
                            PosEnd = (int)rdr["PosEnd"];

                            Currency = (string)rdr["Currency"];
                            GlAccount = (string)rdr["GlAccount"];

                            VostroBank = (string)rdr["VostroBank"];
                            VostroCurr = (string)rdr["VostroCurr"];
                            VostroAcc = (string)rdr["VostroAcc"];

                            MatchingDtTm = (DateTime)rdr["MatchingDtTm"];

                            ProcessMode = (int)rdr["ProcessMode"];

                            Periodicity = (string)rdr["Periodicity"];

                            MatchingStatus = (string)rdr["MatchingStatus"];

                            ReconcDtTm = (DateTime)rdr["ReconcDtTm"];

                            ReconcStatus = (string)rdr["ReconcStatus"];

                            OutstandingUnMatched = (int)rdr["OutstandingUnMatched"];

                            HasOwner = (bool)rdr["HasOwner"];

                            OwnerId = (string)rdr["OwnerId"];

                            Active = (bool)rdr["Active"];

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
        // Find Total for this USER
        // 
        //
        public int TotalCatForUser; 

        public void ReadReconcCategoriesNumberForUser(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalCatForUser = 0; 

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE OwnerId = @OwnerId  AND Active = 1";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@OwnerId", InUserId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalCatForUser = TotalCatForUser + 1; 

                            
                            SeqNo = (int)rdr["SeqNo"];

                            CategoryId = (string)rdr["CategoryId"];

                            CategoryName = (string)rdr["CategoryName"];


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

        // GET Array List Occurance Nm 
        //
        public ArrayList GetCategories(string InOperator)
        {
            ArrayList OccurancesListNm = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * FROM [ATMS].[dbo].[ReconcCategories] WHERE Operator = @Operator  AND Active = 1 Order by CategoryId ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                      
                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;

                            CategoryId = (string)rdr["CategoryId"];

                            CategoryName = (string)rdr["CategoryName"];

                            string CatIdAndName = CategoryId + " " + CategoryName;

                            OccurancesListNm.Add(CatIdAndName);
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
                    ErrorOutput = "An error occured in RECONC Categories Class............. " + ex.Message;
                    //    log4net.Config.XmlConfigurator.Configure();
                    //    RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                    //                                              "ArrayList GetParamOccurancesNm()");
                }

            return OccurancesListNm;
        }

        // Insert Category
        //
        public void InsertCategory()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcCategories]"
                    + "([CategoryId], [CategoryName],  "
                    + " [Origin], [TransTypeAtOrigin], [Product], [CostCentre],"
                    + " [Periodicity], [GroupIdInFiles], [FieldName],  "
                    + " [PosStart], [PosEnd], [Currency], [GlAccount], [VostroBank], [VostroCurr], [VostroAcc],[OwnerId],"
                    + " [Operator] )"
                    + " VALUES (@CategoryId, @CategoryName,"
                    + " @Origin, @TransTypeAtOrigin, @Product, @CostCentre,"
                    + " @Periodicity, @GroupIdInFiles, @FieldName,"
                    + " @PosStart, @PosEnd, @Currency, @GlAccount, @VostroBank, @VostroCurr, @VostroAcc, @OwnerId,"
                    + " @Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@CategoryName", CategoryName);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);
                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@CostCentre", CostCentre);
                        cmd.Parameters.AddWithValue("@Periodicity", Periodicity);

                        cmd.Parameters.AddWithValue("@GroupIdInFiles", GroupIdInFiles);
                        cmd.Parameters.AddWithValue("@FieldName", FieldName);
                        cmd.Parameters.AddWithValue("@PosStart", PosStart);
                        cmd.Parameters.AddWithValue("@PosEnd", PosEnd);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@GlAccount", GlAccount);
                      
                        cmd.Parameters.AddWithValue("@VostroBank", VostroBank);
                        cmd.Parameters.AddWithValue("@VostroCurr", VostroCurr);
                        cmd.Parameters.AddWithValue("@VostroAcc", VostroAcc);

                        cmd.Parameters.AddWithValue("@OwnerId", OwnerId);
                        
                        cmd.Parameters.AddWithValue("@Operator", Operator);

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
                    ErrorOutput = "An error occured in Reconc Category Class............. " + ex.Message;
                }
        }

        // UPDATE Category
        // 
        public void UpdateCategory(string InOperator, string InCategoryId )
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategories] SET "
                            + " CategoryId = @CategoryId, CategoryName = @CategoryName, "
                            + " Origin = @Origin, TransTypeAtOrigin = @TransTypeAtOrigin, "
                            + " Product = @Product, CostCentre = @CostCentre, "
                            + " Periodicity = @Periodicity, GroupIdInFiles = @GroupIdInFiles, FieldName = @FieldName, "
                            + " PosStart = @PosStart, PosEnd = @PosEnd ,"
                            + " Currency = @Currency,GlAccount = @GlAccount,"
                            + " VostroBank = @VostroBank, VostroCurr = @VostroCurr, VostroAcc = @VostroAcc,"
                            + " ReconcDtTm = @ReconcDtTm, ReconcStatus = @ReconcStatus, "
                            + " OutstandingUnMatched = @OutstandingUnMatched, "
                            + " HasOwner = @HasOwner, OwnerId = @OwnerId,  Active = @Active  "
                            + " WHERE CategoryId = @CategoryId", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@CategoryName", CategoryName);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);
                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@CostCentre", CostCentre);
                        cmd.Parameters.AddWithValue("@Periodicity", Periodicity);

                        cmd.Parameters.AddWithValue("@GroupIdInFiles", GroupIdInFiles);
                        cmd.Parameters.AddWithValue("@FieldName", FieldName);
                        cmd.Parameters.AddWithValue("@PosStart", PosStart);
                        cmd.Parameters.AddWithValue("@PosEnd", PosEnd);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@GlAccount", GlAccount);

                        cmd.Parameters.AddWithValue("@VostroBank", VostroBank);
                        cmd.Parameters.AddWithValue("@VostroCurr", VostroCurr);
                        cmd.Parameters.AddWithValue("@VostroAcc", VostroAcc);

                        cmd.Parameters.AddWithValue("@ReconcDtTm", ReconcDtTm);
                        cmd.Parameters.AddWithValue("@ReconcStatus", ReconcStatus);

                        cmd.Parameters.AddWithValue("@OutstandingUnMatched", OutstandingUnMatched);

                        cmd.Parameters.AddWithValue("@HasOwner", HasOwner);
                        cmd.Parameters.AddWithValue("@OwnerId", OwnerId);
                        cmd.Parameters.AddWithValue("@Active", Active);  
                       
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
                    ErrorOutput = "An error occured in Reconc Category Class............. " + ex.Message;
                }
        }

        //
        // DELETE Category
        //
        public void DeleteCategory(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReconcCategories] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

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
                    ErrorOutput = "An error occured in Reconc Category Class............. " + ex.Message;
                }

        }
    }
}
