using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace ExtensibleILRewriter
{
    public static class Verifier
    {
        public static string Verify(string beforeAssemblyPath, string afterAssemblyPath)
        {
            var before = Validate(beforeAssemblyPath);
            var after = Validate(afterAssemblyPath);
            return string.Format("Failed processing {0}\r\n{1}", Path.GetFileName(afterAssemblyPath), after);
        }

        public static string Validate(string assemblyPath2)
        {
            var exePath = GetPathToPEVerify();
            if (!File.Exists(exePath))
            {
                return string.Empty;
            }

            var process = Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath2 + "\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            process.WaitForExit(10000);
            return process.StandardOutput.ReadToEnd().Trim().Replace(assemblyPath2, string.Empty);
        }

        public static string GetPathToPEVerify()
        {
            var exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.2 Tools\PEVerify.exe");

            if (!File.Exists(exePath))
            {
                exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\PEVerify.exe");
            }

            if (!File.Exists(exePath))
            {
                exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\PEVerify.exe");
            }

            if (!File.Exists(exePath))
            {
                exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\PEVerify.exe");
            }

            return exePath;
        }

        public static string TrimLineNumbers(string foo)
        {
            return Regex.Replace(foo, @"0x.*]", string.Empty);
        }
    }
}