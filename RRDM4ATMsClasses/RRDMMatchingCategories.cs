using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;


namespace RRDM4ATMs
{
    public class RRDMMatchingCategories : Logger
    {
        public RRDMMatchingCategories() : base() { }

        public int SeqNo;

        public bool TWIN;

        public string CategoryId;
        public string CategoryName;

        public string Origin;

        public string TransTypeAtOrigin;

        public int TargetSystemId;
        public string TargetSystemNm;

        public bool GetsNotOwnBINS; // This category gets not own BINs 

        public string RunningJobGroup;

        public string Product;
        public string CostCentre;


        public bool Pos_Type;

        //  public bool Pos_Type;
        public int UnMatchedForWorkingDays;
        public int UnMatchedForCalendarDays;



        public string Currency;

        public string EntityA;
        //public bool DR;
        //public bool CR;
        public string EntityB;

        public string GlAccount;

        public string VostroBank;
        public string VostroCurr;
        public string VostroAcc;
        public string InternalAcc;

        public DateTime MatchingDtTm;
        public int ProcessMode;
        public string Periodicity;
        public DateTime NextMatchingDt;
        public string MatchingStatus;
        //public DateTime ReconcDtTm;
        //public string ReconcStatus;
        //public int OutstandingUnMatched;
        public bool ReconcMaster;
        public bool HasOwner;  // Leave it for NOSTRO RECONCILIATION 
        public string OwnerId; // Leave it for NOSTRO RECONCILIATION
        public bool Active;
        public string Operator;

        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();

        // Define the data table 
        public DataTable TableMatchingCateg = new DataTable();

        public DataTable TableMatchingCateg_Matching_Status = new DataTable();

        public int TotalSelected;
        public int TotalMatchingDone;
        public int TotalMatchingNotDone;
        public int TotalReconc;
        public int TotalNotReconc;
        public int TotalUnMatchedRecords;

        string SqlString; // Do not delete 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // Matching Cat Reader Fields 
        private void MatchingCatReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            TWIN = (bool)rdr["TWIN"];

            CategoryId = (string)rdr["CategoryId"];

            CategoryName = (string)rdr["CategoryName"];

            Origin = (string)rdr["Origin"];
            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];

            TargetSystemId = (int)rdr["TargetSystemId"];
            TargetSystemNm = (string)rdr["TargetSystemNm"];

            GetsNotOwnBINS = (bool)rdr["GetsNotOwnBINS"];

            RunningJobGroup = (string)rdr["RunningJobGroup"];
            Product = (string)rdr["Product"];
            CostCentre = (string)rdr["CostCentre"];

            //   GroupIdInFiles = (string)rdr["GroupIdInFiles"];
            Pos_Type = (bool)rdr["Pos_Type"];

            UnMatchedForWorkingDays = (int)rdr["UnMatchedForWorkingDays"];
            UnMatchedForCalendarDays = (int)rdr["UnMatchedForCalendarDays"];

            Currency = (string)rdr["Currency"];
            EntityA = (string)rdr["EntityA"];

            //DR = (bool)rdr["DR"];
            //CR = (bool)rdr["CR"];

            EntityB = (string)rdr["EntityB"];

            GlAccount = (string)rdr["GlAccount"];

            VostroBank = (string)rdr["VostroBank"];
            VostroCurr = (string)rdr["VostroCurr"];
            VostroAcc = (string)rdr["VostroAcc"];
            InternalAcc = (string)rdr["InternalAcc"];

            MatchingDtTm = (DateTime)rdr["MatchingDtTm"];

            ProcessMode = (int)rdr["ProcessMode"];

            Periodicity = (string)rdr["Periodicity"];

            NextMatchingDt = (DateTime)rdr["NextMatchingDt"];

            MatchingStatus = (string)rdr["MatchingStatus"]; ;

            ReconcMaster = (bool)rdr["ReconcMaster"];

            HasOwner = (bool)rdr["HasOwner"];

            OwnerId = (string)rdr["OwnerId"];

            Active = (bool)rdr["Active"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ MatchingCategories  
        // FILL UP A TABLE
        //
        public void ReadMatchingCategoriesAndFillTable(string InOperator, string InOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string ParId;
            RRDMGasParameters Gp = new RRDMGasParameters();

            TableMatchingCateg = new DataTable();
            TableMatchingCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableMatchingCateg.Columns.Add("SeqNo", typeof(int));
            TableMatchingCateg.Columns.Add("Identity", typeof(string));
            TableMatchingCateg.Columns.Add("Category-Name", typeof(string));
            TableMatchingCateg.Columns.Add("Is POS_Type", typeof(bool));
            TableMatchingCateg.Columns.Add("Days W", typeof(int));
            TableMatchingCateg.Columns.Add("Days C", typeof(int));
            TableMatchingCateg.Columns.Add("Is TWIN", typeof(bool));
            TableMatchingCateg.Columns.Add("Is Active", typeof(bool));
            TableMatchingCateg.Columns.Add("Origin", typeof(string));
            TableMatchingCateg.Columns.Add("TransAtOrigin", typeof(string));
            TableMatchingCateg.Columns.Add("Product", typeof(string));

            if (InOrigin == "Nostro - Vostro")
            {
                SqlString = "SELECT * "
                      + " FROM [ATMS].[dbo].[MatchingCategories] "
                      + " WHERE Operator = @Operator AND Origin = @Origin "
                      + " ORDER BY CategoryId ASC ";
            }

            if (InOrigin == "ETISALAT" || InOrigin == "QAHERA" || InOrigin == "IPN" || InOrigin == "EGATE" )
            {
                SqlString = "SELECT * "
                   + " FROM [ATMS].[dbo].[MatchingCategories] "
                   + " WHERE Operator = @Operator and RunningJobGroup = @Origin  "
                   + " ORDER BY CategoryId ASC ";
            }


            if (InOrigin == "ALL_Active" || InOrigin == "ALL")
            {
                SqlString = "SELECT * "
                   + " FROM [ATMS].[dbo].[MatchingCategories] "
                   + " WHERE Operator = @Operator AND Active = 1 AND RunningJobGroup <> 'e_MOBILE' AND RunningJobGroup <> 'Nostro Reconc' "
                   + " ORDER BY CategoryId ASC ";
            }
            if (InOrigin == "ALL_Not_Active")
            {
                SqlString = "SELECT * "
                   + " FROM [ATMS].[dbo].[MatchingCategories] "
                   + " WHERE Operator = @Operator AND Active = 0 AND RunningJobGroup <> 'e_MOBILE' AND RunningJobGroup <> 'Nostro Reconc' "
                   + " ORDER BY CategoryId ASC ";
            }
            if (InOrigin == "Active And Not Active")
            {
                SqlString = "SELECT * "
                   + " FROM [ATMS].[dbo].[MatchingCategories] "
                   + " WHERE Operator = @Operator  AND RunningJobGroup <> 'e_MOBILE' AND RunningJobGroup <> 'Nostro Reconc'  "
                   + " ORDER BY CategoryId ASC ";
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
                      
                        cmd.Parameters.AddWithValue("@Origin", InOrigin);
                        

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            MatchingCatReaderFields(rdr);

                            ParId = "825"; // Check if not to be shown category

                            Gp.ReadParametersSpecificNm(InOperator, ParId, CategoryId);

                            if (Gp.RecordFound == true)
                            {
                                // Not To be shown Category
                            }
                            else
                            {
                                DataRow RowSelected = TableMatchingCateg.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["Identity"] = CategoryId;
                                RowSelected["Category-Name"] = CategoryName;

                                RowSelected["Is POS_Type"] = Pos_Type;
                                RowSelected["Days W"] = UnMatchedForWorkingDays;
                                RowSelected["Days C"] = UnMatchedForCalendarDays;

                                RowSelected["Is TWIN"] = TWIN;

                                RowSelected["Is Active"] = Active;

                                RowSelected["Origin"] = Origin;
                                RowSelected["TransAtOrigin"] = TransTypeAtOrigin;
                                RowSelected["Product"] = Product;

                                // ADD ROW
                                TableMatchingCateg.Rows.Add(RowSelected);
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
        // READ MatchingCategories  
        // FILL UP A TABLE
        ////
        //public void ReadMatchingCategoriesAndFillTable_e_MOBILE(string InOperator, string InOrigin)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    string ParId;
        //    RRDMGasParameters Gp = new RRDMGasParameters();

        //    TableMatchingCateg = new DataTable();
        //    TableMatchingCateg.Clear();

        //    TotalSelected = 0;

        //    // DATA TABLE ROWS DEFINITION 
        //    TableMatchingCateg.Columns.Add("SeqNo", typeof(int));
        //    TableMatchingCateg.Columns.Add("Identity", typeof(string));
        //    TableMatchingCateg.Columns.Add("Category-Name", typeof(string));
        //    TableMatchingCateg.Columns.Add("Is POS_Type", typeof(bool));
        //    TableMatchingCateg.Columns.Add("Days W", typeof(int));
        //    TableMatchingCateg.Columns.Add("Days C", typeof(int));
        //    TableMatchingCateg.Columns.Add("Is TWIN", typeof(bool));
        //    TableMatchingCateg.Columns.Add("Is Active", typeof(bool));
        //    TableMatchingCateg.Columns.Add("Origin", typeof(string));
        //    TableMatchingCateg.Columns.Add("TransAtOrigin", typeof(string));
        //    TableMatchingCateg.Columns.Add("Product", typeof(string));

        //    //if (InOrigin == "ALL_Active" || InOrigin == "ALL")
        //    //{
        //    //    SqlString = "SELECT * "
        //    //       + " FROM [ATMS].[dbo].[MatchingCategories] "
        //    //       + " WHERE Operator = @Operator AND Active = 1 AND RunningJobGroup = 'e_MOBILE' "
        //    //       + " ORDER BY CategoryId ASC ";
        //    //}
        //    //if (InOrigin == "ALL_Not_Active")
        //    //{
        //    //    SqlString = "SELECT * "
        //    //       + " FROM [ATMS].[dbo].[MatchingCategories] "
        //    //       + " WHERE Operator = @Operator AND Active = 0 AND RunningJobGroup = 'e_MOBILE' "
        //    //       + " ORDER BY CategoryId ASC ";
        //    //}
        //    //if (InOrigin == "Active And Not Active")
        //    //{
        //    //    SqlString = "SELECT * "
        //    //       + " FROM [ATMS].[dbo].[MatchingCategories] "
        //    //       + " WHERE Operator = @Operator AND RunningJobGroup = 'e_MOBILE'  "
        //    //       + " ORDER BY CategoryId ASC ";
        //    //}
        //    if (InOrigin == "e_MOBILE")
        //    {
        //        SqlString = "SELECT * "
        //           + " FROM [ATMS].[dbo].[MatchingCategories] "
        //           + " WHERE Operator = @Operator AND RunningJobGroup = 'e_MOBILE'  "
        //           + " ORDER BY CategoryId ASC ";
        //    }


        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {

        //                cmd.Parameters.AddWithValue("@Operator", InOperator);
        //                if (InOrigin == "ALL")
        //                {
        //                    // Do nothing 
        //                }
        //                else
        //                {
        //                    cmd.Parameters.AddWithValue("@Origin", InOrigin);
        //                }

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {

        //                    RecordFound = true;

        //                    TotalSelected = TotalSelected + 1;

        //                    // Read Fields 
        //                    MatchingCatReaderFields(rdr);

        //                    ParId = "825"; // Check if not to be shown category

        //                    Gp.ReadParametersSpecificNm(InOperator, ParId, CategoryId);

        //                    if (Gp.RecordFound == true)
        //                    {
        //                        // Not To be shown Category
        //                    }
        //                    else
        //                    {
        //                        DataRow RowSelected = TableMatchingCateg.NewRow();

        //                        RowSelected["SeqNo"] = SeqNo;
        //                        RowSelected["Identity"] = CategoryId;
        //                        RowSelected["Category-Name"] = CategoryName;

        //                        RowSelected["Is POS_Type"] = Pos_Type;
        //                        RowSelected["Days W"] = UnMatchedForWorkingDays;
        //                        RowSelected["Days C"] = UnMatchedForCalendarDays;

        //                        RowSelected["Is TWIN"] = TWIN;

        //                        RowSelected["Is Active"] = Active;

        //                        RowSelected["Origin"] = Origin;
        //                        RowSelected["TransAtOrigin"] = TransTypeAtOrigin;
        //                        RowSelected["Product"] = Product;

        //                        // ADD ROW
        //                        TableMatchingCateg.Rows.Add(RowSelected);
        //                    }

        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {

        //            conn.Close();

        //            CatchDetails(ex);
        //        }
        //}
        ////
        // Methods 
        // READ MatchingCategories  
        // FILL UP A TABLE with Select
        //
        public void ReadMatchingCategoriesSlavesAndFillTable(string InOperator, string InReconcCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
            RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();
            string SelectionCriteria;

            // FIND CUTOFF CYCLE
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            //    string WOperator = "BCAIEGCX";
            string WJobCategory = "ATMs";

            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(InOperator, WJobCategory);

            TableMatchingCateg = new DataTable();
            TableMatchingCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableMatchingCateg.Columns.Add("SeqNo", typeof(int));
            TableMatchingCateg.Columns.Add("Select", typeof(bool));
            TableMatchingCateg.Columns.Add("CategoryId", typeof(string));
            TableMatchingCateg.Columns.Add("Category-Name", typeof(string));

            TableMatchingCateg.Columns.Add("Is POS_Type", typeof(bool));
            TableMatchingCateg.Columns.Add("Days W", typeof(int));
            TableMatchingCateg.Columns.Add("Days C", typeof(int));

            TableMatchingCateg.Columns.Add("Is TWIN", typeof(bool));

            TableMatchingCateg.Columns.Add("File_A", typeof(string));
            TableMatchingCateg.Columns.Add("File_B", typeof(string));
            TableMatchingCateg.Columns.Add("File_C", typeof(string));


            TableMatchingCateg.Columns.Add("MathingDone", typeof(string));
            TableMatchingCateg.Columns.Add("Assigned-To", typeof(string));
            TableMatchingCateg.Columns.Add("Origin", typeof(string));
            TableMatchingCateg.Columns.Add("TransAtOrigin", typeof(string));
            TableMatchingCateg.Columns.Add("Product", typeof(string));


            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategories] "
               + " WHERE Operator = @Operator AND ReconcMaster = 0 "
               + " Order by CategoryId ";

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

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            MatchingCatReaderFields(rdr);

                            DataRow RowSelected = TableMatchingCateg.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            SelectionCriteria = " WHERE ReconcCategoryId ='" + InReconcCategoryId + "'"
                                + " AND MatchingCategoryId = '" + CategoryId + "'";

                            RcMc.ReadReconcCateqoriesVsMatchingCategoriesbySelectionCriteria(SelectionCriteria);

                            if (RcMc.RecordFound == true)
                            {
                                RowSelected["Select"] = true;
                            }
                            else
                            {
                                RowSelected["Select"] = false;
                            }

                            RowSelected["CategoryId"] = CategoryId;
                            RowSelected["Category-Name"] = CategoryName;

                            RowSelected["Is POS_Type"] = Pos_Type;
                            RowSelected["Days W"] = UnMatchedForWorkingDays;
                            RowSelected["Days C"] = UnMatchedForCalendarDays;

                            RowSelected["Is TWIN"] = TWIN;

                            Mcs.ReadReconcCategoriesVsSourcesAll(CategoryId);
                            RowSelected["File_A"] = Mcs.SourceFileNameA;
                            RowSelected["File_B"] = Mcs.SourceFileNameB;
                            RowSelected["File_C"] = Mcs.SourceFileNameC;

                            // Matching Done ? 
                            // Matching Done
                            bool CategoryMatched = false;
                            //    RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

                            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

                            CategoryMatched = Rcs.ReadReconcCategorySessionBySlaveCatAndRunningJobNo(InOperator, InReconcCategoryId, CategoryId, WReconcCycleNo);

                            if (CategoryMatched == true)
                            {
                                RowSelected["MathingDone"] = "YES";
                            }
                            else
                            {
                                RowSelected["MathingDone"] = "NO";
                            }

                            RowSelected["Origin"] = Origin;

                            // See if already assigned 
                            SelectionCriteria = " WHERE MatchingCategoryId ='" + CategoryId + "' AND ReconcCategoryId ='" + InReconcCategoryId + "'";

                            RcMc.ReadReconcCateqoriesVsMatchingCategoriesbySelectionCriteria(SelectionCriteria);
                            if (RcMc.RecordFound)
                            {
                                RowSelected["Assigned-To"] = RcMc.ReconcCategoryId;
                            }
                            else
                            {
                                RowSelected["Assigned-To"] = "No Assigment";
                            }

                            RowSelected["TransAtOrigin"] = TransTypeAtOrigin;
                            RowSelected["Product"] = Product;

                            // ADD ROW
                            TableMatchingCateg.Rows.Add(RowSelected);

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

        // Methods 
        // READ MatchingCategories  
        // FILL UP A TABLE with categories and files 
        //
        public void ReadMatchingCategoriesAndFillTableInDetail(string InOperator, string W_Application)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
            RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();
            string SelectionCriteria;

            // FIND CUTOFF CYCLE
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            //    string WOperator = "BCAIEGCX";
            string WJobCategory = W_Application;

            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(InOperator, WJobCategory);

            TableMatchingCateg = new DataTable();
            TableMatchingCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableMatchingCateg.Columns.Add("SeqNo", typeof(int));
            TableMatchingCateg.Columns.Add("Select", typeof(bool));
            TableMatchingCateg.Columns.Add("CategoryId", typeof(string));
            TableMatchingCateg.Columns.Add("Category-Name", typeof(string));


            TableMatchingCateg.Columns.Add("Is POS_Type", typeof(bool));
            TableMatchingCateg.Columns.Add("Days W", typeof(int));
            TableMatchingCateg.Columns.Add("Days C", typeof(int));

            TableMatchingCateg.Columns.Add("Is TWIN", typeof(bool));

            TableMatchingCateg.Columns.Add("File_A", typeof(string));
            TableMatchingCateg.Columns.Add("File_B", typeof(string));
            TableMatchingCateg.Columns.Add("File_C", typeof(string));
            TableMatchingCateg.Columns.Add("MathingDone", typeof(string));
            TableMatchingCateg.Columns.Add("Assigned-To", typeof(string));
            TableMatchingCateg.Columns.Add("Origin", typeof(string));
            TableMatchingCateg.Columns.Add("TransAtOrigin", typeof(string));
            TableMatchingCateg.Columns.Add("Product", typeof(string));


            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategories] "
               + " WHERE RunningJobGroup = @RunningJobGroup AND Active = 1  "
               + " Order by CategoryId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@RunningJobGroup", W_Application);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            MatchingCatReaderFields(rdr);

                            DataRow RowSelected = TableMatchingCateg.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            SelectionCriteria = " WHERE ReconcCategoryId ='" + CategoryId + "'"
                                + " AND MatchingCategoryId = '" + CategoryId + "'";

                            RcMc.ReadReconcCateqoriesVsMatchingCategoriesbySelectionCriteria(SelectionCriteria);

                            if (RcMc.RecordFound == true)
                            {
                                RowSelected["Select"] = true;
                            }
                            else
                            {
                                RowSelected["Select"] = false;
                            }

                            RowSelected["CategoryId"] = CategoryId;
                            RowSelected["Category-Name"] = CategoryName;

                            RowSelected["Is POS_Type"] = Pos_Type;
                            RowSelected["Days W"] = UnMatchedForWorkingDays;
                            RowSelected["Days C"] = UnMatchedForCalendarDays;

                            RowSelected["Is TWIN"] = TWIN;

                            Mcs.ReadReconcCategoriesVsSourcesAll(CategoryId);
                            RowSelected["File_A"] = Mcs.SourceFileNameA;
                            RowSelected["File_B"] = Mcs.SourceFileNameB;
                            RowSelected["File_C"] = Mcs.SourceFileNameC;

                            // Matching Done ? 
                            // Matching Done
                            bool CategoryMatched = false;
                            //    RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

                            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
                            if (ReconcMaster == true)
                                CategoryMatched = Rcs.ReadReconcCategorySessionBySlaveCatAndRunningJobNo(InOperator, CategoryId, CategoryId, WReconcCycleNo);

                            if (ReconcMaster == false)
                            {

                                CategoryMatched = Rcs.ReadReconcCategorySessionBySlaveCatAndRunningJobNo(InOperator, CategoryId, CategoryId, WReconcCycleNo);

                            }


                            if (CategoryMatched == true)
                            {
                                RowSelected["MathingDone"] = "YES";
                            }
                            else
                            {
                                RowSelected["MathingDone"] = "NO";
                            }

                            RowSelected["Origin"] = Origin;

                            // See if already assigned 
                            SelectionCriteria = " WHERE MatchingCategoryId ='" + CategoryId + "' AND ReconcCategoryId ='" + CategoryId + "'";

                            RRDMReconcCategories Rc = new RRDMReconcCategories();
                            Rc.ReadReconcCategorybyCategId(InOperator, CategoryId);

                            if (Rc.RecordFound == true)
                            {
                                RowSelected["Assigned-To"] = Rc.OwnerUserID;
                            }
                            else
                            {
                                RowSelected["Assigned-To"] = "No Assigment";
                                if (ReconcMaster == false) RowSelected["Assigned-To"] = "Slave Categ";
                            }



                            RowSelected["TransAtOrigin"] = TransTypeAtOrigin;
                            RowSelected["Product"] = Product;

                            // ADD ROW
                            TableMatchingCateg.Rows.Add(RowSelected);

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
        public void ReadMatchingCategoriesAndFillTableInDetail_Mobile(string InOperator, string InApplication)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingCategoriesVsSourcesFiles Mcs = new RRDMMatchingCategoriesVsSourcesFiles();
            RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();
            string SelectionCriteria;

            // FIND CUTOFF CYCLE
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            //    string WOperator = "BCAIEGCX";
            string WJobCategory = InApplication;

            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(InOperator, WJobCategory);

            TableMatchingCateg = new DataTable();
            TableMatchingCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableMatchingCateg.Columns.Add("SeqNo", typeof(int));
            TableMatchingCateg.Columns.Add("Select", typeof(bool));
            TableMatchingCateg.Columns.Add("CategoryId", typeof(string));
            TableMatchingCateg.Columns.Add("Category-Name", typeof(string));


            TableMatchingCateg.Columns.Add("Is POS_Type", typeof(bool));
            TableMatchingCateg.Columns.Add("Days W", typeof(int));
            TableMatchingCateg.Columns.Add("Days C", typeof(int));

            TableMatchingCateg.Columns.Add("Is TWIN", typeof(bool));

            TableMatchingCateg.Columns.Add("File_A", typeof(string));
            TableMatchingCateg.Columns.Add("File_B", typeof(string));
            TableMatchingCateg.Columns.Add("File_C", typeof(string));
            TableMatchingCateg.Columns.Add("MathingDone", typeof(string));
            TableMatchingCateg.Columns.Add("Assigned-To", typeof(string));
            TableMatchingCateg.Columns.Add("Origin", typeof(string));
            TableMatchingCateg.Columns.Add("TransAtOrigin", typeof(string));
            TableMatchingCateg.Columns.Add("Product", typeof(string));


            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MatchingCategories] "
               + " WHERE Operator = @Operator AND Active = 1  AND RunningJobGroup = @RunningJobGroup "
               + " Order by CategoryId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RunningJobGroup", InApplication);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            MatchingCatReaderFields(rdr);

                            DataRow RowSelected = TableMatchingCateg.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            // FIND THE SLAVES
                            SelectionCriteria = " WHERE ReconcCategoryId ='" + CategoryId + "'"
                                + " AND MatchingCategoryId = '" + CategoryId + "'";

                            RcMc.ReadReconcCateqoriesVsMatchingCategoriesbySelectionCriteria(SelectionCriteria);

                            if (RcMc.RecordFound == true)
                            {
                                RowSelected["Select"] = true;
                            }
                            else
                            {
                                RowSelected["Select"] = false;
                            }

                            RowSelected["CategoryId"] = CategoryId;
                            RowSelected["Category-Name"] = CategoryName;

                            RowSelected["Is POS_Type"] = Pos_Type;
                            RowSelected["Days W"] = UnMatchedForWorkingDays;
                            RowSelected["Days C"] = UnMatchedForCalendarDays;

                            RowSelected["Is TWIN"] = TWIN;

                            Mcs.ReadReconcCategoriesVsSourcesAll(CategoryId);
                            RowSelected["File_A"] = Mcs.SourceFileNameA;
                            RowSelected["File_B"] = Mcs.SourceFileNameB;
                            RowSelected["File_C"] = Mcs.SourceFileNameC;

                            // Matching Done ? 
                            // Matching Done
                            bool CategoryMatched = false;
                            //    RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

                            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
                            if (ReconcMaster == true)
                                CategoryMatched = Rcs.ReadReconcCategorySessionBySlaveCatAndRunningJobNo(InOperator, CategoryId, CategoryId, WReconcCycleNo);

                            if (ReconcMaster == false)
                            {

                                CategoryMatched = Rcs.ReadReconcCategorySessionBySlaveCatAndRunningJobNo(InOperator, CategoryId, CategoryId, WReconcCycleNo);

                            }


                            if (CategoryMatched == true)
                            {
                                RowSelected["MathingDone"] = "YES";
                            }
                            else
                            {
                                RowSelected["MathingDone"] = "NO";
                            }

                            RowSelected["Origin"] = Origin;

                            // See if already assigned 
                            SelectionCriteria = " WHERE MatchingCategoryId ='" + CategoryId + "' AND ReconcCategoryId ='" + CategoryId + "'";

                            RRDMReconcCategories Rc = new RRDMReconcCategories();
                            Rc.ReadReconcCategorybyCategId(InOperator, CategoryId);

                            if (Rc.RecordFound == true)
                            {
                                RowSelected["Assigned-To"] = Rc.OwnerUserID;
                            }
                            else
                            {
                                RowSelected["Assigned-To"] = "No Assigment";
                                if (ReconcMaster == false) RowSelected["Assigned-To"] = "Slave Categ";
                            }



                            RowSelected["TransAtOrigin"] = TransTypeAtOrigin;
                            RowSelected["Product"] = Product;

                            // ADD ROW
                            TableMatchingCateg.Rows.Add(RowSelected);

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
        // READ MatchingCategories with exceptions for Allocation of work   
        // FILL UP A TABLE
        //
        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);
        public void ReadMatchingCategoriesForAllocation(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMatchingCateg = new DataTable();
            TableMatchingCateg.Clear();

            TotalSelected = 0;
            TotalMatchingDone = 0;
            TotalMatchingNotDone = 0;
            TotalReconc = 0;
            TotalNotReconc = 0;
            TotalUnMatchedRecords = 0;

            // DATA TABLE ROWS DEFINITION 
            TableMatchingCateg.Columns.Add("Identity", typeof(string));
            TableMatchingCateg.Columns.Add("Category_Name", typeof(string));
            TableMatchingCateg.Columns.Add("OutStanding", typeof(int));
            TableMatchingCateg.Columns.Add("HasOwner", typeof(string));
            TableMatchingCateg.Columns.Add("OwnerId", typeof(string));
            TableMatchingCateg.Columns.Add("Matching_Dt", typeof(DateTime));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
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


                            // Read Fields 
                            MatchingCatReaderFields(rdr);

                            if (MatchingDtTm.Date == DateTime.Today)
                            {
                                TotalMatchingDone = TotalMatchingDone + 1;
                            }
                            else
                            {
                                TotalMatchingNotDone = TotalMatchingNotDone + 1;
                            }
                            // Read ALL Cycles for this category that has differences and reconciliation didnt start. 
                            Rms.ReadMatchingCategoriesSessionsSpecificCatForExceptions(CategoryId);

                            if (Rms.RecordFound == true)
                            {
                                RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
                                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo
                                    (Rms.Operator, Rms.CategoryId, Rms.RunningJobNo);
                                if (Rcs.EndReconcDtTm == NullPastDate)
                                {
                                    TotalSelected = TotalSelected + 1;

                                    TotalNotReconc = TotalNotReconc + 1;

                                    TotalUnMatchedRecords = TotalUnMatchedRecords + Rms.TotalUnMatchedRecs;

                                    DataRow RowSelected = TableMatchingCateg.NewRow();

                                    RowSelected["Identity"] = CategoryId;
                                    RowSelected["Category_Name"] = CategoryName;

                                    RowSelected["OutStanding"] = Rms.TotalUnMatchedRecs;
                                    RowSelected["HasOwner"] = HasOwner;
                                    RowSelected["OwnerId"] = OwnerId;

                                    RowSelected["Matching_Dt"] = MatchingDtTm;

                                    // ADD ROW
                                    TableMatchingCateg.Rows.Add(RowSelected);
                                }
                                else
                                {
                                    TotalReconc = TotalReconc + 1;
                                }

                            }
                            else
                            {

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
        // READ MatchingCategories to report Matching Status  
        // FILL UP A TABLE
        //
        public void ReadMatchingCategoriesForMatchingStatus(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMatchingCateg = new DataTable();
            TableMatchingCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableMatchingCateg.Columns.Add("Identity", typeof(string));
            TableMatchingCateg.Columns.Add("Category-Name", typeof(string));
            TableMatchingCateg.Columns.Add("Matching-Dt", typeof(DateTime));
            TableMatchingCateg.Columns.Add("Matching-Status", typeof(string));
            TableMatchingCateg.Columns.Add("OutStanding", typeof(int));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
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


                            // Read Fields 
                            MatchingCatReaderFields(rdr);


                            Rms.ReadMatchingCategoriesSessionsSpecificCatForExceptions(CategoryId);

                            TotalSelected = TotalSelected + 1;

                            DataRow RowSelected = TableMatchingCateg.NewRow();

                            RowSelected["Identity"] = CategoryId;
                            RowSelected["Category-Name"] = CategoryName;
                            RowSelected["Matching-Dt"] = MatchingDtTm;
                            RowSelected["Matching-Status"] = MatchingStatus;
                            RowSelected["OutStanding"] = Rms.TotalUnMatchedRecs;

                            // ADD ROW
                            TableMatchingCateg.Rows.Add(RowSelected);

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
        // READ MatchingCategories to report Matching Status  
        // FILL UP A TABLE
        //
        public int TotalCat;
        public int TotalCatReady;
        public string ENQ_CategForMatch;

        //public void ReadMatchingCategoriesForCheckReadyToMatch(string InOperator, int InRMCycleNo, DateTime InCut_Off_Date)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    TotalCat = 0;
        //    TotalCatReady = 0;
        //   // ENQ_CategForMatch = ENQ_CategForMatch + WMatchingCateg + ".. NOT Ready" + "\r\n";
        //    ENQ_CategForMatch = "Ready Categories.." + "\r\n" + " When Fles loaded..." + "\r\n";

        //    bool ReadyCateg = false;

        //    RRDMMatchingCategoriesVsSourcesFiles Msf = new RRDMMatchingCategoriesVsSourcesFiles();
        //    RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();
        //    RRDMReconcFileMonitorLog Rfm = new RRDMReconcFileMonitorLog();

        //    TableMatchingCateg_Matching_Status = new DataTable();
        //    TableMatchingCateg_Matching_Status.Clear();

        //    TotalSelected = 0;

        //    // DATA TABLE ROWS DEFINITION 
        //    TableMatchingCateg_Matching_Status.Columns.Add("CategoryId", typeof(string));
        //    TableMatchingCateg_Matching_Status.Columns.Add("Category-Name", typeof(string));
        //    TableMatchingCateg_Matching_Status.Columns.Add("File A", typeof(string));
        //    TableMatchingCateg_Matching_Status.Columns.Add("R_A", typeof(bool));
        //    TableMatchingCateg_Matching_Status.Columns.Add("File B", typeof(string));
        //    TableMatchingCateg_Matching_Status.Columns.Add("R_B", typeof(bool));
        //    TableMatchingCateg_Matching_Status.Columns.Add("File C", typeof(string));
        //    TableMatchingCateg_Matching_Status.Columns.Add("R_C", typeof(bool));
        //    TableMatchingCateg_Matching_Status.Columns.Add("Status", typeof(string));

        //    SqlString = "SELECT *"
        //            + " FROM [ATMS].[dbo].[MatchingCategories] "
        //            + " WHERE Operator = @Operator AND Active = 1"
        //            + " ORDER BY CategoryId ASC ";

        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {

        //                cmd.Parameters.AddWithValue("@Operator", InOperator);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {

        //                    RecordFound = true;

        //                    // Read Fields 
        //                    MatchingCatReaderFields(rdr);

        //                    TotalSelected = TotalSelected + 1;

        //                    DataRow RowSelected = TableMatchingCateg_Matching_Status.NewRow();

        //                    TotalCat = TotalCat + 1;

        //                    bool File_A_Ready = false;
        //                    bool File_B_Ready = false;
        //                    bool File_C_Ready = false;

        //                    RowSelected["CategoryId"] = CategoryId;
        //                    RowSelected["Category-Name"] = CategoryName;

        //                    Msf.ReadReconcCategoriesVsSourcesAll(CategoryId);


        //                    // FILE A

        //                    if (Msf.SourceFileNameA != "")
        //                    {

        //                        RowSelected["File A"] = Msf.SourceFileNameA;


        //                        Rs.ReadReconcSourceFilesByFileId(Msf.SourceFileNameA);

        //                        if (Rs.LayoutId == "TWIN")
        //                        {
        //                            if (InOperator == "BCAIEGCX")
        //                            {
        //                                // Search for Mother File 
        //                                if (Rs.SourceFileId == "Flexcube_TWIN")
        //                                {
        //                                    Rs.ReadReconcSourceFilesByFileId("Flexcube");
        //                                }
        //                                if (Rs.SourceFileId == "Switch_IST_Txns_TWIN")
        //                                {
        //                                    Rs.ReadReconcSourceFilesByFileId("Switch_IST_Txns");
        //                                }
        //                            }
        //                        }


        //                        Rs.CheckIfFileInDirectory(Rs.SourceFileId, Rs.SourceDirectory, InCut_Off_Date, Rs.FileNameMask);

        //                        if (Rs.IsPresentInDirectory == true & Rs.IsGood == "YES")
        //                       // if (Rfm.RecordFound == true)
        //                        {
        //                            RowSelected["R_A"] = true;
        //                            File_A_Ready = true;
        //                        }
        //                        else
        //                        {
        //                            RowSelected["R_A"] = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        RowSelected["File A"] = "Not Defined Yet";
        //                        RowSelected["R_A"] = false;
        //                    }

        //                    // FILE B
        //                    //
        //                    if (Msf.SourceFileNameB != "")
        //                    {

        //                        RowSelected["File B"] = Msf.SourceFileNameB;

        //                        Rs.ReadReconcSourceFilesByFileId(Msf.SourceFileNameB);

        //                        if (Rs.LayoutId == "TWIN")
        //                        {
        //                            if (InOperator == "BCAIEGCX")
        //                            {
        //                                // Search for Mother File 
        //                                if (Rs.SourceFileId == "Flexcube_TWIN")
        //                                {
        //                                    Rs.ReadReconcSourceFilesByFileId("Flexcube");
        //                                }
        //                                if (Rs.SourceFileId == "Switch_IST_Txns_TWIN")
        //                                {
        //                                    Rs.ReadReconcSourceFilesByFileId("Switch_IST_Txns");
        //                                }
        //                            }
        //                        }

        //                     //   Rfm.ReadRecordByLoadedByCycle(Rs.SourceFileId, InRMCycleNo);


        //                        Rs.CheckIfFileInDirectory(Rs.SourceFileId, Rs.SourceDirectory, InCut_Off_Date, Rs.FileNameMask);

        //                        if (Rs.IsPresentInDirectory == true & Rs.IsGood == "YES")
        //                      //  if (Rfm.RecordFound == true)
        //                        {

        //                            RowSelected["R_B"] = true;
        //                            File_B_Ready = true;
        //                        }
        //                        else
        //                        {
        //                            RowSelected["R_B"] = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        RowSelected["File B"] = "Not Defined Yet";
        //                        RowSelected["R_B"] = false;
        //                    }

        //                    // FILE C
        //                    //

        //                    if (Msf.SourceFileNameC != "")
        //                    {
        //                        RowSelected["File C"] = Msf.SourceFileNameC;

        //                        Rs.ReadReconcSourceFilesByFileId(Msf.SourceFileNameC);

        //                        if (Rs.LayoutId == "TWIN")
        //                        {
        //                            if (InOperator == "BCAIEGCX")
        //                            {
        //                                // Search for Mother File 
        //                                if (Rs.SourceFileId == "Flexcube_TWIN")
        //                                {
        //                                    Rs.ReadReconcSourceFilesByFileId("Flexcube");
        //                                }
        //                                if (Rs.SourceFileId == "Switch_IST_Txns_TWIN")
        //                                {
        //                                    Rs.ReadReconcSourceFilesByFileId("Switch_IST_Txns");
        //                                }
        //                            }
        //                        }

        //                       //     Rfm.ReadRecordByLoadedByCycle(Rs.SourceFileId, InRMCycleNo);


        //                            Rs.CheckIfFileInDirectory(Rs.SourceFileId, Rs.SourceDirectory, InCut_Off_Date, Rs.FileNameMask);

        //                            if (Rs.IsPresentInDirectory == true & Rs.IsGood == "YES")
        //                          //  if (Rfm.RecordFound == true)
        //                            {

        //                            RowSelected["R_C"] = true;
        //                            File_C_Ready = true;
        //                        }
        //                        else
        //                        {
        //                            RowSelected["R_C"] = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        RowSelected["File C"] = "Not Defined Yet";
        //                        RowSelected["R_C"] = false;
        //                    }

        //                    // CHECK FOR READY OR NOT 

        //                    bool PassedThroughHere = false; 

        //                    // Two Files
        //                    if (Msf.SourceFileNameA != "" & Msf.SourceFileNameB != "" & Msf.SourceFileNameC == "")
        //                    {
        //                        if (File_A_Ready == true
        //                              & File_B_Ready == true)
        //                        {
        //                            PassedThroughHere = true; 
        //                            RowSelected["Status"] = "READY";
        //                            TotalCatReady = TotalCatReady + 1;

        //                            ENQ_CategForMatch = ENQ_CategForMatch + CategoryId +".."+ CategoryName+ ".. Ready" + "\r\n";
        //                        }
        //                        else
        //                        {
        //                            PassedThroughHere = true;
        //                            RowSelected["Status"] = "NOT READY";
        //                            ENQ_CategForMatch = ENQ_CategForMatch + CategoryId + ".." + CategoryName + ".. NOT Ready" + "\r\n";
        //                        }

        //                    }

        //                    // THREE FILES
        //                    if (Msf.SourceFileNameA != "" & Msf.SourceFileNameB != "" & Msf.SourceFileNameC != "")
        //                    {
        //                        if (File_A_Ready == true
        //                        & File_B_Ready == true
        //                        & File_C_Ready == true)
        //                        {
        //                            PassedThroughHere = true;
        //                            RowSelected["Status"] = "READY";
        //                            TotalCatReady = TotalCatReady + 1;

        //                            ENQ_CategForMatch = ENQ_CategForMatch + CategoryId + ".." + CategoryName + ".. Ready" + "\r\n";
        //                        }
        //                        else
        //                        {
        //                            PassedThroughHere = true;
        //                            RowSelected["Status"] = "NOT READY";
        //                            ENQ_CategForMatch = ENQ_CategForMatch + CategoryId + ".." + CategoryName + ".. NOT Ready" + "\r\n";
        //                        }
        //                    }

        //                    if (PassedThroughHere == false)
        //                    {
        //                        RowSelected["Status"] = "NOT READY";
        //                        ENQ_CategForMatch = ENQ_CategForMatch + CategoryId + ".." + CategoryName + ".. NOT Ready" + "\r\n";
        //                    }

        //                    // ADD ROW
        //                    TableMatchingCateg_Matching_Status.Rows.Add(RowSelected);

        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {

        //            conn.Close();


        //            CatchDetails(ex);

        //        }
        //}
        //
        // Methods 
        // READ MatchingCategory  by Seq no  
        // 
        //
        public void ReadMatchingCategorybySeqNoActive(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
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


                            // Read Fields 
                            MatchingCatReaderFields(rdr);


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
        // Read Any Cative and not active
        public void ReadMatchingCategorybySeqNo_Any(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
                    + " WHERE Operator = @Operator AND SeqNo = @SeqNo ";

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


                            // Read Fields 
                            MatchingCatReaderFields(rdr);


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
        // ReadMatchingCategorybyGetsAllOtherBINS
        // 
        //
        public void ReadMatchingCategorybyGetsAllOtherBINS(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
                    + " WHERE Operator = @Operator AND GetsNotOwnBINS = 1 AND Active = 1";

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

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            MatchingCatReaderFields(rdr);


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
        // READ MATCHING CATEGORIES BY VOSTRO 
        // AND CREATE RECONCILIATION SESSIONS 
        // 
        public void CreateReconciliationSessionsForMatchingCateg(string InOperator, string InSignedId,
                                              int InRunningJobNo, string InOrigin,
                                              DateTime InWDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMNVReconcCategoriesSessions Rcs = new RRDMNVReconcCategoriesSessions();

            TableMatchingCateg = new DataTable();
            TableMatchingCateg.Clear();

            TotalSelected = 0;

            SqlString =
                " SELECT * FROM [ATMS].[dbo].[MatchingCategories]  "
                + " Where Origin = @Origin "
                + " ORDER BY CategoryId "
                ;

            using (SqlConnection conn =
                        new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Origin", InOrigin);

                        sqlAdapt.Fill(TableMatchingCateg);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableMatchingCateg.Rows.Count - 1))
                        {

                            // For each entry in table Update records. 

                            // READ 
                            SeqNo = (int)TableMatchingCateg.Rows[I]["SeqNo"];

                            CategoryId = (string)TableMatchingCateg.Rows[I]["CategoryId"];
                            CategoryName = (string)TableMatchingCateg.Rows[I]["CategoryName"];
                            Origin = (string)TableMatchingCateg.Rows[I]["Origin"];
                            InternalAcc = (string)TableMatchingCateg.Rows[I]["InternalAcc"];
                            VostroBank = (string)TableMatchingCateg.Rows[I]["VostroBank"];
                            VostroAcc = (string)TableMatchingCateg.Rows[I]["VostroAcc"];
                            VostroCurr = (string)TableMatchingCateg.Rows[I]["VostroCurr"];

                            OwnerId = (string)TableMatchingCateg.Rows[I]["OwnerId"];

                            string SelectionCriteria = " WHERE CategoryId ='" + CategoryId + "' AND RunningJobNo =" + InRunningJobNo + "";

                            Rcs.ReadNVReconcCategoriesSessionsSpecificRunningJobCycle(SelectionCriteria);

                            if (Rcs.RecordFound == true)
                            {
                                I++; // Read Next entry of the table 
                                continue;
                            }

                            Rcs.CategoryId = CategoryId;
                            Rcs.CategoryName = CategoryName;
                            Rcs.Origin = Origin;
                            Rcs.RunningJobNo = InRunningJobNo;
                            Rcs.StartDailyProcess = DateTime.Now;
                            Rcs.InternalAccNo = InternalAcc;
                            Rcs.ExternalBank = VostroBank;
                            Rcs.ExternalAccNo = VostroAcc;
                            Rcs.NostroCcy = VostroCurr;

                            Rcs.OwnerId = OwnerId;
                            Rcs.Operator = InOperator;

                            Rcs.InsertNVReconcCategoriesSessionRecord();

                            I++; // Read Next entry of the table 

                        }
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }
        //
        // Methods 
        // READ ReconcCategory  by EXTERNAL 
        // 
        //
        public void ReadMatchingCategorybyExternalAccNo(string InOperator, string InExternalBank, string InExternalAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
                    + " WHERE Operator = @Operator AND VostroBank = @VostroBank "
                    + " AND VostroAcc = @VostroAcc AND Active = 1 ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@VostroBank", InExternalBank);
                        cmd.Parameters.AddWithValue("@VostroAcc", InExternalAccNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            MatchingCatReaderFields(rdr);


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
        // READ By Selection Criteria 
        // 
        public void ReadMatchingCategoryBySelectionCriteria(string InCategoryId, int InTargetSystemId, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //string SelectionCriteria = " WHERE Origin ='Our Atms' AND TargetSystemId = "
            //                                          + Mpa.TargetSystem;

            //string SelectionCriteria = " WHERE CategoryId = '"
            //                       + Mpa.MatchingCateg + "'";

            //string SelectionCriteria = " WHERE CategoryId = '"
            //                                              + Mpa.MatchingCateg + "'";

            if (InMode == 11)
            {
                SqlString = "SELECT *"
                                  + " FROM [ATMS].[dbo].[MatchingCategories] "
                                  + " WHERE Origin = 'Our Atms' AND TargetSystemId = @TargetSystemId ";

            }
            if (InMode == 12)
            {
                SqlString = "SELECT *"
                                  + " FROM [ATMS].[dbo].[MatchingCategories] "
                                   + " WHERE CategoryId = @CategoryId";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        if (InMode == 11)
                        {
                            cmd.Parameters.AddWithValue("@TargetSystemId", InTargetSystemId);
                        }
                        if (InMode == 12)
                        {
                            cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            MatchingCatReaderFields(rdr);

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
        // READ ReconcCategory  by Origin, TransType, Product 
        // 
        //
        public void ReadMatchingCategoryToFindCategoryId(string InOrigin, string InTransTypeAtOrigin, string InProduct)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
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

                            // Read Fields 
                            MatchingCatReaderFields(rdr);


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
        // READ ReconcCategories by Cat Id   
        // 
        //
        public void ReadMatchingCategorybyActiveCategId(string InOperator, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
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

                            // Read Fields 
                            MatchingCatReaderFields(rdr);

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
        // READ ReconcCategories by Cat Name 
        // 
        //
        public void ReadMatchingCategorybyCategName(string InOperator, string InCategoryName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
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

                            // Read Fields 
                            MatchingCatReaderFields(rdr);


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
        // Find category through the pair of accounts 
        //
        public string ReadMatchingCategoriesIdByPair(string InInternalAcc, string InVostroAcc)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
                    + " WHERE InternalAcc = @InternalAcc AND VostroAcc = @VostroAcc  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@InternalAcc", InInternalAcc);
                        cmd.Parameters.AddWithValue("@VostroAcc", InVostroAcc);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            // Read Fields 
                            MatchingCatReaderFields(rdr);

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

            return CategoryId;
        }
        //
        // Methods 
        // Find Total for this USER
        // 
        //
        public int TotalCatForUser;

        public void ReadMatchingCategoriesNumberForUser(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalCatForUser = 0;

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[MatchingCategories] "
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


                    CatchDetails(ex);

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

            string ParId;
            RRDMGasParameters Gp = new RRDMGasParameters();

            SqlString = "SELECT * FROM [ATMS].[dbo].[MatchingCategories]"
                     + " WHERE Operator = @Operator  AND Active = 1 "
                     + " Order by CategoryId ASC ";


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

                            ParId = "825"; // Check if not to be shown category

                            Gp.ReadParametersSpecificNm(InOperator, ParId, CategoryId);

                            if (Gp.RecordFound == true)
                            {
                                // Not To be shown Category Like BDC280
                            }
                            else
                            {
                                string CatIdAndName = CategoryId + "*" + CategoryName;

                                OccurancesListNm.Add(CatIdAndName);
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

            return OccurancesListNm;
        }

        // GET Array List Occurance Nm 
        //
        public ArrayList GetCategories_e_MOBILE(string InOperator)
        {

            ArrayList OccurancesListNm = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string ParId;
            RRDMGasParameters Gp = new RRDMGasParameters();

            SqlString = "SELECT * FROM [ATMS].[dbo].[MatchingCategories]"
                     + " WHERE Active = 1  AND Operator = @Operator "
                     + " Order by CategoryId ASC ";


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

                            ParId = "825"; // Check if not to be shown category

                            Gp.ReadParametersSpecificNm(InOperator, ParId, CategoryId);

                            if (Gp.RecordFound == true)
                            {
                                // Not To be shown Category Like BDC280
                            }
                            else
                            {
                                string CatIdAndName = CategoryId + "*" + CategoryName;

                                OccurancesListNm.Add(CatIdAndName);
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

            return OccurancesListNm;
        }
        //
        // Methods 
        // Find Position In Grid
        // 
        //
        public int PositionInGrid;
        public void ReadCategoriesToFindPositionOfSeqNo(string InOperator, int InSeqNo, string InOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            PositionInGrid = -1;
            if (InOrigin == "")
            {
                SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[MatchingCategories] "
                   + " WHERE Operator = @Operator AND Active = 1 "
                   + " ORDER BY CategoryId ASC ";
            }
            else
            {
                SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[MatchingCategories] "
                   + " WHERE Operator = @Operator AND Origin = @Origin AND Active = 1 "
                    + " ORDER BY CategoryId ASC ";
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
                        if (InOrigin != "")
                        {
                            cmd.Parameters.AddWithValue("@Origin", InOrigin);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            PositionInGrid = PositionInGrid + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            if (SeqNo == InSeqNo)
                            {
                                break;
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

        // Insert Category
        //
        public int InsertMatchingCategory()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[MatchingCategories]"
                    + "([CategoryId], [CategoryName],  "
                    + " [Origin], [TransTypeAtOrigin], "
                    + " [TargetSystemId], [TargetSystemNm], "
                     + " [GetsNotOwnBINS], "
                    + " [RunningJobGroup],[Product], [CostCentre],"
                    + " [Periodicity],[NextMatchingDt], [Pos_Type],  "
                    + " [UnMatchedForWorkingDays], [UnMatchedForCalendarDays], [Currency],"
                     + " [TWIN], "
                    + " [EntityA],[EntityB],[GlAccount], "
                    + " [VostroBank], [VostroCurr], [VostroAcc], [InternalAcc],"
                    + " [ReconcMaster], "
                    + " [Operator] )"
                    + " VALUES (@CategoryId, @CategoryName,"
                    + " @Origin, @TransTypeAtOrigin, "
                    + " @TargetSystemId, @TargetSystemNm, "
                     + " @GetsNotOwnBINS,  "
                    + " @RunningJobGroup,@Product, @CostCentre,"
                    + " @Periodicity, @NextMatchingDt, @Pos_Type,"
                    + " @UnMatchedForWorkingDays, @UnMatchedForCalendarDays, @Currency,"
                      + " @TWIN,  "
                    + " @EntityA,@EntityB, @GlAccount,"
                    + " @VostroBank, @VostroCurr, @VostroAcc,  @InternalAcc,"
                    + " @ReconcMaster, "
                    + " @Operator )"
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";

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

                        cmd.Parameters.AddWithValue("@TargetSystemId", TargetSystemId);
                        cmd.Parameters.AddWithValue("@TargetSystemNm", TargetSystemNm);

                        cmd.Parameters.AddWithValue("@GetsNotOwnBINS", GetsNotOwnBINS);

                        cmd.Parameters.AddWithValue("@RunningJobGroup", RunningJobGroup);
                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@CostCentre", CostCentre);
                        cmd.Parameters.AddWithValue("@Periodicity", Periodicity);
                        cmd.Parameters.AddWithValue("@NextMatchingDt", NextMatchingDt);

                        //cmd.Parameters.AddWithValue("@GroupIdInFiles", GroupIdInFiles);
                        cmd.Parameters.AddWithValue("@Pos_Type", Pos_Type);
                        cmd.Parameters.AddWithValue("@UnMatchedForWorkingDays", UnMatchedForWorkingDays);
                        cmd.Parameters.AddWithValue("@UnMatchedForCalendarDays", UnMatchedForCalendarDays);
                        cmd.Parameters.AddWithValue("@TWIN", TWIN);

                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@EntityA", EntityA);

                        cmd.Parameters.AddWithValue("@EntityB", EntityB);
                        cmd.Parameters.AddWithValue("@GlAccount", GlAccount);

                        cmd.Parameters.AddWithValue("@VostroBank", VostroBank);
                        cmd.Parameters.AddWithValue("@VostroCurr", VostroCurr);
                        cmd.Parameters.AddWithValue("@VostroAcc", VostroAcc);
                        cmd.Parameters.AddWithValue("@InternalAcc", InternalAcc);
                        cmd.Parameters.AddWithValue("@ReconcMaster", ReconcMaster);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        SeqNo = (int)cmd.ExecuteScalar();
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
            return SeqNo;
        }

        // UPDATE Category
        // 
        public void UpdateMatchingCategory(string InOperator, string InCategoryId)
        {
            Successful = false;
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[MatchingCategories] SET "
                            + " CategoryId = @CategoryId, CategoryName = @CategoryName, "
                            + " Origin = @Origin, TransTypeAtOrigin = @TransTypeAtOrigin, "
                            + " TargetSystemId = @TargetSystemId, TargetSystemNm = @TargetSystemNm, "
                            + " GetsNotOwnBINS = @GetsNotOwnBINS, "
                            + " RunningJobGroup = @RunningJobGroup, "
                            + " Product = @Product, CostCentre = @CostCentre, "
                            + " Periodicity = @Periodicity, NextMatchingDt = @NextMatchingDt, "
                            + " Pos_Type = @Pos_Type, "
                            + " UnMatchedForWorkingDays = @UnMatchedForWorkingDays, UnMatchedForCalendarDays = @UnMatchedForCalendarDays ,"
                            + " TWIN = @TWIN, "
                            + " Currency = @Currency,"
                            + " EntityA = @EntityA, EntityB = @EntityB,"
                            + " GlAccount = @GlAccount,"
                            + " VostroBank = @VostroBank, VostroCurr = @VostroCurr, VostroAcc = @VostroAcc, InternalAcc = @InternalAcc,"
                            + " ReconcMaster = @ReconcMaster, "
                            + " HasOwner = @HasOwner, OwnerId = @OwnerId,  Active = @Active  "
                            + " WHERE CategoryId = @CategoryId", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@CategoryName", CategoryName);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);

                        cmd.Parameters.AddWithValue("@TargetSystemId", TargetSystemId);
                        cmd.Parameters.AddWithValue("@TargetSystemNm", TargetSystemNm);

                        cmd.Parameters.AddWithValue("@GetsNotOwnBINS", GetsNotOwnBINS);

                        cmd.Parameters.AddWithValue("@RunningJobGroup", RunningJobGroup);
                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@CostCentre", CostCentre);
                        cmd.Parameters.AddWithValue("@Periodicity", Periodicity);
                        cmd.Parameters.AddWithValue("@NextMatchingDt", NextMatchingDt);

                        //  cmd.Parameters.AddWithValue("@GroupIdInFiles", GroupIdInFiles);
                        cmd.Parameters.AddWithValue("@Pos_Type", Pos_Type);
                        cmd.Parameters.AddWithValue("@UnMatchedForWorkingDays", UnMatchedForWorkingDays);
                        cmd.Parameters.AddWithValue("@UnMatchedForCalendarDays", UnMatchedForCalendarDays);

                        cmd.Parameters.AddWithValue("@TWIN", TWIN);

                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@EntityA", EntityA);

                        cmd.Parameters.AddWithValue("@EntityB", EntityB);
                        cmd.Parameters.AddWithValue("@GlAccount", GlAccount);

                        cmd.Parameters.AddWithValue("@VostroBank", VostroBank);
                        cmd.Parameters.AddWithValue("@VostroCurr", VostroCurr);
                        cmd.Parameters.AddWithValue("@VostroAcc", VostroAcc);
                        cmd.Parameters.AddWithValue("@InternalAcc", InternalAcc);

                        cmd.Parameters.AddWithValue("@ReconcMaster", ReconcMaster);
                        cmd.Parameters.AddWithValue("@HasOwner", HasOwner);
                        cmd.Parameters.AddWithValue("@OwnerId", OwnerId);
                        cmd.Parameters.AddWithValue("@Active", Active);

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

        //
        // DELETE Category
        //
        public void DeleteMatchingCategory(int InSeqNo, string InCategoryId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingCategories] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);


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

            // Delete other table Entries - Categories Source files 

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingCategoriesVsSourceFiles] "
                            + " WHERE CategoryId =  @CategoryId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);

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
            //
            // Delete Matching fields for this category 
            //
            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[MatchingCategoriesStagesVsMatchingFields] "
                            + " WHERE CategoryId =  @CategoryId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);


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
