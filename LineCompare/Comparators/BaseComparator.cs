using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LineCompare.Comparators
{
    public abstract class BaseComparator : IComparator
    {
        protected readonly string FirstFilePath;
        protected readonly string SecondFilePath;
        
        private bool _comparisonStarted;
        private bool _comparisonComplete;

        protected BaseComparator(string firstFilePath, string secondFilePath)
        {
            FirstFilePath = firstFilePath;
            SecondFilePath = secondFilePath;
        }

        protected abstract Task Compare();

        protected abstract IEnumerable<string> GetMissingInFirst();
        
        protected abstract IEnumerable<string> GetMissingInSecond();
        
        async Task IComparator.Compare()
        {
            if (_comparisonStarted)
            {
                throw new Exception("File comparison already started");
            }
            _comparisonStarted = true;

            await Compare();
            
            _comparisonComplete = true;
        }

        IEnumerable<string> IComparator.GetMissingInFirst()
        {
            if (!_comparisonComplete)
            {
                throw new Exception("Files is not compared, unable to return difference");
            }
            
            return GetMissingInFirst();
        }

        IEnumerable<string> IComparator.GetMissingInSecond()
        {
            if (!_comparisonComplete)
            {
                throw new Exception("Files is not compared, unable to return difference");
            }

            return GetMissingInSecond();
        }
    }
}