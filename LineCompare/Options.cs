using CommandLine;

namespace LineCompare
{
    public class Options
    {
        public Options(string firstFileName, string secondFileName, string compareMethod)
        {
            FirstFileName = firstFileName;
            SecondFileName = secondFileName;
            CompareMethod = compareMethod;
        }

        [Value(0, MetaName = "First", Required = true, HelpText = "First compared file.")]
        public string FirstFileName { get; }

        [Value(1, MetaName = "Second", Required = true, HelpText = "Second compared file.")]
        public string SecondFileName { get; }
        
        [Option('m', "method", HelpText = "Sets file comparison method.", Default = "Simple")]
        public string CompareMethod { get; }
    }
}