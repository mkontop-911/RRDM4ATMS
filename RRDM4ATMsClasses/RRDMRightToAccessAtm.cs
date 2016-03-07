using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRDM4ATMs
{
    public class RRDMRightToAccessAtm
    {
        // See if this ATM belongs to the user 

        public bool RecordFound;

        public void CheckRightToAccessAtm(string UserNo, string AtmNo)
        {
            
            RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();

            Ua.ReadUsersAccessAtmTableSpecific(UserNo, AtmNo, 0); // CHECK IF USER HAS THIS ATM 

            if (Ua.RecordFound == false) // FAILED .. PROCEED WITH GROUP CHECKING 
            {
                // CHECK IF ATM IN GROUP THAT BELONGS TO THIS USER

                RRDMAtmsClass Aa = new RRDMAtmsClass();

                Aa.ReadAtm(AtmNo);

                Ua.ReadUsersAccessAtmTableSpecific(UserNo, "", Aa.AtmsReplGroup);

                if (Ua.RecordFound == false)
                {
                    Ua.ReadUsersAccessAtmTableSpecific(UserNo, "", Aa.AtmsReconcGroup);
                    if (Ua.RecordFound == false)
                    {
                        RecordFound = false;
                        return;
                    }
                }
            }

            RecordFound = true; 
        }
    }
}
    

