using System;
using System.Text;
using System.Security;
using System.Diagnostics;
using System.Security.Principal;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;


namespace RRDM4ATMs
{
    public class RRDMActiveDirectoryHelper : Logger
    {
        public RRDMActiveDirectoryHelper() : base() { }

        #region Properties
        public string usrLogonName = "";            // e.g: Alex
        public string usrDomainName = "";           // e.g: CYLEDRA (the domain 'friendly name')       
        public string usrIdentity = "";             // e.g: CYLEDRA\Alex
        public string usrContextType = "";          // e.g: 'Domain' or 'Machine'

        public string usrPrincipalName = "";        // e.g: Alex@cy.Ledra   
        public string usrDisplayName = "";          // e.g: Alexandros Christofi
        public string usrFirstName = "";            // e.g: Alexandros  
        public string usrLastName = "";             // e.g: Christofi
        public string usrMiddleName = "";
        public string usrTelephone = "";
        public string usrMobilePhone = "";
        public string usrEmailAddress = "";
        public string usrSamAccountName = "";       // e.g: Alex
        public string usrDistinguishedName = "";    // e.g: CN=Alex,OU=BR001,OU=TestOU1,DC=cy,DC=Ledra
        public bool exceptionThrown = false;
        public int exceptionErrorCode = 0;
        public string exceptionSource = "";
        public string exceptionMessage = "";
        private PrincipalContext principalContext;
        #endregion

        #region getLoggedOnUserDetails Method
        /// <summary>
        /// Gets the user under whose credentials the thread is being executed
        /// If the user is logged on to an Active Directory domain (usrContextType="Domain")
        /// the method sets all properties whose name starts with 'usr' from the Active Directory store.
        /// If the user is logged on locally (usrContextType="Machine") then only 'usrIdentity
        /// (in the form MACHINENAME\USERID), 'usrLogonName', 'usrDomainFriendlyName (containg the
        /// machine name) and usrContextType (='Machine') are set.
        /// </summary>
        /// <param> 
        /// No parameters 
        /// </param>
        /// <returns>
        /// Returns True the user is retrieved with success
        /// Returns False only if there was an exception, in which case
        /// 'exceptionThrown' property is set to 'true
        /// and the properties 'exceptionErrorCode', 'exceptionSource' and 'exceptionMessage'
        /// are set accordingly
        /// </returns>
        public bool getLoggedOnUserDetails()
        {
            bool success = false;

            try
            {

                principalContext = UserPrincipal.Current.Context;
                usrIdentity = WindowsIdentity.GetCurrent().Name;
                usrPrincipalName = UserPrincipal.Current.UserPrincipalName;

                // Split usrIdentity in its constituent parts 
                string[] splt = usrIdentity.Split('\\');
                usrDomainName = splt[0];
                usrLogonName = splt[1];

                usrContextType = principalContext.ContextType.ToString();

                bool result = false;
                result = principalContext.ContextType.Equals(ContextType.Domain);
                if (result)
                {
                    usrFirstName = UserPrincipal.Current.GivenName;
                    usrLastName = UserPrincipal.Current.Surname;
                    usrDisplayName = UserPrincipal.Current.DisplayName;
                    usrTelephone = UserPrincipal.Current.VoiceTelephoneNumber;
                    usrEmailAddress = UserPrincipal.Current.EmailAddress;
                    usrSamAccountName = UserPrincipal.Current.SamAccountName;
                    usrDistinguishedName = UserPrincipal.Current.DistinguishedName;
                    usrMiddleName = UserPrincipal.Current.MiddleName;

                    DirectoryEntry ldapDE = (DirectoryEntry)UserPrincipal.Current.GetUnderlyingObject();
                    if (ldapDE.Properties["mobile"].Count > 0)
                    {
                        usrMobilePhone = ldapDE.Properties["mobile"][0].ToString();
                    }
                }
                success = true;
            }
            catch (InvalidOperationException ex)
            {
                success = false;
                exceptionThrown = true;
                exceptionErrorCode = 0xfffb;
                exceptionSource = ex.Source;
                exceptionMessage = "InvalidOperationException: " + ex.Message;
            }
            catch (NoMatchingPrincipalException ex)
            {
                success = false;
                exceptionThrown = true;
                exceptionErrorCode = 0xfffc;
                exceptionSource = ex.Source;
                exceptionMessage = "NoMatchingPrincipalException: " + ex.Message;
            }
            catch (MultipleMatchesException ex)
            {
                success = false;
                exceptionThrown = true;
                exceptionErrorCode = 0xfffd;
                exceptionSource = ex.Source;
                exceptionMessage = "MultipleMatchesException: " + ex.Message;
            }
            catch (SecurityException ex)
            {
                success = false;
                exceptionThrown = true;
                exceptionErrorCode = 0xfffe;
                exceptionSource = ex.Source;
                exceptionMessage = "SecurityException: " + ex.Message;
            }
            catch (Exception ex)
            {
                success = false;
                exceptionThrown = true;
                exceptionErrorCode = 0xffff;
                exceptionSource = ex.Source;
                exceptionMessage = "LoggedOnUserException: " + ex.Message;

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
            return (success);
        }
        #endregion

        #region isDirectMemberOfGroup Method
        /// <summary>
        /// Verifies if the current user, under whose credentials the thread is executed, 
        /// is a direct member (added to the group explicitly) of an Active Directory group 
        /// </summary>
        /// <param> 
        /// groupName
        /// </param>
        /// <returns>
        /// Returns True the user is a direct member of the group
        /// Returns False if the user is not a direct member of the group, or if there was an exception.
        /// In the last case the 'exceptionThrown' property is set to 'true', 
        /// and the properties 'exceptionErrorCode', 'exceptionSource' and 'exceptionMessage'
        /// are set accordingly.
        /// </returns>
        public bool isDirectMemberOfGroup(string groupName)
        {
            bool result = false;
            try
            {
                GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(principalContext, IdentityType.Name, groupName);
                if (groupPrincipal != null)
                {
                    if (UserPrincipal.Current.IsMemberOf(groupPrincipal))
                    {
                        result = true;
                    }
                }
                else
                {
                    // No such group ..
                    result = false;
                }
            }
            catch (MultipleMatchesException ex)
            {
                result = false;
                exceptionThrown = true;
                exceptionErrorCode = 0xfffe;
                exceptionSource = ex.Source;
                exceptionMessage = "MultipleMatchesException: " + ex.Message;
            }
            catch (Exception ex)
            {
                result = false;
                exceptionThrown = true;
                exceptionErrorCode = 0xffff;
                exceptionSource = ex.Source;
                exceptionMessage = "isDirectMemberOfGroupException: " + ex.Message;

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

            return result;
        }
        #endregion

        #region isMemberOfGroup Method
        /// <summary>
        /// Verifies if the current user, under whose credentials the thread is executed, 
        /// is a member of an Active Directory group (added to the group explicitly or implicitly 
        /// as a member of a different group which is a nested member of the qroup in question).
        /// </summary>
        /// <param> groupName </param>
        /// <returns>
        /// Returns True the user is a direct member of the group
        /// Returns False if the user is not a member of the group, or if there was an exception.
        /// In the last case the 'exceptionThrown' property is set to 'true', 
        /// and the properties 'exceptionErrorCode', 'exceptionSource' and 'exceptionMessage'
        /// are set accordingly.
        /// </returns>

        public bool isMemberOfGroup(string groupName)
        {
            bool result = false;
            try
            {
                PrincipalSearchResult<Principal> groups = UserPrincipal.Current.GetAuthorizationGroups();
                if (groups != null)
                {
                    var iterGroup = groups.GetEnumerator();
                    using (iterGroup)
                    {
                        while (iterGroup.MoveNext())
                        {
                            try
                            {
                                Principal p = iterGroup.Current;
                                if (p.Name == groupName)
                                {
                                    result = true;
                                    break;
                                }
                            }
                            catch (NoMatchingPrincipalException pex)
                            {
                                string pexMsg = pex.Message; // for the compiler not to complain...
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    result = false;
                }
                return (result);
            }
            catch (PrincipalOperationException ex)
            {
                result = false;
                exceptionThrown = true;
                exceptionErrorCode = ex.ErrorCode;
                exceptionSource = ex.Source;
                exceptionMessage = "PricipalOperationException: " + ex.Message;
            }
            catch (Exception ex)
            {
                result = false;
                exceptionThrown = true;
                exceptionErrorCode = 0xffff;
                exceptionSource = ex.Source;
                exceptionMessage = "isMemberOfGroupException: " + ex.Message;

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
            return (result);
        }
        #endregion

        #region isUserInAD
        public bool isUserInAD(string userName)
        {
            bool success = false;
            using (var ctx = new PrincipalContext(ContextType.Domain))
            {
                try
                {
                    using (var user = UserPrincipal.FindByIdentity(ctx, userName))
                    {
                        if (user != null)
                        {
                            success = true;
                            usrIdentity = user.Name;    // e.g: CYLEDRA\Alex

                            string[] splt = usrIdentity.Split('\\');
                            if (splt.Length > 1)
                            {
                                usrDomainName = splt[0]; // e.g: CYLEDRA (the domain 'friendly name')       
                                usrLogonName = splt[1]; // e.g: Alex
                                usrPrincipalName = user.UserPrincipalName; // e.g: Alex@cy.Ledra   
                                usrDisplayName = user.DisplayName; // e.g: Alexandros Christofi
                                usrFirstName = user.GivenName; // e.g: Alexandros  
                                usrMiddleName = user.MiddleName;
                                usrLastName = user.Surname; // e.g: Christofi
                                usrTelephone = user.VoiceTelephoneNumber;
                                usrEmailAddress = user.EmailAddress;
                                usrSamAccountName = user.SamAccountName; // e.g: Alex
                                usrDistinguishedName = user.DistinguishedName; // e.g: CN=Alex,OU=BR001,OU=TestOU1,DC=cy,DC=Ledra
                            }
                            else
                            {
                                success = false;
                                string msg = string.Format("Unknown User Identity Format : [{0]", user.Name);
                                WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                            }

                            user.Dispose();
                        }
                    }
                }
                catch (MultipleMatchesException ex)
                {
                    success = false;
                    string msg = CreateExceptionMsg(ex, "MULTIPLEMATCHES", "RRDMActiveDirectoryHelper\\isUserInAD");
                    WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                }
                catch (Exception ex)
                {
                    success = false;
                    string msg = CreateExceptionMsg(ex, "GENERIC", "RRDMActiveDirectoryHelper\\isUserInAD");
                    WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                }
            }
            return success;
        }
        #endregion

        #region EventLogging
        private static void WriteEventLog(string eventSource, EventLogEntryType LogType, string Message)
        {
            EventLog Log = new EventLog();
            if (EventLog.SourceExists(eventSource))
            {
                Log.Source = eventSource;
                EventLog.WriteEntry(eventSource, Message, LogType);
            }
            else
            {
                Log.Source = "Application";
                EventLog.WriteEntry(eventSource, Message, LogType);
            }
            Log.Dispose();
        }
        #endregion

        #region GetExeptionLineNumber
        private int GetExLineNumber(Exception ex)
        {
            var lineNumber = 0;
            const string lineSearch = ":line ";
            var index = ex.StackTrace.LastIndexOf(lineSearch);
            if (index != -1)
            {
                var lineNumberText = ex.StackTrace.Substring(index + lineSearch.Length);
                if (int.TryParse(lineNumberText, out lineNumber))
                {
                }
            }
            return lineNumber;
        }
        #endregion

        #region CreateExceptionMsg()
        private string CreateExceptionMsg(Exception ex, string errorCode, string module)
        {
            int lineNo = GetExLineNumber(ex);
            string exType = ex.GetType().Name;
            string msg = string.Format("[{0}] : Exception at Line:[{0}] Type:[{1}] Message:[{2}]", DateTime.Now.ToString("YYYY-MM-dd HH:mm:ss.fff"), lineNo, exType, ex.Message);
            msg += string.Format("\nModule: [{0}], ErrorCode: [{1}].", module, errorCode);
            Exception ex1 = ex;
            while (ex1.InnerException != null)
            {
                msg += string.Format("\n  -- Type: [{0}] -- Message: [{1}]." + ex1.GetType().Name, ex1.Message);
                ex1 = ex1.InnerException;
            }
            msg += "\nStackTrace:\n" + ex.StackTrace;
            return (msg);
        }
        #endregion
    }
}

