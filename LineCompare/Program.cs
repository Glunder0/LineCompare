using System;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using LineCompare.Comparators;

namespace LineCompare
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var parser = new Parser(settings => settings.HelpWriter = null);
            var parserResult = parser.ParseArguments<Options>(args);
            return parserResult.MapResult(Compare, _ => DisplayHelp(parserResult));
        }

        private static int Compare(Options options)
        {
            if (!File.Exists(options.FirstFileName) || !File.Exists(options.SecondFileName))
            {
                Console.Error.WriteLine("Target file is missing");
                return 2;
            }

            var comparator = new FileComparator(options.FirstFileName, options.SecondFileName) as IComparator;

            comparator.Compare().Wait();

            var diff = new DiffReporter(comparator.GetMissingInFirst(), comparator.GetMissingInSecond());
            var report = diff.ReducedReport();

            if (!string.IsNullOrEmpty(report))
            {
                Console.WriteLine("Difference found:");
                Console.WriteLine(report);
                return 1;
            }

            Console.WriteLine("No difference was found");
            return 0;
        }

        private static int DisplayHelp(ParserResult<Options> parserResult)
        {
            var helpText = HelpText.AutoBuild(parserResult, 
                h =>
                {
                    h.AutoHelp = false;
                    h.AddPostOptionsLines(ComparatorLibrary.GetComparatorNames().Prepend("Available file comparison methods:"));
                    return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                }, e => e);
            Console.Error.WriteLine(helpText);
            return 1;
        }
    }
}