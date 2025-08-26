using System.Data;
using System.Windows.Forms;
using System.Configuration;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form35ITMX : Form
    {    

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
     
        DataTable WTable = new DataTable();
        string WHeading;
        int WAction;
        int WGraph;
        int WPeriod; 

        public Form35ITMX(string InSignedId, int InSignRecordNo, string InOperator,  
            DataTable  InTable, string InHeading, int InAction, int InGraph, int InPeriod )
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
       
            WTable = InTable;
            WHeading = InHeading; 
            WAction = InAction;
            WGraph = InGraph;
            WPeriod = InPeriod; // 1 = daily , 2 = monthly 

            InitializeComponent();
           
            if (WPeriod == 1)
            {
                ShowChartsDaily(WOperator); 
            }

            if (WPeriod == 2)
            {
                ShowChartsMonthly(WOperator);
            }
        }

        //******************
        // SHOW CHARTS DAILY
        //******************
       
        private void ShowChartsDaily(string Operator)
        {

            if (WGraph == 1 )
            {
                labelChartHeading.Text = WHeading;
                // Set chart1  data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "TNXS";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "TNXS";
            }

            if (WGraph == 2)
            {
                labelChartHeading.Text = WHeading;
                // Set chart1  data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "Amount";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "Amount";

                chart1.DataBind();

                chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Chocolate;

            }

            if (WGraph == 3)
            {
                labelChartHeading.Text = WHeading;
                // Set chart3 data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "UnMatched";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "UnMatched";

                // Data bind to the selected data source  
                chart1.DataBind();

                chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            }

            if (WGraph == 4)
            {
                labelChartHeading.Text = WHeading;
                // Set chart4 data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "Fees";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "Fees";

                // Data bind to the selected data source  
                chart1.DataBind();

                chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
            }
            
        }

        //******************
        // SHOW CHARTS Monthly
        //******************

        private void ShowChartsMonthly(string Operator)
        {

            if (WGraph == 1)
            {
                labelChartHeading.Text = WHeading;
                // Set chart1  data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "TNXS";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "perMonth";
                chart1.Series[0].YValueMembers = "TNXS";
            }

            if (WGraph == 2)
            {
                labelChartHeading.Text = WHeading;
                // Set chart1  data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "Amount";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "perMonth";
                chart1.Series[0].YValueMembers = "Amount";

                chart1.DataBind();

                chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Chocolate;
            }

            if (WGraph == 3)
            {
                labelChartHeading.Text = WHeading;
                // Set chart3 data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "UnMatched";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "perMonth";
                chart1.Series[0].YValueMembers = "UnMatched";

                // Data bind to the selected data source  
                chart1.DataBind();

                chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            }

            if (WGraph == 4)
            {
                labelChartHeading.Text = WHeading;
                // Set chart4 data source  
                chart1.DataSource = WTable.DefaultView;
                chart1.Series[0].Name = "Fees";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "perMonth";
                chart1.Series[0].YValueMembers = "Fees";

                // Data bind to the selected data source  
                chart1.DataBind();

                chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel;
            }
        }
    }
}
