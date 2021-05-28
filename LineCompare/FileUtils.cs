using System;
using System.IO;
using System.Text;

namespace LineCompare
{
    public static class FileUtils
    {
        private const int FileStreamBufferSize = 1024 * 32;
        private const int StreamReaderBufferSize = 1024 * 32;
        private static readonly Encoding FileEncoding = Encoding.UTF8;

        /// <summary>
        /// Reads file at target path line by line
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="fileOpened">Callback on file open. First argument - file size</param>
        /// <param name="lineRead">Callback on line read. First argument - line itself, second argument - current file position</param>
        public static void ReadFile(string filePath, Action<long> fileOpened, Action<string, long> lineRead)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, FileStreamBufferSize, FileOptions.SequentialScan);
            using var streamReader = new StreamReader(fileStream, FileEncoding, bufferSize: StreamReaderBufferSize);

            fileOpened.Invoke(fileStream.Length);
            
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine()!; // Line should not be a null if it is not a end of stream
                lineRead?.Invoke(line, fileStream.Position);
            }
        }
    }
}