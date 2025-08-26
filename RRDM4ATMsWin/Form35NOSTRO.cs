using System.Data;
using System.Windows.Forms;
using System.Configuration;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form35NOSTRO : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings
             ["ATMSConnectionString"].ConnectionString;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
     
        DataTable WTable = new DataTable();
        string WHeading;
        int WAction;

        public Form35NOSTRO(string InSignedId, int InSignRecordNo, string InOperator,  
            DataTable  InTable, string InHeading, int InAction )
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
       
            WTable = InTable;
            WHeading = InHeading; 
            WAction = InAction;

            InitializeComponent();
       
            if (WAction == 1) // 
            {
                label14.Text = WHeading;
                // Set chart2 data source  
                chart2.DataSource = WTable.DefaultView;
                chart2.Series[0].Name = "UnMatchedNo";
                chart2.Series.Add("OutstandingAlerts");
                chart2.Series.Add("OutstandingDisputes");
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "Date";
                chart2.Series[0].YValueMembers = "UnMatchedNo";
                chart2.Series[1].XValueMember = "Date";
                chart2.Series[1].YValueMembers = "OutstandingAlerts";
                chart2.Series[2].XValueMember = "Date";
                chart2.Series[2].YValueMembers = "OutstandingDisputes";

                // Data bind to the selected data source  
                chart2.DataBind();
            }

            if (WAction == 2) // Users
            {
                label14.Text = WHeading;
                // Set chart2 data source  
                chart2.DataSource = WTable.DefaultView;
                chart2.Series[0].Name = "UnMatchedNo";
                chart2.Series.Add("OutstandingDisputes");
                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "UserId";
                chart2.Series[0].YValueMembers = "UnMatchedNo";
                chart2.Series[1].XValueMember = "UserId";
                chart2.Series[1].YValueMembers = "OutstandingDisputes";

                // Data bind to the selected data source  
                chart2.DataBind();
            }

            if (WAction == 3) // 
            {
                label14.Text = WHeading;
                // Set chart2 data source  
                chart2.DataSource = WTable.DefaultView;
                chart2.Series[0].Name = "TotalNumberProcessed";
                chart2.Series.Add("MatchedDefault");

                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "Bank";
                chart2.Series[0].YValueMembers = "TotalNumberProcessed";
                chart2.Series[1].XValueMember = "Bank";
                chart2.Series[1].YValueMembers = "MatchedDefault";

                // Data bind to the selected data source  
                chart2.DataBind();
            }

            if (WAction == 4) // 
            {
                label14.Text = WHeading;
                // Set chart2 data source  
                chart2.DataSource = WTable.DefaultView;
                chart2.Series[0].Name = "GrandTotal";
                //chart2.Series.Add("MatchedDefault");

                // Set series members names for the X and Y values  
                chart2.Series[0].XValueMember = "PairID";
                chart2.Series[0].YValueMembers = "GrandTotal";
            }
         
        }
    }
}
