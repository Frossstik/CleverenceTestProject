using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LogStandards
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Неверный аргумент");
                return 1;
            }

            string inputPath = args[0];

            if (!File.Exists(inputPath))
            {
                Console.WriteLine($"Файл не найден: {inputPath}");
                return 2;
            }

            string solutionDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
            string logsDir = Path.Combine(solutionDir, "Logs");
            Directory.CreateDirectory(logsDir);

            string okPath = Path.Combine(logsDir, $"standardized{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}.txt");
            string badPath = Path.Combine(logsDir, $"problems{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}.txt");

            int ok = 0, bad = 0;

            using var reader = new StreamReader(inputPath, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), detectEncodingFromByteOrderMarks: true);
            using var okWriter = new StreamWriter(okPath, false, new UTF8Encoding(false));
            using var badWriter = new StreamWriter(badPath, false, new UTF8Encoding(false));

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (TryParseFormat1(line, out var entry) || TryParseFormat2(line, out entry))
                {
                    okWriter.Write(entry!.DateDDMMYYYY);
                    okWriter.Write('\t');
                    okWriter.Write(entry.TimeRaw);
                    okWriter.Write('\t');
                    okWriter.Write(entry.LevelNormalized);
                    okWriter.Write('\t');
                    okWriter.Write(entry.CallerMethod ?? "DEFAULT");
                    okWriter.Write('\t');
                    okWriter.WriteLine(entry.Message);
                    ok++;
                }
                else
                {
                    badWriter.WriteLine(line);
                    bad++;
                }
            }

            Console.WriteLine($"Готово. OK: {ok}, проблемных строк: {bad}");
            Console.WriteLine($"Результат: {Path.GetFullPath(okPath)}");
            Console.WriteLine($"Проблемы: {Path.GetFullPath(badPath)}");
            return 0;
        }

        private sealed class LogEntry
        {
            public required string DateDDMMYYYY { get; init; }
            public required string TimeRaw { get; init; }
            public required string LevelNormalized { get; init; }
            public string? CallerMethod { get; init; }
            public required string Message { get; init; }
        }

        private static string NormalizeLevel(string level)
        {
            level = level.Trim().ToUpperInvariant();
            return level switch
            {
                "INFORMATION" => "INFO",
                "INFO" => "INFO",
                "WARNING" => "WARN",
                "WARN" => "WARN",
                "ERROR" => "ERROR",
                "DEBUG" => "DEBUG",
                _ => level
            };
        }

        private static readonly Regex RxFormat1 = new Regex(
            @"^(?<date>\d{2}\.\d{2}\.\d{4})\s+(?<time>\d{2}:\d{2}:\d{2}(?:\.\d+)?)\s+(?<level>INFORMATION|WARNING|ERROR|DEBUG)\s+(?<message>.+)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private static bool TryParseFormat1(string line, out LogEntry? entry)
        {
            entry = null;
            var m = RxFormat1.Match(line);
            if (!m.Success) return false;

            string date = m.Groups["date"].Value;
            string time = m.Groups["time"].Value;
            string level = m.Groups["level"].Value;
            string message = m.Groups["message"].Value;

            if (!DateTime.TryParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return false;

            entry = new LogEntry
            {
                DateDDMMYYYY = dt.ToString("dd-MM-yyyy"),
                TimeRaw = time,
                LevelNormalized = NormalizeLevel(level),
                CallerMethod = null,
                Message = message.Trim()
            };
            return true;
        }

        private static readonly Regex RxFormat2 = new Regex(
            @"^(?<date>\d{4}-\d{2}-\d{2})\s+(?<time>\d{2}:\d{2}:\d{2}(?:\.\d+)?)[|]\s*(?<level>INFO|WARN|WARNING|ERROR|DEBUG)\|[^|]*\|(?<method>[^|]*)\|\s*(?<message>.+)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private static bool TryParseFormat2(string line, out LogEntry? entry)
        {
            entry = null;
            var m = RxFormat2.Match(line);
            if (!m.Success) return false;

            string date = m.Groups["date"].Value;
            string time = m.Groups["time"].Value;
            string level = m.Groups["level"].Value;
            string method = m.Groups["method"].Value;
            string message = m.Groups["message"].Value;

            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return false;

            entry = new LogEntry
            {
                DateDDMMYYYY = dt.ToString("dd-MM-yyyy"),
                TimeRaw = time,
                LevelNormalized = NormalizeLevel(level),
                CallerMethod = string.IsNullOrWhiteSpace(method) ? "DEFAULT" : method.Trim(),
                Message = message.Trim()
            };
            return true;
        }
    }
}
