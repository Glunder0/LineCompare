using BenchmarkDotNet.Running;

namespace LineCompareBenchmark
{
    static class Program
    {
        private static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}