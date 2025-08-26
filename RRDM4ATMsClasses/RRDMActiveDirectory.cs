using System;
using System.Text;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMActiveDirectory : Logger
    {
        public RRDMActiveDirectory() : base() { }
        ////      
        ////ACTIVE DIRECTORY CLASS
        //// 
        public string AdDomainName;
        public string AdGroup;

        public bool UserFoundInRRDM; 

        public string AdUserId;
        public string AdUserName;
        public string UserPhone;
        public string UserMail;

        public string BankSwiftId;
        public string Operator;

        public bool UserFound;

        public bool DomainFound;
        public bool ValidDomain; // Searching in Banks
        //public bool AdRRDMYes; // It was defined in Parameters that Active Directory was needed 
        public bool UserInGroup; // User was checked and belongs to active directory group. 

        public bool NameFromActive; // Name is from ACTIVe and not from RRDM
        public bool MobileFromActive; // Mobile is from ACTIVe and not from RRDM
        public bool EmailFromActive;  // Email ..... 

        public bool ErrorFound;
        public string ErrorOutput;

        public int ErrorCode;

        RRDMActiveDirectoryHelper Ad2 = new RRDMActiveDirectoryHelper();

        RRDMBanks Ba = new RRDMBanks();
        RRDMGasParameters Gp = new RRDMGasParameters();

        //
        // READ Domain and other details for signed in user 
        //
        public void CheckActiveDirectory()
        {
            DomainFound = false;
            ValidDomain = false;
            //AdRRDMYes = false;
            UserInGroup = false;

            UserFoundInRRDM = false; 

            NameFromActive = false;
            MobileFromActive = false;
            EmailFromActive = false;

            string ParId;
            string OccurId;

            ErrorFound = false;
            ErrorOutput = "";
            ErrorCode = 0;

            try
            {

                bool CallResponseSuccess = false;

                /* Test for Logged on User Details */
                /* =============================== */
                CallResponseSuccess = Ad2.getLoggedOnUserDetails();
                if (CallResponseSuccess == true)
                {
                    
                    if (Ad2.usrContextType == "Machine")
                    {
                        string Msg;
                        Msg = Ad2.usrContextType;
                        Msg = Msg + Environment.NewLine + Ad2.usrIdentity + "..Domain not found";
                        MessageBox.Show(Msg);
                        //ErrorFound = true;
                        DomainFound = false;
                        return;
                    }

                    if (Ad2.usrContextType == "Domain")
                    {
                        //if (Ad.DomainFound & Ad.ValidDomain & Ad.UserInGroup & Ad.UserFoundInRRDM)
                        //{
                        //    WActiveDirectory = true;
                        //}
                        DomainFound = true;
                        AdDomainName = Ad2.usrDomainName;
                        AdUserId = Ad2.usrLogonName.Trim();

                        // ===========================================
                        //  Check if this domain is in Banks
                        //*********************************************                
                        Ba.ReadBankActiveDirectory(Ad2.usrDomainName);
                        if (Ba.RecordFound == true)
                        {
                            // It is valid
                            ValidDomain = true;
                            AdGroup = Ba.AdGroup;
                            if (Ad2.isMemberOfGroup(AdGroup) == true)
                            {
                                UserInGroup = true; 
                            }
                            else
                            {
                                UserInGroup = false;
                            }
                            Operator = Ba.Operator; 
                        }
                        else
                        {
                            // Not valid domain
                            MessageBox.Show("Domain is wrong");
                            ValidDomain = false;
                            return;
                        }

                        //
                        RRDMUsersRecords Us = new RRDMUsersRecords();
                        Us.ReadUsersRecord(Ad2.usrLogonName.Trim());
                        if (Us.RecordFound == true)
                        {
                            UserFoundInRRDM = true;
                        }
                        else
                        {
                            UserFoundInRRDM = false;
                        }

                       
                        string Msg;
                        
                        Msg = Ad2.usrContextType;
                        Msg = Msg + Environment.NewLine + "Logon Id.." + Ad2.usrIdentity;
                        Msg = Msg + Environment.NewLine + "Domain Name.." + Ad2.usrDomainName;
                        Msg = Msg + Environment.NewLine +"Logon_Name.."+ Ad2.usrLogonName;
                        Msg = Msg + Environment.NewLine + "Is in group? ..=" + Ad2.isMemberOfGroup(AdGroup).ToString();
                        Msg = Msg + Environment.NewLine + "Is in RRDM ? ..=" + UserFoundInRRDM;
                        Msg = Msg + Environment.NewLine + "Logon_Display Name.." + Ad2.usrDisplayName;

                        MessageBox.Show(Msg);

                        // ===========================================
                        // Check if name is defined in Parameters to be changed 
                        //*********************************************
                        ParId = "264";
                        OccurId = "2";
                        Gp.ReadParametersSpecificId(Operator, ParId, OccurId, "", "");
                        if (Gp.RecordFound & Gp.OccuranceNm == "Name Update") // Name is yes 
                        {
                            NameFromActive = true; // Name is from ACTIVe and not from RRDM

                        }

                        // ===========================================
                        // Check if mobile is defined in Parameters to be changed 
                        //*********************************************
                        ParId = "264";
                        OccurId = "3";
                        Gp.ReadParametersSpecificId(Operator, ParId, OccurId, "", "");
                        if (Gp.RecordFound & Gp.OccuranceNm == "Mobile Update") // Mobile is yes 
                        {
                            MobileFromActive = true;

                        }

                        // ===========================================
                        // Check if Email is defined in Parameters to be changed 
                        //*********************************************
                        ParId = "264";
                        OccurId = "4";
                        Gp.ReadParametersSpecificId(Operator, ParId, OccurId, "", "");
                        if (Gp.RecordFound & Gp.OccuranceNm == "Email Update") // Email is yes 
                        {
                            EmailFromActive = true;

                        }
                        //
                        // GET USER AND OTHER DETAILS
                        //
                       ReadParticularUserDetailsFromActive(Ad2.usrIdentity, AdDomainName);
                    }
                }
                else // Error 
                {
                    DomainFound = false;
                    ErrorFound = true;
                    ErrorOutput = Ad2.exceptionMessage;
                }
            }

            catch (Exception ex)
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

                if (Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                             + " . Application will be aborted! Call controller to take care. ");
                }
            }
        }

        // READ Particular User Details based on Domain Name 
        //
        public void ReadParticularUserDetailsFromActive(string InUser, string DomainName)
        {

            UserFound = false;

            // Find details for particular user 

            ErrorFound = false;
            ErrorOutput = "";
            ErrorCode = 0;

            try
            {
               
                AdUserId = Ad2.usrLogonName;

                AdUserName = Ad2.usrDisplayName;
                
                UserPhone = Ad2.usrMobilePhone;
              
                UserMail = Ad2.usrEmailAddress;
               
                UserFound = true;

            }

            catch (Exception ex)
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

                if (Environment.UserInteractive)
                {
                    System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                                                             + " . Application will be aborted! Call controller to take care. ");
                }
            }
        }

    }
}
