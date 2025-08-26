using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
//using System.Collections;


namespace RRDM4ATMs
{
    public class RRDMITMXFeesVersionLayers : Logger
    {
        public RRDMITMXFeesVersionLayers() : base() { }

        public int SeqNo;

        public string Product;
        public string VersionId;
        public string LayerName;

        public int FromAmount;
        public int ToAmount;

        public string Ccy;

        public decimal TotalFees;

        public string EntityA;
        public decimal FeesEntityA;
        public int FeesEntityPercA;

        public string EntityB;
        public decimal FeesEntityB;
        public int FeesEntityPercB;

        public string EntityC;
        public decimal FeesEntityC;
        public int FeesEntityPercC;

        public string EntityD;
        public decimal FeesEntityD;
        public int FeesEntityPercD;

        public string EntityE;
        public decimal FeesEntityE;
        public int FeesEntityPercE;

        public string Operator;

        public bool RecordFound;
        public bool Successful;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data tables 
        public DataTable TableFeesLayers = new DataTable();

        public int TotalSelected;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        //
        // Methods 
        // READ FeesLayers 
        // FILL UP A TABLE
        //

        public bool CorrectSequence;
        public void ReadFeesLayersAndFeelTable(string InOperator, string InSignedId, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableFeesLayers = new DataTable();
            TableFeesLayers.Clear();

            int PreviousFromAmount = 0;
            int PreviousToAmount = 0;

            CorrectSequence = true;

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableFeesLayers.Columns.Add("SeqNo", typeof(int));
            TableFeesLayers.Columns.Add("LayerName", typeof(string));
            TableFeesLayers.Columns.Add("FromAmount", typeof(string));
            TableFeesLayers.Columns.Add("ToAmount", typeof(string));
            TableFeesLayers.Columns.Add("TotalFees", typeof(decimal));


            SqlString = "SELECT *"
                   + " FROM [ATMS].[dbo].[ITMXFeesLayers] "
                   + InSelectionCriteria
                   + " ORDER BY FromAmount ASC ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //  cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            Product = (string)rdr["Product"];

                            VersionId = (string)rdr["VersionId"];

                            LayerName = (string)rdr["LayerName"];

                            FromAmount = (int)rdr["FromAmount"];
                            ToAmount = (int)rdr["ToAmount"];

                            if (FromAmount != PreviousToAmount)
                            {
                                CorrectSequence = false;
                            }

                            PreviousFromAmount = FromAmount;
                            PreviousToAmount = ToAmount;

                            Ccy = (string)rdr["Ccy"];

                            TotalFees = (decimal)rdr["TotalFees"];

                            EntityA = (string)rdr["EntityA"];
                            FeesEntityA = (decimal)rdr["FeesEntityA"];
                            FeesEntityPercA = (int)rdr["FeesEntityPercA"];

                            EntityB = (string)rdr["EntityB"];
                            FeesEntityB = (decimal)rdr["FeesEntityB"];
                            FeesEntityPercB = (int)rdr["FeesEntityPercB"];

                            EntityC = (string)rdr["EntityC"];
                            FeesEntityC = (decimal)rdr["FeesEntityC"];
                            FeesEntityPercC = (int)rdr["FeesEntityPercC"];

                            EntityD = (string)rdr["EntityD"];
                            FeesEntityD = (decimal)rdr["FeesEntityD"];
                            FeesEntityPercD = (int)rdr["FeesEntityPercD"];

                            EntityE = (string)rdr["EntityE"];
                            FeesEntityE = (decimal)rdr["FeesEntityE"];
                            FeesEntityPercE = (int)rdr["FeesEntityPercE"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableFeesLayers.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["LayerName"] = LayerName;
                            RowSelected["FromAmount"] = FromAmount.ToString("#,##0");
                            RowSelected["ToAmount"] = ToAmount.ToString("#,##0");
                            RowSelected["TotalFees"] = TotalFees;


                            // ADD ROW
                            TableFeesLayers.Rows.Add(RowSelected);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    //ReadTable And Insert In Sql Table 
                    RRDMTempTablesForReportsITMX Tr = new RRDMTempTablesForReportsITMX();

                    //Clear Table 
                    Tr.DeleteReport43(InSignedId);

                    int I = 0;

                    while (I <= (TableFeesLayers.Rows.Count - 1))
                    {

                        RecordFound = true;

                        int TempSeqNo = (int)TableFeesLayers.Rows[I]["SeqNo"];

                        Tr.LayerName = (string)TableFeesLayers.Rows[I]["LayerName"];
                        Tr.FromAmount = (string)TableFeesLayers.Rows[I]["FromAmount"];
                        Tr.ToAmount = (string)TableFeesLayers.Rows[I]["ToAmount"];

                        Tr.TotalFees = (decimal)TableFeesLayers.Rows[I]["TotalFees"];

                        ReadFeesLayersbySeqNo(InOperator, TempSeqNo); 


                        Tr.EntityA = EntityA;
                        Tr.FeesEntityA = FeesEntityA;

                        Tr.EntityB = EntityB;
                        Tr.FeesEntityB = FeesEntityB;

                        Tr.EntityC = EntityC;
                        Tr.FeesEntityC = FeesEntityC;

                        Tr.InsertReport43(InSignedId);

                        I++; // Read Next entry of the table 
                    }
                }
                catch (Exception ex)
                {

                    conn.Close();

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ FeesLayers  by Seq no  
        // 
        //
        public void ReadFeesLayersbySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXFeesLayers] "
                    + " WHERE Operator = @Operator AND SeqNo = @SeqNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            Product = (string)rdr["Product"];

                            VersionId = (string)rdr["VersionId"];

                            LayerName = (string)rdr["LayerName"];

                            FromAmount = (int)rdr["FromAmount"];
                            ToAmount = (int)rdr["ToAmount"];

                            Ccy = (string)rdr["Ccy"];

                            TotalFees = (decimal)rdr["TotalFees"];

                            EntityA = (string)rdr["EntityA"];
                            FeesEntityA = (decimal)rdr["FeesEntityA"];
                            FeesEntityPercA = (int)rdr["FeesEntityPercA"];

                            EntityB = (string)rdr["EntityB"];
                            FeesEntityB = (decimal)rdr["FeesEntityB"];
                            FeesEntityPercB = (int)rdr["FeesEntityPercB"];

                            EntityC = (string)rdr["EntityC"];
                            FeesEntityC = (decimal)rdr["FeesEntityC"];
                            FeesEntityPercC = (int)rdr["FeesEntityPercC"];

                            EntityD = (string)rdr["EntityD"];
                            FeesEntityD = (decimal)rdr["FeesEntityD"];
                            FeesEntityPercD = (int)rdr["FeesEntityPercD"];

                            EntityE = (string)rdr["EntityE"];
                            FeesEntityE = (decimal)rdr["FeesEntityE"];
                            FeesEntityPercE = (int)rdr["FeesEntityPercE"];

                            Operator = (string)rdr["Operator"];

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

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // Find Position In Grid
        // 
        //
        public int PositionInGrid;
        public void ReadFeesLayersToFindPositionOfSeqNo(string InOperator, string InProduct , string InVersionId, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            PositionInGrid = -1;

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXFeesLayers] "
                    + " WHERE Operator = @Operator AND Product = @Product AND VersionId = @VersionId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Product", InProduct);
                        cmd.Parameters.AddWithValue("@VersionId", InVersionId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            PositionInGrid = PositionInGrid + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            if (SeqNo == InSeqNo)
                            {
                                break;
                            }

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

                    CatchDetails(ex);

                }
        }

        // Insert FeesLayer
        //
        public int InsertFeesLayer()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ITMXFeesLayers] "
                    + "([Product], [VersionId], [LayerName],  "
                    + " [FromAmount], [ToAmount], [Ccy],"
                    + " [TotalFees], "
                    + " [EntityA], [FeesEntityA], [FeesEntityPercA],"
                    + " [EntityB], [FeesEntityB], [FeesEntityPercB],"
                    + " [EntityC], [FeesEntityC], [FeesEntityPercC],"
                    + " [EntityD], [FeesEntityD], [FeesEntityPercD],"
                    + " [EntityE], [FeesEntityE], [FeesEntityPercE],"
                    + " [Operator] )"
                    + " VALUES (@Product, @VersionId, @LayerName,"
                    + " @FromAmount, @ToAmount, @Ccy,"
                    + " @TotalFees, "
                    + " @EntityA, @FeesEntityA, @FeesEntityPercA,"
                    + " @EntityB, @FeesEntityB, @FeesEntityPercB,"
                    + " @EntityC, @FeesEntityC, @FeesEntityPercC,"
                    + " @EntityD, @FeesEntityD, @FeesEntityPercD,"
                    + " @EntityE, @FeesEntityE, @FeesEntityPercE,"
                    + " @Operator )"
                    +" SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@VersionId", VersionId);
                        cmd.Parameters.AddWithValue("@LayerName", LayerName);

                        cmd.Parameters.AddWithValue("@FromAmount", FromAmount);
                        cmd.Parameters.AddWithValue("@ToAmount", ToAmount);

                        cmd.Parameters.AddWithValue("@Ccy", Ccy);

                        cmd.Parameters.AddWithValue("@TotalFees", TotalFees);

                        cmd.Parameters.AddWithValue("@EntityA", EntityA);
                        cmd.Parameters.AddWithValue("@FeesEntityA", FeesEntityA);
                        cmd.Parameters.AddWithValue("@FeesEntityPercA", FeesEntityPercA);

                        cmd.Parameters.AddWithValue("@EntityB", EntityB);
                        cmd.Parameters.AddWithValue("@FeesEntityB", FeesEntityB);
                        cmd.Parameters.AddWithValue("@FeesEntityPercB", FeesEntityPercB);

                        cmd.Parameters.AddWithValue("@EntityC", EntityC);
                        cmd.Parameters.AddWithValue("@FeesEntityC", FeesEntityC);
                        cmd.Parameters.AddWithValue("@FeesEntityPercC", FeesEntityPercC);

                        cmd.Parameters.AddWithValue("@EntityD", EntityD);
                        cmd.Parameters.AddWithValue("@FeesEntityD", FeesEntityD);
                        cmd.Parameters.AddWithValue("@FeesEntityPercD", FeesEntityPercD);

                        cmd.Parameters.AddWithValue("@EntityE", EntityE);
                        cmd.Parameters.AddWithValue("@FeesEntityE", FeesEntityE);
                        cmd.Parameters.AddWithValue("@FeesEntityPercE", FeesEntityPercE);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        SeqNo = (int)cmd.ExecuteScalar();
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
            return SeqNo;
        }

        // UPDATE FeesLayer
        // 
        public void UpdateFeesLayer(int InSeqNo)
        {
            Successful = false;
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ITMXFeesLayers] SET "
                            + " Product = @Product, VersionId = @VersionId, LayerName = @LayerName, "
                            + " FromAmount = @FromAmount, ToAmount = @ToAmount, "
                            + " Ccy = @Ccy, "
                            + " TotalFees = @TotalFees,"
                            + " EntityA = @EntityA, FeesEntityA = @FeesEntityA , FeesEntityPercA = @FeesEntityPercA ,"
                            + " EntityB = @EntityB, FeesEntityB = @FeesEntityB , FeesEntityPercB = @FeesEntityPercB ,"
                            + " EntityC = @EntityC, FeesEntityC = @FeesEntityC , FeesEntityPercC = @FeesEntityPercC ,"
                            + " EntityD = @EntityD, FeesEntityD = @FeesEntityD , FeesEntityPercD = @FeesEntityPercD ,"
                            + " EntityE = @EntityE, FeesEntityE = @FeesEntityE , FeesEntityPercE = @FeesEntityPercE "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@VersionId", VersionId);
                        cmd.Parameters.AddWithValue("@LayerName", LayerName);

                        cmd.Parameters.AddWithValue("@FromAmount", FromAmount);
                        cmd.Parameters.AddWithValue("@ToAmount", ToAmount);

                        cmd.Parameters.AddWithValue("@Ccy", Ccy);

                        cmd.Parameters.AddWithValue("@TotalFees", TotalFees);

                        cmd.Parameters.AddWithValue("@EntityA", EntityA);
                        cmd.Parameters.AddWithValue("@FeesEntityA", FeesEntityA);
                        cmd.Parameters.AddWithValue("@FeesEntityPercA", FeesEntityPercA);

                        cmd.Parameters.AddWithValue("@EntityB", EntityB);
                        cmd.Parameters.AddWithValue("@FeesEntityB", FeesEntityB);
                        cmd.Parameters.AddWithValue("@FeesEntityPercB", FeesEntityPercB);

                        cmd.Parameters.AddWithValue("@EntityC", EntityC);
                        cmd.Parameters.AddWithValue("@FeesEntityC", FeesEntityC);
                        cmd.Parameters.AddWithValue("@FeesEntityPercC", FeesEntityPercC);

                        cmd.Parameters.AddWithValue("@EntityD", EntityD);
                        cmd.Parameters.AddWithValue("@FeesEntityD", FeesEntityD);
                        cmd.Parameters.AddWithValue("@FeesEntityPercD", FeesEntityPercD);

                        cmd.Parameters.AddWithValue("@EntityE", EntityE);
                        cmd.Parameters.AddWithValue("@FeesEntityE", FeesEntityE);
                        cmd.Parameters.AddWithValue("@FeesEntityPercE", FeesEntityPercE);

                       
                        cmd.ExecuteNonQuery();
                        

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }

        //
        // DELETE FeesLayer
        //
        public void DeleteFeesLayer(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ITMXFeesLayers] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                       
                      cmd.ExecuteNonQuery();
                     

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

        }
        //
        // DELETE FeesLayers for this Version
        //
        public void DeleteFeesLayerForThisVersion(string InVersionId)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ITMXFeesLayers] "
                            + " WHERE VersionId =  @VersionId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@VersionId", InVersionId);

                       
                        cmd.ExecuteNonQuery();
                        

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

        }
    }
}
