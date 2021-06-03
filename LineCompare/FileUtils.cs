using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LineCompare
{
    public static class FileUtils
    {
        private const int FileStreamBufferSize = 1024 * 32;
        private const int StreamReaderBufferSize = 1024 * 32;
        private static readonly Encoding FileEncoding = Encoding.UTF8;

        public static IEnumerable<string> ReadFileSequential(string filePath, IProgress<float>? readProgress = null)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, FileStreamBufferSize, FileOptions.SequentialScan);
            using var streamReader = new StreamReader(fileStream, FileEncoding, bufferSize: StreamReaderBufferSize);
            
            while (!streamReader.EndOfStream)
            {
                readProgress?.Report((float)fileStream.Position / fileStream.Length);
                yield return streamReader.ReadLine()!; // Line should not be a null if it is not a end of stream
            }
        }
    }
}