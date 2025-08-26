
namespace RRDM4ATMs
{
    public class RRDMInputValidationRoutines : Logger
    {
        public RRDMInputValidationRoutines() : base() { }

        // Check for Alfa Numeric
        public static bool IsAlfaNumeric(string InValue)
        {

            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
                                  (@"^[a-zA-Z0-9\s,]*$");

            if (expr.IsMatch(InValue))
            {
                return (true);
            }
            else
            {
                return (false);
            }
         
        }
        // Check for Numeric
        public static bool IsNumeric(string InValue)
        {

            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
                              (@"^[0-9]*$");

            if (expr.IsMatch(InValue))
            {
                return (true);
            }
            else
            {
                return (false);
            }

        }
        // Check for Alfa
        public static bool IsAlfa(string InValue)
        {

            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex
                               (@"^[a-zA-Z]*$");

            if (expr.IsMatch(InValue))
            {
                return (true);
            }
            else
            {
                return (false);
            }

        }
    }
}
