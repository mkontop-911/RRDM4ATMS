using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form75 : Form
    {
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        string SelectionCriteria; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WAtmNo;
        int WSesNo;
     
        int WUniqueRecordId; 

        public Form75(string InSignedId, int SignRecordNo, string InOperator, string InAtmNo, int InSesNo, int InUniqueRecordId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
          
            WUniqueRecordId = InUniqueRecordId; 

            InitializeComponent();
        }

        private void Form75_Load(object sender, EventArgs e)
        {
            SelectionCriteria = " WHERE  UniqueRecordId =" + WUniqueRecordId;

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria,2);

            ShowMask(Mpa.MatchMask);

            textBoxUnMatchedType.Text = Mpa.MatchedType; 

        }

        //
        // Show info within Panel  Right 
        //
        string WSubString;
        string WMask;

        //
        // Show info for Mask
        //
        public void ShowMask(string InMask)
        {

            //****************************************************************************
            //Translate MASK 
            //****************************************************************************

            WMask = InMask;

            if (WMask == "")
            {
                WMask = "EEE";
            }
            // First Line
            if (Mpa.FileId01 != "")
            {
                labelFileA.Show();
                textBox31.Show();

                labelFileA.Text = "File A : " + Mpa.FileId01;
                labelFileA.Show();
                WSubString = WMask.Substring(0, 1);

                if (WSubString == "1")
                {
                    textBox31.BackColor = Color.Lime;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox31.BackColor = Color.Red;
                    textBox31.ForeColor = Color.White;
                    textBox31.Text = WSubString;
                }
            }
            else
            {
                labelFileA.Hide();
                textBox31.Hide();
            }

            // Second Line 
            if (Mpa.FileId02 != "")
            {
                labelFileB.Show();
                textBox32.Show();

                labelFileB.Text = "File B : " + Mpa.FileId02;
                labelFileB.Show();
                WSubString = WMask.Substring(1, 1);

                if (WSubString == "1")
                {
                    textBox32.BackColor = Color.Lime;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox32.BackColor = Color.Red;
                    textBox32.ForeColor = Color.White;
                    textBox32.Text = WSubString;
                }
            }
            else
            {
                labelFileB.Hide();
                textBox32.Hide();
            }

            // Third Line 
            //
            if (Mpa.FileId03 != "")
            {
                labelFileC.Show();
                textBox33.Show();

                labelFileC.Text = "File C : " + Mpa.FileId03;
                labelFileC.Show();
                WSubString = WMask.Substring(2, 1);

                if (WSubString == "1")
                {
                    textBox33.BackColor = Color.Lime;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox33.BackColor = Color.Red;
                    textBox33.ForeColor = Color.White;
                    textBox33.Text = WSubString;
                }
            }
            else
            {
                labelFileC.Hide();
                textBox33.Hide();
            }

            // Forth Line 
            if (Mpa.FileId04 != "")
            {
                labelFileD.Show();
                textBox34.Show();

                labelFileD.Text = "File D : " + Mpa.FileId04;
                labelFileD.Show();
                WSubString = WMask.Substring(3, 1);

                if (WSubString == "1")
                {
                    textBox34.BackColor = Color.Lime;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox34.BackColor = Color.Red;
                    textBox34.ForeColor = Color.White;
                    textBox34.Text = WSubString;
                }
            }
            else
            {
                labelFileD.Hide();
                textBox34.Hide();
            }

            // Fifth Line 
            if (Mpa.FileId05 != "")
            {
                labelFileE.Show();
                textBox35.Show();

                labelFileE.Text = "File E : " + Mpa.FileId05;
                labelFileE.Show();
                WSubString = WMask.Substring(4, 1);


                if (WSubString == "1")
                {
                    textBox35.BackColor = Color.Lime;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox35.BackColor = Color.Red;
                    textBox35.ForeColor = Color.White;
                    textBox35.Text = WSubString;
                }

            }
            else
            {
                labelFileE.Hide();
                textBox35.Hide();
            }
            // sixth Line 
            if (Mpa.FileId06 != "")
            {
                labelFileF.Show();
                textBox36.Show();

                labelFileF.Text = "File F : " + Mpa.FileId06;
                labelFileF.Show();
                WSubString = WMask.Substring(5, 1);

                if (WSubString == "1")
                {
                    textBox36.BackColor = Color.Lime;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = WSubString;
                }

                if (WSubString != "1")
                {
                    textBox36.BackColor = Color.Red;
                    textBox36.ForeColor = Color.White;
                    textBox36.Text = WSubString;
                }
            }
            else
            {
                labelFileF.Hide();
                textBox36.Hide();
            }
        }
        //public void ShowMask(string InMask)
        //{

        //    //****************************************************************************
        //    //Translate MASK 
        //    //****************************************************************************

        //    WMask = InMask;

        //    if (WMask == "")
        //    {
        //        WMask = "EEE";
        //    }
        //    // First Line
        //    if (Mpa.FileId01 != "")
        //    {
        //        labelFileA.Show();
        //        textBox31.Show();

        //        labelFileA.Text = "File A : " + Mpa.FileId01;
        //        labelFileA.Show();
        //        WSubString = WMask.Substring(0, 1);
        //        if (WSubString == "0")
        //        {
        //            textBox31.BackColor = Color.Red;
        //            textBox31.ForeColor = Color.White;
        //            textBox31.Text = "0";
        //        }
        //        if (WSubString == "1")
        //        {
        //            textBox31.BackColor = Color.Lime;
        //            textBox31.ForeColor = Color.White;
        //            textBox31.Text = "1";
        //        }
        //        if (WSubString == ">")
        //        {
        //            textBox31.BackColor = Color.Lime;
        //            textBox31.ForeColor = Color.White;
        //            textBox31.Text = ">";
        //        }
        //        if (WSubString == "E")
        //        {
        //            textBox31.BackColor = Color.Lime;
        //            textBox31.ForeColor = Color.White;
        //            textBox31.Text = "E";
        //        }
        //    }
        //    else
        //    {
        //        labelFileA.Hide();
        //        textBox31.Hide();
        //    }

        //    // Second Line 
        //    if (Mpa.FileId02 != "")
        //    {
        //        labelFileB.Show();
        //        textBox32.Show();

        //        labelFileB.Text = "File B : " + Mpa.FileId02;
        //        labelFileB.Show();
        //        WSubString = WMask.Substring(1, 1);
        //        if (WSubString == "0")
        //        {
        //            textBox32.BackColor = Color.Red;
        //            textBox32.ForeColor = Color.White;
        //            textBox32.Text = "0";
        //        }
        //        if (WSubString == "1")
        //        {
        //            textBox32.BackColor = Color.Lime;
        //            textBox32.ForeColor = Color.White;
        //            textBox32.Text = "1";
        //        }
        //        if (WSubString == ">")
        //        {
        //            textBox32.BackColor = Color.Lime;
        //            textBox32.ForeColor = Color.White;
        //            textBox32.Text = ">";
        //        }
        //        if (WSubString == "E")
        //        {
        //            textBox32.BackColor = Color.Lime;
        //            textBox32.ForeColor = Color.White;
        //            textBox32.Text = "E";
        //        }
        //    }
        //    else
        //    {
        //        labelFileB.Hide();
        //        textBox32.Hide();
        //    }

        //    // Third Line 
        //    //
        //    if (Mpa.FileId03 != "")
        //    {
        //        labelFileC.Show();
        //        textBox33.Show();

        //        labelFileC.Text = "File C : " + Mpa.FileId03;
        //        labelFileC.Show();
        //        WSubString = WMask.Substring(2, 1);
        //        if (WSubString == "0")
        //        {
        //            textBox33.BackColor = Color.Red;
        //            textBox33.ForeColor = Color.White;
        //            textBox33.Text = "0";
        //        }
        //        if (WSubString == "1")
        //        {
        //            textBox33.BackColor = Color.Lime;
        //            textBox33.ForeColor = Color.White;
        //            textBox33.Text = "1";
        //        }
        //        if (WSubString == ">")
        //        {
        //            textBox33.BackColor = Color.Lime;
        //            textBox33.ForeColor = Color.White;
        //            textBox33.Text = ">";
        //        }
        //        if (WSubString == "E")
        //        {
        //            textBox33.BackColor = Color.Lime;
        //            textBox33.ForeColor = Color.White;
        //            textBox33.Text = "E";
        //        }
        //    }
        //    else
        //    {
        //        labelFileC.Hide();
        //        textBox33.Hide();
        //    }

        //    // Forth Line 
        //    if (Mpa.FileId04 != "")
        //    {
        //        labelFileD.Show();
        //        textBox34.Show();

        //        labelFileD.Text = "File D : " + Mpa.FileId04;
        //        labelFileD.Show();
        //        WSubString = WMask.Substring(0, 1);
        //        if (WSubString == "0")
        //        {
        //            textBox34.BackColor = Color.Red;
        //            textBox34.ForeColor = Color.White;
        //            textBox34.Text = "0";
        //        }
        //        if (WSubString == "1")
        //        {
        //            textBox34.BackColor = Color.Lime;
        //            textBox34.ForeColor = Color.White;
        //            textBox34.Text = "1";
        //        }
        //        if (WSubString == ">")
        //        {
        //            textBox34.BackColor = Color.Lime;
        //            textBox34.ForeColor = Color.White;
        //            textBox34.Text = ">";
        //        }
        //        if (WSubString == "E")
        //        {
        //            textBox34.BackColor = Color.Lime;
        //            textBox34.ForeColor = Color.White;
        //            textBox34.Text = "E";
        //        }
        //    }
        //    else
        //    {
        //        labelFileD.Hide();
        //        textBox34.Hide();
        //    }

        //    // Fifth Line 
        //    if (Mpa.FileId05 != "")
        //    {
        //        labelFileE.Show();
        //        textBox35.Show();

        //        labelFileE.Text = "File E : " + Mpa.FileId05;
        //        labelFileE.Show();
        //        WSubString = WMask.Substring(1, 1);
        //        if (WSubString == "0")
        //        {
        //            textBox35.BackColor = Color.Red;
        //            textBox35.ForeColor = Color.White;
        //            textBox35.Text = "0";
        //        }
        //        if (WSubString == "1")
        //        {
        //            textBox35.BackColor = Color.Lime;
        //            textBox35.ForeColor = Color.White;
        //            textBox35.Text = "1";
        //        }
        //        if (WSubString == ">")
        //        {
        //            textBox35.BackColor = Color.Lime;
        //            textBox35.ForeColor = Color.White;
        //            textBox35.Text = ">";
        //        }
        //        if (WSubString == "E")
        //        {
        //            textBox35.BackColor = Color.Lime;
        //            textBox35.ForeColor = Color.White;
        //            textBox35.Text = "E";
        //        }
        //    }
        //    else
        //    {
        //        labelFileE.Hide();
        //        textBox35.Hide();
        //    }
        //    // sixth Line 
        //    if (Mpa.FileId06 != "")
        //    {
        //        labelFileF.Show();
        //        textBox36.Show();

        //        labelFileF.Text = "File F : " + Mpa.FileId06;
        //        labelFileF.Show();
        //        WSubString = WMask.Substring(2, 1);
        //        if (WSubString == "0")
        //        {
        //            textBox36.BackColor = Color.Red;
        //            textBox36.ForeColor = Color.White;
        //            textBox36.Text = "0";
        //        }
        //        if (WSubString == "1")
        //        {
        //            textBox36.BackColor = Color.Lime;
        //            textBox36.ForeColor = Color.White;
        //            textBox36.Text = "1";
        //        }
        //        if (WSubString == ">")
        //        {
        //            textBox36.BackColor = Color.Lime;
        //            textBox36.ForeColor = Color.White;
        //            textBox36.Text = ">";
        //        }
        //        if (WSubString == "E")
        //        {
        //            textBox36.BackColor = Color.Lime;
        //            textBox36.ForeColor = Color.White;
        //            textBox36.Text = "E";
        //        }
        //    }
        //    else
        //    {
        //        labelFileF.Hide();
        //        textBox36.Hide();
        //    }
        //}


        // Trans and Journal 
        private void button6_Click(object sender, EventArgs e)
        {
            if (WAtmNo == "AB102")
            {
                MessageBox.Show("No Journal Testing Data For this ATM");
                return; 
            }

            Form62 NForm62;

            int Action = 26 ;

            string SingleChoice = WUniqueRecordId.ToString(); 

            NForm62 = new Form62(WSignedId, WSignRecordNo, WOperator, WAtmNo, WSesNo, Action,
               NullPastDate, NullPastDate, SingleChoice);
            NForm62.ShowDialog(); ;
        }
    }
}
