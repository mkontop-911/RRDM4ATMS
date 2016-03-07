using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RRDM4ATMsWeb
{
    public partial class WebForm24 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string WAtmNo = (string)Session["WAtmNo"];
            int WSesNo = (int)Session["WReplCycle"];

            string FilterExpression = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND (ErrType = 1) AND OpenErr =1";

            SqlDataSource1.FilterExpression = FilterExpression;

            //bool Replenishment = true;
            //string SearchFilter = "AtmNo = '" + WAtmNo + "' AND SesNo=" + WSesNo + " AND (ErrType = 1) AND OpenErr =1";
            //NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, "", Replenishment, SearchFilter);
            //NForm24.Show();
        }
    }
}