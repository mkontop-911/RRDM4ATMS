using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDM_MOBILE_TABLE_NAMES
    {
        
        public string WTableName_Master_Primary;
        public string WTableName_Master_Matched;

        public void GetFileNames(string InApplication)
        {
            if (InApplication == "EGATE")
            {
                WTableName_Master_Primary = InApplication + ".[dbo].[TXNS_MASTER]";
                WTableName_Master_Matched = InApplication + "_MATCHED_TXNS" + ".[dbo].[TXNS_MASTER]";

            }
            else
            {
                // BDC EWALLETS 
                WTableName_Master_Primary = InApplication + ".[dbo].[" + InApplication + "_TPF_TXNS_MASTER]";
                WTableName_Master_Matched = InApplication + "_MATCHED_TXNS" + ".[dbo].[" + InApplication + "_TPF_TXNS_MASTER]";

            }
        }

    }
}

