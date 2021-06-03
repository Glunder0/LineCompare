using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ShellProgressBar;

namespace LineCompare.Comparators
{
    /// <summary>
    /// Compares two files ignoring line order
    /// </summary>
    public sealed class FileComparator : BaseComparator
    {
        [UsedImplicitly]
        public class Creator : IComparatorCreator
        {
            public string Name => "Simple";
            public IComparator Create(string firstFile, string secondFile)
            {
                return new FileComparator(firstFile, secondFile);
            }
        }

        private readonly Dictionary<string, int> _first = new();
        private readonly Dictionary<string, int> _second = new();

        private readonly bool _fastLookup;
        
        public FileComparator(string firstFilePath, string secondFilePath, bool fastLookup = true) : base(firstFilePath, secondFilePath)
        {
            _fastLookup = fastLookup;
        }

        private static void AddEntry(IDictionary<string, int> dictionary, string entry)
        {
            if (dictionary.TryGetValue(entry, out var value))
            {
                dictionary[entry] = value + 1;
            }
            else
            {
                dictionary[entry] = 1;
            }
        }

        protected override async Task Compare()
        {
            using var overallProgressBar = new ProgressBar(_fastLookup ? 2 : 1, "Reading files", ProgressBarSettings.DefaultOptions);
            
            var firstFileBar = overallProgressBar.Spawn(1000, "First file reading", ProgressBarSettings.DefaultChildOptions);
            var firstTask = Task.Run(() =>
            {
                var progress = firstFileBar.AsProgress<float>();
                foreach (var line in FileUtils.ReadFileSequential(FirstFilePath, progress))
                {
                    AddEntry(_first, line);
                }
                firstFileBar.Dispose();
            });

            var secondFileBar = overallProgressBar.Spawn(1000, "Second file reading", ProgressBarSettings.DefaultChildOptions);
            var secondTask = Task.Run(() =>
            {
                var progress = secondFileBar.AsProgress<float>();
                foreach (var line in FileUtils.ReadFileSequential(SecondFilePath, progress))
                {
                    AddEntry(_second, line);
                }
                secondFileBar.Dispose();

            });

            await Task.WhenAll(firstTask, secondTask);

            if (_fastLookup)
            {
                overallProgressBar.Tick("Comparing files");
                var compareBar = overallProgressBar.Spawn(_first.Count, "Comparing", ProgressBarSettings.DefaultChildOptions);

                foreach (var (line, firstCount) in _first)
                {
                    if (_second.TryGetValue(line, out var secondCount))
                    {
                        var count = secondCount - firstCount;

                        if (count == 0)
                        {
                            _second.Remove(line);
                        }
                        else
                        {
                            _second[line] = count;
                        }
                    }
                    compareBar.Tick();
                }
            }
            
            overallProgressBar.Tick("Completed");
        }

        private static IEnumerable<string> GetMissingInDictionary(IReadOnlyDictionary<string, int> dictionary, IReadOnlyDictionary<string, int> other)
        {
            return other.SelectMany(pair => Enumerable.Repeat(pair.Key, dictionary.TryGetValue(pair.Key, out var existing) ? Math.Max(0, pair.Value - existing) : pair.Value));
        }

        protected override IEnumerable<string> GetMissingInFirst()
        {
            return _fastLookup
                ? _second.SelectMany(pair => Enumerable.Repeat(pair.Key, pair.Value > 0 ? pair.Value : 0))
                : GetMissingInDictionary(_first, _second);
        }

        protected override IEnumerable<string> GetMissingInSecond()
        {
            return _fastLookup
                ? _second.SelectMany(pair => Enumerable.Repeat(pair.Key, pair.Value < 0 ? -pair.Value : 0))
                : GetMissingInDictionary(_second, _first);
        }
    }
}