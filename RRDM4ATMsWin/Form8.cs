using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Drawing;
using System.Configuration;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form8 : Form
    {
        Form16 NForm16; 

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;    

        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        //string ParCriticalProcess;
        DateTime WDtTm;

        string WSignedId;
        string WCriticalProcess; 
        string WOperator;
  
        int WAction;

        public Form8(string InSignedId, string InOperator,string InCriticalProcess, int InAction)
        {
            WSignedId = InSignedId;
          
            WOperator = InOperator;

            WCriticalProcess = InCriticalProcess; 

            WAction = InAction;  // 1 = System Performance 
           
            InitializeComponent();

            // Set Working Date 
             
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            dateTimePicker1.Value = DateTime.Now.AddDays(-10);

            // Critical Process 
            Gp.ParamId = "351"; 
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox2.DisplayMember = "DisplayValue";

            textBoxMsgBoard.Text = " Make your selection ";

            if (WCriticalProcess !="")
            {
                comboBox2.Text = WCriticalProcess;
                textBoxMsgBoard.Text = " Make dates selection ";
                labelStep1.Text = "Performance analysis for :.." + comboBox2.Text;
            }

            labelHeader.Hide();
            panel2.Hide(); 

        }
        // On Load form 
        private void Form8_Load(object sender, EventArgs e)
        {
           // Not needed for now 
           // we are using SHOW
        }
        // SHOW 
        private void button2_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("Select valid dates.");
                return; 
            }
            labelStep1.Text = "Performance analysis for :.." + comboBox2.Text;

            labelHeader.Text = "ANALYSIS PER CUT OFF CYCLE"; 

            WCriticalProcess = comboBox2.Text;

            Pt.ReadPerformanceTraceAndFillTableForPerformance_2(WOperator, comboBox2.Text
                , dateTimePicker1.Value, dateTimePicker2.Value);

            if (Pt.TablePerformance.Rows.Count == 0)
            {
                MessageBox.Show("No data to show.");
                return;
            }
            dataGridView1.DataSource = Pt.TablePerformance.DefaultView;

            // System performance For Model ATM AB104
            // 
            if (WAction == 1) //  
            {
                labelHeader.Show();
                panel2.Show();

                ShowGrid(); 

                // Set chart data source  
                chart1.DataSource = Pt.TablePerformance.DefaultView;
                chart1.Series[0].Name = "Per Minute Efficiency by Date";

                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "RMCycleNo";
                chart1.Series[0].YValueMembers = "Duration_Min";

                // Data bind to the selected data source  
                chart1.DataBind();



            }

        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //WDtTm = (DateTime)rowSelected.Cells[0].Value;
        }

        // Show Grid 2
        private void ShowGrid()
        {
            
            dataGridView1.Show();

            if (dataGridView1.Rows.Count == 0)
            {
                Form2 MessageForm = new Form2("No Data to show.");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }
            //" SELECT RMCycleNo, Details , "
            //           + " CAST( EndDT AS Date) As Entry_Date, "
            //          + " CAST(Duration_Sec as Decimal(12, 2))/ 60 As Duration_Min "

            dataGridView1.Columns[0].Width = 60; // RMCycleNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridViewMyATMS.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 190; // Details
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 80; // Entry_Date
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 90; // Duration_Min
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            //dataGridViewMyATMS.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridViewMyATMS.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;

        }

        // Double Click on Row - EXPAND LINE
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            NForm16 = new Form16("", WCriticalProcess, WDtTm);
            NForm16.Show();

        }
        // EXPAND LINE 
        private void button1_Click(object sender, EventArgs e)
        {
            NForm16 = new Form16("", WCriticalProcess, WDtTm);
            NForm16.Show();
        }

        // Create Excel
        private void buttonExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();
            string Id = "";
            string ExcelPath = "";
            string WorkingDir = "C:\\RRDM\\Working\\";
           
                Id = "Performance_"+comboBox2.Text+"_"+ DateTime.Now.Date.ToString("yyyyMMdd");
                ExcelPath = "C:\\RRDM\\Working\\" + Id + ".xlsx";
                XL.ExportToExcel(Pt.TablePerformance, WorkingDir, ExcelPath);
           
        }
        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

    }
}
