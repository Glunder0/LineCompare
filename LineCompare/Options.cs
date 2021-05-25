using CommandLine;

namespace LineCompare
{
    public class Options
    {
        public Options(string firstFileName, string secondFileName)
        {
            FirstFileName = firstFileName;
            SecondFileName = secondFileName;
        }

        [Value(0, MetaName = "First", Required = true, HelpText = "First compared file")]
        public string FirstFileName { get; }

        [Value(1, MetaName = "Second", Required = true, HelpText = "Second compared file")]
        public string SecondFileName { get; }
    }
}