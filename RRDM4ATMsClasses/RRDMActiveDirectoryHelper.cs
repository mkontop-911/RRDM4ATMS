using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Principal;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;


namespace RRDM4ATMs
{
    public class RRDMActiveDirectoryHelper
    {

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

                    DirectoryEntry ldapDE = (DirectoryEntry) UserPrincipal.Current.GetUnderlyingObject();
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
            }
            return (result);
        }
        #endregion
    }
}
