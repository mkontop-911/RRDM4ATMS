using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMsWin
{
    class RRDMReconcCountersClass
    {

        public int TotalAtms; // 
        public int OutStanding;
        public int MoreThan5;
        public int MoreThan10;
        public int NoOfGroups;
        public int ReconcTotal; 

        string AtmNo; 
        int CurrentSesNo; 
       
        int SessionsInDiff;

   //     bool OffSite; 

        DateTime LastReplDt;

        string TypeOfRepl; 

        DateTime NextReplDt;

        bool ReconcDiff;
                       
        int AtmsStatsGroup;

        int AtmsReplGroup;

        int AtmsReconcGroup;

        int AtmsReconcGroupOld; 

        DateTime LastUpdated;

        int AuthUser;

        string SqlString;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
      //  bool WPrive;

        // READ Session Traces and set counters 
        // 
        public void ReadSessionsStatusTracesCounters(string InSignedId, int InSignRecordNo, string InOperator)
          
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
 
            TotalAtms = 0; 
            OutStanding = 0;
            MoreThan5 = 0;
            MoreThan10 = 0;
            NoOfGroups = 0; 
            ReconcTotal = 0;

          
                SqlString = "SELECT *"
                    + " FROM [dbo].[AtmsMain] "
                    + " WHERE Operator = @Operator AND AuthUser = @AuthUser"
                    + " order by AtmsReconcGroup";
          


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", WOperator);
                        cmd.Parameters.AddWithValue("@AuthUser", WSignedId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true ;

                            AtmNo = (string)rdr["AtmNo"];
                            CurrentSesNo = (int)rdr["CurrentSesNo"];

                            //   OffSite = (bool)rdr["OffSite"];
                            LastReplDt = (DateTime)rdr["LastReplDt"];

                            TypeOfRepl = (string)rdr["TypeOfRepl"];

                            NextReplDt = (DateTime)rdr["NextReplDt"];

                            ReconcDiff = (bool)rdr["ReconcDiff"];

                            AtmsStatsGroup = (int)rdr["AtmsStatsGroup"];
                            AtmsReplGroup = (int)rdr["AtmsReplGroup"];
                            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];
                            LastUpdated = (DateTime)rdr["LastUpdated"];

                            AuthUser = (int)rdr["AuthUser"];

                            Ta.ReadSessionsStatusTraces(AtmNo, CurrentSesNo);
                            SessionsInDiff = Ta.SessionsInDiff;


                            //    ActionNo = (int)rdr["ActionNo"];

                            TotalAtms = TotalAtms + 1;

                            if (ReconcDiff == false) ReconcTotal = ReconcTotal + 1;

                            if (ReconcDiff == true) OutStanding = OutStanding + 1;

                            if (SessionsInDiff > 5)
                            {
                                MoreThan5 = MoreThan5 + 1;
                            }

                            if (SessionsInDiff > 10)
                            {
                                MoreThan10 = MoreThan10 + 1;
                            }

                            if (AtmsReconcGroup == AtmsReconcGroupOld)
                            {
                            }
                            else NoOfGroups = NoOfGroups + 1;

                            AtmsReconcGroupOld = AtmsReconcGroup;

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
                    ErrorOutput = "An error occured in Reconciliation Counters Class............. " + ex.Message;

                }
        }
    }
}

// string SqlString = "Last = 1 AND DiffReconcEnd = 1 AND ToUser= " + WSignedId + ")";
//     string SqlString1 = "ToUser= " + WSignedId + ")";

//   string SqlString2 = "Last = 1 AND DiffReconcEnd = 1 AND ToUser= " + WSignedId + ")";

//  + " AND SessionsInDiff > 10";
//   string MsgFilter = "(OpenMsg= 1 AND AtmNo= '" + WAtmNo + "') OR (OpenMsg= 1 AND ToAllAtms = 1)"
//    + " OR (OpenMsg= 1 AND ToUser= " + WSignedId + ")";

