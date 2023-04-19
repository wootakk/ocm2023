using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ISFMOCM_Project.Function
{
    public static class NumberToText
    {
        private static string[] _ones =
        {
            "សូន",
            "មួយ",
            "ពីរ",
            "បី",
            "បួន",
            "ប្រាំ",
            "ប្រាំមួយ",
            "ប្រាំពីរ",
            "ប្រាំបី",
            "ប្រាំបួន"
        };

        private static string[] _teens =
        {
            "ដប់",
            "ដប់មួយ",
            "ដប់ពីរ",
            "ដប់បី",
            "ដប់បួន",
            "ដប់ប្រាំ",
            "ដប់ប្រាំមួយ",
            "ដប់ប្រាំពីរ",
            "ដប់ប្រាំបី",
            "ដប់ប្រាំបួន"
        };

        private static string[] _tens =
        {
            "",
            "ដប់",
            "ម្ភៃ",
            "សាមសិប",
            "សែសិប",
            "ហាសិប",
            "ហុកសិប",
            "ចិតសិប",
            "ប៉ែតសិប",
            "កៅសិប"
        };

        // US Nnumbering:
        private static string[] _thousands =
        {
            "",
            "ពាន់",
            "លាន",
            "ប៊ីលាន",
            "ទ្រីលាន",
            "ក្វាទ្រីលាន"
        };

        public static string ConvertToKhmerLetter(decimal value)
        {
            string digits, temp;
            bool showThousands = false;
            bool allZeros = true;

            // Use StringBuilder to build result
            StringBuilder builder = new StringBuilder();
            // Convert integer portion of value to string
            digits = ((long)value).ToString();
            // Traverse characters in reverse order
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int ndigit = (int)(digits[i] - '0');
                int column = (digits.Length - (i + 1));

                // Determine if ones, tens, or hundreds column
                switch (column % 3)
                {
                    case 0:        // Ones position
                        showThousands = true;
                        if (i == 0)
                        {
                            // First digit in number (last in loop)
                            temp = String.Format("{0}", _ones[ndigit]);
                        }
                        else if (digits[i - 1] == '1')
                        {
                            // This digit is part of "teen" value
                            temp = String.Format("{0}", _teens[ndigit]);
                            // Skip tens position
                            i--;
                        }
                        else if (ndigit != 0)
                        {
                            // Any non-zero digit
                            temp = String.Format("{0}", _ones[ndigit]);
                        }
                        else
                        {
                            // This digit is zero. If digit in tens and hundreds
                            // column are also zero, don't show "thousands"
                            temp = String.Empty;
                            // Test for non-zero digit in this grouping
                            if (digits[i - 1] != '0' || (i > 1 && digits[i - 2] != '0'))
                                showThousands = true;
                            else
                                showThousands = false;
                        }

                        // Show "thousands" if non-zero in grouping
                        if (showThousands)
                        {
                            if (column > 0)
                            {
                                temp = String.Format("{0}{1}{2}",
                                    temp,
                                    _thousands[column / 3],
                                    allZeros ? "" : "");
                            }
                            // Indicate non-zero digit encountered
                            allZeros = false;
                        }
                        builder.Insert(0, temp);
                        break;

                    case 1:        // Tens column
                        if (ndigit > 0)
                        {
                            temp = String.Format("{0}{1}",
                                _tens[ndigit],
                                (digits[i + 1] != '0') ? "" : "");
                            builder.Insert(0, temp);
                        }
                        break;

                    case 2:        // Hundreds column
                        if (ndigit > 0)
                        {
                            temp = String.Format("{0}រយ", _ones[ndigit]);
                            builder.Insert(0, temp);
                        }
                        break;
                }
            }

            // Append fractional portion/cents
            //builder.AppendFormat("and {0:00}/100", (value - (long)value) * 100);

            // Capitalize first letter
            return String.Format("{0}{1}",
                Char.ToUpper(builder[0]),
                builder.ToString(1, builder.Length - 1)) + " រៀលគត់";
        }
    }
}