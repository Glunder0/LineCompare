using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using LineCompare;

namespace LineCompareBenchmark
{
    public class Benchmark
    {
        private const string FirstFileName = "1.txt";
        private const string SecondFileName = "2.txt";

        [Params(1000, 50000, 1000000)]
        public int FileLength;
        
        [Params(10, 50)]
        public int EntryCount;

        [Params(10, 50)]
        public int EntryLength;

        [ParamsSource(nameof(GetCompareMethods))]
        public string CompareMethod;

        public static IEnumerable<string> GetCompareMethods()
        {
            return ComparatorLibrary.GetComparatorNames();
        }
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            FileUtils.CreateTestFile(FirstFileName, FileLength, EntryCount, EntryLength);
            FileUtils.CreateTestFile(SecondFileName, FileLength, EntryCount, EntryLength);
        }
        
        [Benchmark]
        public int Run()
        {
            var comparator = ComparatorLibrary.GetComparator(CompareMethod, FirstFileName, SecondFileName);
            comparator.Compare().Wait();
            return comparator.GetMissingInFirst().Count() + comparator.GetMissingInSecond().Count();
        }
        
        [GlobalCleanup]
        public void GlobalCleanup()
        {
            FileUtils.DeleteTestFile(FirstFileName);
            FileUtils.DeleteTestFile(SecondFileName);
        }
    }
}