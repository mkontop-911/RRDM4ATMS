using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRDM4ATMs
{
    public abstract class Logger
    {
        public bool HasErrors { get; set; }
        public int LogErrorNo { get; set; }
        // public List<string> Errors { get; set; }
        public string ErrorDetails { get; set; }
        public void CatchDetails(Exception ex)
        {
            HasErrors = true;

            //if (Errors == null)
            //{
            //    Errors = new List<string>();
            //}
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            string WDatetime = DateTime.Now.ToString(); 

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("DtTm : ");
            WParameters.Append(WDatetime);
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            LogErrorNo = Log.ErrorNo; 
            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is an issue to be reported to the helpdesk " + Environment.NewLine
                                                         + "Issue reference number: " + Log.ErrorNo.ToString());
            }
            ErrorDetails = ("There is an issue to be reported to the helpdesk " + Environment.NewLine
                                                         + "Issue reference number: " + Log.ErrorNo.ToString());
            //    Environment.Exit(0);}
        }
    }
}
