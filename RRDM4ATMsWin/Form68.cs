using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs; 
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form68 : Form
    {

        int I;

        int k;
        string SQLString; 
      //  int TraceNo;

        int WTraceNumber;
        string LineAtmNo; 

        int WTranType;

        int WErrId;

        string WCardNoBin;

        string WPrintTrace;
        string WPrintTraceDtTm;

        RRDME_JournalTxtClass Ej = new RRDME_JournalTxtClass();

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

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

        public Form68(string InSignedId, int InSignRecordNo, string InOperator ,  string InJournalId, DateTime InDateStart, DateTime InDateEnd, 
                   string InAtmNo, int InSesNo, int InTraceStart, int InTraceEnd, string InEnteredNumber, int InTraceNo, int InSelModeA, int InSelModeB )
        {
         
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator; 

            WJournalId = InJournalId;
          
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            WDateStart = InDateStart ;
            WDateEnd = InDateEnd ; 

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

            if (WSelModeB == 10 ) // Check last digit if different than zero then this comes from Supervisor Mode. 
            // Turn last digit to one. 
            {
                // Check Trace No
                Int32 LastDigit = WTraceNo % 10;

                if (LastDigit == 0)
                {
                    // OK
                }
                else
                {
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
          
        }
        // ON LOAD 
        private void Form68_Load(object sender, EventArgs e)
        {
            //TEST
            if (WAtmNo == "AB102")
            {
                MessageBox.Show("Not available data for ATM AB102." + Environment.NewLine
                             + " Select Atm AB104 ");
                this.Dispose();
            }

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


            if (WSelModeA == 1 & WSelModeB == 3) // Show all Lines for a period 
            {

                Heading1.Text = "Journal Entries for Atm: " +WAtmNo + " from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString(); 

                 SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                  + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                  + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                   + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                   + " FROM " + WJournalId
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

                        string exception = ex.ToString();
                        //     MessageBox.Show(ex.ToString());
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

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
                  + " FROM " + WJournalId
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

                        string exception = ex.ToString();
                        //     MessageBox.Show(ex.ToString());
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }
            }

            if ( WSelModeB == 5) // Show all lines for a period for ERRORS 
            {
                if (WSelModeA == 1)
                {
                    Heading1.Text = "Journal ERROR Entries for ATM:"+ WAtmNo +" from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString();

                    SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                  + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                  + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                   + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                  + " FROM " + WJournalId
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
                      + " FROM " + WJournalId
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

                        string exception = ex.ToString();
                        //     MessageBox.Show(ex.ToString());
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }
            }

          

            if (WSelModeA == 1 & WSelModeB == 6) // Show all lines for a period Capture cards  
            {

                Heading1.Text = "Journal Capture Card Entries for ATM:" + WAtmNo + " from: " + WDateStart.ToShortDateString() + " to : " + WDateEnd.ToShortDateString(); 

                WErrId = 301 ; // capture card 

                SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                    + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                    + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                    + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                  + " FROM " + WJournalId
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

                        string exception = ex.ToString();
                        //     MessageBox.Show(ex.ToString());
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }
            }

            if (WSelModeA == 1 & WSelModeB == 7) // Show all lines  Repl Cycle id   for a specific ATM
            {

                Heading1.Text = "Journal Entries for Repl. Cycle: " + WSesNo.ToString ()  + " For Atm: " + WAtmNo ;


                SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                    + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                    + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                    + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                  + " FROM " + WJournalId
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

                        string exception = ex.ToString();
                        //     MessageBox.Show(ex.ToString());
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

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
                  + " FROM " + WJournalId
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
                  + " FROM " + WJournalId
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

                        string exception = ex.ToString();
                        //     MessageBox.Show(ex.ToString());
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

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
                                      + " FROM " + WJournalId
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
                                      + " FROM " + WJournalId
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

                        string exception = ex.ToString();
                        //     MessageBox.Show(ex.ToString());
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }
            }

            if (WSelModeA == 1 & WSelModeB == 10) // Show all lines for a TRACE NUMBER  for  a specific ATM
            {

                Heading1.Text = "Journal Entries for TraceNo : " + WTraceNo.ToString() + " For AtmNo:" + WAtmNo;


                SQLString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                    + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                    + "ISNULL(Descr, 'UnSpecified') AS TranDescr, ISNULL(CurNm, '') AS CurNm, ISNULL(TranAmnt, 0) AS TranAmnt, "
                    + " ISNULL(ErrId, 0) AS ErrId, ISNULL(ErrDesc, '') AS ErrDesc "
                  + " FROM " + WJournalId
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

                        string exception = ex.ToString();
                        //     MessageBox.Show(ex.ToString());
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                    }
            }

            dataGridView1.DataSource = JournalLines.DefaultView;

            dataGridView1.Columns[0].Width = 60;
            dataGridView1.Columns[1].Width = 60;
            dataGridView1.Columns[2].Width = 360;
            dataGridView1.Columns[3].Width = 90;
            dataGridView1.Columns[4].Width = 90;
            dataGridView1.Columns[5].Width = 90;
            dataGridView1.Columns[6].Width = 50;
            dataGridView1.Columns[7].Width = 90;
            dataGridView1.Columns[8].Width = 60;
            dataGridView1.Columns[9].Width = 120;
            /*
                       dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
                       dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
                       dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);
             */

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
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //  WRow = e.RowIndex;
            WTraceNumber = (int)rowSelected.Cells[3].Value;
            LineAtmNo = (string)rowSelected.Cells[1].Value;

            textBoxTraceNo.Text = WTraceNumber.ToString(); 
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
            
            Ej.ReadJournalTextByTrace(WOperator, LineAtmNo, WTraceNumber);

            WPrintTraceDtTm = Ej.TransDate.ToString();
            WPrintTrace = WTraceNumber.ToString(); 

           
            // TRACE
            // Show all lines for a TRACE NUMBER  for a specific ATM
            
                Form56R16 PrintJournal = new Form56R16(WOperator, WPrintTrace, WPrintTraceDtTm, LineAtmNo);
                PrintJournal.Show();
            
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

            }
        }


    
