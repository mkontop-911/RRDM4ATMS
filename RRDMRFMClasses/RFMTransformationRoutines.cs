using RRDM4ATMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRDMRFMClasses
{
    public struct Extracted
    {
        public bool Success;
        public string ValueRAW;
        public string ValueExtracted;
        public string ValueType;
        public string ValueString;
        public int ValueInt;
        public decimal ValueDecimal;
        public DateTime ValueDateTime;
        public DateTime ValueDate;
        public DateTime ValueTime;
    };

    public class RFMTransformationRoutines
    {
        public static Extracted Rtn_NBG_CurrencyLookup(string Line, RRDMMappingFileFieldsFromBankToRRDM.SourceFileLayoutStruct FieldAttrib)
        {
            Extracted Xtract = new Extracted();
            Xtract.ValueRAW = "";
            Xtract.ValueExtracted = "";
            Xtract.Success = false;
            Xtract.ValueString = "";
            Xtract.ValueInt = 0;
            Xtract.ValueDecimal = 0;
            Xtract.ValueDateTime = DateTime.MinValue; /// ???

            Xtract.ValueType = FieldAttrib.TargetFieldType;
            string SrcFldVal = FieldAttrib.SourceFieldValue;

            string srcVal = "";
            string translatedlVal;

            int FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
            int FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
            if (FieldAttrib.TargetFieldNm != "NotPresent")
            {
                srcVal = Line.Substring(FieldStart, FieldLength);
                Xtract.ValueRAW = srcVal;
            }

            translatedlVal = CurrencyList.GetValue(srcVal);
            if (translatedlVal != null)
                Xtract.ValueExtracted = translatedlVal;

            return (Xtract);
        }
    }
}
