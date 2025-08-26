using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMTempTablesForReportsATMS : Logger
    {
        public RRDMTempTablesForReportsATMS() : base() { }

        public int RecordId;
        public string Status;
        public bool Select;
        public string MatchMask;
        public string ActionType;
        public string ActionDesc;
        public bool Settled;
        public string Done;

        public string ATMNo;
        public int Exception;

        public string Descr;
        public string Card;

        public string Account;
        public string Ccy;
        public decimal Amount;

        public DateTime Date;
        public string MatchingCateg;
        public string RMCateg;
        public int ExceptID;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // Insert Report54
        ////
        public void InsertReport54(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport54] "
                + " ([UserId], "
                + " [RecordId],"
                + " [Status],"
                + " [Done],"
                + " [ATMNo], "
                + " [Descr],"
                + " [Card], "
                + " [Account], "
                + " [Ccy],"
                + " [Amount], "
                + " [Date],"
                + " [MatchingCateg], "
                + " [RMCateg], "
                + " [ExceptID]  ) "
                + " VALUES "
                + "(@UserId,"
                + "@RecordId,"
                + "@Status,"
                + "@Done,"
                + "@ATMNo,"
                + "@Descr,"
                + "@Card,"
                + "@Account,"
                + "@Ccy,"
                + "@Amount,"
                + "@Date,"
                + "@MatchingCateg,"
                + "@RMCateg,"
                + "@ExceptID )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@RecordId", RecordId);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Done", Done);

                        cmd.Parameters.AddWithValue("@ATMNo", ATMNo);
                        cmd.Parameters.AddWithValue("@Descr", Descr);
                        cmd.Parameters.AddWithValue("@Card", Card);

                        cmd.Parameters.AddWithValue("@Account", Account);
                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@Amount", Amount);

                        cmd.Parameters.AddWithValue("@Date", Date);

                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@ExceptID", ExceptID);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

        //
        // DELETE Report54
        //
        public void DeleteReport54(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport54] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

        // Insert Report55 
        ////

        public string FirstEntry ;
        public string FirstEntryAccno;

        public string SecondEntry;
        public string SecondEntryAccno;

        public void InsertReport55(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport55] "
                + " ([UserId], "
                + " [RecordId],"
                + " [Select],"
                + " [MatchMask],"
                + " [ActionType], "
                + " [ActionDesc], "
                + " [Settled], "
                + " [Date], "
                + " [Descr],"
                + " [Ccy],"
                + " [Amount], "
                  + " [TraceNo], "
                + " [RRNumber], "
                + " [Exception],"
                + " [Card], "
                + " [Account], "
                + " [MatchingCateg], "
                + " [RMCateg], "
                + " [ATMNo],  "
                   + " [FirstEntry], "
                + " [FirstEntryAccno], "
                + " [SecondEntry], "
                + " [SecondEntryAccno]  ) "
                + " VALUES "
                + "(@UserId,"
                + "@RecordId,"
                + "@Select,"
                + "@MatchMask,"
                + "@ActionType,"
                + "@ActionDesc,"
                + "@Settled,"
                + "@Date,"
                + "@Descr,"
                + "@Ccy,"
                + "@Amount,"
                 + "@TraceNo, "
                + "@RRNumber, "
                + " @Exception,"
                + "@Card,"
                + "@Account,"
                + "@MatchingCateg,"
                + "@RMCateg,"
                + "@ATMNo, "
            + "@FirstEntry,"
            + "@FirstEntryAccno,"
            + "@SecondEntry,"
            + "@SecondEntryAccno )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@RecordId", RecordId);

                        cmd.Parameters.AddWithValue("@Select", Select);
                        cmd.Parameters.AddWithValue("@MatchMask", MatchMask);
                        cmd.Parameters.AddWithValue("@ActionType", ActionType);
                        cmd.Parameters.AddWithValue("@ActionDesc", ActionDesc);
                        cmd.Parameters.AddWithValue("@Settled", Settled);
                        cmd.Parameters.AddWithValue("@Date", Date);
                        cmd.Parameters.AddWithValue("@Descr", Descr);
                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@Amount", Amount);

                        cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
                        cmd.Parameters.AddWithValue("@RRNumber", RRNumber);

                        cmd.Parameters.AddWithValue("@Exception", Exception);

                        cmd.Parameters.AddWithValue("@Card", Card);

                        cmd.Parameters.AddWithValue("@Account", Account);

                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@ATMNo", ATMNo);

                        cmd.Parameters.AddWithValue("@FirstEntry", FirstEntry);

                        cmd.Parameters.AddWithValue("@FirstEntryAccno", FirstEntryAccno);
                        cmd.Parameters.AddWithValue("@SecondEntry", SecondEntry);
                        cmd.Parameters.AddWithValue("@SecondEntryAccno", SecondEntryAccno);


                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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
        public string RRNumber;
        public string TXNSRC;
        public string TXNDEST;
        public DateTime CAP_DATE;
        public DateTime SET_DATE;

        public void InsertReport55_2(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

//            Maker nvarchar(50)	Unchecked
//Author  nvarchar(50)    Unchecked
//CAP_DATE    date Unchecked

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport55_2] "
                + " ([UserId], "
                + " [RecordId],"
                //+ " [Select],"
                + " [MatchMask],"
                + " [ActionType], "
                + " [ActionDesc], "
                + " [Settled], "
                + " [Date], "
                + " [Descr],"
                + " [Ccy],"
                + " [Amount], "
                + " [TraceNo], "
                + " [RRNumber], "
                + " [Exception],"
                + " [Card], "
                + " [Account], "
                + " [MatchingCateg], "
                + " [RMCateg], "
                + " [TXNSRC], "
                + " [TXNDEST], "
                + " [ATMNo],  "
                + " CAP_DATE,"
                  + " SET_DATE"
                + ") "
               
                + " VALUES "
                + "(@UserId,"
                + "@RecordId,"
                //+ "@Select,"
                + "@MatchMask,"
                + "@ActionType,"
                + "@ActionDesc,"
                + "@Settled,"
                + "@Date,"
                + "@Descr,"
                + "@Ccy,"
                + "@Amount,"
                + "@TraceNo, "
                + "@RRNumber, "
                + " @Exception,"
                + "@Card,"
                + "@Account,"
                + "@MatchingCateg,"
                + "@RMCateg,"
                + "@TXNSRC,"
                + "@TXNDEST,"
                + "@ATMNo, "
                + " @CAP_DATE,"
                  + " @SET_DATE"
                + ")";
           

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@RecordId", RecordId);

                       // cmd.Parameters.AddWithValue("@Select", Select);
                        cmd.Parameters.AddWithValue("@MatchMask", MatchMask);
                        cmd.Parameters.AddWithValue("@ActionType", ActionType);
                        cmd.Parameters.AddWithValue("@ActionDesc", ActionDesc);
                        cmd.Parameters.AddWithValue("@Settled", Settled);
                        cmd.Parameters.AddWithValue("@Date", Date);
                        cmd.Parameters.AddWithValue("@Descr", Descr);
                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@Amount", Amount);

                        cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
                        cmd.Parameters.AddWithValue("@RRNumber", RRNumber);

                        cmd.Parameters.AddWithValue("@Exception", Exception);

                        cmd.Parameters.AddWithValue("@Card", Card);

                        cmd.Parameters.AddWithValue("@Account", Account);

                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);

                        cmd.Parameters.AddWithValue("@TXNSRC", TXNSRC);
                        cmd.Parameters.AddWithValue("@TXNDEST", TXNDEST);

                        cmd.Parameters.AddWithValue("@ATMNo", ATMNo);
                        cmd.Parameters.AddWithValue("@CAP_DATE", CAP_DATE);

                        cmd.Parameters.AddWithValue("@SET_DATE", SET_DATE);


                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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
        //public int SeqNo;
        //public string RespCode;
        //public string Category;
        //public int RevSeqNo;
        //public string RevResp;

        //public void InsertReport55_3(string InUserId)
        //{
        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    //Tr.SeqNo = (int)ReversalsDataTable.Rows[I]["SeqNo"];
        //    //Tr.ATMNo = (string)ReversalsDataTable.Rows[I]["ATMNo"];
        //    //Tr.Descr = (string)ReversalsDataTable.Rows[I]["Descr"];
        //    //Tr.Card = (string)ReversalsDataTable.Rows[I]["Card"];

        //    //Tr.Account = (string)ReversalsDataTable.Rows[I]["Account"];
        //    //Tr.Ccy = (string)ReversalsDataTable.Rows[I]["Ccy"];
        //    //Tr.Amount = (decimal)ReversalsDataTable.Rows[I]["Amount"];
        //    //Tr.Date = (DateTime)ReversalsDataTable.Rows[I]["Date"];

        //    //Tr.TraceNo = (int)ReversalsDataTable.Rows[I]["TraceNo"];
        //    //Tr.RRNumber = (string)ReversalsDataTable.Rows[I]["RRNumber"];
        //    //Tr.RespCode = (string)ReversalsDataTable.Rows[I]["RespCode"];
        //    //Tr.FileId = (string)ReversalsDataTable.Rows[I]["FileId"];

        //    //Tr.Category = (string)ReversalsDataTable.Rows[I]["Category"];
        //    //Tr.RevSeqNo = (int)ReversalsDataTable.Rows[I]["RevSeqNo"];
        //    //Tr.RevResp = (string)ReversalsDataTable.Rows[I]["RevResp"];

        //    string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport55_3] "
        //        + " ([UserId], "
        //        + " [SeqNo],"
        //        + " [MatchMask],"
        //        + " [ActionType], "
        //        + " [ActionDesc], "
        //        + " [Settled], "
        //        + " [Date], "
        //        + " [Descr],"
        //        + " [Ccy],"
        //        + " [Amount], "
        //          + " [TraceNo], "
        //        + " [RRNumber], "
        //        + " [Exception],"
        //        + " [Card], "
        //        + " [Account], "
        //        + " [MatchingCateg], "
        //        + " [RMCateg], "
        //        + " [ATMNo],  "
        //           + " [FirstEntry], "
        //        + " [FirstEntryAccno], "
        //        + " [SecondEntry], "
        //        + " [SecondEntryAccno]  ) "
        //        + " VALUES "
        //        + "(@UserId,"
        //        + "@SeqNo,"
        //        //+ "@Select,"
        //        + "@MatchMask,"
        //        + "@ActionType,"
        //        + "@ActionDesc,"
        //        + "@Settled,"
        //        + "@Date,"
        //        + "@Descr,"
        //        + "@Ccy,"
        //        + "@Amount,"
        //         + "@TraceNo, "
        //        + "@RRNumber, "
        //        + " @Exception,"
        //        + "@Card,"
        //        + "@Account,"
        //        + "@MatchingCateg,"
        //        + "@RMCateg,"
        //        + "@ATMNo, "
        //    + "@FirstEntry,"
        //    + "@FirstEntryAccno,"
        //    + "@SecondEntry,"
        //    + "@SecondEntryAccno )";

        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =

        //               new SqlCommand(cmdinsert, conn))
        //            {

        //                cmd.Parameters.AddWithValue("@UserId", InUserId);

        //                cmd.Parameters.AddWithValue("@SeqNo", SeqNo);

        //                // cmd.Parameters.AddWithValue("@Select", Select);
        //                cmd.Parameters.AddWithValue("@MatchMask", MatchMask);
        //                cmd.Parameters.AddWithValue("@ActionType", ActionType);
        //                cmd.Parameters.AddWithValue("@ActionDesc", ActionDesc);
        //                cmd.Parameters.AddWithValue("@Settled", Settled);
        //                cmd.Parameters.AddWithValue("@Date", Date);
        //                cmd.Parameters.AddWithValue("@Descr", Descr);
        //                cmd.Parameters.AddWithValue("@Ccy", Ccy);
        //                cmd.Parameters.AddWithValue("@Amount", Amount);

        //                cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
        //                cmd.Parameters.AddWithValue("@RRNumber", RRNumber);

        //                cmd.Parameters.AddWithValue("@Exception", Exception);

        //                cmd.Parameters.AddWithValue("@Card", Card);

        //                cmd.Parameters.AddWithValue("@Account", Account);

        //                cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
        //                cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
        //                cmd.Parameters.AddWithValue("@ATMNo", ATMNo);

        //                cmd.Parameters.AddWithValue("@FirstEntry", FirstEntry);

        //                cmd.Parameters.AddWithValue("@FirstEntryAccno", FirstEntryAccno);
        //                cmd.Parameters.AddWithValue("@SecondEntry", SecondEntry);
        //                cmd.Parameters.AddWithValue("@SecondEntryAccno", SecondEntryAccno);


        //                //rows number of record got updated

        //                rows = cmd.ExecuteNonQuery();
        //                //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
        //                //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

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
        // DELETE Report55
        //
        public void DeleteReport55(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport55] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report55_2
        //
        public void DeleteReport55_2(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport55_2] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report55_3
        //
        public void DeleteReport55_3(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport55_3] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report55_4
        //
        public void DeleteReport55_4(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport55_4] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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

        public void DeleteReport55_4_MOBILE(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport55_4_MOBILE] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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


        public string AtmNo;
        public string AtmName;
        public string Branch;
        public string BranchName;
        public string Street;

        public string Town;
        public string District;
        public string PostalCode;

        public string Country;
        public string Model;
        public string TypeOfRepl;
        public int AtmsReconcGroup;
        public string ATMIPAddress;
        public string SWVersion;

        public double Latitude;
        public double Longitude;
        // Insert Report56 - ATMS Details 
        ////
        public void InsertReport56(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport56] "
                + " ([UserId], "
                + " [AtmNo],"
                + " [AtmName],"
                + " [Branch],"
                + " [BranchName], "
                + " [Street], "
                + " [Town], "
                + " [District], "
                + " [PostalCode],"
                + " [Country],"
                + " [Model], "
                + " [TypeOfRepl],"
                + " [AtmsReconcGroup], "
                + " [SWVersion], "
                + " [ATMIPAddress]  ) "
                + " VALUES "
                + "(@UserId,"
                + " @AtmNo,"
                + " @AtmName,"
                + " @Branch,"
                + " @BranchName, "
                + " @Street, "
                + " @Town, "
                + " @District, "
                + " @PostalCode,"
                + " @Country,"
                + " @Model, "
                + " @TypeOfRepl,"
                + " @AtmsReconcGroup, "
                + " @SWVersion, "
                + " @ATMIPAddress  ) ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@AtmName", AtmName);
                        cmd.Parameters.AddWithValue("@Branch", Branch);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);
                        cmd.Parameters.AddWithValue("@Street", Street);

                        cmd.Parameters.AddWithValue("@Town", Town);
                        cmd.Parameters.AddWithValue("@District", District);
                        cmd.Parameters.AddWithValue("@PostalCode", PostalCode);

                        cmd.Parameters.AddWithValue("@Country", Country);
                        cmd.Parameters.AddWithValue("@Model", Model);
                        cmd.Parameters.AddWithValue("@TypeOfRepl", TypeOfRepl);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);
                        cmd.Parameters.AddWithValue("@ATMIPAddress", ATMIPAddress);
                        cmd.Parameters.AddWithValue("@SWVersion", SWVersion);
                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

        //
        // DELETE Report56
        //
        public void DeleteReport56(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport56] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // Branches 
        public bool E_Finance;
        public string InternalAcno;
        public string ExternalAcno; 

        public void InsertReport76(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";
   
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport76] "
                + " ([UserId], "
                + " [Branch],"
                + " [BranchName], "
                + " [Street], "
                + " [Town], "
                + " [District], "
                + " [PostalCode],"
                + " [Country],"
                + " [Latitude], "
                + " [Longitude], "
                + " [E_Finance],"
                + " [InternalAcno], "
                + " [ExternalAcno] "
                + " ) "
                + " VALUES "
                + "(@UserId,"
                + " @Branch,"
                + " @BranchName, "
                + " @Street, "
                + " @Town, "
                + " @District, "
                + " @PostalCode,"
                + " @Country,"
                + " @Latitude, "
                + " @Longitude,  "
                + " @E_Finance,"
                + " @InternalAcno, "
                + " @ExternalAcno  "
                + ") ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@Branch", Branch);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);
                        cmd.Parameters.AddWithValue("@Street", Street);

                        cmd.Parameters.AddWithValue("@Town", Town);
                        cmd.Parameters.AddWithValue("@District", District);
                        cmd.Parameters.AddWithValue("@PostalCode", PostalCode);

                        cmd.Parameters.AddWithValue("@Country", Country);
                        cmd.Parameters.AddWithValue("@Latitude", Latitude);
                        cmd.Parameters.AddWithValue("@Longitude", Longitude);

                        cmd.Parameters.AddWithValue("@E_Finance", E_Finance);
                        cmd.Parameters.AddWithValue("@InternalAcno", InternalAcno);
                        cmd.Parameters.AddWithValue("@ExternalAcno", ExternalAcno);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

        //
        // DELETE Report76
        //
        public void DeleteReport76(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport76] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // Report WRepor78
        public string LineUserId;
        public string UserName;  
        public string email;

        public string MobileNo; 
        public string DateOpen; 
        public string UserType; 
        public string CitId;
        public void InsertReport78(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport78] "
                + " ([UserId], "
                + " [LineUserId],"
                + " [UserName], "
                + " [email], "
                + " [MobileNo], "
                + " [DateOpen], "
                + " [UserType],"
                + " [CitId] "
                + " ) "
                + " VALUES "
                + "(@UserId,"
                + " @LineUserId,"
                + " @UserName, "
                + " @email, "
                + " @MobileNo, "
                + " @DateOpen, "
                + " @UserType,"
                + " @CitId  "
                + ") ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@LineUserId", LineUserId);
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@email", email);

                        cmd.Parameters.AddWithValue("@MobileNo", MobileNo);
                        cmd.Parameters.AddWithValue("@DateOpen", DateOpen);
                   
                        cmd.Parameters.AddWithValue("@UserType", UserType);
                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

        //
        // DELETE Report78
        //
        public void DeleteReport78(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport78] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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

        // CONTROLS 
        public string MainFormId;
        public string PanelName;
        public string ButtonName;
        public string ButtonText;

        int rows;

        // Insert Report57 - CONTROLS
        ////
        public void InsertReport57(string InOperator, string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport57] "
                + " ([UserId], "
                + " [MainFormId],"
                + " [PanelName],"
                + " [ButtonName],"
                + " [ButtonText],  "
                + " [Operator]  "
                + ") "
                + " VALUES "
                + "(@UserId,"
                + " @MainFormId,"
                + " @PanelName,"
                + " @ButtonName,"
                + " @ButtonText,  "
                + " @Operator  "
                + ") ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@MainFormId", MainFormId);

                        cmd.Parameters.AddWithValue("@PanelName", PanelName);

                        cmd.Parameters.AddWithValue("@ButtonName", ButtonName);

                        cmd.Parameters.AddWithValue("@ButtonText", ButtonText);

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

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

        //
        // DELETE Report57
        //
        public void DeleteReport57(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport57] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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

        // Insert Report58 - ATMS Migration  
        ////
        public string MigrationCycleToString;
        public string UpdateOrInsert;
        public string MigrationResult;
        public void InsertReport58(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport58] "
                + " ([UserId], "
                + " [MigrationCycleToString],"
                + " [AtmNo],"
                + " [Branch],"
                + " [BranchName], "
                + " [Model], "
                + " [AtmsReconcGroup], "
                + " [ATMIPAddress], "
                + " [UpdateOrInsert], "
                + " [MigrationResult]  ) "
                + " VALUES "
                + "(@UserId,"
                + " @MigrationCycleToString,"
                + " @AtmNo,"
                + " @Branch,"
                + " @BranchName, "
                + " @Model, "
                + " @AtmsReconcGroup, "
                + " @ATMIPAddress, "
                + " @UpdateOrInsert, "
                + " @MigrationResult  ) ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@MigrationCycleToString", MigrationCycleToString);
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@Branch", Branch);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);

                        cmd.Parameters.AddWithValue("@Model", Model);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

                        cmd.Parameters.AddWithValue("@ATMIPAddress", ATMIPAddress);
                        cmd.Parameters.AddWithValue("@UpdateOrInsert", UpdateOrInsert);
                        cmd.Parameters.AddWithValue("@MigrationResult", MigrationResult);
                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

        //
        // DELETE Report58
        //
        public void DeleteReport58(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport58] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // Report 61
        //
        public int PostedNo;
        public string Origin;
       
        public string CurrDesc;
        public string TranAmount;

        public string DrCrType;
        public string TransDesc;

        public bool GlEntry;

        public string DrCrType2;
        public string TransDesc2;
        public string AccNo2;
        public bool GlEntry2;

        public int ReplCycle;
        public void InsertReport61(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport61] "
                + " ([UserId], "
                + " [PostedNo],"
                + " [Origin],"
                + " [Done],"
                + " [CurrDesc], "
                + " [TranAmount], "
                + " [DrCrType], "
                + " [TransDesc], "         
                + " [AccNo], "
                + " [GlEntry], "
                + " [DrCrType2], "
                + " [TransDesc2], "
                + " [AccNo2], "
                + " [GlEntry2], "
                + " [AtmNo], "
                + " [ReplCycle]  ) "
                + " VALUES "
                + "(@UserId,"
                + " @PostedNo,"
                + " @Origin,"
                + " @Done,"
                + " @CurrDesc, "
                + " @TranAmount, "
                + " @DrCrType, "
                + " @TransDesc, "
                + " @AccNo, "
                + " @GlEntry, "
                + " @DrCrType2, "
                + " @TransDesc2, "
                + " @AccNo2, "
                + " @GlEntry2, "
                + " @AtmNo, "                   
                + " @ReplCycle  ) ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        cmd.Parameters.AddWithValue("@PostedNo", PostedNo);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@Done", Done);
                        cmd.Parameters.AddWithValue("@CurrDesc", CurrDesc);

                        cmd.Parameters.AddWithValue("@TranAmount", TranAmount);
                        cmd.Parameters.AddWithValue("@DrCrType", DrCrType);

                        cmd.Parameters.AddWithValue("@TransDesc", TransDesc);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@GlEntry", GlEntry);
                        cmd.Parameters.AddWithValue("@DrCrType2", DrCrType2);
                        cmd.Parameters.AddWithValue("@TransDesc2", TransDesc2);
                        cmd.Parameters.AddWithValue("@AccNo2", AccNo2);
                        cmd.Parameters.AddWithValue("@GlEntry2", GlEntry2);
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);
                      
                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

        //
        // DELETE Report61
        //
        public void DeleteReport61(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport61] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // Insert Report67 - Discrepancies During Matching  
        //
        // 

        public int OriginSeqNo;
        public DateTime TransDate;
        public string FileId;
        public string WCase;
        public int Type;
        public int DublInPos;
        public int InPos;
        public int NotInPos;
        public string TerminalId;
        public int TraceNo;
        public string AccNo;
        public decimal TransAmt;
        //public string MatchingCateg;
        public int RMCycle;

        public void InsertReport67(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[WReport67] "
                + " ([UserId], "
                + " [OriginSeqNo],"
                + " [TransDate],"
                + " [FileId],"
                + " [WCase],"
                + " [Type], "
                + " [DublInPos], "
                + " [InPos], "
                + " [NotInPos], "
                + " [TerminalId], "
                + " [TraceNo], "
                //+ " [AccNo], "
                + " [TransAmt], "
                + " [MatchingCateg], "
                + " [RMCycle]  ) "
                + " VALUES "
                + "(@UserId,"
                + " @OriginSeqNo,"
                + " @TransDate,"
                + " @FileId,"
                + " @WCase,"
                + " @Type, "
                + " @DublInPos, "
                + " @InPos, "
                + " @NotInPos, "
                + " @TerminalId, "
                + " @TraceNo, "
                //+ " @AccNo, "
                + " @TransAmt, "
                + " @MatchingCateg, "
                + " @RMCycle  ) ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        cmd.Parameters.AddWithValue("@OriginSeqNo", OriginSeqNo);
                        cmd.Parameters.AddWithValue("@TransDate", TransDate);
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.Parameters.AddWithValue("@WCase", WCase);
                        cmd.Parameters.AddWithValue("@Type", Type);

                        cmd.Parameters.AddWithValue("@DublInPos", DublInPos);
                        cmd.Parameters.AddWithValue("@InPos", InPos);
                        cmd.Parameters.AddWithValue("@NotInPos", NotInPos);

                        cmd.Parameters.AddWithValue("@TerminalId", TerminalId);
                        cmd.Parameters.AddWithValue("@TraceNo", TraceNo);

                        //cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@TransAmt", TransAmt);

                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);
                        cmd.Parameters.AddWithValue("@RMCycle", RMCycle);
                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
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

        //
        // DELETE Report67
        //
        public void DeleteReport67(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int count; 
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport67] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        //rows number of record got updated

                        count = cmd.ExecuteNonQuery();


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
        // DELETE WReport97_ForMatching
        //
        public void DeleteWReport97_ForMatching(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int count;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("Truncate table [ATMS].[dbo].[WReport97_ForMatching] "
                             + "  ", conn))
                    {
                       // cmd.Parameters.AddWithValue("@UserId", InUserId);

                        //rows number of record got updated

                        count = cmd.ExecuteNonQuery();


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
        // DELETE WReport97_ForMatching
        //
        public void DeleteWReport97_ForMatching_MOBILE(string InUserId, string InMatchingCateg)
        {

            ErrorFound = false;
            ErrorOutput = ""; 
            int count;
            string CmdString = "DELETE FROM[ATMS].[dbo].[WReport97_ForMatching_MOBILE] "
                          + " WHERE MatchingCateg ='" + InMatchingCateg + "'";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
//                    DELETE FROM[ATMS].[dbo].[WReport97_ForMatching_MOBILE]
//        where MatchingCateg = 'QAH420'
//GO
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(CmdString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@UserId", InUserId);

                        //rows number of record got updated

                        count = cmd.ExecuteNonQuery();


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
        // DELETE Report70
        //
        public void DeleteReport70(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport70] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report71
        //
        public void DeleteReport71(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport71] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report72
        //
        public void DeleteReport72()
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport72] "
                             + "  ", conn))
                    {
                        //cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report82
        //
        public void DeleteReport82()
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport82] "
                             + "  ", conn))
                    {
                        //cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report73
        //
        public void DeleteReport73()
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport73] "
                             + "  ", conn))
                    {
                        //cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report73
        //
        public void DeleteReport73_2()
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport73_2] "
                             + "  ", conn))
                    {
                        //cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report74
        //
        public void DeleteReport74()
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport74] "
                             + "  ", conn))
                    {
                        //cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report75
        //
        public void DeleteReport75(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport75] "
                             + " WHERE UserId = @UserId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report79
        //
        public void DeleteReport79()
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport79] "
                             + " ", conn))
                    {
                        //cmd.Parameters.AddWithValue("@UserId", InUserId);

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
        // DELETE Report80
        //
        public void DeleteReport80(string InUserId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[WReport80] "
                             + " Where UserId =@UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

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

        // Insert report in WReport80 
        public void InsertReport80(string InOperator, string InSignedId, DataTable InTable)
        {

            if (InTable.Rows.Count > 0)
            {
                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport80]";

                            foreach (var column in InTable.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(InTable);
                        }
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
}
