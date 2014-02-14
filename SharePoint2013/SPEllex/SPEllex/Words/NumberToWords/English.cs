using System;
using System.Collections.Generic;

namespace SPEllex.Words.NumberToWords
{
    internal class English : INumberToWords
    {
        private const string Minus = "minus";
        private static readonly string[] Digits;
        private static readonly Dictionary<int, string> Exponent;
        private static readonly string Sep;

        static English()
        {
            var dictionary = new Dictionary<int, string>
            {
                {0, ""},
                {3, "thousand"},
                {6, "million"},
                {9, "billion"},
                {12, "trillion"},
                {15, "quadrillion"},
                {0x12, "quintillion"},
                {0x15, "sextillion"},
                {0x18, "septillion"},
                {0x1b, "octillion"},
                {30, "nonillion"},
                {0x21, "decillion"},
                {0x24, "undecillion"},
                {0x27, "duodecillion"},
                {0x2a, "tredecillion"},
                {0x2d, "quattuordecillion"},
                {0x30, "quindecillion"},
                {0x33, "sexdecillion"},
                {0x36, "septendecillion"},
                {0x39, "octodecillion"},
                {60, "novemdecillion"},
                {0x3f, "vigintillion"},
                {0x42, "unvigintillion"},
                {0x45, "duovigintillion"},
                {0x48, "trevigintillion"},
                {0x4b, "quattuorvigintillion"},
                {0x4e, "quinvigintillion"},
                {0x51, "sexvigintillion"},
                {0x54, "septenvigintillion"},
                {0x57, "octovigintillion"},
                {90, "novemvigintillion"},
                {0x5d, "trigintillion"},
                {0x60, "untrigintillion"},
                {0x63, "duotrigintillion"}
            };
            Exponent = dictionary;
            Digits = new[] {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"};
            Sep = " ";
        }

        public string ToWords(string number)
        {
            return ToWords(number, 0, "").Trim();
        }

        public string ToWords(string number, int power, string powsuffix)
        {
            string str = "";
            number = number.Trim();
            if (number[0] == '-')
            {
                str = Sep + Minus;
                number = number.Substring(1);
            }
            number = number.TrimStart(new[] {'0'});
            if (number.Length > 3)
            {
                int num = number.Length - 1;
                int num2 = num;
                for (int i = num; i > 0; i--)
                {
                    if (Exponent.ContainsKey(i))
                    {
                        string str2 = number.Substring(num - num2, (num2 - i) + 1).TrimStart(new[] {'0'});
                        if (!string.IsNullOrEmpty(str2))
                        {
                            string str3 = Exponent[power];
                            if (!string.IsNullOrEmpty(powsuffix))
                            {
                                str3 = Sep + powsuffix;
                            }
                            str = str + ToWords(str2, i, str3);
                        }
                        num2 = i - 1;
                    }
                }
                number = number.Substring(num - num2, num2 + 1);
                if (string.IsNullOrEmpty(number))
                {
                    return str;
                }
            }
            else if (string.IsNullOrEmpty(number))
            {
                return (Sep + Digits[0]);
            }
            int index = 0;
            int num5 = 0;
            int num6 = 0;
            switch (number.Length)
            {
                case 0:
                    return "";

                case 1:
                    goto Label_017D;

                case 2:
                    break;

                case 3:
                    index = Convert.ToInt32(number.Substring(number.Length - 3, 1));
                    break;

                default:
                    goto Label_0193;
            }
            num5 = Convert.ToInt32(number.Substring(number.Length - 2, 1));
            Label_017D:
            num6 = Convert.ToInt32(number.Substring(number.Length - 1, 1));
            Label_0193:
            if (index != 0)
            {
                string str4 = str;
                str = str4 + Sep + Digits[index] + Sep + "hundred";
            }
            switch (num5)
            {
                case 1:
                    switch (num6)
                    {
                        case 0:
                            str = str + Sep + "ten";
                            break;

                        case 1:
                            str = str + Sep + "eleven";
                            break;

                        case 2:
                            str = str + Sep + "twelve";
                            break;

                        case 3:
                            str = str + Sep + "thirteen";
                            break;

                        case 4:
                        case 6:
                        case 7:
                        case 9:
                            str = str + Sep + Digits[num6] + "teen";
                            break;

                        case 5:
                            str = str + Sep + "fifteen";
                            break;

                        case 8:
                            str = str + Sep + "eighteen";
                            break;
                    }
                    break;

                case 2:
                    str = str + Sep + "twenty";
                    break;

                case 3:
                    str = str + Sep + "thirty";
                    break;

                case 4:
                    str = str + Sep + "forty";
                    break;

                case 5:
                    str = str + Sep + "fifty";
                    break;

                case 6:
                case 7:
                case 9:
                    str = str + Sep + Digits[num5] + "ty";
                    break;

                case 8:
                    str = str + Sep + "eighty";
                    break;
            }
            if ((num5 != 1) && (num6 > 0))
            {
                if (num5 > 1)
                {
                    str = str + "-" + Digits[num6];
                }
                else
                {
                    str = str + Sep + Digits[num6];
                }
            }
            if (power > 0)
            {
                if (!Exponent.ContainsKey(power))
                {
                    return null;
                }
                str = str + Sep + Exponent[power];
            }
            if (!string.IsNullOrEmpty(powsuffix))
            {
                str = str + Sep + powsuffix;
            }
            return str;
        }
    }
}