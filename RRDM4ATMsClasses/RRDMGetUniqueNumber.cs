using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMGetUniqueNumber : Logger
    {
        public RRDMGetUniqueNumber() : base() { }

        public int TotalSelected;

        //string SqlString; // Do not delete

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Get Next Unique Id 
        public int GetNextValue()
        {
            int iResult = 0;

            string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[usp_GetNextUniqueId]";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters
                        SqlParameter iNextValue = new SqlParameter("@iNextValue", SqlDbType.Int);
                        iNextValue.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(iNextValue);
                        cmd.ExecuteNonQuery();
                        string sResult = cmd.Parameters["@iNextValue"].Value.ToString();
                        int.TryParse(sResult, out iResult);

                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return iResult;
        }

    
    }
}


