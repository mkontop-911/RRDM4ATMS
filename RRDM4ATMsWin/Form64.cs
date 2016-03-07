using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs; 
using System.Data.SqlClient;
using System.Globalization;
using System.Configuration;

using System.Net;

namespace RRDM4ATMsWin
{
    public partial class Form64 : Form
    {
        string WAtmNo;
      //   bool RecordFound;
        int I;
        DateTime TrDt;
        int StartTrxn;
        int EndTrxn; 

        string TRanDate;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass(); 

        public Form64()
        {
            InitializeComponent();
        }
        // RUN THE STORED PROCEDURE 
        private void button1_Click(object sender, EventArgs e)
        {
            // Read Ejournal and create Data bases  
            WAtmNo = textBox1.Text;

            string RCT = "[ATMdemo].[dbo].[Stp_Run_Process]"; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                      //  cmd.Parameters.AddWithValue("@AtmNo", WAtmNo);
                       

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
                    string exception = ex.ToString();
                    // MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }

        // Read Trans File 

        private void button2_Click(object sender, EventArgs e)
        {

             // READ Group 
     /*       [atmno] = <atmno, char(4),>
      ,[TRACENUMBER] = <TRACENUMBER, varchar(20),>
      ,[fuid] = <fuid, int,>
      ,[trandesc] = <trandesc, varchar(400),>
      ,[currency] = <currency, varchar(100),>
      ,[camount] = <camount, varchar(100),>
      ,[ruid] = <ruid, int,>
      ,[trace] = <trace, char(7),>
      ,[cardnum] = <cardnum, char(20),>
      ,[source] = <source, varchar(10),>
      ,[TxtLine] = <TxtLine, varchar(1000),>
      ,[comments] = <comments, varchar(100),>
      ,[CardCaptured] = <CardCaptured, varchar(100),>
      ,[CardCapturedMES] = <CardCapturedMES, varchar(100),>
      ,[PowerInterup] = <PowerInterup, varchar(100),>
      ,[TRanDate] = <TRanDate, char(12),>
      ,[TranTime] = <TranTime, char(6),>
      ,[acct1] = <acct1, varchar(100),>
      ,[acct2] = <acct2, varchar(100),>
      ,[Result] = <Result, char(100),>
      ,[PresenterError] = <PresenterError, varchar(100),>
      ,[PresenterErrordesc] = <PresenterErrordesc, varchar(100),>
      ,[SuspectDesc] = <SuspectDesc, varchar(100),>
      ,[SuspectNotes] = <SuspectNotes, varchar(100),>
      ,[contactbank] = <contactbank, varchar(100),>
      ,[eur5] = <eur5, varchar(10),>
      ,[eur10] = <eur10, varchar(10),>
      ,[eur20] = <eur20, varchar(10),>
      ,[eur50] = <eur50, varchar(10),>
      ,[eur100] = <eur100, varchar(10),>
      ,[eur200] = <eur200, varchar(10),>
      ,[eur500] = <eur500, varchar(10),>
      ,[starttxn] = <starttxn, int,>
      ,[midtxn] = <midtxn, int,>
      ,[endtxn] = <endtxn, int,>
      ,[remain1] = <remain1, int,>
      ,[remain2] = <remain2, int,>
      ,[remain3] = <remain3, int,>
      ,[remain4] = <remain4, int,>
      ,[cashin] = <cashin, int
      */

            WAtmNo = "AB104";

            string TRACENUMBER;
            int fuid ;
            string trandesc ;

            string currency;

            string camount;
            decimal TranAmount; 

            string cardnum;

            string Yyyy ;
            int YyyyN ;
            string Mm ;
            int MmN ;
            string Dd ;
            int DdN ;

            string HHMM ; // Hour Minute 
            int HH;
            int MM;

            string AccNo ; 

            string Result ; 

            int Type1;
            int Type2;
            int Type3;
            int Type4;
            // ISNULL(TRanDate, '') AS TRanDate,
       
        //    RecordFound = false;

            string SqlString = "SELECT atmno, TRACENUMBER, ISNULL(fuid, 0) AS fuid, ISNULL(trandesc, '') AS trandesc," 
                + "ISNULL(currency, '') AS currency, ISNULL(camount, '') AS camount,ISNULL(cardnum, '') AS cardnum,"
                + "ISNULL(starttxn, 0) AS starttxn,ISNULL(endtxn, 0) AS endtxn,ISNULL(TRanDate, '') AS TRanDate,"
                + "ISNULL(trantime, '') AS trantime,ISNULL(acct1, '') AS acct1,ISNULL(Result, '') AS Result,"
                + "Type1, type2, type3, type4" 
          + " FROM [ATMdemo].[dbo].[hstATMTxns] "
          + " WHERE atmno = @atmno" 
            +" order by cast(trace as int),TRACENUMBER";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@atmno", WAtmNo);

                        // Read table 

                        I = 1;

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                         //   RecordFound = true;

                           

                            TRACENUMBER = (string)rdr["TRACENUMBER"];
                            int TraceNo = int.Parse(TRACENUMBER);

                            if (TraceNo == 10058470)
                            {
                                fuid = (int)rdr["fuid"];
                                trandesc = (string)rdr["trandesc"];

                                currency = (string)rdr["currency"];

                                camount = (string)rdr["camount"];
                             //   TranAmount = decimal.Parse(camount);

                                if (decimal.TryParse(camount, out TranAmount))
                                {
                                }
                                else
                                {
                                    MessageBox.Show(camount, "Please enter a valid number!");
                                    return;
                                }

                                TranAmount = TranAmount / 100; 

                                if (trandesc == "WITHDRAWAL")
                                {
                                    cardnum = (string)rdr["cardnum"];
                                    AccNo = (string)rdr["acct1"];
                                    TRanDate = (string)rdr["TRanDate"];
                                    // Convert date 
                                    Yyyy = TRanDate.Substring(0, 4);
                                    YyyyN = int.Parse(Yyyy);
                                    Mm = TRanDate.Substring(4, 2);
                                    MmN = int.Parse(Mm);
                                    Dd = TRanDate.Substring(6, 2);
                                    DdN = int.Parse(Dd);

                                    HHMM = (string)rdr["trantime"]; 

                                    HH = int.Parse(HHMM.Substring(0,2));
                                    MM = int.Parse(HHMM.Substring(3,2));

                                    TrDt = new DateTime(YyyyN, MmN, DdN, HH, MM, 0 );


                                    StartTrxn = (int)rdr["starttxn"];

                                    EndTrxn = (int)rdr["endtxn"];

                                    Result = (string)rdr["acct1"];

                                    Tc.OriginName = "OurATMs";
                                    Tc.AtmTraceNo = TraceNo;
                                    Tc.EJournalTraceNo = TraceNo;

                                    Tc.AtmNo = WAtmNo;
                                    Tc.SesNo = 122;
                                    Tc.BankId = "Pereos";
                              
                                    Tc.BranchId = "BNBNB";

                                    Tc.AtmDtTime = TrDt;
                                    //Tc.HostDtTime = TrDt;

                                    Tc.SystemTarget = 1;
                                    Tc.TransType = 11; // If Withdrawl 
                                    Tc.TransDesc = trandesc;
                                    Tc.CardNo = cardnum;
                                    Tc.CardOrigin = 1;

                                    Tc.AccNo = AccNo;
                                //    Tc.CurrCode = 978;
                                    Tc.CurrDesc = currency;
                                    Tc.TranAmount = TranAmount;
                                    Tc.AuthCode = 0;
                                    Tc.RefNumb = 0;
                                    Tc.RemNo = 0;

                                    Tc.TransMsg = "";
                                    Tc.AtmMsg = "";
                                    Tc.ErrNo = 0;
                                    Tc.StartTrxn = StartTrxn;
                                    Tc.EndTrxn = EndTrxn;
                                    if (Result == "OK")  Tc.SuccTran = true;
                                    else Tc.SuccTran = false;

                                    Tc.InsertTransInPool(WAtmNo);

                                }


                            }

                            if (TraceNo == 00010058483)
                            {
                                fuid = (int)rdr["fuid"];
                                trandesc = (string)rdr["trandesc"];

                           //     currency = (string)rdr["currency"];

                           //     camount = (string)rdr["camount"];
                           //     TranAmount = decimal.Parse(camount);

                          //      if (trandesc == "WITHDRAWAL")
                         //       {
                          //      }

                        //        cardnum = (string)rdr["cardnum"];
/*
                                TRanDate = (string)rdr["TRanDate"];
                                // Convert date 
                                Yyyy = TRanDate.Substring(0, 4);
                                YyyyN = int.Parse(Yyyy);
                                Mm = TRanDate.Substring(4, 2);
                                MmN = int.Parse(Mm);
                                Dd = TRanDate.Substring(6, 2);
                                DdN = int.Parse(Dd);

                                TrDt = new DateTime(YyyyN, MmN, DdN);
 */

                                // TrDt = (string)rdr["trantime"];

                            //    StartTrxn = (int)rdr["starttxn"];

                             //   EndTrxn = (int)rdr["endtxn"];

                                Type1 = (int)rdr["Type1"];
                                Type2 = (int)rdr["Type2"];
                                Type3 = (int)rdr["Type3"];
                                Type4 = (int)rdr["Type4"];

                            }

                          
 
 

                        //    string date = "01/08/2008";
                        //    DateTime TrDt = Convert.ToDateTime(TRanDate);
                   //       Console.WriteLine("Year: {0}, Month: {1}, Day: {2}", TrDt.Year, TrDt.Month, TrDt.Day);

                        //    this.Text = "22/11/2009";

                 //         if (I==1)  TrDt = DateTime.ParseExact(this.TRanDate, "dd/mm/yyyy", null);
                  //        if (I == 2) TrDt = DateTime.ParseExact(this.TRanDate, "yyyy/mm/dd", null);
                  //        if (I == 3) TrDt = DateTime.ParseExact(this.TRanDate, "yyyymmdd", null);


                           

                            I++; 
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
                    //       MessageBox.Show(ex.ToString());
                    //     MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            string sourcePath = @"C:\Journals\TESTING THREE FILES";
            string destinationPath = @"C:\Journals\KONTO";
            string sourceFileName = "EJDATA.LOG.10.1.86.16.20140213.030935.2 - Copy.LOG";
            string destinationFileName = "ATM 1"+" EJDATA.LOG.10.1.86.16.20140213.030935.2.LOG";
            string sourceFile = System.IO.Path.Combine(sourcePath, sourceFileName);
            string destinationFile = System.IO.Path.Combine(destinationPath, destinationFileName);

            if (!System.IO.Directory.Exists(destinationPath))
            {
                System.IO.Directory.CreateDirectory(destinationPath);
            }
            System.IO.File.Copy(sourceFile, destinationFile, true);

            //Delete source file
            System.IO.File.Delete(sourcePath+@"\"+sourceFileName);
        }

        }
        }
    

