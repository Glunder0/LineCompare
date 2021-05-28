using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace LineCompare.Comparators
{
    /// <summary>
    /// Compares two files ignoring line order
    /// </summary>
    public sealed class FileComparator : IComparator
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
        
        private readonly string _firstFilePath;
        private readonly string _secondFilePath;
        
        private bool _comparisonStarted;
        private bool _comparisonComplete;

        private long _firstFileSize;
        private long _secondFileSize;

        private long _firstFilePosition;
        private long _secondFilePosition;
        
        private readonly Dictionary<string, int> _first = new();
        private readonly Dictionary<string, int> _second = new();

        public FileComparator(string firstFilePath, string secondFilePath)
        {
            _firstFilePath = firstFilePath;
            _secondFilePath = secondFilePath;
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

        private void UpdateProgress(IProgress<float> progress)
        {
            var size = _firstFileSize + _secondFileSize;
            if (size == 0)
            {
                progress.Report(0);
            }
            else
            {
                progress.Report((float)(_firstFilePosition + _secondFilePosition) / size);
            }
        }

        public async Task Compare(IProgress<float> progress)
        {
            if (_comparisonStarted)
            {
                throw new Exception("File comparison already started");
            }
            _comparisonStarted = true;

            var firstTask = Task.Run(
                () => FileUtils.ReadFile(_firstFilePath, 
                    fileSize => _firstFileSize = fileSize, 
                    (line, filePosition) =>
                    {
                        AddEntry(_first, line);
                        _firstFilePosition = filePosition;
                        UpdateProgress(progress);
                    }));

            var secondTask = Task.Run(
                () => FileUtils.ReadFile(_secondFilePath, 
                    fileSize => _secondFileSize = fileSize, 
                    (line, filePosition) =>
                    {
                        AddEntry(_second, line);
                        _secondFilePosition = filePosition;
                        UpdateProgress(progress);
                    }));

            await Task.WhenAll(firstTask, secondTask);
            
            _comparisonComplete = true;
        }

        public IEnumerable<string> GetMissingInFirst()
        {
            if (!_comparisonComplete)
            {
                throw new Exception("Files is not compared, unable to return difference");
            }

            return _second.SelectMany(pair => Enumerable.Repeat(pair.Key,
                _first.TryGetValue(pair.Key, out var existing) ? Math.Max(0, pair.Value - existing) : pair.Value));
        }

        public IEnumerable<string> GetMissingInSecond()
        {
            if (!_comparisonComplete)
            {
                throw new Exception("Files is not compared, unable to return difference");
            }
            
            return _first.SelectMany(pair => Enumerable.Repeat(pair.Key,
                _second.TryGetValue(pair.Key, out var existing) ? Math.Max(0, pair.Value - existing) : pair.Value));
        }
    }
}