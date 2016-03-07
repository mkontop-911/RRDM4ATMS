using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

using System.DirectoryServices;

using System.DirectoryServices.AccountManagement;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    class RRDMActiveDirectory
    {
        ////      
        ////ACTIVE DIRECTORY CLASS
        //// 
        public string AdDomainName;
        public string AdGroup;

        public string UserId;
        public string UserName;
        public string UserPhone;
        public string UserMail;

        public string BankSwiftId;
        public string Operator;

        public bool UserFound;

        public bool DomainFound;
        public bool ValidDomain; // Searching in Banks
        public bool AdRRDMYes; // It was defined in Parameters that Active Directory was needed 
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
            AdRRDMYes = false;
            UserInGroup = false;

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
                        DomainFound = false;
                        return;
                    }

                    if (Ad2.usrContextType == "Domain")
                    {

                        DomainFound = true;

                        AdDomainName = Ad2.usrDomainName;

                        // ===========================================
                        //  Check if this domain is in Banks
                        //*********************************************                
                        Ba.ReadBankActiveDirectory(AdDomainName);
                        if (Ba.RecordFound == true)
                        {
                            // It is valid
                            ValidDomain = true;
                            AdGroup = Ba.AdGroup;
                            BankSwiftId = Ba.BankSwiftId;
                            Operator = Ba.Operator;
                        }
                        else
                        {
                            // Not valid domain
                            ValidDomain = false;
                            return;
                        }

                        // ===========================================
                        // Check if define in Parameters ACCess control on Active Directory 
                        //*********************************************
                        ParId = "264";
                        OccurId = "1";
                        Gp.ReadParametersSpecificId(Operator, ParId, OccurId, "", "");
                        if (Gp.OccuranceNm == "YES") // Active is yes 
                        {
                            AdRRDMYes = true;
                        }
                        else
                        {
                            AdRRDMYes = false;
                            return;
                        }

                        // ===========================================
                        // Find Group and find if User in Group 
                        //*********************************************
                        ParId = "264";
                        OccurId = "10";
                        Gp.ReadParametersSpecificId(Operator, ParId, OccurId, "", "");
                        if (Gp.OccuranceNm == "NoGroup") // THERE IS NO GROUP
                        {
                            UserInGroup = true;
                            return;
                        }
                        else
                        {
                            // Check Group 
                            if (Ad2.isDirectMemberOfGroup(Gp.OccuranceNm) == true)
                            {
                                // User is in group 
                                UserInGroup = true;
                            }
                            else
                            {
                                // User is not in group 
                                UserInGroup = false;
                            }
                        }

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
                ErrorFound = true;
                ErrorOutput = "An error occured in Active Directory ........ " + ex.Message;
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
                UserId = Ad2.usrIdentity;
                UserId = "1005";

                UserName = Ad2.usrDisplayName;
                UserName = "Nicos Ioannou";

                UserPhone = Ad2.usrMobilePhone;
                UserPhone = "0035799622248";

                UserMail = Ad2.usrEmailAddress;
                UserMail = "panicos.michael@cablenet.com.cy";

                UserFound = true;

            }

            catch (Exception ex)
            {
                ErrorFound = true;
                ErrorOutput = "An error occured in Active Directory ........ " + ex.Message;
            }
        }

    }
}
