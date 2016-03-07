using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMSpecialRoutines
    {
       private int[] TypeOfNotes;
        private int tempAmount=0;

        private int[] SortedResults;
        private int[] Results;

     //   public int[] StavrosResults;

        public RRDMSpecialRoutines(int[] TypeNotes)
        {
            TypeOfNotes = TypeNotes;
            SortedResults = new int[TypeOfNotes.Length];
            Results = new int[TypeOfNotes.Length];
        }

        public int[] Calculate(int amount)
        {
            int[] TypeNotesSorted = new int[TypeOfNotes.Length];
            TypeOfNotes.CopyTo(TypeNotesSorted,0);
            Array.Sort(TypeNotesSorted);

            //Check if the amount can be divided by any type of notes in the cassette
            /*bool PossibleDivision = false;
            foreach (int value in TypeNotesSorted)
            {
                if (amount% value == 0)
                {
                    if (!PossibleDivision)
                    {
                        PossibleDivision = true;
                    }
                }
            }

            if (!PossibleDivision)
            {
                //throw new Exception("No possible combination");
            }
            */

            //Allocate amount
            for (int x = TypeNotesSorted.Length-1; x>=0; x--)
            {
                int revisedAmount = amount - tempAmount;

                int tempDivision = revisedAmount / TypeNotesSorted[x];

                tempAmount = tempAmount+TypeNotesSorted[x] * tempDivision;
                SortedResults[x] = tempDivision;
            }

            //Check if remainder remains...If yes then add back last type of note and divide with next... if still remainder then not possible combination?
            if (amount != tempAmount)
            {
                for (int x = 1; x <= SortedResults[1]; x++)
                {
                    int revisedAmount = (amount - tempAmount) + TypeNotesSorted[1]*x;

                    if (revisedAmount % TypeNotesSorted[0] == 0)
                    {
                        SortedResults[0] = revisedAmount / TypeNotesSorted[0];
                        if (SortedResults[1] > 0)
                        {
                            SortedResults[1] = SortedResults[1] - x;
                        }
                        tempAmount = amount;
                        break;
                    }
                }
                if (amount != tempAmount)
                {
                      throw new Exception("No possible combination");
                 //     MessageBox.Show("No possible combination");
                }
            }

            //Sort results array
            for(int x=0;x<SortedResults.Length;x++)
            {
                int index = Array.IndexOf(TypeOfNotes, TypeNotesSorted[x]);

                Results[index] = Results[index]+SortedResults[x];
            }
         
            return Results;
        }
    }
}
