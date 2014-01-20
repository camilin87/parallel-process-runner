using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ParallelRunner
{
    class Program
    {
        private static void Main(string[] args)
        {
            var linesCount = ReadLastLinesCount(args);
            var commandsToExecute = ReadExecutableFullPaths(args).ToList();
            var lLogFiles = ReadLogFiles(args).ToList();

            var lProcess = new List<Process>();

            Console.WriteLine("Starting {0} processes...", commandsToExecute.Count);

            foreach (var fileName in commandsToExecute)
            {
                var process = new Process();
                process.StartInfo.FileName = fileName;
                process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                lProcess.Add(process);
            }

            lProcess.ForEach(p => p.Start());

            Console.WriteLine("Waiting for completion...");

            foreach (var proc in lProcess)
            {
                proc.WaitForExit();
                Console.WriteLine("Process \"{0}\" returned with exit code {1}", proc.StartInfo.FileName, proc.ExitCode);
            }

            foreach (var logFile in lLogFiles)
            {
                if (File.Exists(logFile))
                {
                    Console.WriteLine("Displaying last lines from {0}", logFile);
                    var fileContents = File.ReadAllText(logFile);
                    DisplayLastLines(fileContents, linesCount);
                }
            }

            Console.ReadLine();
        }

        private static void DisplayLastLines(string fileContents, int linesCount)
        {
            var stdOutLines = fileContents.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = stdOutLines.Length - linesCount; i < stdOutLines.Length; i++)
            {
                if (i >= 0)
                {
                    Console.WriteLine(stdOutLines[i]);
                }
            }
        }

        private static int ReadLastLinesCount(IEnumerable<string> args)
        {
            var lineCountArgStr = (args.FirstOrDefault(str => str.StartsWith("lc=")) ?? "lc=10");
            var lineCountStr = lineCountArgStr.Split(new[] {"="},StringSplitOptions.RemoveEmptyEntries)[1];
            return int.Parse(lineCountStr);
        }

        private static IEnumerable<string> ReadLogFiles(IEnumerable<string> args)
        {
            foreach (var arg in args.Where(a => a.StartsWith("lf=")))
            {
                var logfile = arg.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                if (logfile.EndsWith(".log"))
                {
                    yield return logfile.Replace('"', ' ').Trim();
                }
            }
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
