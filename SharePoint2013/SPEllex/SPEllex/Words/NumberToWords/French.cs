using System.Collections.Generic;

namespace SPEllex.Words.NumberToWords
{
    internal class French : INumberToWords
    {
        private static readonly string _and = "et";
        private static readonly string _dash = "-";
        private static readonly string[] _digits;
        private static readonly Dictionary<int, string> _exponent;
        private static readonly string _infinity = "infini";
        private static readonly string _minus = "moins";
        private static readonly Dictionary<int, string> _miscNumbers;
        private static readonly string _plural = "s";
        private static readonly string _sep = " ";
        private static readonly string _zero = "z\x00e9ro";

        static French()
        {
            var dictionary = new Dictionary<int, string>
            {
                {10, "dix"},
                {11, "onze"},
                {12, "douze"},
                {13, "treize"},
                {14, "quatorze"},
                {15, "quinze"},
                {0x10, "seize"},
                {20, "vingt"},
                {30, "trente"},
                {40, "quarante"},
                {50, "cinquante"},
                {60, "soixante"},
                {100, "cent"}
            };
            _miscNumbers = dictionary;
            _digits = new[] {"", "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf"};
            var dictionary2 = new Dictionary<int, string>
            {
                {0, ""},
                {3, "mille"},
                {6, "million"},
                {9, "milliard"},
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
            _exponent = dictionary2;
        }

        public string ToWords(string num)
        {
            if (string.IsNullOrEmpty(num))
            {
                return _zero;
            }
            num = num.Trim();
            if (int.Parse(num) == 0)
            {
                return _zero;
            }
            string str = "";
            if (num[0] == '-')
            {
                str = _minus + _sep;
                num = num.Substring(1);
            }
            num = num.TrimStart(new[] {'0'});
            string[] strArray = splitNumber(num);
            for (int i = 0; i < strArray.Length; i++)
            {
                int num3 = strArray.Length - i;
                string s = strArray[i];
                int num4 = int.Parse(s);
                if (s != "000")
                {
                    if ((num4 != 1) || (num3 != 2))
                    {
                        str = str + showDigitsGroup(num4, (i + 1) == strArray.Length) + _sep;
                    }
                    str = str + _exponent[(num3 - 1)*3];
                    if ((num3 > 2) && (num4 > 1))
                    {
                        str = str + _plural;
                    }
                    str = str + _sep;
                }
            }
            return str.TrimEnd(_sep.ToCharArray());
        }

        private string showDigitsGroup(int num, bool isLast = false)
        {
            string str = "";
            int index = num%10;
            int num3 = ((num - index)%100)/10;
            int num4 = (((num - (num3*10)) - index)%0x3e8)/100;
            if (num4 != 0)
            {
                if (num4 > 1)
                {
                    str = str + _digits[num4] + _sep + _miscNumbers[100];
                    if ((isLast && (index == 0)) && (num3 == 0))
                    {
                        str = str + _plural;
                    }
                }
                else
                {
                    str = str + _miscNumbers[100];
                }
                str = str + _sep;
            }
            if (num3 != 0)
            {
                if (num3 == 1)
                {
                    if (index <= 6)
                    {
                        str = str + _miscNumbers[10 + index];
                    }
                    else
                    {
                        str = str + _miscNumbers[10] + "-" + _digits[index];
                    }
                    index = 0;
                }
                else if (num3 > 5)
                {
                    if (num3 < 8)
                    {
                        str = str + _miscNumbers[60];
                        int num5 = ((num3*10) + index) - 60;
                        if (index == 1)
                        {
                            str = str + _sep + _and + _sep;
                        }
                        else if (num5 != 0)
                        {
                            str = str + _dash;
                        }
                        if (num5 != 0)
                        {
                            str = str + showDigitsGroup(num5, false);
                        }
                        index = 0;
                    }
                    else
                    {
                        str = str + _digits[4] + _dash + _miscNumbers[20];
                        int num6 = ((num3*10) + index) - 80;
                        if (num6 != 0)
                        {
                            str = str + _dash + showDigitsGroup(num6, false);
                            index = 0;
                        }
                        else
                        {
                            str = str + _plural;
                        }
                    }
                }
                else
                {
                    str = str + _miscNumbers[num3*10];
                }
            }
            if (index != 0)
            {
                if (num3 != 0)
                {
                    if (index == 1)
                    {
                        str = str + _sep + _and + _sep;
                    }
                    else
                    {
                        str = str + _dash;
                    }
                }
                str = str + _digits[index];
            }
            return str.TrimEnd(_sep.ToCharArray());
        }

        private string[] splitNumber(string num)
        {
            var list = new List<string>();
            while ((num.Length%3) != 0)
            {
                num = "0" + num;
            }
            for (int i = num.Length; i > 0; i -= 3)
            {
                list.Add(string.Format("{0}{1}{2}", num[i - 3], num[i - 2], num[i - 1]));
            }
            list.Reverse();
            return list.ToArray();
        }
    }
}