using System;
using System.Text;
using System.Xml;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDM_IST_WebService : Logger
    {
        public RRDM_IST_WebService() : base() { }

        public string cassette_cfg_id;
        public string cassette_count;
        public string group_name;

        public string location;
        public string make; //  NDC
        public string model; // 7080

        public string sdedc;
        public string status;
        public string status_bna;

        public string status_dispense;
        public string unit;

        // 
        // Cassette 1
        public string cash_cur_bal_11;
        public string cassette_number_11;
        public string currency_11;
        public string denomination_11;
        public string reset_time_11;

        // Cassette 2
        public string cash_cur_bal_12;
        public string cassette_number_12;
        public string currency_12;
        public string denomination_12;
        public string reset_time_12;

        // Cassette 3
        public string cash_cur_bal_13;
        public string cassette_number_13;
        public string currency_13;
        public string denomination_13;
        public string reset_time_13;

        // Cassette 4
        public string cash_cur_bal_14;
        public string cassette_number_14;
        public string currency_14;
        public string denomination_14;
        public string reset_time_14;

        // Cassette 5
        public string cash_cur_bal_15;
        public string cassette_number_15;
        public string currency_15;
        public string denomination_15;
        public string reset_time_15;

        public decimal AvailableBalance; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;


        public DataTable Table_IST_ATM;
        public DataTable Table_IST_Cassettes;


        //
        // READ Fields from WebService
        // 
        // Returns two tables for the calling ATM
        // Also all fields 
        // 
        public bool CassettesFound;
        public DataSet ReadFieldsFromWebService(int myAtmId) //1 change
        {
            // 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            CassettesFound = false;

            Table_IST_ATM = new DataTable();
            Table_IST_ATM.Clear();

            Table_IST_Cassettes = new DataTable();
            Table_IST_Cassettes.Clear();

            var appSettingsReaderX = new System.Configuration.AppSettingsReader();
            var mdslWsUrl = Convert.ToString(appSettingsReaderX.GetValue("WS_URL", typeof(string)));
            // Task<DataSet> dsAtmInfo = GetAtmInfoAsync(mdslWsUrl, Convert.ToInt32(this.txtAtmId.Text));

            var page = mdslWsUrl + myAtmId;

            var ds = new DataSet(); // Create the DataSet

            try
            {
                // Use HttpClient in Using-statement.
                // ... Use GetAsync to get the page data.
                using (var client = new HttpClient())
                {
                    using (var response = client.GetAsync(page).Result)//2 change
                    {
                        using (var content = response.Content)
                        {
                            // Get contents of page as a String.
                            var wsResult = content.ReadAsStringAsync().Result;//3 change

                            // If data exists
                            if (wsResult != null & wsResult.Length > 0)
                            {
                                var xmlDoc = new XmlDocument(); // Create new xmlDocument


                                xmlDoc.LoadXml(wsResult); // load information from result
                                var xmlNodeRdr = new XmlNodeReader(xmlDoc); // xmlDoc is your XmlDocument
                                ds.ReadXml(xmlNodeRdr); // Load xlm document to DataTables - 2 Tables ATMStatus and ATM Cassetes

                                //foreach (DataTable dsTable in ds.Tables)
                                //{
                                  
                                    if (ds.Tables.Count <= 0)
                                    {
                                        RecordFound = false;
                                    }
                                    else
                                    {
                                        RecordFound = true;

                                        if (ds.Tables.Count > 0)
                                        {
                                            Table_IST_ATM = ds.Tables[0];

                                            PopulateFieldsTable_IST_ATM();

                                        }

                                        if (ds.Tables.Count > 1)
                                        {
                                            if (ds.Tables[1].Rows.Count > 0)
                                            {
                                                CassettesFound = true;

                                                Table_IST_Cassettes = ds.Tables[1];

                                                PopulateFieldsTable_IST_Cassettes();
                                            }
                                            else
                                            {
                                                CassettesFound = false;
                                            }

                                        }
                                        else
                                        {
                                            CassettesFound = false;
                                        }


                                    }

                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //  conn.Close();

                CatchDetails(ex);
            }

            return ds;
        }
        //
        // Populate Fields for Table_IST_ATM
        //
        public void PopulateFieldsTable_IST_ATM()
        {
            //
            
            // This table contains just one row
            int I = 0;

            while (I <= (Table_IST_ATM.Rows.Count - 1))
            {

                RecordFound = true;

                cassette_cfg_id = (string)Table_IST_ATM.Rows[I]["cassette_cfg_id"];
                cassette_count = (string)Table_IST_ATM.Rows[I]["cassette_count"];
                group_name = (string)Table_IST_ATM.Rows[I]["group_name"];

                location = (string)Table_IST_ATM.Rows[I]["location"];
                make = (string)Table_IST_ATM.Rows[I]["make"]; //  NDC
                model = (string)Table_IST_ATM.Rows[I]["model"]; // 7080

                sdedc = (string)Table_IST_ATM.Rows[I]["sdedc"];
                status = (string)Table_IST_ATM.Rows[I]["status"];
                status_bna = (string)Table_IST_ATM.Rows[I]["status_bna"];

                status_dispense = (string)Table_IST_ATM.Rows[I]["status_dispense"];
                unit = (string)Table_IST_ATM.Rows[I]["unit"];

                I++; // Read Next entry of the table 

            }

        }

        //
        // Populate Fields for Table_IST_Cassettes
        //
        public void PopulateFieldsTable_IST_Cassettes()
        {
            string cash_cur_bal;
            string cassette_number;
            string currency;
            string denomination;
            string reset_time;
            // Init 1
            cash_cur_bal_11 = "";
            cassette_number_11 = "";
            currency_11 = "";
            denomination_11 = "";
            reset_time_11 = "";

            // Init 2
            cash_cur_bal_12 = "";
            cassette_number_12 = "";
            currency_12 = "";
            denomination_12 = "";
            reset_time_12 = "";

            // Init 3
            cash_cur_bal_13 = "";
            cassette_number_13 = "";
            currency_13 = "";
            denomination_13 = "";
            reset_time_13 = "";

            // Init 4
            cash_cur_bal_14 = "";
            cassette_number_14 = "";
            currency_14 = "";
            denomination_14 = "";
            reset_time_14 = "";

            // Init 5
            cash_cur_bal_15 = "";
            cassette_number_15 = "";
            currency_15 = "";
            denomination_15 = "";
            reset_time_15 = "";

            AvailableBalance = 0;
            decimal tempAmt = 0; 

            // This table contains just one row
            int I = 0;

            while (I <= (Table_IST_Cassettes.Rows.Count - 1))
            {

                RecordFound = true;

                // cassettes 
                cash_cur_bal = (string)Table_IST_Cassettes.Rows[I]["cash_cur_bal"];

                cassette_number = (string)Table_IST_Cassettes.Rows[I]["cassette_number"];

                currency = (string)Table_IST_Cassettes.Rows[I]["currency"];
                denomination = (string)Table_IST_Cassettes.Rows[I]["denomination"];
                reset_time = (string)Table_IST_Cassettes.Rows[I]["reset_time"];
                //  string ATMStatus_Id = (string)Table_IST_Cassettes.Rows[I]["ATMStatus_Id"];

                if (cassette_number == "1")
                {
                    cash_cur_bal_11 = cash_cur_bal;
                    cassette_number_11 = cassette_number;
                    currency_11 = currency;
                    denomination_11 = denomination;
                    reset_time_11 = reset_time;
                    
                    if (decimal.TryParse(cash_cur_bal, out tempAmt))
                    {
                        AvailableBalance = AvailableBalance + tempAmt; 
                    }
                }
                if (cassette_number == "2")
                {
                    cash_cur_bal_12 = cash_cur_bal;
                    cassette_number_12 = cassette_number;
                    currency_12 = currency;
                    denomination_12 = denomination;
                    reset_time_12 = reset_time;

                    if (decimal.TryParse(cash_cur_bal, out tempAmt))
                    {
                        AvailableBalance = AvailableBalance + tempAmt;
                    }
                }
                if (cassette_number == "3")
                {
                    cash_cur_bal_13 = cash_cur_bal;
                    cassette_number_13 = cassette_number;
                    currency_13 = currency;
                    denomination_13 = denomination;
                    reset_time_13 = reset_time;

                    if (decimal.TryParse(cash_cur_bal, out tempAmt))
                    {
                        AvailableBalance = AvailableBalance + tempAmt;
                    }
                }
                if (cassette_number == "4")
                {
                    cash_cur_bal_14 = cash_cur_bal;
                    cassette_number_14 = cassette_number;
                    currency_14 = currency;
                    denomination_14 = denomination;
                    reset_time_14 = reset_time;

                    if (decimal.TryParse(cash_cur_bal, out tempAmt))
                    {
                        AvailableBalance = AvailableBalance + tempAmt;
                    }
                }
                if (cassette_number == "5")
                {
                    cash_cur_bal_15 = cash_cur_bal;
                    cassette_number_15 = cassette_number;
                    currency_15 = currency;
                    denomination_15 = denomination;
                    reset_time_15 = reset_time;

                    if (decimal.TryParse(cash_cur_bal, out tempAmt))
                    {
                        AvailableBalance = AvailableBalance + tempAmt;
                    }
                }

                I++; // Read Next entry of the table 

            }

        }


    }
}
