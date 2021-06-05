using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ShellProgressBar;

namespace LineCompare.Comparators
{
    public class UniqueComparator : BaseComparator
    {
        [UsedImplicitly]
        public class Creator : IComparatorCreator
        {
            public string Name => "Unique";
            public IComparator Create(string firstFile, string secondFile)
            {
                return new UniqueComparator(firstFile, secondFile);
            }
        }

        private readonly HashSet<string> _first = new ();
        private readonly HashSet<string> _second = new ();

        public UniqueComparator([NotNull] string firstFilePath, [NotNull] string secondFilePath) : base(firstFilePath, secondFilePath) { }

        protected override async Task Compare()
        {
            using var overallProgressBar = new ProgressBar(1, "Reading files", ProgressBarSettings.DefaultOptions);
            
            var firstFileBar = overallProgressBar.Spawn(1000, "First file reading", ProgressBarSettings.DefaultChildOptions);
            var firstTask = Task.Run(() =>
            {
                var progress = firstFileBar.AsProgress<float>();
                foreach (var line in FileUtils.ReadFileSequential(FirstFilePath, progress))
                {
                    _first.Add(line);
                }
                firstFileBar.Dispose();
            });

            var secondFileBar = overallProgressBar.Spawn(1000, "Second file reading", ProgressBarSettings.DefaultChildOptions);
            var secondTask = Task.Run(() =>
            {
                var progress = secondFileBar.AsProgress<float>();
                foreach (var line in FileUtils.ReadFileSequential(SecondFilePath, progress))
                {
                    _second.Add(line);
                }
                secondFileBar.Dispose();

            });

            await Task.WhenAll(firstTask, secondTask);

            overallProgressBar.Tick("Completed");
        }

        protected override IEnumerable<string> GetMissingInFirst()
        {
            return _second.Where(s => !_first.Contains(s));
        }

        protected override IEnumerable<string> GetMissingInSecond()
        {
            return _first.Where(s => !_second.Contains(s));
        }
    }
}