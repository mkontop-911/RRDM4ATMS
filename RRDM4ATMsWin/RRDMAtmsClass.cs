using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
////using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMsWin
{
    class RRDMAtmsClass
    {
        
        public string AtmNo; 
        public string AtmName; 
        public string BankId;
 
        public string Branch; // Branch Identity 
        public string BranchName;

        public string Street ;
        public string Town ; 
        public string District ; 	
        public string PostalCode ; 	
        public string Country ; 	

        public double Latitude;
        public double Longitude;

        public byte[] LocationScreenshot ;

        public int AtmsStatsGroup; 
        public int AtmsReplGroup; 
        public int AtmsReconcGroup;

        public bool Loby;
        public bool Wall;
        public bool Drive;
        public bool OffSite;

        public string TypeOfRepl;

        public string CashInType ;

        public string HolidaysVersion;
        public string MatchDatesCateg; 
        public int OverEst; 

        public decimal MinCash;
        public decimal MaxCash;

        public int ReplAlertDays;

        public decimal InsurOne;
        public decimal InsurTwo;
        public decimal InsurThree;
        public decimal InsurFour;

        public string Supplier;
        public string Model;

        public string CitId; 

        public int NoCassettes;

        public bool DepoReader;
        public bool ChequeReader;
        public bool EnvelopDepos;

        public string DepCurNm; 

        public int CasNo_11;
        public string CurName_11;
        public int FaceValue_11;
        public int CasCapacity_11;

        public int CasNo_12;
        public string CurName_12;
        public int FaceValue_12;
        public int CasCapacity_12;

        public int CasNo_13;
        public string CurName_13;
        public int FaceValue_13;
        public int CasCapacity_13;

        public int CasNo_14;
        public string CurName_14;
        public int FaceValue_14;
        public int CasCapacity_14;

        public bool ActiveAtm;

        public string Operator; 

    //    public string CashAccount;
    //    public string SuspAccount;
    //    public string OtherAccount;

        public string IpAddress;

        public string AtmReplUserId;
        public string AtmReplUserName;
        public string AtmReplUserEmail;
        public string AtmReplMobileNo;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ ATM

        public void ReadAtm(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
               + " FROM TableATMsBasic"
               + " WHERE AtmNo=@AtmNo"; 
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            AtmNo = (string)rdr["AtmNo"];
                            AtmName = (string)rdr["AtmName"];
                            BankId = (string)rdr["BankId"];
                 
                            Branch = (string)rdr["Branch"];
                            BranchName = (string)rdr["BranchName"];

                            Street = (string)rdr["Street"];
                            Town = (string)rdr["Town"];
                            District = (string)rdr["District"];
                 
                            PostalCode = (string)rdr["PostalCode"];
                            Country = (string)rdr["Country"];

                            
                            Latitude = (double)rdr["Latitude"];
                            Longitude = (double)rdr["Longitude"];

                            LocationScreenshot = (byte[])rdr["LocationImage"];                          

                            AtmsStatsGroup = (int)rdr["AtmsStatsGroup"];
                            AtmsReplGroup = (int)rdr["AtmsReplGroup"];
                            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];

                            Loby = (bool)rdr["Loby"];
                            Wall = (bool)rdr["Wall"];
                            Drive = (bool)rdr["Drive"];

                            OffSite = (bool)rdr["OffSite"];

                            TypeOfRepl = (string)rdr["TypeOfRepl"];
                            CashInType = (string)rdr["CashInType"];
                            
                            HolidaysVersion = (string)rdr["HolidaysVersion"];
                            MatchDatesCateg = (string)rdr["MatchDatesCateg"];

                            OverEst = (int)rdr["OverEst"];

                            MinCash = (decimal)rdr["MinCash"];
                            MaxCash = (decimal)rdr["MaxCash"];

                            ReplAlertDays = (int)rdr["ReplAlertDays"];

                            InsurOne = (decimal)rdr["InsurOne"];
                            InsurTwo = (decimal)rdr["InsurTwo"];
                            InsurThree = (decimal)rdr["InsurThree"];
                            InsurFour = (decimal)rdr["InsurFour"];

                            Supplier = (string)rdr["Supplier"];
                            Model = (string)rdr["Model"];

                            CitId = (string)rdr["CitId"];

                            NoCassettes = (int)rdr["NoCassettes"];

                            DepoReader= (bool)rdr["DepoReader"];
                            ChequeReader= (bool)rdr["ChequeReader"];
                            EnvelopDepos= (bool)rdr["EnvelopDepos"];

                  
                            DepCurNm = (string)rdr["DepCurNm"];

                            CasNo_11 = (int)rdr["CasNo_11"];
                  
                            CurName_11 = (string)rdr["CurName_11"];
                            FaceValue_11 = (int)rdr["FaceValue_11"];
                            CasCapacity_11 = (int)rdr["CasCapacity_11"];

                            CasNo_12 = (int)rdr["CasNo_12"];
                  
                            CurName_12 = (string)rdr["CurName_12"];
                            FaceValue_12 = (int)rdr["FaceValue_12"];
                            CasCapacity_12 = (int)rdr["CasCapacity_12"];

                            CasNo_13 = (int)rdr["CasNo_13"];
                      
                            CurName_13 = (string)rdr["CurName_13"];
                            FaceValue_13 = (int)rdr["FaceValue_13"];
                            CasCapacity_13 = (int)rdr["CasCapacity_13"];

                            CasNo_14 = (int)rdr["CasNo_14"];
                       
                            CurName_14 = (string)rdr["CurName_14"];
                            FaceValue_14 = (int)rdr["FaceValue_14"];
                            CasCapacity_14 = (int)rdr["CasCapacity_14"];
                            ActiveAtm = (bool)rdr["ActiveAtm"];

                            IpAddress = (string)rdr["IpAddress"];

                            Operator = (string)rdr["Operator"];

                    //        CashAccount = (string)rdr["CashAccount"];
                    //        SuspAccount = (string)rdr["SuspAccount"];

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
                    ErrorOutput = "An error occured in ATMs Class............. " + ex.Message;

                }
        }
        // 
        // UPDATE ATMs Basic 
        //
        public void UpdateAtmsBasic(string InAtmNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE TableATMsBasic SET "
                            + "[ActiveAtm] =@ActiveAtm " 
                            + " WHERE AtmNo= @AtmNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ActiveAtm", ActiveAtm);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                           
                        }

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ATMs Class............. " + ex.Message;
                   
                }

            //  return outcome;

        }

        // 
        // FIND ATM OWNER for Replenishment  
        //
        public void ReadAtmOwner(string InAtmNo)
        {
            RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms();

            RRDMUsersAndSignedRecord Ua = new RRDMUsersAndSignedRecord();

            ReadAtm(InAtmNo); 

            ErrorFound = false;
            ErrorOutput = "";
       
                try
                {
                    // Get the USER RESPONSIBLE FOR THIS ATM
                    if (AtmsReplGroup > 0) Uaa.FindUserForRepl("", AtmsReplGroup);

                 //       UsersAccessToAtms Uaa = new UsersAccessToAtms(); 
                    //    UsersAndSignedRecord Ua = new UsersAndSignedRecord();

                    else Uaa.FindUserForRepl(InAtmNo, 0);

                    // FIND USER FOR THIS 

                    if (Uaa.RecordFound == true)
                    {
                        Ua.ReadUsersRecord(Uaa.UserId); // Get Info for User 

                        AtmReplUserId = Ua.UserId;
                        AtmReplUserName = Ua.UserName;
                        AtmReplUserEmail = Ua.email;
                        AtmReplMobileNo = Ua.MobileNo;

                    }
             
                   
                }
                catch (Exception ex)
                {
                   
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ATMs Class............. " + ex.Message;

                }

            //  return outcome;

        }       
    
    }
}
