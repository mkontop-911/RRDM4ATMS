using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Artem.Google.UI;
using Artem.Google.Net;
using System.Configuration;

using RRDM4ATMs;

namespace RRDM4ATMsGIS
{
    public partial class GeoQuery : System.Web.UI.Page
    {
        /* The list of ATMs displayed in GridView
         * from which we position the markers on Map */
        private static List<RRDMTempAtmsLocation.ATMGeo> ListOfAddrs = new List<RRDMTempAtmsLocation.ATMGeo>();

        /* The marker position on the Map when in Single-Marker mode */
        private static Marker MarkerShown = new Marker();
        private static int LocMode = 1; //1: edit/drag 2: show only/non-draggable

        /* ========================================================= */
        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("");
            PrintLog("");
            PrintLog("");
            PrintLog("");
            PrintLog("");
            PrintLog("Page_Load()..");
#endif
            if (!IsPostBack) // Initial load
            {
#if DEBUG
                PrintLog("   --> Initial Load");
#endif

                Int32 ATMId;
                string Id = Request.QueryString["AtmSeqNo"];
                if (Id == null)
                {
                    // OpMode = 1;
                    pnlDetails.Visible = false;
                    return;
                }

                try
                {
                    ATMId = int.Parse(Id);
                }
                catch
                {
                    string strError = "Invalid AtmSeqNo in query string...";
                    UpdateStatusLine(strError);
                    pnlDetails.Visible = false;
                    ATMId = -1;
#if DEBUG
                    PrintLog(strError);
#endif
                }

                if (ATMId > -1)
                {
                    PopulateDetails(ATMId);
                }

            }
            else // IsPostBack
            {
#if DEBUG
                PrintLog("   --> IsPostBack");
#endif
            }
        }

        protected void PopulateDetails(Int32 ATMId)
        {
#if DEBUG
            PrintLog("Populate Details()..");
#endif
            RRDMTempAtmsLocation DetRec = new RRDMTempAtmsLocation();
            DetRec.ReadTempAtmLocationSpecificBySeqNo(ATMId);


            if (DetRec.RecordFound == true)
            {
                LocMode = DetRec.Mode;

                txAtmId.Text = Convert.ToString(DetRec.SeqNo);
                txAtmNo.Text = DetRec.AtmNo;
                tbAddress.Text = DetRec.Street;
                tbTown.Text = DetRec.Town;
                tbPostalCode.Text = DetRec.PostalCode;
                tbDistrict.Text = DetRec.District;
                tbCountry.Text = DetRec.Country;
                tbLat.Text = Convert.ToString(DetRec.Latitude);
                tbLon.Text = Convert.ToString(DetRec.Longitude);

                /* Set the read-only property to the fields*/
                txAtmId.ReadOnly = true;
                txAtmNo.ReadOnly = true;

                ///* Set the border color for the editable fields */
                //tbLat.BorderColor = ColorTranslator.FromHtml("#FF0000");
                //tbLon.BorderColor = ColorTranslator.FromHtml("#FF0000");
                //tbAddress.BorderColor = ColorTranslator.FromHtml("#FF0000");


                if (DetRec.Mode == 2) // Show the ATM on the map. No editing/geocoding allowed
                {
                    btFind.Visible = false;
                    btReload.Visible = false;
                    btSave.Visible = false;
                    btShow.Visible = true;
                    gvATMs.Visible = false;
                    
                    tbAddress.ReadOnly = true;
                    tbTown.ReadOnly = true;
                    tbPostalCode.ReadOnly = true;
                    tbDistrict.ReadOnly = true;
                    tbCountry.ReadOnly = true;
                    tbLat.ReadOnly = true;
                    tbLon.ReadOnly = true;
                }

                pnlDetails.Visible = true;
            }
            else
            {
                string strError = "Specified ATM SeqNo in query string does not exist in the database...";
#if DEBUG
                PrintLog(strError);
                PrintLog(DetRec.ErrorOutput);
#endif
                UpdateStatusLine(strError);
                pnlDetails.Visible = false;
                ATMId = -1;
            }
        }

        protected void gvATMs_DataBound(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("gvATMs_DataBound()..");
#endif
        }

        protected void gvATMs_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("gvATMs_SelectedIndexChanged()..");
#endif
            if (gvATMs.SelectedDataKey != null)
            {
                CreateGeoMarkerFromDataKey(gvATMs.SelectedDataKey);
                PlaceGeoMarker();
            }
        }

        private void CreateGeoMarkerFromDataKey(DataKey key)
        {
#if DEBUG
            PrintLog("CreateGeoMarkerFromDataKey()..");
#endif
            int id;
            double Lat = 0;
            double Lon = 0;
            RRDMTempAtmsLocation.ATMGeo Rec = new RRDMTempAtmsLocation.ATMGeo();

            id = Convert.ToInt16(key.Value);
            Rec = ListOfAddrs[id];
            Lat = Rec.Lat;
            Lon = Rec.Lon;

            MarkerShown.Position.Latitude = Lat;
            MarkerShown.Position.Longitude = Lon;
            MarkerShown.Info = Rec.FormattedAddress;
        }

        protected void ShowGeoMarker()
        {
#if DEBUG
            PrintLog("ShowGeoMarker()..");
#endif
            double Lat = 0;
            double Lon = 0;

            try
            {
                Lat = double.Parse(tbLat.Text);
                Lon = double.Parse(tbLon.Text);
            }
            catch
            {
                UpdateStatusLine("Invalid Coordinates!");
                return;
            }
            MarkerShown.Position.Latitude = Lat;
            MarkerShown.Position.Longitude = Lon;
            MarkerShown.Info = tbAddress.Text;
 
            PlaceGeoMarker();
        }

        protected void PlaceGeoMarker()
        {
#if DEBUG
            PrintLog("PlaceGeoMarker()..");
#endif
            if (IsPostBack)
            {
                if (MarkerShown.Position.Latitude != 0)
                {
                    string ConfigZoom;
                    int ZoomLevel;

                    ConfigZoom = ConfigurationManager.AppSettings["SingleMarkerInitialZoomIn"];
                    if (!Int32.TryParse(ConfigZoom, out ZoomLevel))
                    {
                        ZoomLevel = 17;
                    }

                    GoogleMarkers1.Markers.Clear();

                    if (LocMode == 2)
                    {
                        MarkerShown.Draggable = false;
                    }
                    else
                    {
                        MarkerShown.Draggable = true;
                    }
 
                    GoogleMarkers1.Markers.Add(MarkerShown);

                    GoogleMap1.Bounds = null;
                    GoogleMap1.Zoom = ZoomLevel;
                    GoogleMap1.Center.Latitude = MarkerShown.Position.Latitude;
                    GoogleMap1.Center.Longitude = MarkerShown.Position.Longitude;

                    tbLat.Text = MarkerShown.Position.Latitude.ToString();
                    tbLon.Text = MarkerShown.Position.Longitude.ToString();
                }
            }
        }

        //        protected void btClear_Click(object sender, EventArgs e)
        //        {
        //#if DEBUG
        //            PrintLog("   btClear_Click()..");
        //#endif
        //            TextBox1.Text = "";
        //            lblStatus.Text = "";
        //            lblStatus.Visible = false;
        //            ListOfAddrs.Clear();
        //            gvATMs.DataBind();
        //            UpdateStatusLine("");
        //            MarkerShown.Position.Latitude = 0;
        //            MarkerShown.Position.Longitude = 0;
        //            MarkerShown.Info = "";
        //        }

        /* Find the coordinates of the address in the details panel.
           The query to Google Maps may return multiple results */
        protected void btFind_Click(object sender, EventArgs e)
        {
            // if (!string.IsNullOrWhiteSpace(tbAddress.Text))
            {
                lblStatus.Text = "";
                lblStatus.Visible = false;
                ListOfAddrs.Clear();
                gvATMs.DataBind();
                UpdateStatusLine("");
                MarkerShown.Position.Latitude = 0;
                MarkerShown.Position.Longitude = 0;
                MarkerShown.Info = "";

                GeoSearch();
            }
        }

        /* Show the coordinates in the Lat/Lon fields on the map */
        protected void btShow_Click(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("btShow_Click()..");
#endif
            ShowGeoMarker();
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("btSave_Click()..");
#endif

            double dLat = 0;
            double dLon = 0;
            Int32 SeqNo = Convert.ToInt32(txAtmId.Text);
            RRDMTempAtmsLocation TmpAtmRec = new RRDMTempAtmsLocation();

            try
            {
                dLat = double.Parse(tbLat.Text);
                dLon = double.Parse(tbLon.Text);
            }
            catch
            {
                string strError = "Invalid Coordinates! Please correct...";
                UpdateStatusLine(strError);
                return;
            }

            if (dLat == 0 || dLon == 0)
            {
                string strError = " Invalid Coordinates! Please correct...";
                UpdateStatusLine(strError);
                return;
            }

            bool AddressChanged = false;

            TmpAtmRec.ReadTempAtmLocationSpecificBySeqNo(SeqNo);

            if (TmpAtmRec.Street != tbAddress.Text)
            {
                AddressChanged = true;
            }
            else if (TmpAtmRec.Town != tbTown.Text)
            {
                AddressChanged = true;
            }
            else if (TmpAtmRec.PostalCode != tbPostalCode.Text)
            {
                AddressChanged = true;
            }
            else if (TmpAtmRec.District != tbDistrict.Text)
            {
                AddressChanged = true;
            }
            else if (TmpAtmRec.Country != tbCountry.Text)
            {
                AddressChanged = true;
            }

            if (AddressChanged)
            {
                TmpAtmRec.NewStreet = tbAddress.Text;
                TmpAtmRec.NewTown = tbTown.Text;
                TmpAtmRec.NewPostalCode = tbPostalCode.Text;
                TmpAtmRec.NewDistrict = tbDistrict.Text;
                TmpAtmRec.NewCountry = tbCountry.Text;
                TmpAtmRec.AddressChanged = true;
            }

            if (dLat != TmpAtmRec.Latitude)
            {
                TmpAtmRec.Latitude = dLat;
                TmpAtmRec.LocationFound = true;
            }
            if (dLon != TmpAtmRec.Longitude)
            {
                TmpAtmRec.Longitude = dLon;
                TmpAtmRec.LocationFound = true;
            }

            TmpAtmRec.DtTmUpdated = DateTime.Now;

            // UpdateStatusLine("ToDo!");
            TmpAtmRec.UpdateTempAtmLocationRecordBySeqNo(SeqNo);
            if (TmpAtmRec.ErrorFound)
            {
                string ErrMsg = TmpAtmRec.ErrorOutput.Substring(0, 45);
                UpdateStatusLine(ErrMsg);
            }
        }

        /* Reloads the URL */
        protected void btReload_Click(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("btReload_Click()..");
#endif
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        /* Request the coordinates from Google Maps given the address 
           Google Maps may return multiple results */
        private void GeoSearch()
        {
            ListOfAddrs.Clear();

            string sQueryAddress = "";
            if (!string.IsNullOrWhiteSpace(tbAddress.Text))
            {
                sQueryAddress += string.Format("{0}, ", tbAddress.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(tbPostalCode.Text.Trim()))
            {
                sQueryAddress += string.Format("{0} ", tbPostalCode.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(tbTown.Text.Trim()))
            {
                sQueryAddress += string.Format("{0}, ", tbTown.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(tbDistrict.Text.Trim()))
            {
                sQueryAddress += string.Format("{0}, ", tbDistrict.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(tbCountry.Text.Trim()))
            {
                sQueryAddress += string.Format("{0}", tbCountry.Text.Trim());
            }


            if (!string.IsNullOrWhiteSpace(sQueryAddress))
            {
                string strStatus;
                try
                {
                    GeoRequest request = new GeoRequest(sQueryAddress);
                    GeoResponse response = request.GetResponse();

                    strStatus = response.Status.ToString();
                    lblStatus.Text = "Google API returned:  " + strStatus;
                    lblStatus.Visible = true;

                    if (strStatus == "OK")
                    {
                        int Indx = 0;
                        int Count = response.Results.Count;

                        UpdateStatusLine("");

                        for (Indx = 0; Indx < Count; Indx++)
                        {
                            RRDMTempAtmsLocation.ATMGeo Addr = new RRDMTempAtmsLocation.ATMGeo();
                            Addr.Id = Indx;
                            Addr.FormattedAddress = response.Results[Indx].FormattedAddress;
                            //Addr.AddressType = response.Results[Indx].Types[0];
                            Addr.AddressType = "|";
                            foreach (string str in response.Results[Indx].Types)
                            {
                                Addr.AddressType += " " + str.Trim() +" |";
                            }
                            Addr.LocationType = response.Results[Indx].Geometry.LocationType.ToString();
                            Addr.Lat = response.Results[Indx].Geometry.Location.Latitude;
                            Addr.Lon = response.Results[Indx].Geometry.Location.Longitude;
                            ListOfAddrs.Add(Addr);
                        }

                        //int count = response.Results.Count;
                        //string status = response.Status.ToString();

                        /* Populate the GridView */
                        gvATMs.DataSource = ListOfAddrs;
                        gvATMs.DataBind();
                        gvATMs.SelectRow(-1);

                        if (gvATMs.Rows.Count == 1)
                        {
                            gvATMs.SelectedIndex = 0;
                            CreateGeoMarkerFromDataKey(gvATMs.SelectedDataKey);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string strError;
                    string err = "An error occured with GoogleMaps API!";
                    UpdateStatusLine(err);

                    strError = ex.ToString();
#if DEBUG
                    PrintLog(strError);
#endif
                    return;
                }
            }
        }

        protected void GoogleMap1_Load(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("GoogleMap1_Load()..");
#endif
        }


        protected void GoogleMap1_PreRender(object sender, EventArgs e)
        {
#if DEBUG
            PrintLog("GoogleMap1_PreRender()..");
#endif
            PlaceGeoMarker();
        }

        /* ================================================================================== */
        protected void MarkerDragEnd(object sender, MarkerEventArgs e)
        {
#if DEBUG
            PrintLog("MarkerDragEnd()..");
#endif
            MarkerShown.Position.Latitude = e.Position.Latitude;
            MarkerShown.Position.Longitude = e.Position.Longitude;
            if (pnlDetails.Visible == true)
            {
                tbLat.Text = Convert.ToString(e.Position.Latitude);
                tbLon.Text = Convert.ToString(e.Position.Longitude);
            }
            UpdateStatusLine("");
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
#if DEBUG
        protected void PrintLog(string Text)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0} : {1}", DateTime.Now, Text));
        }
#endif

    }
}