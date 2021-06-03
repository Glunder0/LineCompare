using System;
using System.Collections.Generic;
using System.Text;

namespace LineCompare
{
    public class DiffReporter
    {
        private readonly IEnumerable<string> _missingInFirst;
        private readonly IEnumerable<string> _missingInSecond;
        
        public DiffReporter(IEnumerable<string> missingInFirst, IEnumerable<string> missingInSecond)
        {
            _missingInFirst = missingInFirst ?? throw new ArgumentNullException(nameof(missingInFirst));
            _missingInSecond = missingInSecond ?? throw new ArgumentNullException(nameof(missingInSecond));
        }
        
        /// <summary>
        /// Creates report of the difference
        /// </summary>
        /// <param name="maxLines">Maximum amount of lines to include in the report</param>
        /// <returns></returns>
        public string ReducedReport(int maxLines = 10)
        {
            var stringBuilder = new StringBuilder();

            WriteToStringBuilder(stringBuilder, _missingInFirst, maxLines, "Lines missing in first file:");
            WriteToStringBuilder(stringBuilder, _missingInSecond, maxLines, "Lines missing in second file:");

            return stringBuilder.ToString();
        }

        private static void WriteToStringBuilder(StringBuilder stringBuilder, IEnumerable<string> elements, int maxCount, string header)
        {
            var itemCount = 0;
            foreach (var line in elements)
            {
                if (itemCount == 0)
                {
                    stringBuilder.Append($"# {header}\n");
                }

                if (++itemCount > maxCount)
                {
                    stringBuilder.Append("# and more;\n");
                    break;
                }

                stringBuilder.Append(line);
                stringBuilder.Append('\n');
            }
        }
    }
}