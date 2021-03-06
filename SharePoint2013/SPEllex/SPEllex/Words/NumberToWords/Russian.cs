﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SPEllex.Words.NumberToWords
{
    internal class Russian : INumberToWords
    {
        private static readonly string[,] Digits;
        private static readonly Dictionary<int, string> Exponent;
        private static readonly Dictionary<int, string> Hundreds;
        private static readonly string Minus;
        private static readonly string Sep;
        private static readonly Dictionary<int, string> Teens;
        private static readonly Dictionary<int, string> Tens;

        static Russian()
        {
            var dictionary = new Dictionary<int, string>
            {
                {0, ""},
                {6, "миллион"},
                {9, "миллиард"},
                {12, "триллион"},
                {15, "квадриллион"},
                {0x12, "квинтиллион"},
                {0x15, "секстиллион"},
                {0x18, "септиллион"},
                {0x1b, "октиллион"},
                {30, "нониллион"},
                {0x21, "дециллион"},
                {0x24, "ундециллион"},
                {0x27, "дуодециллион"},
                {0x2a, "тредециллион"},
                {0x2d, "кватуордециллион"},
                {0x30, "квиндециллион"},
                {0x33, "сексдециллион"},
                {0x36, "септендециллион"},
                {0x39, "октодециллион"},
                {60, "новемдециллион"},
                {0x3f, "вигинтиллион"},
                {0x42, "унвигинтиллион"},
                {0x45, "дуовигинтиллион"},
                {0x48, "тревигинтиллион"},
                {0x4b, "кватуорвигинтиллион"},
                {0x4e, "квинвигинтиллион"},
                {0x51, "сексвигинтиллион"},
                {0x54, "септенвигинтиллион"},
                {0x57, "октовигинтиллион"},
                {90, "новемвигинтиллион"},
                {0x5d, "тригинтиллион"},
                {0x60, "унтригинтиллион"},
                {0x63, "дуотригинтиллион"}
            };
            Exponent = dictionary;
            var dictionary2 = new Dictionary<int, string>
            {
                {11, "одиннадцать"},
                {12, "двенадцать"},
                {13, "тринадцать"},
                {14, "четырнадцать"},
                {15, "пятнадцать"},
                {0x10, "шестнадцать"},
                {0x11, "семнадцать"},
                {0x12, "восемнадцать"},
                {0x13, "девятнадцать"}
            };
            Teens = dictionary2;
            var dictionary3 = new Dictionary<int, string>
            {
                {2, "двадцать"},
                {3, "тридцать"},
                {4, "сорок"},
                {5, "пятьдесят"},
                {6, "шестьдесят"},
                {7, "семьдесят"},
                {8, "восемьдесят"},
                {9, "девяносто"}
            };
            Tens = dictionary3;
            var dictionary4 = new Dictionary<int, string>
            {
                {1, "сто"},
                {2, "двести"},
                {3, "триста"},
                {4, "четыреста"},
                {5, "пятьсот"},
                {6, "шестьсот"},
                {7, "семьсот"},
                {8, "восемьсот"},
                {9, "девятьсот"}
            };
            Hundreds = dictionary4;
            Minus = "минус";
            Sep = " ";
            Digits = new[,]
            {
                {"ноль", "одно", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять"},
                {"ноль", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять"},
                {"ноль", "одна", "две", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять"}
            };
        }

        public string ToWords(string number)
        {
            return ToWordsWithCase(number, 0, 1).Trim();
        }

        private string GroupToWords(string num, int gender, out int curcase)
        {
            string str = "";
            curcase = 3;
            int num2 = Convert.ToInt32(num);
            if (num2 == 0)
            {
                return "";
            }
            if (num2 >= 10)
            {
                while ((num.Length%3) != 0)
                {
                    num = "0" + num;
                }
                int num3 = int.Parse(num[0].ToString());
                if (num3 != 0)
                {
                    str = Hundreds[num3];
                    if (num.Substring(1) != "00")
                    {
                        str = str + Sep;
                    }
                    curcase = 3;
                }
                int num4 = int.Parse(num[1].ToString(CultureInfo.InvariantCulture));
                int num5 = int.Parse(num[2].ToString(CultureInfo.InvariantCulture));
                if ((num4 != 0) || (num5 != 0))
                {
                    if ((num4 == 1) && (num5 == 0))
                    {
                        return (str + "десять");
                    }
                    if (num4 == 1)
                    {
                        return (str + Teens[num5 + 10]);
                    }
                    if (num4 > 0)
                    {
                        str = str + Tens[num4];
                    }
                    if (num5 > 0)
                    {
                        str = str.TrimEnd(Sep.ToArray()) + Sep + Digits[gender, num5];
                    }
                    if (num5 == 1)
                    {
                        curcase = 1;
                        return str;
                    }
                    if (num5 < 5)
                    {
                        curcase = 2;
                        return str;
                    }
                    curcase = 3;
                }
                return str;
            }
            str = Digits[gender, num2];
            if (num2 == 1)
            {
                curcase = 1;
                return str;
            }
            if (num2 < 5)
            {
                curcase = 2;
                return str;
            }
            curcase = 3;
            return str;
        }

        private string ToWordsWithCase(string num, int cas, int gender = 1)
        {
            string str = "";
            cas = 3;
            num = num.Trim();
            string str2 = "";
            if (num[0] == '-')
            {
                str2 = Minus + Sep;
                num = num.Substring(1);
            }
            while ((num.Length%3) != 0)
            {
                num = "0" + num;
            }
            if (!string.IsNullOrEmpty(num) && (num != "000"))
            {
                for (int i = 0; i < num.Length; i += 3)
                {
                    int num4;
                    int num3 = 1;
                    switch (i)
                    {
                        case 0:
                            num3 = gender;
                            break;

                        case 3:
                            num3 = 2;
                            break;
                    }
                    string str3 = GroupToWords(num.Substring((num.Length - i) - 3, 3), num3, out num4);
                    switch (i)
                    {
                        case 0:
                            cas = num4;
                            break;

                        case 3:
                            switch (num4)
                            {
                                case 1:
                                    str3 = str3 + Sep + "тысяча";
                                    goto Label_015E;

                                case 2:
                                    str3 = str3 + Sep + "тысячи";
                                    goto Label_015E;
                            }
                            str3 = str3 + Sep + "тысяч";
                            goto Label_015E;
                    }
                    if ((!string.IsNullOrEmpty(str3) && (i > 3)) && Exponent.ContainsKey(i))
                    {
                        str3 = str3 + Sep + Exponent[i];
                        switch (num4)
                        {
                            case 2:
                                str3 = str3 + "а";
                                break;

                            case 3:
                                str3 = str3 + "ов";
                                break;
                        }
                    }
                    Label_015E:
                    if (!string.IsNullOrEmpty(str3))
                    {
                        str = str3 + Sep + str;
                    }
                }
            }
            else
            {
                str = str + Digits[gender, 0];
            }
            return (str2 + str);
        }
    }
}