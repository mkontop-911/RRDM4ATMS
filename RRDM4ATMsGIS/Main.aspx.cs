using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Artem.Google.UI;
using RRDM4ATMs;


namespace RRDM4ATMsGIS
{
    public partial class _Main : System.Web.UI.Page
    {
        /* 
         * Keep a global list of ATM records. It is updated in Populate_GridView() 
         * every time records are brought from the database
         */
        static private List<ATMDetails> ATMsCache = new List<ATMDetails>();


        static private int GroupFilter;

        /* ================================================================================== */
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack) // Initial load
            {
                Int32 GroupNo;
#if DEBUG
                PrintLog("   --> Initial Load");
#endif
                UpdateFooter("ftrCenter", " ");

                string Id = Request.QueryString["GroupNo"];
                if (Id != null)
                {
                    try
                    {
                        GroupNo = int.Parse(Id);
                        GroupFilter = GroupNo;

                        string strMsg = string.Format("Displaying ATMs from Group No: {0}", GroupFilter);
                        UpdateStatusLine(strMsg);
#if DEBUG
                        PrintLog(strMsg);
#endif
                    }
                    catch
                    {
                        string strError = "Invalid GroupNo in query string... Displaying ALL ATMs!";
                        UpdateStatusLine(strError);
                        GroupFilter = -1;
#if DEBUG
                        PrintLog(strError);
#endif
                    }
                }
                else
                {
                    GroupFilter = -1;
                }
#if DEBUG
                PrintLog(string.Format(" GroupFilter = {0}", GroupFilter));
#endif

            }
        }
        /* ================================================================================== */
        protected void Page_PreRender(object sender, EventArgs e)
        {
            GridViewRow r = gvATMs.SelectedRow;
#if DEBUG
            PrintLog("Page_PreRender() event...");
            PrintLog(string.Format("   --> GV Selected Index = {0}", gvATMs.SelectedIndex));
#endif
            if (gvATMs.SelectedIndex < 0)
            {
                pnlDetails.Visible = false;
            }
            else
            {
                pnlDetails.Focus();
                pnlDetails.Visible = true;
            }
        }

        /* ================================================================================== */
        protected void cbAllRecords_StatusChanged(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("cbAllRecords_StatusChanged()..");
#endif
            if (IsPostBack)
            {
                if (cbAllRecords.Checked)
                {
                    cblDistricts_SetItemsStatus(true);
                }
                else
                {
                    cblDistricts_SetItemsStatus(false);
                }

                Populate_GridView();
            }
        }

        /* ================================================================================== */
        protected void cblDistricts_SetItemsStatus(bool NewStatus)
        {
            int i = 0;
            int Count = 0;

#if DEBUG
            PrintLog("cblDistricts_SetItemsStatus()");
#endif
            Count = cblDistricts.Items.Count;
            for (i = 0; i < Count; i++)
            {
                cblDistricts.Items[i].Selected = NewStatus;
            }
        }

        /* ================================================================================== */
        protected int cblDistricts_GetItemCount()
        {
            int i = 0;
            int Count = 0;
            int CountChecked = 0;

            Count = cblDistricts.Items.Count;
            for (i = 0; i < Count; i++)
            {
                if (cblDistricts.Items[i].Selected)
                {
                    CountChecked += 1;
                }
            }
            return (CountChecked);
        }

        /* ================================================================================== */
        protected void cblDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("cblDistricts_SelectedIndexChanged()");
#endif
            cbAllRecords.Checked = false;
            Populate_GridView();
        }

        /* ================================================================================== */
        protected void Populate_GridView()
        {
#if DEBUG
            PrintLog("Populate_GridView()");
#endif
            ATMsCache.Clear();
            List<ATMDetails> ListOfATMs = new List<ATMDetails>();
            string CS = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                try
                {
                    string sqlText = "";
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = con;
                    if (cbAllRecords.Checked)
                    {
                        /* The DisplayAll checkbox is checked, so get all records.. */
                        if (GroupFilter == -1)
                        {
                            sqlText = "SELECT * from [TempAtmLocation]";
                        }
                        else
                        {
                            sqlText = " SELECT * FROM TempAtmLocation WHERE GroupNo = @GN";
                            SqlParameter sqlparam = new SqlParameter();
                            sqlparam.ParameterName = "@GN";
                            sqlparam.SqlDbType = SqlDbType.Int;
                            sqlparam.Value = GroupFilter;
                            sqlCmd.Parameters.Add(sqlparam);
                        }

                    }
                    else
                    {
                        /* Select records from database based on checked items in the checkbox list */
                        string sqlFilter = "";
                        int index = 0;

                        if (GroupFilter != -1)
                        {
                            SqlParameter sqlparam = new SqlParameter();
                            sqlparam.ParameterName = "@GN";
                            sqlparam.SqlDbType = SqlDbType.Int;
                            sqlparam.Value = GroupFilter;
                            sqlCmd.Parameters.Add(sqlparam);

                            sqlFilter = " GroupNo = @GN and (";
                        }

                        foreach (ListItem item in cblDistricts.Items)
                        {
                            if (item.Selected)
                            {
                                index += 1;
                                string paramName = "@param" + index.ToString().Trim();
                                SqlParameter param = new SqlParameter(paramName, SqlDbType.VarChar);
                                param.Value = item.Value.Trim();
                                sqlCmd.Parameters.Add(param);
                                sqlFilter += " District = " + paramName + " or ";
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(sqlFilter))
                        {
                            sqlText += "SELECT * from [TempAtmLocation] Where " + sqlFilter.Substring(0, sqlFilter.Length - 3);
                            if (GroupFilter != -1)
                            {
                                sqlText += ")";
                            }
                        }

                    }
#if DEBUG
                    PrintLog(string.Format("   --> SQL: [{0}]", sqlText));
#endif
                    if (!string.IsNullOrWhiteSpace(sqlText))
                    {
                        /* Execute the SQL cmd and fill in the table */
                        sqlCmd.CommandText = sqlText;

                        con.Open();
                        SqlDataReader rdr = sqlCmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            ATMDetails Rec = new ATMDetails();

                            Rec.ATMId = Convert.ToInt32(rdr["SeqNo"]);
                            Rec.ATMNumber = rdr["AtmNo"].ToString();
                            Rec.ATMColorId = rdr["ColorId"].ToString();
                            Rec.ATMColorDesc = rdr["ColorDesc"].ToString();
                            Rec.ATMGroupNo = Convert.ToInt32(rdr["GroupNo"]);
                            Rec.ATMGroupDesc = rdr["GroupDesc"].ToString();
                            Rec.ATMStreet = rdr["Street"].ToString();
                            Rec.ATMTown = rdr["Town"].ToString();
                            Rec.ATMPostalCode = rdr["PostalCode"].ToString();
                            Rec.ATMDistrict = rdr["District"].ToString();
                            Rec.ATMCountry = rdr["Country"].ToString();
                            Rec.ATMLat = Convert.ToDouble(rdr["Latitude"]);
                            Rec.ATMLon = Convert.ToDouble(rdr["Longitude"]);

                            ListOfATMs.Add(Rec);
                            ATMsCache.Add(Rec);
                        }
                    }
                    else
                    {
                        /* There is no checkboxlist selection so create an empty list */
                        ListOfATMs.Clear();
                        ATMsCache.Clear();
                    }
                    /* Populate the GridView */
                    gvATMs.DataSource = ListOfATMs;
                    gvATMs.DataBind();
                    gvATMs.SelectRow(-1);
                }
                catch (Exception ex)
                {
                    //// To Investigate...
                    string strError;
                    strError = ex.ToString();
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} : {1}", DateTime.Now, strError));
#endif
                }
            }
        }

        /* ===  Data bound to GridView.  Display the markers ================================ */
        protected void gvATMs_DataBound(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog(string.Format("gvATMs_DataBound() --> SelectedIndex={0}", gvATMs.SelectedIndex));
#endif
            //if (cblDistricts_GetItemCount() > 0)
            if (ATMsCache.Count > 0)
            {
                Place_GoogleMapMarkers();
            }
            else
            {
                GoogleMap1.Markers.Clear();
            }
        }


        /* === New item is selected in GV. Display its details in the Details Panel ========= */
        protected void gvATMs_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog(string.Format("gvATMs_SelectedIndexChanged() --> SelectedIndex={0}", gvATMs.SelectedIndex));
            PrintLog(string.Format("   --> Sender= {0}", sender.ToString()));
#endif
            if (gvATMs.SelectedDataKey != null)
            {
#if DEBUG
                PrintLog(string.Format("   --> DataKey={0}", gvATMs.SelectedDataKey.Value));
#endif
                PopulateDetails(gvATMs.SelectedDataKey);
                UpdateStatusLine("");

                PlaceSingle_GoogleMapMarker(gvATMs.SelectedDataKey);
            }
        }

        /* ================================================================================== */
        private void PopulateDetails(DataKey dataKey)
        {
#if DEBUG
            PrintLog(string.Format("PopulateDetails()   --> DataKey={0}", dataKey.Value));
#endif
            /* Get the record from the database */
            ATMDetails DetRec = new ATMDetails();

            DetRec = AtmDataAccess.GetATMDetails((int)dataKey.Value);

            if (DetRec != null)
            {
                tbAtmNo.Text = DetRec.ATMNumber;
                tbColorId.Text = DetRec.ATMColorId;
                tbStreet.Text = DetRec.ATMStreet;
                tbTown.Text = DetRec.ATMTown;
                tbPostalCode.Text = DetRec.ATMPostalCode;
                tbDistrict.Text = DetRec.ATMDistrict;
                tbCountry.Text = DetRec.ATMCountry;
                tbGroupDesc.Text = DetRec.ATMGroupDesc;

                /* Set the read-only property to the fields */
                tbAtmNo.ReadOnly = true;
                tbStreet.ReadOnly = true;
                tbTown.ReadOnly = true;
                tbPostalCode.ReadOnly = true;
                tbDistrict.ReadOnly = true;
                tbCountry.ReadOnly = true;
                tbGroupDesc.ReadOnly = true;
                tbColorId.ReadOnly = true;

                /* Set the border color for the Description field */
                tbGroupDesc.BorderColor = ColorTranslator.FromHtml("#FF0000");
            }
        }

        /* ================================================================================== */
        private void Place_GoogleMapMarkers()
        {
            bool DoneAlredy = false;
            double Lat = 0;
            double Lon = 0;
            double LatN = 0;
            double LatS = 0;
            double LonE = 0;
            double LonW = 0;
            double LatCenter = 0;
            double LonCenter = 0;
            Int32 iID;
            // string sID;
            string sTitle;
            string sInfo;
            string sStreet;

            Bounds bounds = new Bounds();

#if DEBUG
            PrintLog("Place_GoogleMapMarkers()");
#endif
            GoogleMarkers1.Markers.Clear();

            /* Use the ATMs Cache global List */
            foreach (ATMDetails Rec in ATMsCache)
            {
                Marker marker = new Marker();

                iID = Convert.ToInt32(Rec.ATMId);
                sTitle = Rec.ATMNumber;     // the marker title
                sInfo = Rec.ATMGroupDesc;  // the marker info
                sStreet = Rec.ATMStreet.Trim();
                Lat = Rec.ATMLat;
                Lon = Rec.ATMLon;

                if (Lat == 0 || Lon == 0)
                {
#if DEBUG
                    PrintLog(string.Format("   --> Placing marker: AtmNo={0}", Rec.ATMNumber));
#endif
                }
                if (!DoneAlredy)
                {
                    LatN = LatS = Lat;
                    LonE = LonW = Lon;
                    DoneAlredy = true;
                }
                else
                {
                    if (Lat > LatN) { LatN = Lat; }
                    if (Lat < LatS) { LatS = Lat; }
                    if (Lon > LonE) { LonE = Lon; }
                    if (Lon < LonW) { LonW = Lon; }
                }
                marker.Title = sTitle;
                marker.Info = sInfo;
                marker.Address = sStreet;
                marker.Position.Latitude = Lat;
                marker.Position.Longitude = Lon;

                marker.Draggable = false;

                string ColorId = (Rec.ATMColorId).Trim();
                switch (ColorId)
                {
                    case "1":
                        {
                            marker.Icon = "/icons/marker-red.png";
                            break;
                        }
                    case "2":
                        {
                            marker.Icon = "/icons/marker-green.png";
                            break;
                        }
                    case "3":
                        {
                            marker.Icon = "/icons/marker-blue.png";
                            break;
                        }
                    case "4":
                        {
                            marker.Icon = "/icons/marker-black.png";
                            break;
                        }
                    default:
                        {
                            marker.Icon = "/icons/marker-white.png";
                            break;
                        }
                }

                GoogleMarkers1.Markers.Add(marker);
            }

            bounds.NorthEast.Latitude = LatN;
            bounds.NorthEast.Longitude = LonE;
            bounds.SouthWest.Latitude = LatS;
            bounds.SouthWest.Longitude = LonW;
            GoogleMap1.Bounds = bounds;

            LatCenter = (LatN + LatS) / 2;
            LonCenter = (LonE + LonW) / 2;

            if (ATMsCache.Count > 0)
            { GoogleMap1.Zoom = 16; }
            else
            { GoogleMap1.Zoom = 2; }
            GoogleMap1.Center.Latitude = LatCenter;
            GoogleMap1.Center.Longitude = LonCenter;

#if DEBUG
            PrintLog(string.Format("   --> Bounds: NE [{0},{1}]  SW [{2},{3}]",
            bounds.NorthEast.Latitude, bounds.NorthEast.Longitude, bounds.SouthWest.Latitude, bounds.SouthWest.Longitude));
#endif
        }

        /* ================================================================================== */
        private void PlaceSingle_GoogleMapMarker(DataKey dataKey)
        {
            double Lat = 0;
            double Lon = 0;
            Int32 iID;
            string sTitle;
            string sInfo;
            string sStreet;

#if DEBUG
            PrintLog("PlaceSingle_GoogleMapMarker()");
#endif

            /* Get the record from the database */
            ATMDetails Rec = new ATMDetails();
            Rec = AtmDataAccess.GetATMDetails((int)dataKey.Value);

            if (Rec != null)
            {
                iID = Convert.ToInt32(Rec.ATMId);
                sTitle = Rec.ATMNumber;     // the marker title
                sInfo = Rec.ATMGroupDesc;  // the marker info
                sStreet = Rec.ATMStreet.Trim();
                Lat = Rec.ATMLat;
                Lon = Rec.ATMLon;


                Marker marker = new Marker();
                GoogleMarkers1.Markers.Clear();

                marker.Title = sTitle;
                marker.Info = sInfo;
                marker.Address = sStreet;
                marker.Position.Latitude = Lat;
                marker.Position.Longitude = Lon;
                marker.Draggable = false;

                string ColorId = (Rec.ATMColorId).Trim();
                switch (ColorId)
                {
                    case "1":
                        {
                            marker.Icon = "/icons/marker-red.png";
                            break;
                        }
                    case "2":
                        {
                            marker.Icon = "/icons/marker-green.png";
                            break;
                        }
                    case "3":
                        {
                            marker.Icon = "/icons/marker-blue.png";
                            break;
                        }
                    case "4":
                        {
                            marker.Icon = "/icons/marker-black.png";
                            break;
                        }
                    default:
                        {
                            marker.Icon = "/icons/marker-white.png";
                            break;
                        }
                }

                GoogleMarkers1.Markers.Add(marker);

                GoogleMap1.Zoom = 19;
                if (Lat == 0 && Lon == 0)
                {
                    GoogleMap1.Zoom = 3;
                }
                else
                {
                    GoogleMap1.Center.Latitude = Lat;
                    GoogleMap1.Center.Longitude = Lon;
                }
            }
            else
            {
                // failed to read record.
#if DEBUG
                PrintLog("Failled to read record...");
#endif

            }
        }

        /* ================================================================================== */
        protected void GoogleMap1_Load(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("GoogleMap1_Load");
#endif
        }

        /* ================================================================================== */
        protected void GoogleMap1_PreRender(object sender, EventArgs e)
        {
            Bounds bnd = new Bounds();
#if DEBUG
            PrintLog("GoogleMap1_PreRender()");
#endif
//            if (IsPostBack)
//            {
//#if DEBUG
//                PrintLog("   -->PostBack");
//#endif
//                /* TO DO */
//                /* Do not Invoke when in Single Marker Mode */
//                bool dvVisible = pnlDetails.Visible;
//                if (!dvVisible)
//                {
//                    if (ATMsCache.Count > 0)
//                        Place_GoogleMapMarkers();
//                }
//                else
//                {
//                    //PlaceSingle_GoogleMapMarker();
//                }
//            }

//            if (GoogleMap1.Bounds != null)
//            {
//                bnd.NorthEast.Latitude = GoogleMap1.Bounds.NorthEast.Latitude;
//                bnd.NorthEast.Longitude = GoogleMap1.Bounds.NorthEast.Longitude;
//                bnd.SouthWest.Latitude = GoogleMap1.Bounds.SouthWest.Latitude;
//                bnd.SouthWest.Longitude = GoogleMap1.Bounds.SouthWest.Longitude;
//#if DEBUG
//                PrintLog(string.Format("   --> Bounds: NE [{0},{1}]  SW [{2},{3}]",
//                bnd.NorthEast.Latitude, bnd.NorthEast.Longitude, bnd.SouthWest.Latitude, bnd.SouthWest.Longitude));
//#endif
//            }
//            else
//            {
//#if DEBUG
//                PrintLog("   --> GoogleMap1.Bounds is NULL!");
//#endif
//            }
        }


        /* ================================================================================== */
        private void UpdateStatusLine(string p)
        {
            ContentPlaceHolder mpCPH;
            Label lbl;
            mpCPH = (ContentPlaceHolder)Master.FindControl("ContentPlaceHolderStatus");
            if (mpCPH != null)
            {
                lbl = (Label)mpCPH.FindControl("lblStatus");
                if (lbl != null)
                {
                    lbl.Text = p;
                }
            }
        }

        /* ================================================================================== */
        private void UpdateFooter(string ftrID, string txt)
        {
            /* IDs:
             *      ftrLeft
             *      ftrCenter
             *      ftrRight -- overrides date/time
             */
            ContentPlaceHolder mpCPH;
            TableCell cell;
            mpCPH = (ContentPlaceHolder)Master.FindControl("ContentPlaceHolderFooter");
            if (mpCPH != null)
            {
                cell = (TableCell)mpCPH.FindControl(ftrID);
                if (cell != null)
                {
                    cell.Text = txt;
                }
            }
        }

        /* ================================================================================== */
#if DEBUG
        protected void PrintLog(string Text)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0} : {1}", DateTime.Now, Text));
        }
#endif



    }
}