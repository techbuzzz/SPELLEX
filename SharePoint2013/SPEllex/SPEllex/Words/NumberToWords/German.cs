using System.Collections.Generic;

namespace SPEllex.Words.NumberToWords
{
    internal class German : INumberToWords
    {
        private static readonly string[] _digits;
        private static readonly Dictionary<int, string[]> _exponent;
        private static readonly string _minus = "Minus";
        private static readonly string _sep;
        private static readonly string _sep2;

        static German()
        {
            var dictionary = new Dictionary<int, string[]>
            {
                {0, new[] {"", ""}},
                {3, new[] {"tausend", "tausend"}},
                {6, new[] {"Million", "Millionen"}},
                {9, new[] {"Milliarde", "Milliarden"}},
                {12, new[] {"Billion", "Billionen"}},
                {15, new[] {"Billiarde", "Billiarden"}},
                {0x12, new[] {"Trillion", "Trillionen"}},
                {0x15, new[] {"Trilliarde", "Trilliarden"}},
                {0x18, new[] {"Quadrillion", "Quadrillionen"}},
                {0x1b, new[] {"Quadrilliarde", "Quadrilliarden"}},
                {30, new[] {"Quintillion", "Quintillionen"}},
                {0x21, new[] {"Quintilliarde", "Quintilliarden"}},
                {0x24, new[] {"Sextillion", "Sextillionen"}},
                {0x27, new[] {"Sextilliarde", "Sextilliarden"}},
                {0x2a, new[] {"Septillion", "Septillionen"}},
                {0x2d, new[] {"Septilliarde", "Septilliarden"}},
                {0x30, new[] {"Oktillion", "Oktillionen"}},
                {0x33, new[] {"Oktilliarde", "Oktilliarden"}},
                {0x36, new[] {"Nonillion", "Nonillionen"}},
                {0x39, new[] {"Nonilliarde", "Nonilliarden"}},
                {60, new[] {"Dezillion", "Dezillionen"}},
                {0x3f, new[] {"Dezilliarde", "Dezilliarden"}},
                {120, new[] {"Vigintillion", "Vigintillionen"}}
            };
            _exponent = dictionary;
            _digits = new[] {"null", "ein", "zwei", "drei", "vier", "f\x00fcnf", "sechs", "sieben", "acht", "neun"};
            _sep = "";
            _sep2 = " ";
        }

        public string ToWords(string num)
        {
            return ToWords(num, 0, "").Trim();
        }

        private static string ToWords(string num, int power, string powsuffix)
        {
            string str = "";
            num = num.Trim();
            if (num[0] == '-')
            {
                str = _sep + _minus;
                num = num.Substring(1);
            }
            num = num.TrimStart(new[] {'0'});
            if (num.Length > 3)
            {
                int num2 = num.Length - 1;
                int num3 = num2;
                for (int i = num2; i > 0; i--)
                {
                    if (_exponent.ContainsKey(i))
                    {
                        string str2 = num.Substring(num2 - num3, (num3 - i) + 1).TrimStart(new[] {'0'});
                        if (!string.IsNullOrEmpty(str2))
                        {
                            string str3 = _exponent[power][1];
                            if (!string.IsNullOrEmpty(powsuffix))
                            {
                                str3 = str3 + _sep + powsuffix;
                            }
                            str = str + ToWords(str2, i, str3);
                        }
                        num3 = i - 1;
                    }
                }
                num = num.Substring(num2 - num3, num3 + 1);
                if (string.IsNullOrEmpty(num))
                {
                    return str;
                }
            }
            else if (string.IsNullOrEmpty(num))
            {
                return (_sep + _digits[0]);
            }
            int index = 0;
            int num6 = 0;
            int num7 = 0;
            switch (num.Length)
            {
                case 0:
                    return "";

                case 1:
                    goto Label_0179;

                case 2:
                    break;

                case 3:
                    index = int.Parse(num.Substring(num.Length - 3, 1));
                    break;

                default:
                    goto Label_018F;
            }
            num6 = int.Parse(num.Substring(num.Length - 2, 1));
            Label_0179:
            num7 = int.Parse(num.Substring(num.Length - 1, 1));
            Label_018F:
            if (index != 0)
            {
                string str4 = str;
                str = str4 + _sep + _digits[index] + _sep + "hundert";
            }
            if ((num6 != 1) && (num7 > 0))
            {
                if (num6 > 0)
                {
                    str = str + _digits[num7] + "und";
                }
                else
                {
                    str = str + _digits[num7];
                    if (num7 == 1)
                    {
                        if (power == 0)
                        {
                            str = str + "s";
                        }
                        else if (power != 3)
                        {
                            str = str + "e";
                        }
                    }
                }
            }
            switch (num6)
            {
                case 1:
                    switch (num7)
                    {
                        case 0:
                            str = str + _sep + "zehn";
                            break;

                        case 1:
                            str = str + _sep + "elf";
                            break;

                        case 2:
                            str = str + _sep + "zw\x00f6lf";
                            break;

                        case 3:
                        case 4:
                        case 5:
                        case 8:
                        case 9:
                            str = str + _sep + _digits[num7] + "zehn";
                            break;

                        case 6:
                            str = str + _sep + "sechzehn";
                            break;

                        case 7:
                            str = str + _sep + "siebzehn";
                            break;
                    }
                    break;

                case 2:
                    str = str + _sep + "zwanzig";
                    break;

                case 3:
                    str = str + _sep + "drei\x00dfig";
                    break;

                case 4:
                    str = str + _sep + "vierzig";
                    break;

                case 5:
                case 8:
                case 9:
                    str = str + _sep + _digits[num6] + "zig";
                    break;

                case 6:
                    str = str + _sep + "sechzig";
                    break;

                case 7:
                    str = str + _sep + "siebzig";
                    break;
            }
            if (power > 0)
            {
                string[] strArray = null;
                if (!_exponent.TryGetValue(power, out strArray))
                {
                    return null;
                }
                if (power == 3)
                {
                    str = str + _sep + strArray[0];
                }
                else if ((num7 == 1) && ((num6 + index) == 0))
                {
                    str = str + _sep2 + strArray[0] + _sep2;
                }
                else
                {
                    str = str + _sep2 + strArray[1] + _sep2;
                }
            }
            if (!string.IsNullOrEmpty(powsuffix))
            {
                str = str + _sep + powsuffix;
            }
            return str;
        }
    }
}