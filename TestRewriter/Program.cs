﻿using ExtensibleILRewriter;
using ExtensibleILRewriter.Logging;
using ExtensibleILRewriter.MsBuild;
using System;
using System.IO;
using System.Reflection;

namespace TestRewriter
{
    internal class Program
    {
#if DEBUG
        private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif

        private static void Main(string[] args)
        {
            //const string ProjectName = "AssemblyToProcessFlow";

            const string ProjectName = "Ascgen2";

            const string ProjectExtension = ".exe";

            var executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            var projectBinariesPath = Path.Combine(Path.GetDirectoryName(executingAssemblyLocation), @"..\..\..", ProjectName, "bin", Configuration);
            var assemblyToRewritePath = Path.Combine(projectBinariesPath, ProjectName + ProjectExtension);
            var rewrittenAssemblyPath = Path.Combine(projectBinariesPath, Path.GetFileNameWithoutExtension(assemblyToRewritePath) + "_Rewritten" + ProjectExtension);

            Console.WriteLine($"Rewriting:\t{assemblyToRewritePath}");
            Console.WriteLine($"Output:\t\t{rewrittenAssemblyPath}");
            Console.WriteLine();

            var rewriteTask = new AssemblyRewrite()
            {
                AssemblyPath = assemblyToRewritePath,
                ConfigurationPath = Path.Combine( projectBinariesPath, @"..\..\RewriteConfiguration.xml")
            };
            rewriteTask.Execute(rewrittenAssemblyPath, new ConsoleLogger());

            var result = Verifier.Verify(assemblyToRewritePath, rewrittenAssemblyPath);
            Console.WriteLine("Weaving verified");
            Console.WriteLine(result);

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    internal class ConsoleLogger : ILogger
    {
        public void Message(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Notice:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Progress:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    throw new NotSupportedException($"Unknown {nameof(LogLevel)}: '{level}'");
            }

            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void MessageDetailed(LogLevel level, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message)
        {
            switch (level)
            {
                case LogLevel.Notice:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Progress:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    throw new NotSupportedException($"Unknown {nameof(LogLevel)}: '{level}'");
            }

            Console.WriteLine($"File: {file}; lineNumber: {lineNumber}; columnNumber: {columnNumber}; endLineNumber: {endLineNumber}; endColumnNumber: {endColumnNumber}; {message}");
            Console.ResetColor();
        }
    }
}
