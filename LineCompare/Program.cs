﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            var progressTracker = new ProgressTracker();

            var comparator = new FileComparator(options.FirstFileName, options.SecondFileName);
            var compareTask = comparator.Compare(progressTracker);
            WaitAndReport(compareTask, progressTracker);

            var diff = new DiffReporter(comparator.GetMissingInFirst(), comparator.GetMissingInSecond());
            var report = diff.Report();

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

        /// <summary>
        /// Continuously prints progress to the console while task is running
        /// </summary>
        /// <param name="taskToWait">Awaited task</param>
        /// <param name="progressToReport">Progress to display in the console</param>
        private static void WaitAndReport(Task taskToWait, ProgressTracker progressToReport)
        {
            var checkDelay = 100;
            var maxReports = Console.IsOutputRedirected ? 10 : 10000; // Don't spam to the output if it is not a console window
            var prevProgress = 0.0f;

            if (!Console.IsOutputRedirected)
            {
                Console.WriteLine();
            }
            
            while (!taskToWait.IsCompleted)
            {
                var progress = progressToReport.Progress;
                if (progress - prevProgress < 1.0f / maxReports)
                {
                    continue;
                }
                prevProgress = progress;

                if (Console.IsOutputRedirected)
                {
                    Console.WriteLine($"Progress: {progress:P2}");
                }
                else
                {
                    Console.Write($"\rProgress: {progress:P2}");
                }

                Thread.Sleep(checkDelay);
            }
            
            if (Console.IsOutputRedirected)
            {
                Console.WriteLine($"Progress: {1.0f:P2}");
            }
            else
            {
                Console.WriteLine($"\rProgress: {1.0f:P2}");
            }
        }
    }
}