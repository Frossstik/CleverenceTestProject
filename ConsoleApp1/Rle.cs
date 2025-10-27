using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class Rle
    {
        public static string Compress(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var sb = new StringBuilder();
            char curr = input[0];
            int count = 1;

            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == curr)
                {
                    count++;
                }
                else
                {
                    sb.Append(curr);
                    if (count > 1) sb.Append(count);
                    curr = input[i];
                    count = 1;
                }
            }

            sb.Append(curr);
            if (count > 1) sb.Append(count);

            return sb.ToString();
        }

        public static string Decompress(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var sb = new StringBuilder();
            int i = 0;

            while (i < input.Length)
            {
                char ch = input[i];

                if (!char.IsLetter(ch))
                    throw new ArgumentException("Некорректный формат");

                i++;

                int count = 0;
                while (i < input.Length && char.IsDigit(input[i]))
                {
                    checked
                    {
                        count = count * 10 + (input[i] - '0');
                    }
                    i++;
                }

                if (count == 0) count = 1;

                sb.Append(new string(ch, count));
            }

            return sb.ToString();
        }
    }
}
