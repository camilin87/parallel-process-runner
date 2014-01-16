using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ParallelRunner
{
    class Program
    {
        private static void Main(string[] args)
        {
            var linesCount = ReadLastLinesCount(args);
            var commandsToExecute = ReadExecutableFullPaths(args);
        }

        private static int ReadLastLinesCount(IEnumerable<string> args)
        {
            var lineCountArgStr = (args.FirstOrDefault(str => str.StartsWith("lc=")) ?? "lc=10");
            var lineCountStr = lineCountArgStr.Split(new[] {"="},StringSplitOptions.RemoveEmptyEntries)[1];
            return int.Parse(lineCountStr);
        }

        private static IEnumerable<string> ReadExecutableFullPaths(IEnumerable<string> lArgs)
        {
            foreach (var arg in lArgs)
            {
                if (File.Exists(arg))
                {
                    yield return arg;
                }
            }
        }
    }
}
