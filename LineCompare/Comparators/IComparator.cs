using System.Collections.Generic;
using System.Threading.Tasks;

namespace LineCompare.Comparators
{
    public interface IComparator
    {
        /// <summary>
        /// Loads and compares files
        /// </summary>
        /// <returns></returns>
        Task Compare();
        
        /// <summary>
        /// Returns lines which is present in the second file but missing in the first file
        /// </summary>
        /// <returns>Unmatched lines from the second file</returns>
        IEnumerable<string> GetMissingInFirst();
        
        /// <summary>
        /// Returns lines which is present in the first file but missing in the second file
        /// </summary>
        /// <returns>Unmatched lines from the first file</returns>
        IEnumerable<string> GetMissingInSecond();
    }
}