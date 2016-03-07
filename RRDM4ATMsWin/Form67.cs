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
    public partial class Form67 : Form
    {
    //    string Transfilter;

   //     int TranNo;

        int I;

        int k;

        string WPrintTrace;
        string WPrintTraceDtTm;

        RRDME_JournalTxtClass Ej = new RRDME_JournalTxtClass();

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //multilingual
        CultureInfo culture;

        RRDMUsersAndSignedRecord Xa = new RRDMUsersAndSignedRecord(); // Make class availble 
        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

    //    string SWTraceStart;
      //  string SWTraceEnd;
    //    int length;
        string SqlString;

        string WSignedId;
        int WSignRecordNo;
        string WOperator; 
    //    string SQLString; 
        string WJournalId;
        int WFileInJournal; 
        string WAtmNo;
        //int WSesNo; 
        int WTraceStart;
        int WTraceEnd;
       
        int WMode;

        public Form67(string InSignedId ,int InSignRecordNo, string InOperator,  string InJournalId, int InFileInJournal, 
                   string InAtmNo,  int InTraceStart, int InTraceEnd, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator; 
            WJournalId = InJournalId;
            WFileInJournal = InFileInJournal;  
            WAtmNo = InAtmNo;
           
            WTraceStart = InTraceStart;
            WTraceEnd = InTraceEnd;
            
            WMode = InMode; // Mode 1 = single trace ---- Mode = 2 Whole Journal 

            if (WMode == 1 || WMode ==2 ) // Check last digit if different than zero then this comes from Supervisor Mode. 
            // Turn last digit to one. 
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

                // Check End 
                LastDigit = WTraceEnd % 10;

                if (LastDigit == 0)
                {
                    // OK
                }
                else
                {
                    WTraceEnd = (WTraceEnd - LastDigit) + 1;
                }
            }
            
                   
            InitializeComponent();

            textBoxTraceNo.Text = InTraceStart.ToString(); 
    
        }
        //
        // With LOAD LOAD DATA GRID
        //
        private void Form67_Load(object sender, EventArgs e)
        {
            if (WAtmNo == "AB102")
            {
                MessageBox.Show("Not available data for ATM AB102." + Environment.NewLine
                             + " Select Atm AB104 ");
                this.Dispose();
                return; 
            }
            

            Xa.ReadSignedActivityByKey(WSignRecordNo);  

            if (Xa.Culture == "English")
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (Xa.Culture =="Français")
            {
                culture = CultureInfo.CreateSpecificCulture("fr-FR");
            }

            if (WMode == 1)
            {
                label14.Text = LocRM.GetString("Form67label14a", culture);
                label1.Text = WTraceStart.ToString(); 
            }
            if (WMode == 2)
            {
                label14.Text = LocRM.GetString("Form67label14b", culture);
                //label1.Text = WSesNo.ToString(); 
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

            if (WMode == 3)
            {
                SqlString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                                   + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(TraceNumber, 0) AS TraceNumber "
                                    + " FROM " + WJournalId
                                    + " WHERE AtmNo = @AtmNo and FuId = @FuId "
                                    + " Order by FuId, RuId ";
                using (SqlConnection conn =
                          new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                            cmd.Parameters.AddWithValue("@FuId", WFileInJournal);

                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                I = I + 1;
                                // Read GROUP Details

                                k = (int)rdr["Ruid"];

                                DataRow RowJ = JournalLines.NewRow();

                                //   RowJ["LineNo"] = (int)rdr["Ruid"];
                                RowJ["LineNo"] = I;
                                RowJ["TxtDescription"] = (string)rdr["TxtLine"];
                                RowJ["TraceNumber"] = (int)rdr["TraceNumber"];
                                RowJ["TransDate"] = (DateTime)rdr["TransDate"];

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
            else
            {
                SqlString = "SELECT ISNULL(TxtLine, '') AS TxtLine, TransDate, ISNULL(Ruid, 0)"
                   + " AS Ruid, ISNULL(fuid, 0) AS fuid, ISNULL(AtmNo, '') AS AtmNo, ISNULL(TraceNumber, 0) AS TraceNumber, "
                    + "ISNULL(Descr, '') AS TranDescr "
                    + " FROM " + WJournalId
                    + " WHERE AtmNo = @AtmNo and TraceNumber >=@TraceStart AND TraceNumber <= @TraceEnd "
                    + " Order by TraceNumber, RuId ";

                using (SqlConnection conn =
                          new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                            cmd.Parameters.AddWithValue("@TraceStart", WTraceStart);
                            cmd.Parameters.AddWithValue("@TraceEnd", WTraceEnd);

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
            

      //     MessageBox.Show(" Number of lines = " + I);

            if (JournalLines.Rows.Count > 0)
            {
                dataGridView1.DataSource = JournalLines.DefaultView;
                dataGridView1.Show();

                dataGridView1.Columns[0].Width = 60;
                dataGridView1.Columns[1].Width = 60;
                dataGridView1.Columns[2].Width = 360;
                dataGridView1.Columns[3].Width = 90;
                dataGridView1.Columns[4].Width = 90;
                dataGridView1.Columns[5].Width = 90;
            }
            else
            {
                MessageBox.Show("No Data to show for this selection. "); 
            }

          
/*
           dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
           dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
           dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);
 */
        }
// Print Trace Journal 
        private void buttonPrintTrace_Click(object sender, EventArgs e)
        {
            Ej.ReadJournalTextByTrace(WOperator, WAtmNo, WTraceStart);

            WPrintTraceDtTm = Ej.TransDate.ToString();
            WPrintTrace = textBoxTraceNo.Text;


            // TRACE
            // Show all lines for a TRACE NUMBER  for a specific ATM

            Form56R16 PrintJournal = new Form56R16(WOperator, WPrintTrace, WPrintTraceDtTm, WAtmNo);
            PrintJournal.Show();
        }            

    }
}
