using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form503Fees : Form
    {
        RRDMITMXFeesVersions FV = new RRDMITMXFeesVersions();

        RRDMITMXFeesVersionLayers FL = new RRDMITMXFeesVersionLayers();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMBanks Ba = new RRDMBanks();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMCaseNotes Cn = new RRDMCaseNotes();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool ViewWorkFlow;
        string WNotesMode;
        // NOTES 
        string Order;

        string WParameter4;
        string WSearchP4;

        //bool FirstProductLoad;
        int WCurrentVersionSeqNo; 
        string WCurrentVersionId; 

        string WProduct;
        string WVersionId;
        string SelectionCriteria;

        int WSeqNoVersion;
        int WSeqNoLayer;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WMode;

        public Form503Fees(string InSignedId, int SignRecordNo, string InOperator, string InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WMode = InMode; // = Configure, View

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = WSignedId;

            Us.ReadUsersRecord(WSignedId);

            Gp.ParamId = "201"; // Currencies  
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            Gp.ParamId = "752"; // Products 
            comboBoxFeesProducts.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxFeesProducts.DisplayMember = "DisplayValue";

            Gp.ParamId = "751"; // Fees Entities A
            comboBoxEntities.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxEntities.DisplayMember = "DisplayValue";

            Gp.ParamId = "751"; // Fees Entities B
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox2.DisplayMember = "DisplayValue";

            Gp.ParamId = "751"; // Fees Entities C
            comboBox3.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox3.DisplayMember = "DisplayValue";

            if (WMode == "View")
            {
                checkBoxMakeNewVersion.Hide();
                checkBoxMakeNewLayer.Hide();
                buttonAdd.Hide();
                buttonAddVersion.Hide();
                buttonUpdate.Hide();
                buttonUpdateVersion.Hide();
                buttonDelete.Hide();
                buttonDeleteVersion.Hide();

                textBoxMsgBoard.Text = "VIEW";

                ViewWorkFlow = true; 
            }
            else
            {
                ViewWorkFlow = false;
            }
          
            // ADD for Version
            buttonAddVersion.Hide();
            buttonAdd.Hide();
        }

        // On Load
        private void Form503FeesNEW_Load(object sender, EventArgs e)
        {
            WProduct = comboBoxFeesProducts.Text;
            SelectionCriteria = " WHERE Operator ='" + WOperator
              + "' AND Product ='" + WProduct + "'";

            FV.ReadFeesVersionsAndFeelTable(WOperator, SelectionCriteria);

            if (FV.RecordFound == false)
            {
                //MessageBox.Show("No Versions for this product");
                dataGridView1.DataSource = FV.TableFeesVersions.DefaultView;
                checkBoxMakeNewVersion.Checked = true;
                dateTimePicker1.Enabled = true;
                dateTimePicker2.Enabled = true;
                dateTimePicker1.Value = DateTime.Today;
                dateTimePicker2.Value = DateTime.Today;
                return; 
            }

            dataGridView1.DataSource = FV.TableFeesVersions.DefaultView;

            dataGridView1.Columns[0].Width = 50; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Visible = false; //Product

            dataGridView1.Columns[2].Width = 80; // VersionId
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Visible = false; //Description

            dataGridView1.Columns[4].Width = 100; // VersionId
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            if (LoadCounter>0)
             {
            FV.ReadFeesVersionToFindPositionOfCurrentVersion(WOperator, WProduct);

            if (FV.RecordFound == true & FV.CurrentVersionFound == true)
            {
                textBoxMsgBoard.Text = "Current Version is : " + FV.VersionId;

                WCurrentVersionSeqNo = FV.SeqNo;
                WCurrentVersionId = FV.VersionId;

                dataGridView1.Rows[FV.PositionInGrid].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, FV.PositionInGrid));
            }
            else
            {
                textBoxMsgBoard.Text = "There is no current Version";
            }
                LoadCounter = LoadCounter -1 ; 
            }
            //TableFeesVersions.Columns.Add("SeqNo", typeof(int));
            //TableFeesVersions.Columns.Add("Product", typeof(string));
            //TableFeesVersions.Columns.Add("VersionId", typeof(string));
            //TableFeesVersions.Columns.Add("Description", typeof(string));
            //TableFeesVersions.Columns.Add("FromDate", typeof(DateTime));
        }
        //On Row Enter
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNoVersion = (int)rowSelected.Cells[0].Value;

            FV.ReadFeesVersionToFindPositionOfCurrentVersion(WOperator, WProduct);

            textBoxMsgBoard.Text = "Current Version is : " + FV.VersionId;

            WCurrentVersionSeqNo = FV.SeqNo;
            WCurrentVersionId = FV.VersionId;

            //if (WSeqNoVersion < WCurrentVersionSeqNo)
            //{
            //    checkBoxMakeNewVersion.Hide();
            //    checkBoxMakeNewLayer.Hide();
            //    buttonUpdateVersion.Hide();
            //    buttonUpdate.Hide();
            //    buttonDeleteVersion.Hide();
            //    buttonDelete.Hide();
            //}
            //else
            //{
            //    if (WMode != "View")
            //    {
            //        checkBoxMakeNewVersion.Show();
            //        checkBoxMakeNewLayer.Show();
            //        buttonUpdateVersion.Show();
            //        buttonUpdate.Show();
            //        buttonDeleteVersion.Show();
            //        buttonDelete.Show();
            //    }
            //}


            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Version with Seq No" + WSeqNoVersion.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

            FV.ReadFeesVersionbySeqNo(WOperator, WSeqNoVersion);

            WVersionId = FV.VersionId;

            textBox16.Text = FV.VersionId;

            textBox17.Text = FV.Description;

            if (FV.DividingMethod == "Fixed")
            {
                radioButtonFixed.Checked = true;
            }
            if (FV.DividingMethod == "Percent")
            {
                radioButtonPercent.Checked = true;
            }

            dateTimePicker1.Value = FV.FromDate;
            dateTimePicker2.Value = FV.ToDate;

            label32.Text = "User Id :" + FV.UserId; 

            if (FV.DividingMethod == "Fixed")
            {
                textBox7.Enabled = false;
                textBox8.Enabled = false;
                textBox9.Enabled = false;
            }
            else
            {
                textBox7.Enabled = true;
                textBox8.Enabled = true;
                textBox9.Enabled = true;
            }
            if (FV.DividingMethod == "Percent")
            {
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
            }
            else
            {
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
            }

            FV.ReadFeesVersionToFindLastSeqAndToDate(WOperator, WProduct);

            if (WSeqNoVersion == FV.LastSeqNoVersion)
            {
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = true;
            }
            else
            {
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
            }

            // Get Layers 
            SelectionCriteria = " WHERE Operator ='" + WOperator
               + "' AND Product ='" + WProduct + "' AND VersionId ='" + WVersionId + "'";

            FL.ReadFeesLayersAndFeelTable(WOperator, WSignedId, SelectionCriteria);

            dataGridView2.DataSource = FL.TableFeesLayers.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //radioButtonFixed.Enabled = true;
                //radioButtonPercent.Enabled = true;

                buttonUpdate.Hide();
                buttonDelete.Hide();

                checkBoxMakeNewLayer.Checked = true; 

                //MessageBox.Show("No entries for this selection. Add entries if you wish");
                textBox1.Text = "";
                textBoxLayerName.Text = "";
                textBox11.Text = "0";
                textBox12.Text = "0";
                textBox13.Text = "0";
                textBox14.Text = "0";
                textBox15.Text = "0";

                textBox2.Text = "0";
                textBox3.Text = "0";
                textBox4.Text = "0";

                textBox7.Text = "0";
                textBox8.Text = "0";
                textBox9.Text = "0";
                return;
            }
            else
            {
                radioButtonFixed.Enabled = false;
                radioButtonPercent.Enabled = false;

                buttonUpdate.Show();
                buttonDelete.Show();

                if (FL.CorrectSequence == false)
                {
                    textBox6.Show();
                    textBoxMsgBoard.Text = "Please correct the version layers.";
                }
                else
                {
                    textBox6.Hide();
                }


            }

            dataGridView2.Columns[0].Width = 50; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);

            dataGridView2.Columns[1].Width = 230; //LayerName 
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Columns[1].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[2].Width = 90; // FromAmount
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[2].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[3].Width = 90; // ToAmount
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[3].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[4].Width = 80; // TotalFees
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[4].DefaultCellStyle.ForeColor = Color.LightSlateGray;


            //TableFeesLayers.Columns.Add("SeqNo", typeof(int));
            //TableFeesLayers.Columns.Add("LayerName", typeof(string));
            //TableFeesLayers.Columns.Add("FromAmount", typeof(string));
            //TableFeesLayers.Columns.Add("ToAmount", typeof(string));
            //TableFeesLayers.Columns.Add("TotalFees", typeof(decimal));


        }
        //On Row Enter 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNoLayer = (int)rowSelected.Cells[0].Value;

            FL.ReadFeesLayersbySeqNo(WOperator, WSeqNoLayer);

            textBox1.Text = FL.SeqNo.ToString();

            comboBox1.Text = FL.Ccy;

            textBox11.Text = FL.FromAmount.ToString();
            textBox12.Text = FL.ToAmount.ToString();

            textBox13.Text = FL.TotalFees.ToString();

            textBox14.Text = FL.FromAmount.ToString("#,##0");
            textBox15.Text = FL.ToAmount.ToString("#,##0");
            textBoxLayerName.Text = textBoxLayerName.Text = "Range from : " + textBox14.Text + " to : " + textBox15.Text;


            //dateTimePicker1.Value = FL.EffectiveDate;

            comboBoxEntities.Text = FL.EntityA;
            comboBox2.Text = FL.EntityB;
            comboBox3.Text = FL.EntityC;

            textBox2.Text = FL.FeesEntityA.ToString();
            textBox3.Text = FL.FeesEntityB.ToString();
            textBox4.Text = FL.FeesEntityC.ToString();

            textBox5.Text = (FL.FeesEntityA + FL.FeesEntityB + FL.FeesEntityC).ToString();

            textBox7.Text = FL.FeesEntityPercA.ToString();
            textBox8.Text = FL.FeesEntityPercB.ToString();
            textBox9.Text = FL.FeesEntityPercC.ToString();

            textBox10.Text = (FL.FeesEntityPercA + FL.FeesEntityPercB + FL.FeesEntityPercC).ToString();
        }
        // ADD
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (WSeqNoVersion < WCurrentVersionSeqNo)
            {
                MessageBox.Show("Old Version.Not Allowed to Add");
                return;
            }
            FV.ReadFeesVersionbySeqNo(WOperator, WSeqNoVersion);

            FL.Product = WProduct;

            FL.VersionId = WVersionId;

            FL.LayerName = textBoxLayerName.Text;

            FL.Ccy = comboBox1.Text;

            if (int.TryParse(textBox11.Text, out FL.FromAmount))
            {
            }
            else
            {

                MessageBox.Show(textBox11.Text, "Please enter a valid number in field From Amount. ");
                return;
            }

            if (int.TryParse(textBox12.Text, out FL.ToAmount))
            {
            }
            else
            {
                MessageBox.Show(textBox12.Text, "Please enter a valid number in field To Amount. ");
                return;
            }

            if (decimal.TryParse(textBox13.Text, out FL.TotalFees))
            {
            }
            else
            {
                MessageBox.Show(textBox13.Text, "Please enter a valid number in field To Amount. ");
                return;
            }

            //FL.EffectiveDate = dateTimePicker1.Value;
            FL.EntityA = comboBoxEntities.Text;

            FL.EntityB = comboBox2.Text;
            FL.EntityC = comboBox3.Text;

            if (decimal.TryParse(textBox2.Text, out FL.FeesEntityA))
            {
            }
            else
            {
                MessageBox.Show(textBox2.Text, "Please enter a valid number in FeesEntityA. ");
                return;
            }
            if (decimal.TryParse(textBox3.Text, out FL.FeesEntityB))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number in field FeesEntityB. ");
                return;
            }
            if (decimal.TryParse(textBox4.Text, out FL.FeesEntityC))
            {
            }
            else
            {
                MessageBox.Show(textBox4.Text, "Please enter a valid number in field FeesEntityC. ");
                return;
            }
            if (FV.DividingMethod == "Fixed")
            {
                if ((FL.FeesEntityA + FL.FeesEntityB + FL.FeesEntityC) != FL.TotalFees)
                {
                    MessageBox.Show("Fees do not sum up!");
                    return;
                }
            }

            //textBox5.Text = (FL.FeesEntityA + FL.FeesEntityB + FL.FeesEntityC).ToString();

            if (int.TryParse(textBox7.Text, out FL.FeesEntityPercA))
            {
            }
            else
            {
                MessageBox.Show(textBox7.Text, "Please enter a valid number in FeesEntityPercA. ");
                return;
            }
            if (int.TryParse(textBox8.Text, out FL.FeesEntityPercB))
            {
            }
            else
            {
                MessageBox.Show(textBox8.Text, "Please enter a valid number in field FeesEntityPercB. ");
                return;
            }
            if (int.TryParse(textBox9.Text, out FL.FeesEntityPercC))
            {
            }
            else
            {
                MessageBox.Show(textBox9.Text, "Please enter a valid number in field FeesEntityPercC. ");
                return;
            }
            if (FV.DividingMethod == "Percent")
            {
                if ((FL.FeesEntityPercA + FL.FeesEntityPercB + FL.FeesEntityPercC) != 100
                            & (FL.FeesEntityPercA + FL.FeesEntityPercB + FL.FeesEntityPercC) > 0
                   )
                {
                    MessageBox.Show("Total Percentage not equal to 100");
                    return;
                }
            }     

            int InsertSeq = FL.InsertFeesLayer();

            checkBoxMakeNewLayer.Checked = false;

            FL.ReadFeesLayersToFindPositionOfSeqNo(WOperator, WProduct ,WVersionId, InsertSeq);

            int PositionInGrid = FL.PositionInGrid; 

            int WRowGrid1 = dataGridView1.SelectedRows[0].Index;

            Form503FeesNEW_Load(this, new EventArgs());

            dataGridView1.Rows[WRowGrid1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowGrid1));

            dataGridView2.Rows[PositionInGrid].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, PositionInGrid));

            //WSeqNoVersion = FV.InsertFeesVersion();

            //FV.ReadFeesVersionToFindPositionOfSeqNo(WOperator, WSeqNoVersion);

            //checkBoxMakeNewVersion.Checked = false;

            //Form503FeesNEW_Load(this, new EventArgs());

            //dataGridView1.Rows[FV.PositionInGrid].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, FV.PositionInGrid));

            //textBox10.Text = (FL.FeesEntityPercA + FL.FeesEntityPercB + FL.FeesEntityPercC).ToString();

        }
        // UPDATE Layer
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (WSeqNoVersion < WCurrentVersionSeqNo)
            {
                MessageBox.Show("Old Version.Not Allowed to Update Layer");
                return;
            }

            FV.ReadFeesVersionbySeqNo(WOperator, WSeqNoVersion);
            FL.ReadFeesLayersbySeqNo(WOperator, WSeqNoLayer);

            FL.Product = WProduct;

            FL.VersionId = WVersionId;

            FL.LayerName = textBoxLayerName.Text;

            FL.Ccy = comboBox1.Text;

            if (int.TryParse(textBox11.Text, out FL.FromAmount))
            {
            }
            else
            {
                MessageBox.Show(textBox11.Text, "Please enter a valid number in field From Amount. ");
                return;
            }

            if (int.TryParse(textBox12.Text, out FL.ToAmount))
            {
            }
            else
            {
                MessageBox.Show(textBox12.Text, "Please enter a valid number in field To Amount. ");
                return;
            }

            if (decimal.TryParse(textBox13.Text, out FL.TotalFees))
            {
            }
            else
            {
                MessageBox.Show(textBox13.Text, "Please enter a valid number in field To Amount. ");
                return;
            }

            //FL.EffectiveDate = dateTimePicker1.Value;
            FL.EntityA = comboBoxEntities.Text;

            FL.EntityB = comboBox2.Text;
            FL.EntityC = comboBox3.Text;

            if (decimal.TryParse(textBox2.Text, out FL.FeesEntityA))
            {
            }
            else
            {
                if (textBox2.Text != "")
                {
                    MessageBox.Show(textBox2.Text, "Please enter a valid number in FeesEntityA. ");
                    return;
                }
               
            }
            if (decimal.TryParse(textBox3.Text, out FL.FeesEntityB))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number in field FeesEntityB. ");
                return;
            }
            if (decimal.TryParse(textBox4.Text, out FL.FeesEntityC))
            {
            }
            else
            {
                MessageBox.Show(textBox4.Text, "Please enter a valid number in field FeesEntityC. ");
                return;
            }

            if (FV.DividingMethod == "Fixed")
            {
                if ((FL.FeesEntityA + FL.FeesEntityB + FL.FeesEntityC) != FL.TotalFees)
                {
                    MessageBox.Show("Fees do not sum up!");
                    return;
                }
            }

            //textBox5.Text = (FL.FeesEntityA + FL.FeesEntityB + FL.FeesEntityC).ToString();

            if (int.TryParse(textBox7.Text, out FL.FeesEntityPercA))
            {
            }
            else
            {
                MessageBox.Show(textBox7.Text, "Please enter a valid number in FeesEntityPercA. ");
                return;
            }
            if (int.TryParse(textBox8.Text, out FL.FeesEntityPercB))
            {
            }
            else
            {
                MessageBox.Show(textBox8.Text, "Please enter a valid number in field FeesEntityPercB. ");
                return;
            }
            if (int.TryParse(textBox9.Text, out FL.FeesEntityPercC))
            {
            }
            else
            {
                MessageBox.Show(textBox9.Text, "Please enter a valid number in field FeesEntityPercC. ");
                return;
            }

            if (FV.DividingMethod == "Percent")
            {
                if ((FL.FeesEntityPercA + FL.FeesEntityPercB + FL.FeesEntityPercC) != 100
                            & (FL.FeesEntityPercA + FL.FeesEntityPercB + FL.FeesEntityPercC) > 0
                   )
                {
                    MessageBox.Show("Total Percentage not equal to 100");
                    return;
                }
            }
            int WRowIndex1 = dataGridView1.SelectedRows[0].Index;
            int WRowIndex2 = dataGridView2.SelectedRows[0].Index;
            int scrollPosition = dataGridView2.FirstDisplayedScrollingRowIndex;

            FL.UpdateFeesLayer(WSeqNoLayer);

            MessageBox.Show("Updating Done!");

            textBoxMsgBoard.Text = "Layer updated.";

            Form503FeesNEW_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex1].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));

            dataGridView2.Rows[WRowIndex2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex2));

            dataGridView2.FirstDisplayedScrollingRowIndex = scrollPosition;

        }
        //DELETE
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (WSeqNoVersion < WCurrentVersionSeqNo)
            {
                MessageBox.Show("Old Version.Not Allowed to Delete Layer");
                return;
            }
            if (MessageBox.Show("Warning: Do you want to delete this LAYER?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                int WRowIndex1 = dataGridView1.SelectedRows[0].Index;
                int WRowIndex2 = dataGridView2.SelectedRows[0].Index;
              
                FL.DeleteFeesLayer(WSeqNoLayer);

                MessageBox.Show("Deleted!");

                textBoxMsgBoard.Text = "Layer Deleted.";

                Form503FeesNEW_Load(this, new EventArgs());

                dataGridView1.Rows[WRowIndex1].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                if (WRowIndex2 >0)
                {
                    WRowIndex2 = WRowIndex2 - 1; 
                    dataGridView2.Rows[WRowIndex2].Selected = true;
                    dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex2));
                }

            }
            else
            {
                return;
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // Validation

        private void Validation()
        {
            //
            //  Validation
            //

            if (int.TryParse(textBox11.Text, out FL.FromAmount))
            {
            }
            else
            {
                MessageBox.Show(textBox5.Text, "Please enter a valid number in field PosStart. ");
                return;
            }
        }
        //On Text change 
        private void textBox11_TextChanged(object sender, EventArgs e)
        {

            if (int.TryParse(textBox11.Text, out FL.FromAmount))
            {
            }
            else
            {
                if (textBox11.Text != "")
                {
                    MessageBox.Show(textBox11.Text, "Please enter a valid number in field From Amount. ");
                    return;
                }
               
            }

            textBox14.Text = FL.FromAmount.ToString("#,##0");

            textBoxLayerName.Text = textBoxLayerName.Text = "Range from : " + textBox14.Text + " to : " + textBox15.Text;

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox12.Text, out FL.ToAmount))
            {
            }
            else
            {
                if (textBox12.Text != "")
                {
                    MessageBox.Show(textBox12.Text, "Please enter a valid number in field From Amount. ");
                    return;
                }            
            }

            textBox15.Text = FL.ToAmount.ToString("#,##0");

            textBoxLayerName.Text = textBoxLayerName.Text = "Range from : " + textBox14.Text + " to : " + textBox15.Text;
        }
        //Fee A
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox2.Text, out FL.FeesEntityA))
            {
            }
            else
            {
                if (textBox2.Text != "")
                {
                    MessageBox.Show(textBox2.Text, "Please enter a valid number in FeesEntityA. ");
                    return;
                }
            }
            if (textBox2.Text != "") textBox5.Text = (FL.FeesEntityA + FL.FeesEntityB + FL.FeesEntityC).ToString();
        }
        // Fee B
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox3.Text, out FL.FeesEntityB))
            {
            }
            else
            {
                if (textBox3.Text != "")
                {
                    MessageBox.Show(textBox3.Text, "Please enter a valid number in fees B. ");
                    return;
                }
            }
            if (textBox3.Text != "") textBox5.Text = (FL.FeesEntityA + FL.FeesEntityB + FL.FeesEntityC).ToString();
        }
        // Fee C
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox4.Text, out FL.FeesEntityC))
            {
            }
            else
            {
                if (textBox4.Text != "")
                {
                    MessageBox.Show(textBox4.Text, "Please enter a valid number in fees C. ");
                    return;
                }
            }
            if (textBox4.Text != "") textBox5.Text = (FL.FeesEntityA + FL.FeesEntityB + FL.FeesEntityC).ToString();
        }

        private void checkBoxMakeNewVersion_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMakeNewVersion.Checked == true)
            {
                buttonAddVersion.Show();
                buttonUpdateVersion.Hide();
                buttonDeleteVersion.Hide();

                // FRom layers

                panel3.Hide(); 

                buttonUpdate.Hide();
                buttonDelete.Hide();

                radioButtonFixed.Enabled = true;
                radioButtonPercent.Enabled = true; 

                textBox16.ReadOnly = false;
                textBox17.ReadOnly = false;

                textBox16.Text = "";
                textBox17.Text = "";

                label32.Hide(); 

                //Find Next Start Date 
                FV.ReadFeesVersionToFindLastSeqAndToDate(WOperator, WProduct);

                dateTimePicker1.Value = FV.ToDate.AddDays(1);
                dateTimePicker1.Enabled = false;

                dateTimePicker2.Value = FV.ToDate.AddDays(1);
                dateTimePicker2.Enabled = true;

                dataGridView1.Enabled = false; 
            }
            else
            {
                textBox16.ReadOnly = true;

                buttonAddVersion.Hide();
                buttonUpdateVersion.Show();
                buttonDeleteVersion.Show();

                // FRom layers
                panel3.Show();
                buttonUpdate.Show();
                buttonDelete.Show();

                radioButtonFixed.Enabled = false;
                radioButtonPercent.Enabled = false;

                dataGridView1.Enabled = true;

                label32.Show();

                Form503FeesNEW_Load(this, new EventArgs());
            }
        }
        // Add Version
        private void buttonAddVersion_Click(object sender, EventArgs e)
        {
            FV.Product = WProduct;
            FV.VersionId = textBox16.Text;
            FV.Description = textBox17.Text;
            FV.DividingMethod = "NoValue";
            if (radioButtonFixed.Checked == true)
            {
                FV.DividingMethod = "Fixed";
            }
            if (radioButtonPercent.Checked == true)
            {
                FV.DividingMethod = "Percent";
            }
            FV.FromDate = dateTimePicker1.Value.Date;
            FV.ToDate = dateTimePicker2.Value.Date;
            FV.Locked = false;
            FV.UserId = WSignedId;
            FV.Operator = WOperator;

            label32.Show();

            int NewSeqNo = FV.InsertFeesVersion();

            FV.ReadFeesVersionToFindPositionOfSeqNo(WOperator, WProduct, NewSeqNo);

            int PositionInGrid = FV.PositionInGrid; 

            checkBoxMakeNewVersion.Checked = false;

            Form503FeesNEW_Load(this, new EventArgs());

            dataGridView1.Rows[PositionInGrid].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, PositionInGrid));
        }

        private void buttonUpdateVersion_Click(object sender, EventArgs e)
        {
            FV.ReadFeesVersionbySeqNo(WOperator, WSeqNoVersion);

            if (WSeqNoVersion<WCurrentVersionSeqNo)
            {
                MessageBox.Show("Old Version.Not Allowed to Update");
                return; 
            }

            FV.Description = textBox17.Text;

            FV.DividingMethod = "NoValue";
            if (radioButtonFixed.Checked == true)
            {
                FV.DividingMethod = "Fixed";
            }
            if (radioButtonPercent.Checked == true)
            {
                FV.DividingMethod = "Percent";
            }
            //FV.FromDate = dateTimePicker1.Value.Date;

            if (dateTimePicker2.Value.Date != FV.ToDate)
            {
                if (dateTimePicker2.Value.Date < FV.FromDate)
                {
                    MessageBox.Show("Invalid value of End Date");
                    return;
                }
            }
            FV.ToDate = dateTimePicker2.Value.Date;

            FV.UserId = WSignedId;

            int WRowIndex = dataGridView1.SelectedRows[0].Index;

            FV.UpdateFeesVersion(WSeqNoVersion);

            MessageBox.Show("Updating Done!");

            textBoxMsgBoard.Text = "Version updated.";

            Form503FeesNEW_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));


        }
        //Delete Version
        private void buttonDeleteVersion_Click(object sender, EventArgs e)
        {
            if (WSeqNoVersion < WCurrentVersionSeqNo)
            {
                MessageBox.Show("Old Version.Not Allowed to Delete");
                return;
            }
            if (MessageBox.Show("Warning: Do you want to delete this Fess Version? ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                       == DialogResult.Yes)
            {
                if (WSeqNoVersion != FV.LastSeqNoVersion)
                {
                    MessageBox.Show("You can not delete this version. You can delete only the last one.");
                    return;
                }

                FV.DeleteFeesVersion(WSeqNoVersion);

                FL.DeleteFeesLayerForThisVersion(WVersionId); 

                Form503FeesNEW_Load(this, new EventArgs());

                checkBoxMakeNewLayer.Checked = false; 

                if (dataGridView1.RowCount == 0)
                {
                    buttonUpdateVersion.Hide();
                    buttonDeleteVersion.Hide();

                    textBoxMsgBoard.Text = "Add Version"; 
                }
            }
            else
            {
                return;
            }
        }
        //Make New Layer
        int WRowIndex1;
        int WRowIndex2;
        private void checkBoxMakeNewLayer_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMakeNewLayer.Checked == true)
            {
                buttonAdd.Show();
                buttonUpdate.Hide();
                buttonDelete.Hide();

                //buttonUpdateVersion.Hide();
                //buttonDeleteVersion.Hide();

                dataGridView2.Enabled = false; 

                textBox11.Text = "0";
                textBox12.Text = "0";
                textBox13.Text = "0";
                textBox14.Text = "0";
                textBox15.Text = "0";

                textBox2.Text = "0";
                textBox3.Text = "0";
                textBox4.Text = "0";

                textBox7.Text = "0";
                textBox8.Text = "0";
                textBox9.Text = "0";

                textBox10.Text = "0";

            }
            else
            {
                buttonAdd.Hide();
                buttonUpdate.Show();
                buttonDelete.Show();

                buttonUpdateVersion.Show();
                buttonDeleteVersion.Show();

                dataGridView2.Enabled = true;
                WRowIndex2 = -1; 
                if (dataGridView2.Rows.Count > 0)
                {
                    WRowIndex2 = dataGridView2.SelectedRows[0].Index;
                }
                if (dataGridView1.Rows.Count > 0)
                {
                    WRowIndex1 = dataGridView1.SelectedRows[0].Index;
                }
                
                Form503FeesNEW_Load(this, new EventArgs());

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Rows[WRowIndex1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                }

                if (WRowIndex2 != -1)
                {
                    dataGridView2.Rows[WRowIndex2].Selected = true;
                    dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex2));
                }
               

            }
        }
        //FINISH 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (FL.CorrectSequence == false)
            {
                if (MessageBox.Show("Warning: Version incorrect. Do you want to leave? ", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    this.Dispose();
                }
                else
                {
                    return;
                }
            }
            else
            {
                this.Dispose();
            }

        }
        //Print
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            string P1 = WVersionId;
            string P2 = FV.FromDate.ToString();
            string P3 = FV.ToDate.ToString();
            string P4 = Us.BankId;
            string P5 = WSignedId; 

            Form56R43ITMX ReportITMX43 = new Form56R43ITMX(P1, P2, P3, P4, P5);
            ReportITMX43.Show();
        }

        private void button52_Click(object sender, EventArgs e)
        {
            FormHelp helpForm = new FormHelp("Fees Versions And Layers Definition");
            helpForm.ShowDialog();
        }
//Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "Version with Seq No" + WSeqNoVersion.ToString();
            string SearchP4 = "";
            if (ViewWorkFlow == true) WNotesMode = "Read";
            else WNotesMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WNotesMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Version with Seq No" + WSeqNoVersion.ToString();
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }
        //Perc Fees A 

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox7.Text, out FL.FeesEntityPercA))
            {
            }
            else
            {
                if (textBox7.Text != "")
                {
                    MessageBox.Show(textBox7.Text, "Please enter a valid number in Percent fro fees A. ");
                    return;
                }
            }

            if (textBox7.Text != "") textBox10.Text = (FL.FeesEntityPercA + FL.FeesEntityPercB + FL.FeesEntityPercC).ToString();
        }
// Perc B
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox8.Text, out FL.FeesEntityPercB))
            {
            }
            else
            {
                if (textBox8.Text != "")
                {
                    MessageBox.Show(textBox8.Text, "Please enter a valid number in Percent fro fees B. ");
                    return;
                }
            }

            if (textBox8.Text != "") textBox10.Text = (FL.FeesEntityPercA + FL.FeesEntityPercB + FL.FeesEntityPercC).ToString();
        }
//Perc C
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox9.Text, out FL.FeesEntityPercC))
            {
            }
            else
            {
                if (textBox9.Text != "")
                {
                    MessageBox.Show(textBox9.Text, "Please enter a valid number in Percent fro fees C. ");
                    return;
                }
            }

            if (textBox9.Text != "") textBox10.Text = (FL.FeesEntityPercA + FL.FeesEntityPercB + FL.FeesEntityPercC).ToString();
        }
        //Change Product 
        int LoadCounter; 
        private void comboBoxFeesProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxMakeNewVersion.Checked = false;
            LoadCounter = 2; 
            Form503FeesNEW_Load(this, new EventArgs());

            if (dataGridView1.RowCount == 0)
            {
                textBoxMsgBoard.Text = "Add Version";
            }
        }
    }
}
