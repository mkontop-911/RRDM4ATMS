using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;
using System.Text;
using System.Drawing;


namespace RRDM4ATMsWin
{
    public partial class Form68 : Form
    {
        int I;
        int k;
        string SQLString;

        string SelectionCriteria;


        int WMasterTraceNo;
        string LineAtmNo;

        int UniqueRecordId;

        int WTranType;

        int WErrId;

        string WCardNoBin;

        string WPrintTrace;
        string WPrintTraceDtTm;

        RRDMJournalReadTxns_Text_Class Ej = new RRDMJournalReadTxns_Text_Class();
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        string WWJournalId;

        string WSignedId;
        int WSignRecordNo;

        string WOperator;

        string WJournalId;
        //        int WFileInJournal;
        string WAtmNo;
        int WSesNo;

        DateTime WDateStart;
        DateTime WDateEnd;

        int WTraceStart;
        int WTraceEnd;

        string WEnteredNumber;
        int WTraceNo;

        int WSelModeA;
        int WSelModeB;

        //string JournalString;

        public Form68(string InSignedId, int InSignRecordNo, string InOperator, string InJournalId, DateTime InDateStart, DateTime InDateEnd,
                   string InAtmNo, int InSesNo, int InTraceStart, int InTraceEnd, string InEnteredNumber, int InTraceNo, int InSelModeA, int InSelModeB)
        {

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WJournalId = InJournalId;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            WDateStart = InDateStart;
            WDateEnd = InDateEnd;

            WTraceStart = InTraceStart;
            WTraceEnd = InTraceEnd;

            WEnteredNumber = InEnteredNumber;

            WTraceNo = InTraceNo;

            WSelModeA = InSelModeA;

            WSelModeB = InSelModeB;

            if (WSelModeA == 1 & WSelModeB == 7) // Check last digit if different than zero then this comes from Supervisor Mode. 
            {
                // Check START 
                Int32 LastDigit = WTraceStart % 10;

                if (LastDigit == 0)
                {
                    // OK
                }
                else
                {
                    WTraceStart = (WTraceStart - LastDigit) + 1;
                }
            }

            if (WSelModeB == 10) // This is for for single trace for a normal TXN or SM  
            // If SM Turn last digit to one. 
            {
                // Check Trace No
                Int32 LastDigit = WTraceNo % 10;

                if (LastDigit == 0)
                {
                    // OK
                    // It is just a transaction
                }
                else
                {
                    // It is the supervisor mode
                    WTraceNo = (WTraceNo - LastDigit) + 1;
                }
            }

            InitializeComponent();

            // Repl Cycle printing button. 
            if ((WSelModeA == 1 & WSelModeB == 7)) // Show all lines for a Repl Cycle id   for an ATM
            // OR TRACE NUMBER was inputed 
            {
                buttonPrintFull.Show();
            }
            else buttonPrintFull.Hide();

            if (WOperator == "ETHNCY2N")
            {
                WWJournalId = "[ATM_MT_Journals].[dbo].[tblHstEjText]";
            }
            if (WOperator == "CRBAGRAA")
            {
                WWJournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";
            }

        }
        // ON LOAD 
        private void Form68_Load(object sender, EventArgs e)
        {
            ////TEST
            //if (WAtmNo == "AB102")
            //{
            //    MessageBox.Show("Not available data for ATM AB102." + Environment.NewLine
            //                 + " Select Atm AB104 ");
            //    this.Dispose();
            //}

            DataTable JournalLines = new DataTable();
            JournalLines = new DataTable();
            JournalLines.Clear();

            // DATA TABLE ROWS DEFINITION 
            //
            JournalLines.Columns.Add("LineNo", typeof(int));
            JournalLines.Columns.Add("AtmNo", typeof(string));
            JournalLines.Columns.Add("TxtDescription", typeof(string));
            JournalLines.Columns.Add("TraceNumber", typeof(int));
            JournalLines.Columns.Add("TransDate", typeof(DateTime));
            JournalLines.Columns.Add("TranDescr", typeof(string));

            JournalLines.Columns.Add("CurNm", typeof(string));
            JournalLines.Columns.Add("TranAmnt", typeof(decimal));

            JournalLines.Columns.Add("ErrId", typeof(int));
            JournalLines.Columns.Add("ErrDesc", typeof(string));

            JournalLines.Columns.Add("FileId", typeof(int));
            JournalLines.Columns.Add("RowId", typeof(int));

            if (WSelModeA == 1 & WSelModeB == 3) // Show all Lines for a period 
            {

                Heading1.Text = "Journal Entries for Atm: " + WAtmNo + " from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString();

                SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                 + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                 + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                  + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                  + " FROM " + WWJournalId
                  + " WHERE AtmNo = @AtmNo and TransDate >=@TransDateStart AND TransDate <= @TransDateEnd "
                  + " Order by TraceNumber, RuId ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLString, conn))
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                            cmd.Parameters.AddWithValue("@TransDateStart", WDateStart);
                            cmd.Parameters.AddWithValue("@TransDateEnd", WDateEnd);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                I = I + 1;
                                // Read GROUP Details

                                k = (int)rdr["Ruid"];

                                DataRow RowJ = JournalLines.NewRow();

                                RowJ["LineNo"] = I;
                                RowJ["AtmNo"] = (string)rdr["AtmNo"];
                                RowJ["TxtDescription"] = (string)rdr["TxtLine"];
                                RowJ["TraceNumber"] = (int)rdr["TraceNumber"];
                                RowJ["TransDate"] = (DateTime)rdr["TransDate"];
                                RowJ["TranDescr"] = (string)rdr["TranDescr"];

                                RowJ["CurNm"] = (string)rdr["CurNm"];
                                RowJ["TranAmnt"] = (decimal)rdr["TranAmnt"];

                                RowJ["ErrId"] = (int)rdr["ErrId"];
                                RowJ["ErrDesc"] = (string)rdr["ErrDesc"];

                                RowJ["FileId"] = (int)rdr["fuid"];
                                RowJ["RowId"] = (int)rdr["Ruid"];

                                JournalLines.Rows.Add(RowJ);

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

            if (WSelModeA == 1 & WSelModeB == 4) // Show all lines for a period for Repl Cycles 
            {

                WTranType = 77;

                Heading1.Text = "Repl. Cycles Entries for Atm: " + WAtmNo + " from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString();

                SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                   + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                   + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                    + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                    + " FROM " + WWJournalId
                  + " WHERE AtmNo = @AtmNo and TransDate >=@TransDateStart AND TransDate <= @TransDateEnd and TranType = @TranType  "
                  + " Order by TraceNumber, RuId ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLString, conn))
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                            cmd.Parameters.AddWithValue("@TransDateStart", WDateStart);
                            cmd.Parameters.AddWithValue("@TransDateEnd", WDateEnd);
                            cmd.Parameters.AddWithValue("@TranType", WTranType);
                            //    cmd.Parameters.AddWithValue("@TransDateEnd", WDateEnd);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                I = I + 1;
                                // Read GROUP Details

                                k = (int)rdr["Ruid"];

                                DataRow RowJ = JournalLines.NewRow();

                                RowJ["LineNo"] = I;
                                RowJ["AtmNo"] = (string)rdr["AtmNo"];
                                RowJ["TxtDescription"] = (string)rdr["TxtLine"];
                                RowJ["TraceNumber"] = (int)rdr["TraceNumber"];
                                RowJ["TransDate"] = (DateTime)rdr["TransDate"];
                                RowJ["TranDescr"] = (string)rdr["TranDescr"];

                                RowJ["CurNm"] = (string)rdr["CurNm"];
                                RowJ["TranAmnt"] = (decimal)rdr["TranAmnt"];

                                RowJ["ErrId"] = (int)rdr["ErrId"];
                                RowJ["ErrDesc"] = (string)rdr["ErrDesc"];

                                RowJ["FileId"] = (int)rdr["fuid"];
                                RowJ["RowId"] = (int)rdr["Ruid"];

                                JournalLines.Rows.Add(RowJ);

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

            if (WSelModeB == 5) // Show all lines for a period for ERRORS 
            {
                if (WSelModeA == 1)
                {
                    Heading1.Text = "Journal ERROR Entries for ATM:" + WAtmNo + " from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString();

                    SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                  + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                  + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                   + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                    + " FROM " + WWJournalId
                  + " WHERE AtmNo = @AtmNo and TransDate >=@TransDateStart AND TransDate <= @TransDateEnd and ErrId > 0  "
                  + " Order by TraceNumber, RuId ";
                }


                if (WSelModeA == 2)
                {
                    Heading1.Text = "Journal ERROR Entries for all ATMs from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString();

                    SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                  + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                  + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                   + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                        + " FROM " + WWJournalId
                      + " WHERE TransDate >=@TransDateStart AND TransDate <= @TransDateEnd and ErrId > 0  "
                      + " Order by TraceNumber, RuId ";
                }

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLString, conn))
                        {
                            if (WSelModeA == 1)
                            {
                                cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                            }
                            cmd.Parameters.AddWithValue("@TransDateStart", WDateStart);
                            cmd.Parameters.AddWithValue("@TransDateEnd", WDateEnd);
                            //   cmd.Parameters.AddWithValue("@TranType", WTranType);
                            //    cmd.Parameters.AddWithValue("@TransDateEnd", WDateEnd);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                I = I + 1;
                                // Read GROUP Details

                                k = (int)rdr["Ruid"];

                                DataRow RowJ = JournalLines.NewRow();

                                RowJ["LineNo"] = I;
                                RowJ["AtmNo"] = (string)rdr["AtmNo"];
                                RowJ["TxtDescription"] = (string)rdr["TxtLine"];
                                RowJ["TraceNumber"] = (int)rdr["TraceNumber"];
                                RowJ["TransDate"] = (DateTime)rdr["TransDate"];
                                RowJ["TranDescr"] = (string)rdr["TranDescr"];

                                RowJ["CurNm"] = (string)rdr["CurNm"];
                                RowJ["TranAmnt"] = (decimal)rdr["TranAmnt"];

                                RowJ["ErrId"] = (int)rdr["ErrId"];
                                RowJ["ErrDesc"] = (string)rdr["ErrDesc"];

                                RowJ["FileId"] = (int)rdr["fuid"];
                                RowJ["RowId"] = (int)rdr["Ruid"];

                                JournalLines.Rows.Add(RowJ);

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

            if (WSelModeA == 1 & WSelModeB == 6) // Show all lines for a period Capture cards  
            {

                Heading1.Text = "Journal Capture Card Entries for ATM:" + WAtmNo + " from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString();

                WErrId = 301; // capture card 

                SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                    + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                    + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                    + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                    + " FROM " + WWJournalId
                  + " WHERE AtmNo = @AtmNo and TransDate >=@TransDateStart AND TransDate <= @TransDateEnd and ErrId =@ErrId  "
                  + " Order by TraceNumber, RuId ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLString, conn))
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                            cmd.Parameters.AddWithValue("@TransDateStart", WDateStart);
                            cmd.Parameters.AddWithValue("@TransDateEnd", WDateEnd);
                            cmd.Parameters.AddWithValue("@ErrId", WErrId);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                I = I + 1;
                                // Read GROUP Details

                                k = (int)rdr["Ruid"];

                                DataRow RowJ = JournalLines.NewRow();

                                RowJ["LineNo"] = I;
                                RowJ["AtmNo"] = (string)rdr["AtmNo"];
                                RowJ["TxtDescription"] = (string)rdr["TxtLine"];
                                RowJ["TraceNumber"] = (int)rdr["TraceNumber"];
                                RowJ["TransDate"] = (DateTime)rdr["TransDate"];
                                RowJ["TranDescr"] = (string)rdr["TranDescr"];

                                RowJ["CurNm"] = (string)rdr["CurNm"];
                                RowJ["TranAmnt"] = (decimal)rdr["TranAmnt"];

                                RowJ["ErrId"] = (int)rdr["ErrId"];
                                RowJ["ErrDesc"] = (string)rdr["ErrDesc"];

                                RowJ["FileId"] = (int)rdr["fuid"];
                                RowJ["RowId"] = (int)rdr["Ruid"];

                                JournalLines.Rows.Add(RowJ);

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

            if (WSelModeA == 1 & WSelModeB == 7) // Show all lines  Repl Cycle id   for a specific ATM
            {

                Heading1.Text = "Journal Entries for Repl. Cycle: " + WSesNo.ToString() + " For Atm: " + WAtmNo;

                SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                    + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                    + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                    + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                    + " FROM " + WWJournalId
                     + " WHERE AtmNo = @AtmNo and TraceNumber >=@TraceStart AND TraceNumber <= @TraceEnd "
                  + " Order by TraceNumber, RuId ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLString, conn))
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                            cmd.Parameters.AddWithValue("@TraceStart", WTraceStart);
                            cmd.Parameters.AddWithValue("@TraceEnd", WTraceEnd);
                            //    cmd.Parameters.AddWithValue("@TransDateEnd", WDateEnd);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                I = I + 1;
                                // Read GROUP Details

                                k = (int)rdr["Ruid"];

                                DataRow RowJ = JournalLines.NewRow();

                                RowJ["LineNo"] = I;
                                RowJ["AtmNo"] = (string)rdr["AtmNo"];
                                RowJ["TxtDescription"] = (string)rdr["TxtLine"];
                                RowJ["TraceNumber"] = (int)rdr["TraceNumber"];
                                RowJ["TransDate"] = (DateTime)rdr["TransDate"];
                                WPrintTraceDtTm = ((DateTime)rdr["TransDate"]).ToString();
                                RowJ["TranDescr"] = (string)rdr["TranDescr"];

                                RowJ["CurNm"] = (string)rdr["CurNm"];
                                RowJ["TranAmnt"] = (decimal)rdr["TranAmnt"];

                                RowJ["ErrId"] = (int)rdr["ErrId"];
                                RowJ["ErrDesc"] = (string)rdr["ErrDesc"];

                                RowJ["FileId"] = (int)rdr["fuid"];
                                RowJ["RowId"] = (int)rdr["Ruid"];

                                JournalLines.Rows.Add(RowJ);

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

            if (WSelModeB == 8) // Show all lines for a period CARD NUMBER   
            {

                WCardNoBin = WEnteredNumber.Substring(0, 6) + "******" + WEnteredNumber.Substring(12, 4);

                if (WSelModeA == 1)
                {
                    Heading1.Text = "Journal Entries for Card : " + WCardNoBin + " For AtmNo:" + WAtmNo
                        + " from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString();

                    SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                        + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                        + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                        + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                    + " FROM " + WWJournalId
                    + " WHERE AtmNo = @AtmNo and TransDate >=@TransDateStart AND TransDate <= @TransDateEnd and CardNo =@CardNo  "
                  + " Order by TraceNumber, RuId ";
                }
                if (WSelModeA == 2)
                {
                    Heading1.Text = "Journal Entries for Card : " + WCardNoBin + " For all Atms"
                       + " from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString();

                    SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                        + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                        + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                        + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                    + " FROM " + WWJournalId
                    + " WHERE TransDate >=@TransDateStart AND TransDate <= @TransDateEnd and CardNo =@CardNo  "
                  + " Order by TraceNumber, RuId ";
                }

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLString, conn))
                        {
                            if (WSelModeA == 1)
                            {
                                cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                            }

                            cmd.Parameters.AddWithValue("@TransDateStart", WDateStart);
                            cmd.Parameters.AddWithValue("@TransDateEnd", WDateEnd);
                            cmd.Parameters.AddWithValue("@CardNo", WCardNoBin);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                I = I + 1;
                                // Read GROUP Details

                                k = (int)rdr["Ruid"];

                                DataRow RowJ = JournalLines.NewRow();

                                RowJ["LineNo"] = I;
                                RowJ["AtmNo"] = (string)rdr["AtmNo"];
                                RowJ["TxtDescription"] = (string)rdr["TxtLine"];
                                RowJ["TraceNumber"] = (int)rdr["TraceNumber"];
                                RowJ["TransDate"] = (DateTime)rdr["TransDate"];
                                RowJ["TranDescr"] = (string)rdr["TranDescr"];

                                RowJ["CurNm"] = (string)rdr["CurNm"];
                                RowJ["TranAmnt"] = (decimal)rdr["TranAmnt"];

                                RowJ["ErrId"] = (int)rdr["ErrId"];
                                RowJ["ErrDesc"] = (string)rdr["ErrDesc"];

                                RowJ["FileId"] = (int)rdr["fuid"];
                                RowJ["RowId"] = (int)rdr["Ruid"];

                                JournalLines.Rows.Add(RowJ);

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

            if (WSelModeB == 9) // Show all lines for a period ACCOUNT NUMBER   
            {
                if (WSelModeA == 1)
                {
                    Heading1.Text = "Journal Entries for AccNo : " + WEnteredNumber + " For AtmNo:" + WAtmNo
                       + " from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString();

                    SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                        + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                        + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                        + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                                       + " FROM " + WWJournalId
                                        + " WHERE AtmNo = @AtmNo and TransDate >=@TransDateStart AND TransDate <= @TransDateEnd "
                                        + "and AccNo like @AccNo  "
                                      + " Order by TraceNumber, RuId ";
                }
                if (WSelModeA == 2)
                {
                    Heading1.Text = "Journal Entries for AccNo : " + WEnteredNumber + " For all Atms "
                       + " from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString();


                    SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                        + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                        + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                        + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                                       + " FROM " + WWJournalId
                                        + " WHERE TransDate >=@TransDateStart AND TransDate <= @TransDateEnd "
                                        + " and AccNo like @AccNo  "
                                      + " Order by TraceNumber, RuId ";
                }

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLString, conn))
                        {
                            if (WSelModeA == 1)
                            {
                                cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                            }

                            string ParEnteredNumber = "%" + WEnteredNumber + "%";
                            cmd.Parameters.AddWithValue("@TransDateStart", WDateStart);
                            cmd.Parameters.AddWithValue("@TransDateEnd", WDateEnd);
                            cmd.Parameters.AddWithValue("@AccNo", ParEnteredNumber);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                I = I + 1;
                                // Read GROUP Details

                                k = (int)rdr["Ruid"];

                                DataRow RowJ = JournalLines.NewRow();

                                RowJ["LineNo"] = I;
                                RowJ["AtmNo"] = (string)rdr["AtmNo"];
                                RowJ["TxtDescription"] = (string)rdr["TxtLine"];
                                RowJ["TraceNumber"] = (int)rdr["TraceNumber"];
                                RowJ["TransDate"] = (DateTime)rdr["TransDate"];
                                RowJ["TranDescr"] = (string)rdr["TranDescr"];

                                RowJ["CurNm"] = (string)rdr["CurNm"];
                                RowJ["TranAmnt"] = (decimal)rdr["TranAmnt"];

                                RowJ["ErrId"] = (int)rdr["ErrId"];
                                RowJ["ErrDesc"] = (string)rdr["ErrDesc"];

                                RowJ["FileId"] = (int)rdr["fuid"];
                                RowJ["RowId"] = (int)rdr["Ruid"];

                                JournalLines.Rows.Add(RowJ);

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

            if (WSelModeA == 1 & WSelModeB == 10) // Show all lines for a TRACE NUMBER  for  a specific ATM
            {

                Heading1.Text = "Journal Entries for TraceNo : " + WTraceNo.ToString() + " For AtmNo:" + WAtmNo;


                SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                    + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                    + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                    + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                    + " FROM " + WWJournalId
                    + " WHERE AtmNo = @AtmNo and TraceNumber = @TraceNumber   "
                  + " Order by TraceNumber, RuId ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLString, conn))
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                            cmd.Parameters.AddWithValue("@TraceNumber", WTraceNo);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                I = I + 1;
                                // Read GROUP Details

                                k = (int)rdr["Ruid"];

                                DataRow RowJ = JournalLines.NewRow();

                                RowJ["LineNo"] = I;
                                RowJ["AtmNo"] = (string)rdr["AtmNo"];
                                RowJ["TxtDescription"] = (string)rdr["TxtLine"];
                                RowJ["TraceNumber"] = (int)rdr["TraceNumber"];
                                WPrintTrace = ((int)rdr["TraceNumber"]).ToString();
                                RowJ["TransDate"] = (DateTime)rdr["TransDate"];
                                WPrintTraceDtTm = ((DateTime)rdr["TransDate"]).ToString();
                                RowJ["TranDescr"] = (string)rdr["TranDescr"];

                                RowJ["CurNm"] = (string)rdr["CurNm"];
                                RowJ["TranAmnt"] = (decimal)rdr["TranAmnt"];

                                RowJ["ErrId"] = (int)rdr["ErrId"];
                                RowJ["ErrDesc"] = (string)rdr["ErrDesc"];

                                RowJ["FileId"] = (int)rdr["fuid"];
                                RowJ["RowId"] = (int)rdr["Ruid"];

                                JournalLines.Rows.Add(RowJ);

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

            dataGridView1.DataSource = JournalLines.DefaultView;

            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].Width = 65;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[2].Width = 380;
            dataGridView1.Columns[3].Width = 90;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].Width = 90;
            dataGridView1.Columns[5].Width = 90;
            dataGridView1.Columns[6].Width = 40;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].Width = 90;
            dataGridView1.Columns[8].Width = 60;
            dataGridView1.Columns[9].Width = 120;


            dataGridView1.Show();

        }
        // Video Clip 
        private void button3_Click(object sender, EventArgs e)
        {
            /*
             // Based on trace Number and start of transaction find the seconds. 
             
             SELECT SUBSTRING(LTRIM(RTRIM(TxtLine)), 6, 8) As TranDate
                   ,SUBSTRING(LTRIM(RTRIM(TxtLine)), 23, 8) As TransTime
                    FROM [ATMS_Journals].[dbo].[tblHstEjText]
                    where TraceNumber = 10042920 and Ruid = (44+3) 
                    order by traceNumber, RuId 
             */

            // Based on Transaction No show video clip 

            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();

        }
        // on Row enter 
        bool ReadFromInPool;
        DateTime LineDateTime; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //  WRow = e.RowIndex;
            WMasterTraceNo = (int)rowSelected.Cells[3].Value;
            LineAtmNo = (string)rowSelected.Cells[1].Value;
            LineDateTime = (DateTime)rowSelected.Cells[4].Value;
            SelectionCriteria = "";
            ReadFromInPool = false;

            // Check START 
            Int32 LastDigit = WMasterTraceNo % 10;

            if (LastDigit == 0 || LastDigit == 1)
            {
                if (LastDigit == 0 ) textBoxTraceNo.Text = (WMasterTraceNo/10).ToString();
                // OK
                SelectionCriteria = " WHERE AtmNo ='" + LineAtmNo + "' AND MasterTraceNo = " + WMasterTraceNo;
            }
            else
            {
                MessageBox.Show("These are invalid lines");
                return;
            }

            // Read Transactions 
            Mpa.ReadInPoolTransByMasterTraceNoDataTable(LineAtmNo, WMasterTraceNo);

            if (Mpa.RecordFound == true)
            {
                ReadFromInPool = true;

                if (Mpa.TotalSelected == 1)
                {
                    label12.Text = "One Trace";
                }
                else
                {
                    // More than one 
                    label12.Text = "More than one Trace";
                }

                dataGridView2.DataSource = Mpa.InPoolMasterTraceNoDataTable.DefaultView;
            }
            else
            {
                ReadFromInPool = false;

                ReadFromJournalTranByTraceNoDataTable();

                dataGridView2.DataSource = TraceNoDataTableEnquiries.DefaultView;
                ////dataGridView2.DataSource = null;
                ////dataGridView2.Refresh();

            }

           // textBoxTraceNo.Text = WMasterTraceNo.ToString();
        }

        // Grid2 Row Enter 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            if (ReadFromInPool == true)
            {
                //  
                UniqueRecordId = (int)rowSelected.Cells[0].Value;
                Mpa.ReadInPoolTransSpecificUniqueRecordId(UniqueRecordId,2);

                if (Mpa.RecordFound == true)
                {
                    textBox1.Text = Mpa.AtmTraceNo.ToString();
                    textBox2.Text = Mpa.TerminalId;
                    textBox3.Text = Mpa.TransDescr;
                    textBox4.Text = Mpa.TransCurr;
                    textBox5.Text = Mpa.TransAmount.ToString("#,##0.00");
                    textBox6.Text = Mpa.TransDate.ToString();
                    textBox7.Text = "Target";
                    if (Mpa.MetaExceptionNo > 0)
                    {
                        //textBox8.Hide();
                        textBox8.Text = Mpa.MetaExceptionNo.ToString();
                        textBox9.Text = "Error Description";
                    }
                    else
                    {
                        textBox8.Text = "N/A";
                        textBox9.Text = "N/A";
                    }
                }
                else
                {

                }
            }
            if (ReadFromInPool == false)
            {

                textBox1.Text = TraceNumber.ToString();
                textBox2.Text = AtmNo;
                textBox3.Text = TranDescr;
                textBox4.Text = CurNm;
                textBox5.Text = TranAmnt.ToString("#,##0.00");
                textBox6.Text = TransDate.ToString();
                textBox7.Text = "";
                if (ErrId > 0)
                {
                    //textBox8.Hide();
                    textBox8.Text = ErrId.ToString();
                    textBox9.Text = ErrDesc;
                }
                else
                {
                    textBox8.Text = "N/A";
                    textBox9.Text = "N/A";
                }

            }
        }

        //
        // READ SPECIFIC ALL TRACES THAT HAVE SAME MASTERTraceNo and Fill table 
        //
        DataTable TraceNoDataTableEnquiries = new DataTable();

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;
        public int TotalSelected;

        string TxtLine;
        DateTime TransDate;
        int Ruid;
        int fuid;
        string AtmNo;
        int TraceNumber;

        string TranDescr;
        string CurNm;
        decimal TranAmnt;

        int ErrId;
        string ErrDesc;

        public void ReadFromJournalTranByTraceNoDataTable()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SelectionCriteria = " WHERE AtmNo ='" + LineAtmNo + "' AND TraceNumber = " + WMasterTraceNo;

            TraceNoDataTableEnquiries = new DataTable();
            TraceNoDataTableEnquiries.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TraceNoDataTableEnquiries.Columns.Add("TraceNumber", typeof(int));
            TraceNoDataTableEnquiries.Columns.Add("AtmNo", typeof(string));
            TraceNoDataTableEnquiries.Columns.Add("TranDescr", typeof(string));

            SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                       + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                       + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                       + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                                     + " FROM " + WWJournalId
                                     + " WHERE AtmNo = @AtmNo AND TraceNumber = @TraceNumber" 
                                     + " Order By Fuid, Ruid " ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", LineAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNumber", WMasterTraceNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            TxtLine = (string)rdr["TxtLine"];
                            TransDate = (DateTime)rdr["TransDate"];
                            Ruid = (int)rdr["Ruid"];
                            fuid = (int)rdr["fuid"];
                            AtmNo = (string)rdr["AtmNo"];
                            TraceNumber = (int)rdr["TraceNumber"];

                            TranDescr = (string)rdr["TranDescr"];
                            CurNm = (string)rdr["CurNm"];
                            TranAmnt = (decimal)rdr["TranAmnt"];

                            ErrId = (int)rdr["ErrId"];
                            ErrDesc = (string)rdr["ErrDesc"];

                            //
                            //Fill In Table
                            //
                            DataRow RowSelected = TraceNoDataTableEnquiries.NewRow();

                            RowSelected["TraceNumber"] = TraceNumber;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["TranDescr"] = TranDescr;
                            // ADD ROW
                            TraceNoDataTableEnquiries.Rows.Add(RowSelected);

                            break; 
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

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            /*
                     // Based on trace Number and start of transaction find the seconds. 
             
                     SELECT SUBSTRING(LTRIM(RTRIM(TxtLine)), 6, 8) As TranDate
                           ,SUBSTRING(LTRIM(RTRIM(TxtLine)), 23, 8) As TransTime
                            FROM [ATMS_Journals].[dbo].[tblHstEjText]
                            where TraceNumber = 10042920 and Ruid = (44+3) 
                            order by traceNumber, RuId 
                     */

            // Based on Transaction No show video clip 

            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();

        }
        // Print 

        private void buttonPrintTrace_Click(object sender, EventArgs e)
        {

            Form67 NForm67;

            int Mode = 5; // Specific
            //NForm67 = new Form67(WSignedId, 0, WOperator, 0, Mpa.TerminalId, Mpa.AtmTraceNo, Mpa.AtmTraceNo, Mpa.TransDate.Date, Mode);
            NForm67 = new Form67(WSignedId, 0, WOperator, 0, LineAtmNo, WMasterTraceNo, WMasterTraceNo, LineDateTime, LineDateTime, Mode);
            NForm67.ShowDialog();

        }
        // Print Full 
        private void buttonPrintFull_Click(object sender, EventArgs e)
        {

            // Repl Cycle 
            if (WSelModeA == 1 & WSelModeB == 7) // Show all lines for a Repl Cycle id   for an ATM
            {
                string WPrintTraceStart = WTraceStart.ToString();
                string WPrintTraceEnd = WTraceEnd.ToString();
                Form56R17 PrintJournal = new Form56R17(WOperator, WAtmNo, WPrintTraceStart, WPrintTraceEnd, WPrintTraceDtTm);
                PrintJournal.Show();
            }
        }
        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        // Catch Details 
        //
        private static void CatchDetails(Exception ex)
        {
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

            System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                + " . Application will be aborted! Call controller to take care. ");

          //  Environment.Exit(0);
        }
    }
}



