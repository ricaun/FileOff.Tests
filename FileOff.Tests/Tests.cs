using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileOff.Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        public static string[] Files()
        {
            var location = Path.GetDirectoryName(typeof(Tests).Assembly.Location);
            return Directory.GetFiles(location, "*.off", SearchOption.AllDirectories).Select(Path.GetFileName).ToArray();
        }

        public static string GetFile(string file)
        {
            var location = Path.GetDirectoryName(typeof(Tests).Assembly.Location);
            return Directory.GetFiles(location, file, SearchOption.AllDirectories).FirstOrDefault();
        }

        [TestCaseSource(nameof(Files))]
        public void Test2(string file)
        {
            string path = GetFile(file);
            string[] lines = File.ReadAllLines(path);
            var off = Off.Parse(lines);
            var content = off.ToString();
            //Console.WriteLine(off);
            //File.WriteAllText(file, off.ToString());
        }
    }
}