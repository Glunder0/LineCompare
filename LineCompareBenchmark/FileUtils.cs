using System;
using System.Collections.Generic;
using System.IO;

namespace LineCompareBenchmark
{
    public static class FileUtils
    {
        private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private static readonly Random Rnd = new Random();

        private static char GenerateChar()
        {
            return Chars[Rnd.Next(Chars.Length)];
        }
        
        private static string GenerateLine(int entryCount, int entryLength)
        {
            var count = entryCount * (entryLength + 1) - 1;
            var arr = new char[count];
            for (var i = 0; i < count; i++)
            {
                arr[i] = (i + 1) % (entryLength + 1) == 0 ? ',' : GenerateChar();
            }
            return new string(arr);
        }

        private static IEnumerable<string> GenerateFile(int length, int entryCount, int entryLength)
        {
            for (var i = 0; i < length; i++)
            {
                yield return GenerateLine(entryCount, entryLength);
            }
        }

        public static void CreateTestFile(string path, int length, int entryCount, int entryLength)
        {
            File.WriteAllLines(path, GenerateFile(length, entryCount, entryLength));
        }

        public static void DeleteTestFile(string path)
        {
            File.Delete(path);
        }
    }
}