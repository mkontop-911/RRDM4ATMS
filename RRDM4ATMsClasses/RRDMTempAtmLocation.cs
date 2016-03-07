using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{

    public class RRDMTempAtmLocation
    {
        public int SeqNo;

        public string AtmNo;
        public int Mode; // 1: Find new coordinates based on Address
        // 2: Show on Map 
        public string GroupDesc; // This is the header to be used in Google maps display 
        public int GroupNo; // This is the Group id. All ATMs with same group id belongs to the same exception. 

        public string BankId;
        public DateTime DtTmCreated;

        public string Street;
        public string Town;
        public string District;
        public string PostalCode;
        public string Country;
        public string NewStreet;
        public string NewTown;
        public string NewDistrict;
        public string NewPostalCode;
        public string NewCountry;
        public double Latitude;
        public double Longitude;
        public string ColorId;
        public string ColorDesc;
        public DateTime DtTmUpdated;
        public bool AddressChanged;
        public bool LocationFound;

        public struct MapGroupFields
        {
            public int SSeqNo;
            public string SGroupDesc;
            public string SAtmNo;
            public string SStreet;
            public string STown;
            public string SDistrict;
            public string SPostalCode;
            public string SCountry;
            public double SLatitude;
            public double SLongitude;
            public string SColorId;
            public string SColorDesc;
        };

        public MapGroupFields MapGroupFields1; // Declare 

        public ArrayList GroupLocationArray = new ArrayList();

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //
        // READ TempAtmLocation Record for ATM (using AtmNo)
        //
        public void ReadTempAtmLocationSpecific(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[TempAtmLocation]"
               + " WHERE AtmNo = @AtmNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            Mode = (int)rdr["Mode"];

                            GroupNo = (int)rdr["GroupNo"];
                            GroupDesc = (string)rdr["GroupDesc"];

                            AtmNo = (string)rdr["AtmNo"];

                            BankId = (string)rdr["BankId"];
                            DtTmCreated = (DateTime)rdr["DtTmCreated"];

                            Street = (string)rdr["Street"];

                            Town = (string)rdr["Town"];

                            District = (string)rdr["District"];

                            PostalCode = (string)rdr["PostalCode"];

                            Country = (string)rdr["Country"];

                            NewStreet = (string)rdr["NewStreet"];
                            NewTown = (string)rdr["NewTown"];

                            NewDistrict = (string)rdr["NewDistrict"];
                            NewPostalCode = (string)rdr["NewPostalCode"];
                            NewCountry = (string)rdr["NewCountry"];

                            Latitude = (double)rdr["Latitude"];
                            Longitude = (double)rdr["Longitude"];

                            ColorId = (string)rdr["ColorId"];
                            ColorDesc = (string)rdr["ColorDesc"];

                            DtTmUpdated = (DateTime)rdr["DtTmUpdated"];

                            AddressChanged = (bool)rdr["AddressChanged"];
                            LocationFound = (bool)rdr["LocationFound"];

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occurred in TempATMLocation Class............. " + ex.Message;
                }
        }

        //
        // READ TempAtmLocation Record for ATM (using SeqNo)
        //
        public void ReadTempAtmLocationSpecificBySeqNo(Int32 Id)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[TempAtmLocation]"
               + " WHERE SeqNo = @Id ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", Id);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            Mode = (int)rdr["Mode"];

                            GroupNo = (int)rdr["GroupNo"];
                            GroupDesc = (string)rdr["GroupDesc"];

                            AtmNo = (string)rdr["AtmNo"];

                            BankId = (string)rdr["BankId"];
                            DtTmCreated = (DateTime)rdr["DtTmCreated"];


                            Street = (string)rdr["Street"];

                            Town = (string)rdr["Town"];

                            District = (string)rdr["District"];

                            PostalCode = (string)rdr["PostalCode"];
                            Country = (string)rdr["Country"];

                            NewStreet = (string)rdr["NewStreet"];
                            NewTown = (string)rdr["NewTown"];

                            NewDistrict = (string)rdr["NewDistrict"];
                            NewPostalCode = (string)rdr["NewPostalCode"];

                            NewCountry = (string)rdr["NewCountry"];

                            Latitude = (double)rdr["Latitude"];
                            Longitude = (double)rdr["Longitude"];

                            ColorId = (string)rdr["ColorId"];
                            ColorDesc = (string)rdr["ColorDesc"];

                            DtTmUpdated = (DateTime)rdr["DtTmUpdated"];

                            AddressChanged = (bool)rdr["AddressChanged"];
                            LocationFound = (bool)rdr["LocationFound"];

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occurred in TempATMLocation Class............. " + ex.Message;
                }
        }

        //
        // READ TempAtmLocation Records for Group No
        //
        public void ReadTempAtmLocationByGroup(int InGroupNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [dbo].[TempAtmLocation]"
               + " WHERE GroupNo = @GroupNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupNo", InGroupNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            Mode = (int)rdr["Mode"];

                            GroupNo = (int)rdr["GroupNo"];
                            GroupDesc = (string)rdr["GroupDesc"];

                            AtmNo = (string)rdr["AtmNo"];

                            BankId = (string)rdr["BankId"];
                            DtTmCreated = (DateTime)rdr["DtTmCreated"];


                            Street = (string)rdr["Street"];

                            Town = (string)rdr["Town"];

                            District = (string)rdr["District"];
                            PostalCode = (string)rdr["PostalCode"];
                            Country = (string)rdr["Country"];

                            NewStreet = (string)rdr["NewStreet"];
                            NewTown = (string)rdr["NewTown"];

                            NewDistrict = (string)rdr["NewDistrict"];
                            NewPostalCode = (string)rdr["NewPostalCode"];
                            NewCountry = (string)rdr["NewCountry"];

                            Latitude = (double)rdr["Latitude"];
                            Longitude = (double)rdr["Longitude"];
                            ColorId = (string)rdr["ColorId"];
                            ColorDesc = (string)rdr["ColorDesc"];

                            DtTmUpdated = (DateTime)rdr["DtTmUpdated"];

                            AddressChanged = (bool)rdr["AddressChanged"];
                            LocationFound = (bool)rdr["LocationFound"];

                            MapGroupFields1.SSeqNo = SeqNo;
                            MapGroupFields1.SGroupDesc = GroupDesc;
                            MapGroupFields1.SAtmNo = AtmNo;

                            MapGroupFields1.SStreet = AtmNo;
                            MapGroupFields1.STown = Town;
                            MapGroupFields1.SDistrict = District;
                            MapGroupFields1.SPostalCode = PostalCode;

                            MapGroupFields1.SCountry = Country;

                            MapGroupFields1.SLatitude = Latitude;
                            MapGroupFields1.SLongitude = Longitude;
                            MapGroupFields1.SColorId = ColorId;
                            MapGroupFields1.SColorDesc = ColorDesc;

                            GroupLocationArray.Add(MapGroupFields1);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occurred in TempATMLocation Class............. " + ex.Message;
                }
        }

        // Insert TempAtmLocation Record 
        //
        public void InsertTempAtmLocationRecord(string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[TempAtmLocation]"
                    + "([AtmNo],"
                    + " [Mode],  [GroupNo], [GroupDesc], "
                    + " [BankId], [DtTmCreated],  "
                    + " [Street], [Town], [District], [PostalCode], [Country],  "
                    + " [Latitude] , [Longitude], [ColorId] , [ColorDesc])"
                    + " VALUES (@AtmNo,"
                    + " @Mode,  @GroupNo, @GroupDesc,"
                    + " @BankId, @DtTmCreated,"
                    + " @Street, @Town, @District, @PostalCode, @Country, "
                    + " @Latitude, @Longitude, @ColorId , @ColorDesc )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@Mode", Mode);

                        cmd.Parameters.AddWithValue("@GroupNo", GroupNo);
                        cmd.Parameters.AddWithValue("@GroupDesc", GroupDesc);

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@DtTmCreated", DtTmCreated);

                        cmd.Parameters.AddWithValue("@Street", Street);
                        cmd.Parameters.AddWithValue("@Town", Town);
                        cmd.Parameters.AddWithValue("@District", District);

                        cmd.Parameters.AddWithValue("@PostalCode", PostalCode);

                        cmd.Parameters.AddWithValue("@Country", Country);

                        cmd.Parameters.AddWithValue("@Latitude", Latitude);
                        cmd.Parameters.AddWithValue("@Longitude", Longitude);

                        cmd.Parameters.AddWithValue("@ColorId", ColorId);
                        cmd.Parameters.AddWithValue("@ColorDesc", ColorDesc);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occurred in TempATMLocation  Class ... " + ex.Message;
                }
        }
        //
        // UPDATE TempAtmLocation Record 
        // 
        public void UpdateTempAtmLocationRecord(string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[TempAtmLocation] SET "
                            + " NewStreet = @NewStreet, NewTown = @NewTown, "
                            + " NewDistrict = @NewDistrict, NewPostalCode = @NewPostalCode, NewCountry = @NewCountry, "
                            + " Latitude = @Latitude, Longitude = @Longitude,"
                            + " DtTmUpdated = @DtTmUpdated, AddressChanged = @AddressChanged,"
                            + " LocationFound = @LocationFound "
                            + " WHERE AtmNo = @AtmNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@NewStreet", NewStreet);
                        cmd.Parameters.AddWithValue("@NewTown", NewTown);
                        cmd.Parameters.AddWithValue("@NewDistrict", NewDistrict);

                        cmd.Parameters.AddWithValue("@NewPostalCode", NewPostalCode);

                        cmd.Parameters.AddWithValue("@NewCountry", NewCountry);

                        cmd.Parameters.AddWithValue("@Latitude", Latitude);

                        cmd.Parameters.AddWithValue("@Longitude", Longitude);

                        cmd.Parameters.AddWithValue("@DtTmUpdated", DtTmUpdated);
                        cmd.Parameters.AddWithValue("@AddressChanged", AddressChanged);

                        cmd.Parameters.AddWithValue("@LocationFound", LocationFound);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occurred in TempAtmLocation Class............. " + ex.Message;
                }
        }

        public void UpdateTempAtmLocationRecordBySeqNo(int SeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[TempAtmLocation] SET "
                            + " NewStreet = @NewStreet, NewTown = @NewTown, "
                            + " NewDistrict = @NewDistrict , NewPostalCode = @NewPostalCode, NewCountry = @NewCountry, "
                            + " Latitude = @Latitude, Longitude = @Longitude,"
                            + " DtTmUpdated = @DtTmUpdated, AddressChanged = @AddressChanged,"
                            + " LocationFound = @LocationFound "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);

                        cmd.Parameters.AddWithValue("@NewStreet", NewStreet);
                        cmd.Parameters.AddWithValue("@NewTown", NewTown);
                        cmd.Parameters.AddWithValue("@NewDistrict", NewDistrict);

                        cmd.Parameters.AddWithValue("@NewPostalCode", NewPostalCode);

                        cmd.Parameters.AddWithValue("@NewCountry", NewCountry);

                        cmd.Parameters.AddWithValue("@Latitude", Latitude);

                        cmd.Parameters.AddWithValue("@Longitude", Longitude);

                        cmd.Parameters.AddWithValue("@DtTmUpdated", DtTmUpdated);
                        cmd.Parameters.AddWithValue("@AddressChanged", AddressChanged);

                        cmd.Parameters.AddWithValue("@LocationFound", LocationFound);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occurred in TempAtmLocation Class .. " + ex.Message;
                }
        }

        //
        // DELETE Authorization record 
        //
        public void DeleteTempAtmLocationRecord(string InAtmNo, int InMode)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[TempAtmLocation] "
                            + " WHERE AtmNo =  @AtmNo AND Mode =  @Mode ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Mode", InMode);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }

                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occurred in TempAtmLocation Class............. " + ex.Message;
                }

        }

        //
        // DELETE GROUP
        //
        public void DeleteTempAtmLocationGroup(int InGroupNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[TempAtmLocation] "
                            + " WHERE GroupNo =  @GroupNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupNo", InGroupNo);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }

                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occurred in TempAtmLocation Class............. " + ex.Message;
                }

        }
        //
        // READ LAST Record Insert 
        //
        public void FindTempAtmLocationLastNo(string InAtmNo, int InMode)
        {
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * FROM [ATMS].[dbo].[TempAtmLocation]"
                   + " WHERE SeqNo = (SELECT MAX(SeqNo) FROM [ATMS].[dbo].[TempAtmLocation])"
                   + " AND AtmNo = @AtmNo AND Mode = @Mode ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@Mode", InMode);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occurred in TempATMLocation Class............. " + ex.Message;
                }
        }

    }
    // -------------------------
    /* ATM Basic */
    public class ATMBasic
    {
        public Int32 ATMId { get; set; }
        public string ATMNumber { get; set; }
        public string ATMColorId { get; set; }
    }

    /* ATM Details */
    public class ATMDetails : ATMBasic
    {
        public string ATMStreet { get; set; }
        public string ATMTown { get; set; }
        public string ATMPostalCode { get; set; }
        public string ATMDistrict { get; set; }
        public string ATMCountry { get; set; }
        public double ATMLat { get; set; }
        public double ATMLon { get; set; }

        public int ATMGroupNo { get; set; }
        public string ATMGroupDesc { get; set; }
        public string ATMColorDesc { get; set; }
    }

    /* ATM Districts */
    public class ATMDistrict
    {
        public string District { get; set; }
    }

    /* ATM GeoLocations, returned by Coogle Maps API */
    public class ATMGeo
    {
        public int Id { get; set; } // Set by the reading routing
        public string FormattedAddress { get; set; }
        public string AddressType { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string LocationType { get; set; }
    }


    public class AtmDataAccess
    {
        /* Get List of ATMs  */
        public static List<ATMDetails> GetListofATMs()
        {
            List<ATMDetails> ListATMs = new List<ATMDetails>();
            string CS = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                try
                {
                    SqlCommand sqlcmd = new SqlCommand(" SELECT * FROM TempAtmLocation", con);
                    con.Open();
                    SqlDataReader rdr = sqlcmd.ExecuteReader();
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
                        Rec.ATMDistrict = rdr["District"].ToString();
                        Rec.ATMCountry = rdr["Country"].ToString();
                        Rec.ATMLat = Convert.ToDouble(rdr["Latitude"]);
                        Rec.ATMLon = Convert.ToDouble(rdr["Longitude"]);

                        ListATMs.Add(Rec);
                    }
                }
                catch (Exception ex)
                {
                    //// To Investigate...
                    string strError;
                    strError = ex.ToString();
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} : {1}", DateTime.Now, strError));
#endif
                    return (null);
                }
            }
            return (ListATMs);
        }

        /* Get specific ATM details */
        public static ATMDetails GetATMDetails(Int32 Id)
        {
            ATMDetails Rec = null;
            string CS = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                try
                {
                    SqlCommand sqlcmd = new SqlCommand(" SELECT * FROM TempAtmLocation WHERE SeqNo = @Id", con);
                    SqlParameter sqlparam = new SqlParameter();
                    sqlparam.ParameterName = "@Id";
                    sqlparam.Value = Id;
                    sqlcmd.Parameters.Add(sqlparam);
                    con.Open();
                    SqlDataReader rdr = sqlcmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Rec = new ATMDetails();

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
                    }
                }
                catch (Exception ex)
                {
                    //// To Investigate...
                    string strError;
                    strError = ex.ToString();
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} : {1}", DateTime.Now, strError));
#endif
                    return (null);
                }
            }
            return (Rec);
        }

        /* Get List of ATM Districts */
        public static List<ATMDistrict> GetListofDistricts()
        {
            List<ATMDistrict> ListDistricts = new List<ATMDistrict>();
            string CS = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                try
                {
                    SqlCommand sqlcmd = new SqlCommand(" SELECT DISTINCT District FROM TempAtmLocation", con);
                    con.Open();
                    SqlDataReader rdr = sqlcmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ATMDistrict Distr = new ATMDistrict();
                        Distr.District = rdr["District"].ToString();
                        ListDistricts.Add(Distr);
                    }
                }
                catch (Exception ex)
                {
                    //// To Investigate...
                    string strError;
                    strError = ex.ToString();
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} : {1}", DateTime.Now, strError));
#endif
                    return (null);
                }
            }
            return (ListDistricts);
        }


    }

    // --------------------------
     
}
