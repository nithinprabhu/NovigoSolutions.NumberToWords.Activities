using System;
using System.Activities;
using System.ComponentModel;

namespace NovigoSolutions.NumberToWords.Activities
{
    [Category("Novigo Solutions")]
    [DisplayName("Number To Words")]
    public class NumberToWords : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        [Description("Number to be expressed in Words")]
        [DisplayName("Number")]
        public InArgument<decimal> Number { get; set; }

        [Category("Options")]
        [Description("Enter Currency Name")]
        [DisplayName("Currency")]
        public InArgument<String> Currency { get; set; }

        [Category("Options")]
        [Description("Enter Currency Subdivision Name")]
        [DisplayName("Currency Subdivision")]
        public InArgument<String> CurrencySubdivision { get; set; }

        [Category("Output")]
        [DisplayName("Words")]
        [Description("Represents Number converted to Words")]
        public OutArgument<String> Words { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            String numberStr = this.Number.Get(context).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            String[] numParts = numberStr.Split('.');
            Int64 n1 = Int64.Parse(numParts[0]);
            Int64 n2 = Int64.Parse(numParts[1]);

            String currency = "";
            if (this.Currency.Get(context) == null)
            {
                currency = GetWords(n1);
            } else
            {
                currency = GetWords(n1) + " " + this.Currency.Get(context);
            }

            String currencySubdivision = "";
            String currencySubdivisionRaw = GetWords(n2);

            if (this.CurrencySubdivision.Get(context) == null & !currencySubdivisionRaw.Equals("Zero"))
            {
                currencySubdivision = " and " + currencySubdivisionRaw;
            }
            else if (!currencySubdivisionRaw.Equals("Zero"))
            {
                currencySubdivision = " and " + currencySubdivisionRaw + " " + this.CurrencySubdivision.Get(context);
            }
            
            Words.Set(context, currency + currencySubdivision);
        }

        private static string GetWords(Int64 numbers)
        {
            Int64 number = numbers;

            if (number == 0) return "Zero";
            Int64[] num = new Int64[4];
            Int64 first = 0;
            Int64 units, hundreds, tens;
            System.Text.StringBuilder strb = new System.Text.StringBuilder();
            if (number < 0)
            {
                strb.Append("Minus ");
                number = -number;
            }
            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ", "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ","Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };
            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (Int64 i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                units = num[i] % 10; // ones
                tens = num[i] / 10;
                hundreds = num[i] / 100; // hundreds
                tens = tens - 10 * hundreds; // tens
                if (hundreds > 0) strb.Append(words0[hundreds] + "Hundred ");
                if (units > 0 || tens > 0)
                {
                    if (tens == 0)
                        strb.Append(words0[units]);
                    else if (tens == 1)
                        strb.Append(words1[units]);
                    else
                        strb.Append(words2[tens - 2] + words0[units]);
                }
                if (i != 0) strb.Append(words3[i - 1]);
            }
            return strb.ToString().TrimEnd();
        }
    }
}