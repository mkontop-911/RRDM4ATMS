using System;
using System.Collections.Generic;
using RRDM4ATMs;

namespace AD_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            /// <summary>
            /// Gets the user under whose credentials the program is being executed.
            /// 
            /// If the user is logged on to an Active Directory domain (ContextType="Domain")
            /// the program prints out a number of properties from the Active Directory store.
            /// 
            /// If the user is logged on locally (ContextType="Machine") then only its identity
            /// is set (in the form MACHINENAME\USERID), his/her 'LogonName' and the machine name
            /// ('usrFriendyDomainName' property of the class. 
            /// </summary>
            /// <param> 
            /// The name(s) of the group(s) to check if the current user is a member of
            /// If no parameters are passed then the program checks for the following groups
            /// - RRDMLocal
            /// - RRDMGlobal
            /// - RRDMUniv
            /// </param>
            /// <returns>
            /// Displays its output to the console, no return code
            /// </returns>

            bool bl = false;

            RRDMActiveDirectoryHelper ad = new RRDMActiveDirectoryHelper();

            Console.WriteLine("\n Program started...");

            /* Test for Logged on User Details */
            /* =============================== */
            bl = ad.getLoggedOnUserDetails();
            if (bl)
            {
                Console.WriteLine(" usrContextType ----------> " + ad.usrContextType);
                Console.WriteLine(" usrIdentity -------------> " + ad.usrIdentity);
                Console.WriteLine(" usrDomainName -----------> " + ad.usrDomainName);
                Console.WriteLine(" usrLogonName ------------> " + ad.usrLogonName);
                Console.WriteLine(" usrPrincipalName --------> " + ad.usrPrincipalName);
                Console.WriteLine(" usrSamAccountName -------> " + ad.usrSamAccountName);
                Console.WriteLine(" usrDistinguishedName ----> " + ad.usrDistinguishedName);
                Console.WriteLine(" usrFirstName ------------> " + ad.usrFirstName);
                Console.WriteLine(" usrLastName -------------> " + ad.usrLastName);
                Console.WriteLine(" usrMiddleName -----------> " + ad.usrMiddleName);
                Console.WriteLine(" usrDisplayName ----------> " + ad.usrDisplayName);
                Console.WriteLine(" usrTelephone ------------> " + ad.usrTelephone);
                Console.WriteLine(" usrMobilePhone ----------> " + ad.usrMobilePhone);
                Console.WriteLine(" usrEmailAddress ---------> " + ad.usrEmailAddress);
            }
            else
            {
                Console.WriteLine(" ===> LoggedOnUserDetails returned 'false'");
                if (ad.exceptionThrown)
                {
                    Console.WriteLine("     An Exception was Thrown:");
                    Console.WriteLine("     Exception Error:   " + ad.exceptionErrorCode);
                    Console.WriteLine("     Exception Source:  " + ad.exceptionSource);
                    Console.WriteLine("     Exception Message: " + ad.exceptionMessage);
                }
            }
            Console.WriteLine("\n");

            // Test for group membership only if ContextType="Domain"
            if (ad.usrContextType == "Domain")
            {
                List<string> testGroups = new List<string>();
                if (args.Length == 0) // If no parameters were passed to the program then use our testing ones...
                {
                    testGroups.Add("RRDMLocal");
                    testGroups.Add("RRDMGlobal");
                    testGroups.Add("RRDMUniv");
                }
                else // use the parameters passed to the program
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        testGroups.Add(args[i]);
                    }
                }

                /* Test for direct Group membership */
                /* ---------------------------------*/
                foreach (string groupName in testGroups)
                {
                    bl = ad.isDirectMemberOfGroup(groupName);
                    Console.WriteLine(" isDirectMemberOfGroup(" + groupName + ") returned '" + bl + "'");
                    if (bl == false)
                    {
                        if (ad.exceptionThrown)
                        {
                            Console.WriteLine("     An Exception was Thrown:");
                            Console.WriteLine("     Exception Error:   " + ad.exceptionErrorCode);
                            Console.WriteLine("     Exception Source:  " + ad.exceptionSource);
                            Console.WriteLine("     Exception Message: " + ad.exceptionMessage);
                        }
                    }
                }
                Console.WriteLine("");

                /* Test for Group membership recursively */
                /* ------------------------------------- */
                /* Searches recursively for the case where the user is a direct member */
                /* of a group that is a member of the group we are looking for         */
                /* ------------------------------------------------------------------- */
                foreach (string groupName in testGroups)
                {
                    bl = ad.isMemberOfGroup(groupName);
                    Console.WriteLine(" isMemberOfGroup(" + groupName + ") returned '" + bl + "'");
                    if (bl == false)
                    {
                        if (ad.exceptionThrown)
                        {
                            Console.WriteLine("An Exception was Thrown:");
                            Console.WriteLine("     Exception Error:   " + ad.exceptionErrorCode);
                            Console.WriteLine("     Exception Source:  " + ad.exceptionSource);
                            Console.WriteLine("     Exception Message: " + ad.exceptionMessage);
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine(" DomainContext != 'Domain',  bypassing group membership verification!");
            }
            Console.WriteLine("\n ... Finished... Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}
