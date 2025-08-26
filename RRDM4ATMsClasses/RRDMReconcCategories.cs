using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMReconcCategories : Logger
    {
        public RRDMReconcCategories() : base() { }

        public int SeqNo;

        public string CategoryId;
        public string CategoryName;

        public string Origin;
        public int AtmGroup;

        public bool IsOneMatchingCateg;

        public bool HasOwner;

        public string OwnerUserID;

        public DateTime OpeningDateTm;
        public string Operator;

        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

       
        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();


        // Define the data table 
        public DataTable TableReconcCateg;

        public int TotalSelected;
        public int TotalMatchingDone;
        public int TotalMatchingNotDone;
        public int TotalReconc;
        public int TotalNotReconc;
        public int TotalUnMatchedRecords;

        string SqlString; // Do not delete 

        readonly string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // Reconc Cat Reader Fields 
        private void ReconcCatReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            CategoryId = (string)rdr["CategoryId"];

            CategoryName = (string)rdr["CategoryName"];

            Origin = (string)rdr["Origin"];

            AtmGroup = (int)rdr["AtmGroup"];
            IsOneMatchingCateg = (bool)rdr["IsOneMatchingCateg"];

            HasOwner = (bool)rdr["HasOwner"];
            OwnerUserID = (string)rdr["OwnerUserID"];
            OpeningDateTm = (DateTime)rdr["OpeningDateTm"];

            Operator = (string)rdr["Operator"];
        }

        //
        // Methods 
        // READ ReconcCategories  
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesAndFillTable(string InOperator, string InOrigin)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InOrigin explanation 
            // ATMS/CARDs = ALL in MatchingCateg but RunningJobGroup not 'Nostro Reconc' and Not 'e_MOBILE'
            // Nostro Reconc =  MatchingCateg RunningJobGroup = 'Nostro Reconc' 
            // e_MOBILE = MatchingCateg RunningJobGroup = 'e_MOBILE' 

            RRDMMatchingCategories Mc = new RRDMMatchingCategories(); 

            TableReconcCateg = new DataTable();
            CultureInfo invC = CultureInfo.InvariantCulture;
            //CultureInfo currentCultureInfo = new CultureInfo("fr-FR");
            TableReconcCateg.Locale = invC;

            TableReconcCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableReconcCateg.Columns.Add("SeqNo", typeof(int));
            TableReconcCateg.Columns.Add("Identity", typeof(string));
            TableReconcCateg.Columns.Add("Category-Name", typeof(string));
            TableReconcCateg.Columns.Add("Origin", typeof(string));
            TableReconcCateg.Columns.Add("AtmGroup", typeof(int));
            TableReconcCateg.Columns.Add("OwnerUserID", typeof(string));

           
                SqlString = "SELECT * "
                   + " FROM [ATMS].[dbo].[ReconcCategories] "
                   + " WHERE Operator = @Operator  "
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
                        //if (InOrigin == "ALL")
                        //{
                        //    // Do nothing 
                        //}
                        //else
                        //{
                            cmd.Parameters.AddWithValue("@Origin", InOrigin);
                        //}

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            ReconcCatReaderFields(rdr);

                            bool IsGood = false;

                            Mc.ReadMatchingCategorybyActiveCategId(Operator, CategoryId); 

                            if (Mc.RecordFound & InOrigin == "ATMS/CARDs")
                            {
                                // TO COVER THE ACTIVE CATEGORIES
                                // Check RunningJobGroup
                                //RunningJobGroup <> 'e_MOBILE' AND RunningJobGroup <> 'Nostro Reconc' 
                                if (Mc.RunningJobGroup != "e_MOBILE" & Mc.RunningJobGroup != "Nostro Reconc")
                                {
                                    IsGood = true;
                                }

                            }

                            if (Mc.RecordFound & (InOrigin == "ETISALAT" || InOrigin == "QAHERA" || InOrigin == "IPN" || InOrigin == "EGATE"))
                            {
                                // TO COVER THE ACTIVE CATEGORIES
                                // Check RunningJobGroup
                                string Prefix_Category = CategoryId.Substring(0, 3);
                                string Prefix_Application = InOrigin.Substring(0, 3);
                                
                                if (Prefix_Category == Prefix_Application || InOrigin == "EGATE")
                                {
                                    IsGood = true;
                                }

                            }

                            if (Mc.RecordFound & InOrigin == "Nostro Reconc")
                            {
                                // TO COVER THE ACTIVE CATEGORIES
                                // Check RunningJobGroup
                              
                                if (Mc.RunningJobGroup == "Nostro Reconc")
                                {
                                    IsGood = true;
                                }
                            }

                            if (AtmGroup != 0 & InOrigin == "ATMS/CARDs")
                            {
                                // TO COVER THE GROUPS 
                                IsGood = true;
                            }

                            if (IsGood == true)
                            {
                                DataRow RowSelected = TableReconcCateg.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["Identity"] = CategoryId;
                                RowSelected["Category-Name"] = CategoryName;
                                RowSelected["Origin"] = Origin;

                                RowSelected["AtmGroup"] = AtmGroup;
                                RowSelected["OwnerUserID"] = OwnerUserID;

                                // ADD ROW
                                TableReconcCateg.Rows.Add(RowSelected);
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
        // READ ReconcCategories  
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesAndFillTableByOwner(string InOwnerUserID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconcCateg = new DataTable();
            TableReconcCateg.Clear();

            TotalSelected = 0;

        // DATA TABLE ROWS DEFINITION 
            TableReconcCateg.Columns.Add("SeqNo", typeof(int));
            TableReconcCateg.Columns.Add("CategoryId", typeof(string));
            TableReconcCateg.Columns.Add("CategoryName", typeof(string));
            TableReconcCateg.Columns.Add("Origin", typeof(string));
            TableReconcCateg.Columns.Add("OpeningDateTm", typeof(DateTime));
          

            SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[ReconcCategories] "
               + " WHERE OwnerUserID = @OwnerUserID  "
               + " ORDER BY CategoryId ASC ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@OwnerUserID", InOwnerUserID);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            ReconcCatReaderFields(rdr);
                  
                                DataRow RowSelected = TableReconcCateg.NewRow();
                            
                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["CategoryId"] = CategoryId;
                                RowSelected["CategoryName"] = CategoryName;
                                RowSelected["Origin"] = Origin;
                                RowSelected["OpeningDateTm"] = OpeningDateTm;
                               
                                // ADD ROW
                                TableReconcCateg.Rows.Add(RowSelected);

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
        // READ ReconcCategories  
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesAndFillTableWithDiscrepancies(string InOperator, string InSignedId, int InLimitRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SelectionCriteria; 

            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            RRDMGasParameters Gp = new RRDMGasParameters();

            bool Is_Presenter_InReconciliation = false;
            // Presenter
            string ParId = "946";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                Is_Presenter_InReconciliation = true;
            }

            TableReconcCateg = new DataTable();
            CultureInfo invC = CultureInfo.InvariantCulture;
            //CultureInfo currentCultureInfo = new CultureInfo("fr-FR");
            TableReconcCateg.Locale = invC;

            TableReconcCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableReconcCateg.Columns.Add("SeqNo", typeof(int));
            TableReconcCateg.Columns.Add("Identity", typeof(string));
            TableReconcCateg.Columns.Add("Category-Name", typeof(string));
            TableReconcCateg.Columns.Add("Exceptions", typeof(int));

            TableReconcCateg.Columns.Add("Atms_GL_Diff", typeof(int));
            
            TableReconcCateg.Columns.Add("Origin", typeof(string));
            TableReconcCateg.Columns.Add("AtmGroup", typeof(int));
            TableReconcCateg.Columns.Add("OwnerUserID", typeof(string));

           
                SqlString = "SELECT * "
                   + " FROM [ATMS].[dbo].[ReconcCategories] "
                   + " WHERE Operator = @Operator "
                   + " AND OwnerUserID = @OwnerUserID "
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
                      
                        cmd.Parameters.AddWithValue("@OwnerUserID", InSignedId);
                     
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            ReconcCatReaderFields(rdr);

                            
                            // Look to find if there is record in Categories Sessions for this user
                            Rcs.ReadReconcCategoriesSessionsByCategoryId_And_Userid(CategoryId, InSignedId); 
                            
                            if (Rcs.RecordFound == true)
                            {
                                DataRow RowSelected = TableReconcCateg.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["Identity"] = CategoryId;
                                RowSelected["Category-Name"] = CategoryName;

                                Rcs.ReadReconcCategoriesSessionsForRemainUnMatched(Operator, CategoryId, InLimitRMCycle, 2);
                               
                                if (Origin == "Our Atms" & Is_Presenter_InReconciliation == true)
                                {
                                  Mpa.ReadPoolAndFindTotals_Presenter_PerRMCategory(0, CategoryId, 1);
                                  RowSelected["Exceptions"] = Rcs.TotalRemainReconcExceptions + Mpa.Presenter_Not_Settled;
                                }
                                else
                                {
                                  RowSelected["Exceptions"] = Rcs.TotalRemainReconcExceptions ;
                                }

                                RowSelected["Atms_GL_Diff"] = 0;

                                RowSelected["Origin"] = Origin;

                                RowSelected["AtmGroup"] = AtmGroup;
                                RowSelected["OwnerUserID"] = OwnerUserID;

                                // ADD ROW
                                TableReconcCateg.Rows.Add(RowSelected);

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
                    MessageBox.Show("Error In ReadReconcCategoriesAndFillTableWithDiscrepancies");
                    conn.Close();

                    CatchDetails(ex);
                }
        }


        //
        public void ReadReconcCategoriesAndFillTableWithDiscrepancies_MOBILE(string InOperator, string InSignedId, int InLimitRMCycle
                                                                                                        , string InW_Application)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SelectionCriteria;
            string W_ApplicationPrefix = InW_Application.Substring(0, 3); 
            RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            RRDMGasParameters Gp = new RRDMGasParameters();

            //bool Is_Presenter_InReconciliation = false;
            //// Presenter
            //string ParId = "946";
            //string OccurId = "1";
            //Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");

            //if (Gp.OccuranceNm == "YES")
            //{
            //    Is_Presenter_InReconciliation = true;
            //}

            TableReconcCateg = new DataTable();
            CultureInfo invC = CultureInfo.InvariantCulture;
            //CultureInfo currentCultureInfo = new CultureInfo("fr-FR");
            TableReconcCateg.Locale = invC;

            TableReconcCateg.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableReconcCateg.Columns.Add("SeqNo", typeof(int));
            TableReconcCateg.Columns.Add("Identity", typeof(string));
            TableReconcCateg.Columns.Add("Category-Name", typeof(string));
            TableReconcCateg.Columns.Add("Exceptions", typeof(int));

            TableReconcCateg.Columns.Add("Atms_GL_Diff", typeof(int));

            TableReconcCateg.Columns.Add("Origin", typeof(string));
            TableReconcCateg.Columns.Add("AtmGroup", typeof(int));
            TableReconcCateg.Columns.Add("OwnerUserID", typeof(string));
            
            SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[ReconcCategories] "
               + " WHERE Operator = @Operator "
               + " AND OwnerUserID = @OwnerUserID AND  Left(Origin,3)=@Prefix"
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

                        cmd.Parameters.AddWithValue("@OwnerUserID", InSignedId);

                        cmd.Parameters.AddWithValue("@Prefix", W_ApplicationPrefix);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            ReconcCatReaderFields(rdr);


                            // Look to find if there is record in Categories Sessions for this user
                            Rcs.ReadReconcCategoriesSessionsByCategoryId_And_Userid(CategoryId, InSignedId);

                            if (Rcs.RecordFound == true)
                            {
                                DataRow RowSelected = TableReconcCateg.NewRow();

                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["Identity"] = CategoryId;
                                RowSelected["Category-Name"] = CategoryName;

                                Rcs.ReadReconcCategoriesSessionsForRemainUnMatched_MOBILE(Operator, CategoryId, InLimitRMCycle, InW_Application, 2);
                               

                                //if (Origin == "Our Atms" & Is_Presenter_InReconciliation == true)
                                //{
                                //    Mpa.ReadPoolAndFindTotals_Presenter_PerRMCategory(0, CategoryId, 1);
                                //    RowSelected["Exceptions"] = Rcs.TotalRemainReconcExceptions + Mpa.Presenter_Not_Settled;
                                //}
                                //else
                                //{
                                    RowSelected["Exceptions"] = Rcs.TotalRemainReconcExceptions;
                                //}

                                RowSelected["Atms_GL_Diff"] = 0;

                                RowSelected["Origin"] = Origin;

                                RowSelected["AtmGroup"] = AtmGroup;
                                RowSelected["OwnerUserID"] = OwnerUserID;

                                // ADD ROW
                                TableReconcCateg.Rows.Add(RowSelected);

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
                    MessageBox.Show("Error In ReadReconcCategoriesAndFillTableWithDiscrepancies");
                    conn.Close();

                    CatchDetails(ex);
                }
        }


        // ????????
        // Methods 
        // READ ReconcCategories with exceptions for Allocation of work   
        // FILL UP A TABLE
        //
        DateTime NullPastDate = new DateTime(1900, 01, 01);
        public void ReadReconcCategoriesForAllocation(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            TableReconcCateg = new DataTable();
            CultureInfo invC = CultureInfo.InvariantCulture;
            TableReconcCateg.Locale = invC;

            TableReconcCateg.Clear();

            TotalSelected = 0;
            TotalMatchingDone = 0;
            TotalMatchingNotDone = 0;
            TotalReconc = 0;
            TotalNotReconc = 0;
            TotalUnMatchedRecords = 0;

            // DATA TABLE ROWS DEFINITION 
            TableReconcCateg.Columns.Add("Identity", typeof(string));
            TableReconcCateg.Columns.Add("Category_Name", typeof(string));
            TableReconcCateg.Columns.Add("OutStanding", typeof(int));
            TableReconcCateg.Columns.Add("HasOwner", typeof(string));
            TableReconcCateg.Columns.Add("OwnerId", typeof(string));
            TableReconcCateg.Columns.Add("Matching_Dt", typeof(DateTime));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator "
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
                            ReconcCatReaderFields(rdr);

                            //if (MatchingDtTm.Date == DateTime.Today)
                            //{
                            //    TotalMatchingDone = TotalMatchingDone + 1;
                            //}
                            //else
                            //{
                            //    TotalMatchingNotDone = TotalMatchingNotDone + 1;
                            //}
                            // Read ALL Cycles for this category that has differences and reconciliation didnt start. 
                            Rms.ReadMatchingCategoriesSessionsSpecificCatForExceptions(CategoryId);

                            if (Rms.RecordFound == true)
                            {
                                //RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
                                Rcs.ReadReconcCategorySessionByCatAndRunningJobNo
                                    (Rms.Operator, Rms.CategoryId, Rms.RunningJobNo);
                                if (Rcs.EndReconcDtTm == NullPastDate)
                                {
                                    TotalSelected = TotalSelected + 1;

                                    TotalNotReconc = TotalNotReconc + 1;

                                    TotalUnMatchedRecords = TotalUnMatchedRecords + Rms.TotalUnMatchedRecs;

                                    DataRow RowSelected = TableReconcCateg.NewRow();

                                    RowSelected["Identity"] = CategoryId;
                                    RowSelected["Category_Name"] = CategoryName;

                                    RowSelected["OutStanding"] = Rms.TotalUnMatchedRecs;
                                    RowSelected["HasOwner"] = HasOwner;
                                    //RowSelected["OwnerId"] = OwnerId;

                                    //RowSelected["Matching_Dt"] = MatchingDtTm;

                                    // ADD ROW
                                    TableReconcCateg.Rows.Add(RowSelected);
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

        // ????????
        // Methods 
        // READ ReconcCategories with exceptions for Allocation of work   
        // FILL UP A TABLE
        //
        public int TotalRemain;
        string WSelectionCriteria;
        public int Cycle1;
        public int Cycle2;
        public int Cycle3;
        public int Cycle4;
        public int Cycle5;

        public string StatusCycle1;

        public int Inop; 

        public void ReadReconcCategoriesForMatrix(string InOperator, int InJobCycleNo, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Inop = 0;

            int WMode = InMode; // 2 Gives Total of all from InJobCycleNo till now 
                                // 3 Gives Total for all less than InJobCycleNo 

            //RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();
            //RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            TableReconcCateg = new DataTable();
            CultureInfo invC = CultureInfo.InvariantCulture;
            //CultureInfo currentCultureInfo = new CultureInfo("fr-FR");
            TableReconcCateg.Locale = invC;

            TableReconcCateg.Clear();

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = "ATMs";

            Rjc.ReadLastReconcJobCycleLastFive(InOperator, InJobCycleNo, WJobCategory);

            Cycle1 = Rjc.Cycle1;
            Cycle2 = Rjc.Cycle2;
            Cycle3 = Rjc.Cycle3;
            Cycle4 = Rjc.Cycle4;
            Cycle5 = Rjc.Cycle5;

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableReconcCateg.Columns.Add("CategoryId", typeof(string));

            TableReconcCateg.Columns.Add("This Cycle", typeof(string));

            TableReconcCateg.Columns.Add("All UnMatched", typeof(string));

            TableReconcCateg.Columns.Add("Cycle1", typeof(string));
            //TableReconcCateg.Columns.Add("Cycle2", typeof(string));
            //TableReconcCateg.Columns.Add("Cycle3", typeof(string));
            //TableReconcCateg.Columns.Add("Cycle4", typeof(string));
            //TableReconcCateg.Columns.Add("Cycle5", typeof(string));


            //// First row
            //DataRow RowSelected = TableReconcCateg.NewRow();

            //RowSelected["CategoryId"] = "Reconc Cycle No";

            //RowSelected["UnMatched"] = "N/A";

            //RowSelected["Cycle1"] = Rjc.Cycle1.ToString();
            //RowSelected["Cycle2"] = Rjc.Cycle2.ToString();
            //RowSelected["Cycle3"] = Rjc.Cycle3.ToString();
            //RowSelected["Cycle4"] = Rjc.Cycle4.ToString();
            //RowSelected["Cycle5"] = Rjc.Cycle5.ToString();

            //// ADD ROW
            //TableReconcCateg.Rows.Add(RowSelected);

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND HasOwner = 1 "
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
                            ReconcCatReaderFields(rdr);

                            // Second row

                            DataRow RowSelected2 = TableReconcCateg.NewRow();

                            RowSelected2["CategoryId"] = CategoryId;

                            Rcs.ReadReconcCategoriesSessionsForRemainUnMatched(Operator, CategoryId, InJobCycleNo, 1);

                            RowSelected2["This Cycle"] = Rcs.TotalRemainReconcExceptions.ToString();

                            Rcs.ReadReconcCategoriesSessionsForRemainUnMatched(Operator, CategoryId, InJobCycleNo, WMode);

                            RowSelected2["All UnMatched"] = Rcs.TotalRemainReconcExceptions.ToString();

                            //Rcs.ReadReconcCategoriesSessionsForRemainUnMatched(Operator, CategoryId, InJobCycleNo, 2);

                            //RowSelected2["UnMatched"] = Rcs.TotalRemainReconcExceptions.ToString();

                            RowSelected2["Cycle1"]  = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                                                  (InOperator, CategoryId, Rjc.Cycle1);
                            StatusCycle1 = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                                                  (InOperator, CategoryId, Rjc.Cycle1);
                            if (StatusCycle1 == "Inop") Inop = Inop + 1; 
                            //TotalRemain = TotalRemain + Rcs.RemainReconcExceptions; 
                            //RowSelected2["Cycle2"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                            //                      (InOperator, CategoryId, Rjc.Cycle2);
                            ////TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;
                            //RowSelected2["Cycle3"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                            //                      (InOperator, CategoryId, Rjc.Cycle3);
                            ////TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;
                            //RowSelected2["Cycle4"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                            //                      (InOperator, CategoryId, Rjc.Cycle4);
                            ////TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;
                            //RowSelected2["Cycle5"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                            //                      (InOperator, CategoryId, Rjc.Cycle5);
                            //TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;

                            // ADD ROW
                            TableReconcCateg.Rows.Add(RowSelected2);

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


        public void ReadReconcCategoriesForMatrixShort(string InOperator, int InJobCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Inop = 0;

            //RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();
            //RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

            TableReconcCateg = new DataTable();
            CultureInfo invC = CultureInfo.InvariantCulture;
            //CultureInfo currentCultureInfo = new CultureInfo("fr-FR");
            TableReconcCateg.Locale = invC;

            TableReconcCateg.Clear();

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = "ATMs";

            Rjc.ReadLastReconcJobCycleLastFive(InOperator, InJobCycleNo, WJobCategory);

            Cycle1 = Rjc.Cycle1;
            Cycle2 = Rjc.Cycle2;
            Cycle3 = Rjc.Cycle3;
            Cycle4 = Rjc.Cycle4;
            Cycle5 = Rjc.Cycle5;

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableReconcCateg.Columns.Add("CategoryId", typeof(string));

            TableReconcCateg.Columns.Add("This Cycle", typeof(string));

            TableReconcCateg.Columns.Add("All UnMatched", typeof(string));

            TableReconcCateg.Columns.Add("Cycle1", typeof(string));
            //TableReconcCateg.Columns.Add("Cycle2", typeof(string));
            //TableReconcCateg.Columns.Add("Cycle3", typeof(string));
            //TableReconcCateg.Columns.Add("Cycle4", typeof(string));
            //TableReconcCateg.Columns.Add("Cycle5", typeof(string));


            //// First row
            //DataRow RowSelected = TableReconcCateg.NewRow();

            //RowSelected["CategoryId"] = "Reconc Cycle No";

            //RowSelected["UnMatched"] = "N/A";

            //RowSelected["Cycle1"] = Rjc.Cycle1.ToString();
            //RowSelected["Cycle2"] = Rjc.Cycle2.ToString();
            //RowSelected["Cycle3"] = Rjc.Cycle3.ToString();
            //RowSelected["Cycle4"] = Rjc.Cycle4.ToString();
            //RowSelected["Cycle5"] = Rjc.Cycle5.ToString();

            //// ADD ROW
            //TableReconcCateg.Rows.Add(RowSelected);

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND HasOwner = 1 "
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
                            ReconcCatReaderFields(rdr);

                            // Second row

                            DataRow RowSelected2 = TableReconcCateg.NewRow();

                            RowSelected2["CategoryId"] = CategoryId;

                            Rcs.ReadReconcCategoriesSessionsForRemainUnMatched(Operator, CategoryId, InJobCycleNo, 1);

                            RowSelected2["This Cycle"] = Rcs.TotalRemainReconcExceptions.ToString();

                            Rcs.ReadReconcCategoriesSessionsForRemainUnMatched(Operator, CategoryId, InJobCycleNo, 2);

                            RowSelected2["All UnMatched"] = Rcs.TotalRemainReconcExceptions.ToString();

                            //Rcs.ReadReconcCategoriesSessionsForRemainUnMatched(Operator, CategoryId, InJobCycleNo, 2);

                            //RowSelected2["UnMatched"] = Rcs.TotalRemainReconcExceptions.ToString();

                            RowSelected2["Cycle1"] = StatusCycle1 = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                                                  (InOperator, CategoryId, Rjc.Cycle1);
                            //StatusCycle1 = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                            //                      (InOperator, CategoryId, Rjc.Cycle1);
                            if (StatusCycle1 == "Inop") Inop = Inop + 1;
                            //TotalRemain = TotalRemain + Rcs.RemainReconcExceptions; 
                            //RowSelected2["Cycle2"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                            //                      (InOperator, CategoryId, Rjc.Cycle2);
                            ////TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;
                            //RowSelected2["Cycle3"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                            //                      (InOperator, CategoryId, Rjc.Cycle3);
                            ////TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;
                            //RowSelected2["Cycle4"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                            //                      (InOperator, CategoryId, Rjc.Cycle4);
                            ////TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;
                            //RowSelected2["Cycle5"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                            //                      (InOperator, CategoryId, Rjc.Cycle5);
                            //TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;

                            // ADD ROW
                            TableReconcCateg.Rows.Add(RowSelected2);

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



        public void ReadReconcCategoriesForMatrix_MOBILE(string InOperator, int InJobCycleNo, string InApplication)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool IsGood = false; 

            Inop = 0;

            //RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();
            //RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
            RRDMMatchingCategories Mc = new RRDMMatchingCategories(); 

            TableReconcCateg = new DataTable();
            CultureInfo invC = CultureInfo.InvariantCulture;
            //CultureInfo currentCultureInfo = new CultureInfo("fr-FR");
            TableReconcCateg.Locale = invC;

            TableReconcCateg.Clear();

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

            string WJobCategory = InApplication ;

            Rjc.ReadLastReconcJobCycleLastFive(InOperator, InJobCycleNo, WJobCategory);

            Cycle1 = Rjc.Cycle1;
            Cycle2 = Rjc.Cycle2;
            Cycle3 = Rjc.Cycle3;
            Cycle4 = Rjc.Cycle4;
            Cycle5 = Rjc.Cycle5;

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableReconcCateg.Columns.Add("CategoryId", typeof(string));

            TableReconcCateg.Columns.Add("This Cycle", typeof(string));

            TableReconcCateg.Columns.Add("All UnMatched", typeof(string));

            TableReconcCateg.Columns.Add("Cycle1", typeof(string));
            TableReconcCateg.Columns.Add("Cycle2", typeof(string));
            TableReconcCateg.Columns.Add("Cycle3", typeof(string));
            TableReconcCateg.Columns.Add("Cycle4", typeof(string));
            TableReconcCateg.Columns.Add("Cycle5", typeof(string));



            //// ADD ROW
            //TableReconcCateg.Rows.Add(RowSelected);
            // WHERE Left(CategoryId , 3) = 'ETI'
            string InMobileType = InApplication.Substring(0, 3); 

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND HasOwner = 1 AND AtmGroup = 0 "
                    + " AND Left(CategoryId , 3) = @MobileType "
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
                        cmd.Parameters.AddWithValue("@MobileType", InMobileType);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            IsGood = false; 
                            // Read Fields 
                            ReconcCatReaderFields(rdr);

                            Mc.ReadMatchingCategorybyActiveCategId(InOperator, CategoryId);
                            
                            if (Mc.RunningJobGroup == "ETISALAT" || Mc.RunningJobGroup == "QAHERA" 
                                             || Mc.RunningJobGroup == "IPN" || Mc.RunningJobGroup == "EGATE")
                            {
                                IsGood = true; 
                            }
                            

                            // Second row
                            if (IsGood == true)
                            {
                                DataRow RowSelected2 = TableReconcCateg.NewRow();

                                RowSelected2["CategoryId"] = CategoryId;

                                Rcs.ReadReconcCategoriesSessionsForRemainUnMatched(Operator, CategoryId, InJobCycleNo, 1);

                                RowSelected2["This Cycle"] = Rcs.TotalRemainReconcExceptions.ToString();

                                Rcs.ReadReconcCategoriesSessionsForRemainUnMatched(Operator, CategoryId, InJobCycleNo, 2);

                                RowSelected2["All UnMatched"] = Rcs.TotalRemainReconcExceptions.ToString();

                                //Rcs.ReadReconcCategoriesSessionsForRemainUnMatched(Operator, CategoryId, InJobCycleNo, 2);

                                //RowSelected2["UnMatched"] = Rcs.TotalRemainReconcExceptions.ToString();

                                RowSelected2["Cycle1"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                                                      (InOperator, CategoryId, Rjc.Cycle1);
                                StatusCycle1 = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                                                      (InOperator, CategoryId, Rjc.Cycle1);
                                if (StatusCycle1 == "Inop") Inop = Inop + 1;
                                //TotalRemain = TotalRemain + Rcs.RemainReconcExceptions; 
                                RowSelected2["Cycle2"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                                                      (InOperator, CategoryId, Rjc.Cycle2);
                                //TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;
                                RowSelected2["Cycle3"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                                                      (InOperator, CategoryId, Rjc.Cycle3);
                                //TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;
                                RowSelected2["Cycle4"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                                                      (InOperator, CategoryId, Rjc.Cycle4);
                                //TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;
                                RowSelected2["Cycle5"] = Rcs.ReadReconcCategoriesSessionsSpecificForTotalPicture
                                                      (InOperator, CategoryId, Rjc.Cycle5);
                                //TotalRemain = TotalRemain + Rcs.RemainReconcExceptions;

                                // ADD ROW
                                TableReconcCateg.Rows.Add(RowSelected2);
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
        // READ ReconcCategory  by Seq no  
        // 
        //
        public void ReadReconcCategoriesbySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
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
                            ReconcCatReaderFields(rdr);

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
        public void ReadReconcCategorybyCategId(string InOperator, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND CategoryId = @CategoryId ";

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
                            ReconcCatReaderFields(rdr);


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
        public void ReadReconcCategorybyGroupId(int InAtmGroup)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE AtmGroup = @AtmGroup ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
              
                        cmd.Parameters.AddWithValue("@AtmGroup", InAtmGroup);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            ReconcCatReaderFields(rdr);

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
        public void ReadReconcCategorybyCategName(string InOperator, string InCategoryName)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND CategoryName = @CategoryName ";


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
                            ReconcCatReaderFields(rdr);

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
        // READ ReconcCategories by User Id
        // 
        //
        public void ReadReconcCategorybyUserId(string InOperator, string InOwnerUserID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE Operator = @Operator AND OwnerUserID = @OwnerUserID ";


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@OwnerUserID", InOwnerUserID);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            // Read Fields 
                            ReconcCatReaderFields(rdr);

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
        // Find Reconciliation Categories for this USER for the ATMs 
        // 
        //

        public void ReadReconcCategories_Fill_Table_ForUser(string InOwnerUserID)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableReconcCateg = new DataTable();
            TableReconcCateg.Clear();

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[ReconcCategories] "
                    + " WHERE OwnerUserID = @OwnerUserID AND Origin = 'Our Atms' ";

            using (SqlConnection conn =
            new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                         sqlAdapt.SelectCommand.Parameters.AddWithValue("@OwnerUserID", InOwnerUserID);
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableReconcCateg);

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

            SqlString = "SELECT * FROM [ATMS].[dbo].[ReconcCategories]"
                     + " WHERE Operator = @Operator   Order by CategoryId ASC ";


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

                            string CatIdAndName = CategoryId + "*" + CategoryName;

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
                   + " FROM [ATMS].[dbo].[ReconcCategories] "
                   + " WHERE Operator = @Operator "
                   + " ORDER BY CategoryId ASC ";
            }
            else
            {
                SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[ReconcCategories] "
                   + " WHERE Operator = @Operator AND Origin = @Origin "
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

        // Insert Reconc Category
        //
        public int InsertReconcCategory()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ReconcCategories]"
                    + "([CategoryId], [CategoryName],  "
                    + " [Origin], "
                    + " [AtmGroup], [IsOneMatchingCateg], "
                    + " [HasOwner], [OwnerUserID],"
                    + " [OpeningDateTm], "
                    + " [Operator] )"
                    + " VALUES (@CategoryId, @CategoryName,"
                    + " @Origin, "
                    + " @AtmGroup, @IsOneMatchingCateg,"
                    + " @HasOwner, @OwnerUserID, "
                    + " @OpeningDateTm, "
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

                        cmd.Parameters.AddWithValue("@AtmGroup", AtmGroup);
                        cmd.Parameters.AddWithValue("@IsOneMatchingCateg", IsOneMatchingCateg);
                        cmd.Parameters.AddWithValue("@HasOwner", HasOwner);
                        cmd.Parameters.AddWithValue("@OwnerUserID", OwnerUserID);

                        cmd.Parameters.AddWithValue("@OpeningDateTm", OpeningDateTm);

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
        public void UpdateReconcCategory(string InOperator, string InCategoryId)
        {
            Successful = false;
            ErrorFound = false;
            ErrorOutput = "";
           // OpeningDateTm
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategories] SET "
                            + " CategoryId = @CategoryId, CategoryName = @CategoryName, "
                            + " Origin = @Origin, "
                            + " AtmGroup = @AtmGroup, IsOneMatchingCateg = @IsOneMatchingCateg, "
                            + " HasOwner = @HasOwner,  "
                            + " OpeningDateTm = @OpeningDateTm,  "
                            + " OwnerUserID = @OwnerUserID  "
                            + " WHERE CategoryId = @CategoryId", conn))
                    {

                        cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
                        cmd.Parameters.AddWithValue("@CategoryName", CategoryName);

                        cmd.Parameters.AddWithValue("@Origin", Origin);

                        cmd.Parameters.AddWithValue("@AtmGroup", AtmGroup);
                        cmd.Parameters.AddWithValue("@IsOneMatchingCateg", IsOneMatchingCateg);
                        cmd.Parameters.AddWithValue("@HasOwner", HasOwner);
                        cmd.Parameters.AddWithValue("@OpeningDateTm", DateTime.Now);

                        cmd.Parameters.AddWithValue("@OwnerUserID", OwnerUserID);


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

        //
        // DELETE Category
        //
        public void DeleteReconcCategory(string InCategoryId)
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
        
            // Delete other table Entries - Categories Matching Categories

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReconcCateqoriesVsMatchingCategories] "
                            + " WHERE ReconcCategoryId =  @ReconcCategoryId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InCategoryId);

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
        //
        // READ RECONC CATEGORIES
        // AND CREATE RECONCILIATION SESSIONS 
        // 
        public void CreateReconciliationSessionsForAtms_AND_JCC(string InOperator, string InSignedId,
                                              int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            WSelectionCriteria = "";

            RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();

            TableReconcCateg = new DataTable();
            CultureInfo invC = CultureInfo.InvariantCulture;
            TableReconcCateg.Locale = invC;
            TableReconcCateg.Clear();

            TotalSelected = 0;

            SqlString =

                " SELECT * FROM [ATMS].[dbo].[ReconcCategories]  "
                + " Where Operator = @Operator "
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
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                        //Create a datatable that will be filled with the data retrieved from the command
                      
                        sqlAdapt.Fill(TableReconcCateg);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableReconcCateg.Rows.Count - 1))
                        {

                            // For each enry in table Update records. 

                            // READ 
                            SeqNo = (int)TableReconcCateg.Rows[I]["SeqNo"];

                            CategoryId = (string)TableReconcCateg.Rows[I]["CategoryId"];
                            CategoryName = (string)TableReconcCateg.Rows[I]["CategoryName"];
                            Origin = (string)TableReconcCateg.Rows[I]["Origin"];

                            AtmGroup = (int)TableReconcCateg.Rows[I]["AtmGroup"];
                            IsOneMatchingCateg = (bool)TableReconcCateg.Rows[I]["IsOneMatchingCateg"];

                            HasOwner = (bool)TableReconcCateg.Rows[I]["HasOwner"];
                            OwnerUserID = (string)TableReconcCateg.Rows[I]["OwnerUserID"];

                            OpeningDateTm = (DateTime)TableReconcCateg.Rows[I]["OpeningDateTm"];
                            Operator = (string)TableReconcCateg.Rows[I]["Operator"];
                            // Get Fields of last record for initialisation 
                            Rcs.ReadReconcCategorySessionByCatToFindTheLastRecord(Operator, CategoryId);

                            Rcs.CategoryId = CategoryId;
                            Rcs.CategoryName = CategoryName;
                            Rcs.AtmGroup = AtmGroup;
                            //******************************************
                            // FIND OUT THE DETAILS AND ASSIGN THEM BELOW
                            //******************************************
                            //******************************************
                            Rcs.GlAccountNo = "";
                            Rcs.GlYesterdaysBalance = 0;
                            Rcs.GlTodaysBalance = 0; 

                            Rcs.OwnerUserID = OwnerUserID;
                            Rcs.RunningJobNo = InRunningJobNo;

                            WSelectionCriteria = " WHERE ReconcCategoryId ='" + CategoryId + "'";
                            RcMc.ReadReconcCateqoriesVsMatchingCategoriesbySelectionCriteria(WSelectionCriteria);

                            if (RcMc.RecordFound == false)
                            {
                                // JCC say
                                Rcs.MatchingCat01 = CategoryId;
                                Rcs.MatchingCat02 = "";
                                Rcs.MatchingCat03 = "";
                                Rcs.MatchingCat04 = "";
                                Rcs.MatchingCat05 = "";
                                Rcs.MatchingCat06 = "";
                                Rcs.MatchingCat07 = "";
                                Rcs.MatchingCat08 = "";
                                Rcs.MatchingCat09 = "";

                            }
                            else
                            {
                                // Group of ATNs
                                Rcs.MatchingCat01 = RcMc.WMatchingCat01;
                                Rcs.MatchingCat02 = RcMc.WMatchingCat02;
                                Rcs.MatchingCat03 = RcMc.WMatchingCat03;
                                Rcs.MatchingCat04 = RcMc.WMatchingCat04;
                                Rcs.MatchingCat05 = RcMc.WMatchingCat05;
                                Rcs.MatchingCat06 = RcMc.WMatchingCat06;
                                Rcs.MatchingCat07 = RcMc.WMatchingCat07;
                                Rcs.MatchingCat08 = RcMc.WMatchingCat08;
                                Rcs.MatchingCat09 = RcMc.WMatchingCat09;
                            }
                         
                            //
                            Rcs.ProcessMode = -1; // 
                            //
                            Rcs.Operator = Operator;

                            Rcs.InsertReconcCategoriesSessionRecord();

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

        public void CreateReconciliationSessionsFor_MOBILE(string InJobCategory, string InOperator, string InSignedId,
                                      int InRunningJobNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            WSelectionCriteria = "";

            RRDMReconcCateqoriesVsMatchingCategories RcMc = new RRDMReconcCateqoriesVsMatchingCategories();

            TableReconcCateg = new DataTable();
            CultureInfo invC = CultureInfo.InvariantCulture;
            TableReconcCateg.Locale = invC;
            TableReconcCateg.Clear();

            TotalSelected = 0;

            SqlString =

                " SELECT * FROM [ATMS].[dbo].[ReconcCategories]  "
                + " Where Operator = @Operator "
                + " AND Left(CategoryId, 3) = '" + InJobCategory.Substring(0, 3) + "'"
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
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableReconcCateg);

                        // Close conn
                        conn.Close();

                        int I = 0;

                        while (I <= (TableReconcCateg.Rows.Count - 1))
                        {

                            // For each enry in table Update records. 

                            // READ 
                            SeqNo = (int)TableReconcCateg.Rows[I]["SeqNo"];

                            CategoryId = (string)TableReconcCateg.Rows[I]["CategoryId"];
                            CategoryName = (string)TableReconcCateg.Rows[I]["CategoryName"];
                            Origin = (string)TableReconcCateg.Rows[I]["Origin"];

                            AtmGroup = (int)TableReconcCateg.Rows[I]["AtmGroup"];
                            IsOneMatchingCateg = (bool)TableReconcCateg.Rows[I]["IsOneMatchingCateg"];

                            HasOwner = (bool)TableReconcCateg.Rows[I]["HasOwner"];
                            OwnerUserID = (string)TableReconcCateg.Rows[I]["OwnerUserID"];

                            OpeningDateTm = (DateTime)TableReconcCateg.Rows[I]["OpeningDateTm"];
                            Operator = (string)TableReconcCateg.Rows[I]["Operator"];
                            // Get Fields of last record for initialisation 
                            Rcs.ReadReconcCategorySessionByCatToFindTheLastRecord(Operator, CategoryId);

                            Rcs.CategoryId = CategoryId;
                            Rcs.CategoryName = CategoryName;
                            Rcs.AtmGroup = AtmGroup;
                            //******************************************
                            // FIND OUT THE DETAILS AND ASSIGN THEM BELOW
                            //******************************************
                            //******************************************
                            Rcs.GlAccountNo = "";
                            Rcs.GlYesterdaysBalance = 0;
                            Rcs.GlTodaysBalance = 0;

                            Rcs.OwnerUserID = OwnerUserID;
                            Rcs.RunningJobNo = InRunningJobNo;

                            WSelectionCriteria = " WHERE ReconcCategoryId ='" + CategoryId + "'";
                            RcMc.ReadReconcCateqoriesVsMatchingCategoriesbySelectionCriteria(WSelectionCriteria);

                            if (RcMc.RecordFound == false)
                            {
                                // JCC say
                                Rcs.MatchingCat01 = CategoryId;
                                Rcs.MatchingCat02 = "";
                                Rcs.MatchingCat03 = "";
                                Rcs.MatchingCat04 = "";
                                Rcs.MatchingCat05 = "";
                                Rcs.MatchingCat06 = "";
                                Rcs.MatchingCat07 = "";
                                Rcs.MatchingCat08 = "";
                                Rcs.MatchingCat09 = "";

                            }
                            else
                            {
                                // Group of ATNs
                                Rcs.MatchingCat01 = RcMc.WMatchingCat01;
                                Rcs.MatchingCat02 = RcMc.WMatchingCat02;
                                Rcs.MatchingCat03 = RcMc.WMatchingCat03;
                                Rcs.MatchingCat04 = RcMc.WMatchingCat04;
                                Rcs.MatchingCat05 = RcMc.WMatchingCat05;
                                Rcs.MatchingCat06 = RcMc.WMatchingCat06;
                                Rcs.MatchingCat07 = RcMc.WMatchingCat07;
                                Rcs.MatchingCat08 = RcMc.WMatchingCat08;
                                Rcs.MatchingCat09 = RcMc.WMatchingCat09;
                            }

                            //
                            Rcs.ProcessMode = -1; // 
                            //
                            Rcs.Operator = Operator;

                            Rcs.InsertReconcCategoriesSessionRecord();

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



    }
}
