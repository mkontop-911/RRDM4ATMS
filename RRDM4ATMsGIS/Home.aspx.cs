using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RRDM4ATMsGIS
{
    public partial class Home : System.Web.UI.Page
    {
 
        protected void Page_Load(object sender, EventArgs e)
        {

        }


 
        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            string url = "~/Main.aspx";
            Response.Redirect(url);
        }

        protected void btAddr_Click(object sender, EventArgs e)
        {
            string id = tbATMID.Text;
            string url;
            url = "~/GeoQuery.aspx?AtmSeqNo=" + id.ToString();
            Response.Redirect(url);
        }


        protected void btGroup_Click(object sender, EventArgs e)
        {
            string id = tbGroupNo.Text;
            string url;
            url = "~/Main.aspx?GroupNo=" + id.ToString();
            Response.Redirect(url);
        }
    }
}