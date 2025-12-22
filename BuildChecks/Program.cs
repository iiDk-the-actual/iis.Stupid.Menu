using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BuildChecks
{
    class Program
    {
        private static readonly string[] ExpectedHeaderStart = new[]
        {
            "/*",
            " * ii's Stupid Menu"
        };

        private const string SkipFileName = "SKIP_BUILD_CHECKS";

        static int Main(string[] args)
        {
            string rootDirectory = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
            Console.WriteLine($"Scanning directory: {rootDirectory}");

            var csFiles = Directory.GetFiles(rootDirectory, "*.cs", SearchOption.AllDirectories);
            bool hasErrors = false;
            int skippedCount = 0;

            foreach (var file in csFiles)
            {
                if (ShouldSkipFile(file, rootDirectory))
                {
                    skippedCount++;
                    continue;
                }

                string content = File.ReadAllText(file);

                if (!HasValidHeader(content))
                {
                    Console.WriteLine($"[ERROR] Missing or incorrect license header: {file}");
                    hasErrors = true;
                }

                if (!IsNamespaceValid(content))
                {
                    Console.WriteLine($"[ERROR] File not in iiMenu namespace: {file}");
                    hasErrors = true;
                }

                if (!IsHeaderFilePathCorrect(content, file, rootDirectory))
                {
                    Console.WriteLine($"[ERROR] License header file path does not match actual path: {file}");
                    hasErrors = true;
                }
            }

            if (skippedCount > 0)
                Console.WriteLine($"\nSkipped {skippedCount} file(s) due to {SkipFileName}");

            if (hasErrors)
            {
                Console.WriteLine("Validation failed: Fix errors before building");
                return 1;
            }

            Console.WriteLine("All files passed validation");
            return 0;
        }

        private static bool ShouldSkipFile(string filePath, string rootDirectory)
        {
            var normalizedPath = filePath.Replace('\\', '/');
            if (normalizedPath.Contains("/obj/"))
                return true;

            var directory = Path.GetDirectoryName(filePath);

            while (directory != null && directory.StartsWith(rootDirectory))
            {
                var skipFilePath = Path.Combine(directory, SkipFileName);
                if (File.Exists(skipFilePath))
                    return true;

                directory = Path.GetDirectoryName(directory);
            }

            return false;
        }

        private static bool HasValidHeader(string content)
        {
            var lines = content.Split(["\r\n", "\n"], StringSplitOptions.None);
            if (lines.Length < ExpectedHeaderStart.Length) return false;

            for (int i = 0; i < ExpectedHeaderStart.Length; i++)
            {
                if (!lines[i].StartsWith(ExpectedHeaderStart[i]))
                    return false;
            }

            return true;
        }

        private static bool IsNamespaceValid(string content)
        {
            var tree = CSharpSyntaxTree.ParseText(content);
            var root = tree.GetCompilationUnitRoot();

            var namespaces = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            return namespaces.Any(ns => ns.Name.ToString() == "iiMenu" || ns.Name.ToString().StartsWith("iiMenu."));
        }

        private static bool IsHeaderFilePathCorrect(string content, string actualFilePath, string rootDirectory)
        {
            var lines = content.Split(["\r\n", "\n"], StringSplitOptions.None);
            if (lines.Length < 3) return false;

            var headerLine = lines[1].Trim();
            var match = Regex.Match(headerLine, @"\* ii's Stupid Menu\s+(.*)$");
            if (!match.Success) return false;

            var headerPath = match.Groups[1].Value.Trim().Replace('\\', '/');
            var relativePath = Path.GetRelativePath(rootDirectory, actualFilePath).Replace('\\', '/');

            return string.Equals(headerPath, relativePath, StringComparison.OrdinalIgnoreCase);
        }
    }
}