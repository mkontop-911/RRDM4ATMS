using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form2Testing : Form
    {
        int WAmount;

        public struct MapGroupFields
        {
            public int SSeqNo;
            public string SGroupDesc;
            public string SAtmNo;
            public string SStreet;
            public string STown;
            public string SDistrict;
            public string SCountry;
            public double SLatitude;
            public double SLongitude;
            public string SColorId;
            public string SColorDesc;
        };

        //MapGroupFields MapGroupFields1; // Declare 

        RRDMTempAtmLocation Tl = new RRDMTempAtmLocation(); 

        private int[] InNotesTypes = new int[4]; 

        public Form2Testing()
        {
            InitializeComponent();
        }

        private void Form2Testing_Load(object sender, EventArgs e)
        {

           
        }
        // Calculate 
        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out WAmount))
            {
            }
            else
            {
                MessageBox.Show(textBox1.Text, "Please enter a valid number!");

                return;
            }

            if (int.TryParse(textBox2.Text, out InNotesTypes[0]))
            {
            }
            else
            {
                MessageBox.Show(textBox2.Text, "Please enter a valid number!");

                return;
            }

            if (int.TryParse(textBox3.Text, out InNotesTypes[1]))
            {
            }
            else
            {
                MessageBox.Show(textBox3.Text, "Please enter a valid number!");

                return;
            }

            if (int.TryParse(textBox4.Text, out InNotesTypes[2]))
            {
            }
            else
            {
                MessageBox.Show(textBox4.Text, "Please enter a valid number!");

                return;
            }
            if (int.TryParse(textBox5.Text, out InNotesTypes[3]))
            {
            }
            else
            {
                MessageBox.Show(textBox5.Text, "Please enter a valid number!");

                return;
            }

            RRDMSpecialRoutines Csp = new RRDMSpecialRoutines(InNotesTypes);

            try
            {
                int[] StavrosResults = Csp.Calculate(WAmount);

                textBox9.Text = StavrosResults[0].ToString();
                textBox8.Text = StavrosResults[1].ToString();
                textBox7.Text = StavrosResults[2].ToString();
                textBox6.Text = StavrosResults[3].ToString();

                int temp = InNotesTypes[0] * StavrosResults[0]
                                 + InNotesTypes[1] * StavrosResults[1]
                                + InNotesTypes[2] * StavrosResults[2]
                                   + InNotesTypes[3] * StavrosResults[3];
                ;

                textBox10.Text = temp.ToString();
            

          if (temp != WAmount)
              {
               MessageBox.Show(" Wrong result ");
               }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
// TEST ARRAY
        private void button2_Click(object sender, EventArgs e)
        {
           
            Tl.ReadTempAtmLocationByGroup(27); 
            if (Tl.RecordFound == true)
            {
                
                //textBox2.Text = Cn.PublicNotesString; 
                for (int x = 0; Tl.GroupLocationArray.Count > x; x++)
                {
                    //MapGroupFields1  = ()Tl.GroupLocationArray[x];
                    string test1 = ((RRDMTempAtmLocation.MapGroupFields)Tl.GroupLocationArray[x]).SCountry;
                    string test2 = ((RRDMTempAtmLocation.MapGroupFields)Tl.GroupLocationArray[x]).SDistrict;
                    //i.deleteNote += i_deleteNote;
                    //panelNotes.Controls.Add(i);
                    //panelNotes.Controls[x].Dock = DockStyle.Top;
                }
            }

            
            //if (Cn.RecordFound == true)
            //{
            //    //textBox2.Text = Cn.PublicNotesString; 
            //    for (int x = 0; Cn.NoteControlsArray.Count > x; x++)
            //    {
            //        Control197 i = (Control197)Cn.NoteControlsArray[x];
            //        i.deleteNote += i_deleteNote;
            //        panelNotes.Controls.Add(i);
            //        panelNotes.Controls[x].Dock = DockStyle.Top;
            //    }
            //}
        }
    }
}
