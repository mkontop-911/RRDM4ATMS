using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs;

using System.Configuration;
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form503 : Form
    {

        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WCategoryId;
        string WMainCateg;

        int BIN; 

        int WSeqNo; 

        int WRowIndex ; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        public Form503(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
            labelUserId.Text = WSignedId;    

            Gp.ParamId = "201"; // Currencies  
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            Gp.ParamId = "201"; // Currencies  
            comboBox6.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox6.DisplayMember = "DisplayValue";

            Gp.ParamId = "707"; // Origin  
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox2.DisplayMember = "DisplayValue";

            Gp.ParamId = "708"; // Transaction At Origin   
            comboBox3.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox3.DisplayMember = "DisplayValue";

            Gp.ParamId = "709"; // Products   
            comboBoxProducts.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxProducts.DisplayMember = "DisplayValue";

            Gp.ParamId = "710"; // Cost Centre  
            comboBox5.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox5.DisplayMember = "DisplayValue";

            Gp.ParamId = "711"; // Periodicity 
            comboBox7.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox7.DisplayMember = "DisplayValue"; 

            Gp.ParamId = "707"; // fILTER 
            comboBoxFilter.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxFilter.DisplayMember = "DisplayValue";
         
        }
// Load 
        private void Form503_Load(object sender, EventArgs e)
        {

            Rc.ReadReconcCategories(WOperator, comboBoxFilter.Text, "ALL");

            dataGridView1.DataSource = Rc.ReconcCateg.DefaultView;

            dataGridView1.Columns[0].Width = 60; // Seq No
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);

            dataGridView1.Columns[1].Width = 60; // Category Id
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 270; // Name
        }
// On Row Enter 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
        
            WSeqNo = (int)rowSelected.Cells[0].Value;

            Rc.ReadReconcCategorybySeqNo(WOperator, WSeqNo);

            WCategoryId = Rc.CategoryId;

            WMainCateg = WCategoryId.Substring(0, 4); 

            textBox1.Text = Rc.CategoryId;
            
            comboBox2.Text = Rc.Origin;
            comboBox3.Text = Rc.TransTypeAtOrigin;
            comboBoxProducts.Text = Rc.Product;
           
            //Debit Card
            //    Prepaid Card
            //        Gift Card
            //            Travel Money Card 

            comboBox1.Text = Rc.Currency;
            textBox7.Text = Rc.GlAccount;

            textBox9.Text = Rc.VostroBank;
            comboBox6.Text = Rc.VostroCurr;
            textBox2.Text = Rc.VostroAcc;

            if (WMainCateg == "EWB3" & comboBox3.Text != "EWB4" & comboBoxProducts.Text != "EWB5")
            {
                textBoxCategDescr.Text = comboBox2.Text + " " + comboBox3.Text + " - " + comboBoxProducts.Text;
                textBoxCategDescr.ReadOnly = true;
            }
            else
            {
                if (WMainCateg == "EWB8") // Nostro Vostro 
                {
                      textBoxCategDescr.Text = "NoVo " + comboBox1.Text + textBox7.Text +" And " + textBox2.Text ;
                      textBoxCategDescr.ReadOnly = true;
                }
                else
                {
                    textBoxCategDescr.Text = Rc.CategoryName;
                    textBoxCategDescr.ReadOnly = false;
                }  
            }

            comboBox5.Text = Rc.CostCentre;
            comboBox7.Text = Rc.Periodicity; 
            textBoxBIN.Text = Rc.GroupIdInFiles;
            textBox4.Text = Rc.FieldName;
            textBox5.Text = Rc.PosStart.ToString();
            textBox6.Text = Rc.PosEnd.ToString();
          
            if (Rc.HasOwner == true)
            {
                Us.ReadUsersRecord(Rc.OwnerId);

                textBoxOwner.Text = Rc.OwnerId + " " + Us.UserName;
                buttonChangeOwner.Text = "Change Owner";
            }
            else
            {
                textBoxOwner.Text = "Category has no Owner.";
                buttonChangeOwner.Text = "Assign Owner";
            }
        }
// ADD
        private void buttonAdd_Click(object sender, EventArgs e)
        {

            Rc.ReadReconcCategorybyCategId(WOperator, textBox8.Text);
                if (Rc.RecordFound == true)
                {
                     MessageBox.Show(textBox1.Text, "This Category Already exist.  ");
                    return;
                }
            //
           //  Validation
            //
             if (int.TryParse(textBox5.Text, out Rc.PosStart))
                {
                }
                else
                {
                    MessageBox.Show(textBox5.Text, "Please enter a valid number in field PosStart. ");
                    return;
                }

              if (int.TryParse(textBox6.Text, out Rc.PosEnd))
                {
                }
                else
                {
                    MessageBox.Show(textBox6.Text, "Please enter a valid number in field PosEnd. ");
                    return;
                }

              if (textBox8.Text == "")
              {
                  MessageBox.Show(textBox6.Text, "Please enter a value for NEW Category Id. ");
                  return;
              }

              if (textBoxCategDescr.Text == "")
              {
                  MessageBox.Show(textBox6.Text, "Please enter a value for Category Name. ");
                  return;
              }
    
            Rc.CategoryId = textBox8.Text ;

            if (WMainCateg == "EWB3" & comboBox3.Text != "EWB4" & comboBoxProducts.Text != "EWB5")
            {
                textBoxCategDescr.Text = comboBox2.Text + " " + comboBox3.Text + " - " + comboBoxProducts.Text;
                textBoxCategDescr.ReadOnly = true;
            }
            else
            {
                if (WMainCateg == "EWB8") // Nostro Vostro 
                {
                    textBoxCategDescr.Text = "NoVo " + comboBox1.Text + textBox7.Text + " And " + textBox2.Text;
                    textBoxCategDescr.ReadOnly = true;
                }
                else
                {
                    //textBoxCategDescr.Text = Rc.CategoryName;
                    textBoxCategDescr.ReadOnly = false;
                }  
              
            }
            Rc.CategoryName = textBoxCategDescr.Text ;

            Rc.Origin = comboBox2.Text  ;
            Rc.TransTypeAtOrigin = comboBox3.Text ;
            Rc.Product = comboBoxProducts.Text ;
            Rc.CostCentre = comboBox5.Text ;
            Rc.Periodicity = comboBox7.Text;
            Rc.GroupIdInFiles = textBoxBIN.Text ;
            Rc.FieldName = textBox4.Text ;
            Rc.Currency = comboBox1.Text; 
            Rc.GlAccount = textBox7.Text;

            Rc.VostroBank = textBox9.Text ;
            Rc.VostroCurr = comboBox6.Text ;
            Rc.VostroAcc = textBox2.Text;

            Rc.InsertCategory();

            Form503_Load(this, new EventArgs());

        }

// UPDATE CATEGORY 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Rc.ReadReconcCategorybySeqNo(WOperator, WSeqNo);

            //
            //  Validation
            //
            if (int.TryParse(textBox5.Text, out Rc.PosStart))
            {
            }
            else
            {
                MessageBox.Show(textBox5.Text, "Please enter a valid number in field PosStart. ");
                return;
            }

            if (int.TryParse(textBox6.Text, out Rc.PosEnd))
            {
            }
            else
            {
                MessageBox.Show(textBox6.Text, "Please enter a valid number in field PosEnd. ");
                return;
            }

            if (textBox1.Text == "")
            {
                MessageBox.Show(textBox6.Text, "Please enter a value for Category Id. ");
                return;
            }

            if (textBoxCategDescr.Text == "")
            {
                MessageBox.Show(textBox6.Text, "Please enter a value for Category Name. ");
                return;
            }
            Rc.CategoryId = textBox1.Text;

            if (WMainCateg == "EWB3" & comboBox3.Text != "EWB4" & comboBoxProducts.Text != "EWB5")
            {
                textBoxCategDescr.Text = comboBox2.Text + " " + comboBox3.Text + " - " + comboBoxProducts.Text;
                textBoxCategDescr.ReadOnly = true; 
            }
            else
            {
                if (WMainCateg == "EWB8") // Nostro Vostro 
                {
                    textBoxCategDescr.Text = "NoVo " + comboBox1.Text + textBox7.Text + " And " + textBox2.Text;
                    textBoxCategDescr.ReadOnly = true;
                }
                else
                {
                    //textBoxCategDescr.Text = Rc.CategoryName;
                    textBoxCategDescr.ReadOnly = false;
                }   
            }

            Rc.CategoryName = textBoxCategDescr.Text;

            Rc.Origin = comboBox2.Text;
            Rc.TransTypeAtOrigin = comboBox3.Text;
            Rc.Product = comboBoxProducts.Text;
            Rc.CostCentre = comboBox5.Text;
            Rc.Periodicity = comboBox7.Text; 
            Rc.GroupIdInFiles = textBoxBIN.Text;
            Rc.FieldName = textBox4.Text;
            Rc.Currency = comboBox1.Text; 
            Rc.GlAccount = textBox7.Text;

            Rc.VostroBank = textBox9.Text;
            Rc.VostroCurr = comboBox6.Text;
            Rc.VostroAcc = textBox2.Text;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Rc.UpdateCategory(WOperator, Rc.CategoryId); 

            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }

 // DELETE 
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete(Close) this category?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                          == DialogResult.Yes)
            {
                //Rc.DeleteCategory(WSeqNo);

                Rc.ReadReconcCategorybySeqNo(WOperator, WSeqNo);

                Rc.Active = false; 

                Rc.UpdateCategory(WOperator, WCategoryId); 

                Form503_Load(this, new EventArgs());
            }
            else
            {
                return; 
            }
        }

// DEFINE GL ACCOUNTS 
        private void button1_Click(object sender, EventArgs e)
        {
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form85 NForm85 ;

            int WSecLevel = 4; 

            NForm85 = new Form85(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm85.FormClosed += NForm85_FormClosed;
            NForm85.ShowDialog(); ;
        }

        void NForm85_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }

// Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

        private void buttonChangeOwner_Click(object sender, EventArgs e)
        {
            Form503_CategOwners NForm503_CategOwners; 
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            NForm503_CategOwners = new Form503_CategOwners(WSignedId, WSignRecordNo, WOperator, WCategoryId);
            NForm503_CategOwners.FormClosed += NForm503_CategOwners_FormClosed;
            NForm503_CategOwners.ShowDialog(); 
        }

        void NForm503_CategOwners_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }

// Change Filter 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form503_Load(this, new EventArgs());
        }
// Check Origin and Show Panel
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Nostro - Vostro")
            {
                panel3.Show();
                label10.Text = "Nostro";
            }
            else
            {
                panel3.Hide();
                label10.Text = "GL Account"; 
            }

        }
// Product has changed  - change the TextBoxBIN
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool found = false;

            Gp.ReadParametersSpecificNm("709", comboBoxProducts.Text); 
            if (Gp.RecordFound == true)
            {
                BIN = (int)Gp.Amount;
                textBoxBIN.Text = BIN.ToString(); 
            }


            //if (comboBoxProducts.Text == "Debit Card")
            //{
            //    Gp.ReadParametersSpecificId(WOperator, "709", "1", "", ""); // 
            //    BIN = (int)Gp.Amount;
            //    found = true; 
            //}
            //if (comboBoxProducts.Text == "Prepaid Card")
            //{
            //    Gp.ReadParametersSpecificId(WOperator, "709", "2", "", ""); // 
            //    BIN = (int)Gp.Amount;
            //    found = true; 
            //}
            //if (comboBoxProducts.Text == "Gift Card")
            //{
            //    Gp.ReadParametersSpecificId(WOperator, "709", "3", "", ""); // 
            //    BIN = (int)Gp.Amount;
            //    found = true; 
            //}
            //if (comboBoxProducts.Text == "Travel Money Card")
            //{
            //    Gp.ReadParametersSpecificId(WOperator, "709", "4", "", ""); // 
            //    BIN = (int)Gp.Amount;
            //    found = true; 
            //}
            //if (found == true)
            //{
            //    textBoxBIN.Text = BIN.ToString(); 
            //}
            //else
            //{
            //    // 
            //}
            
        }
    }
}
