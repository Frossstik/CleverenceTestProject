using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;


namespace TestProject1
{
    public class LogStandardsTests
    {
        private static Type ProgramType =>
            typeof(LogStandards.Program); 

        private static MethodInfo GetPrivateMethod(string name) =>
            ProgramType.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Метод {name} не найден");

        private static object? GetProp(object target, string prop) =>
            target.GetType().GetProperty(prop, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(target);

        [Fact]
        public void Format1_Information_To_Info()
        {
            var line = "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'";
            var tryParse = GetPrivateMethod("TryParseFormat1");

            object?[] args = new object?[] { line, null };
            var ok = (bool)tryParse.Invoke(null, args)!;
            ok.Should().BeTrue();

            var entry = args[1]!;
            GetProp(entry, "DateDDMMYYYY").Should().Be("10-03-2025");
            GetProp(entry, "TimeRaw").Should().Be("15:14:49.523");
            GetProp(entry, "LevelNormalized").Should().Be("INFO");
            GetProp(entry, "CallerMethod").Should().BeNull(); //
            GetProp(entry, "Message").Should().Be("Версия программы: '3.4.0.48729'");
        }

        [Fact]
        public void Format1_Warning_To_Warn()
        {
            var line = "11.03.2025 09:27:10.104 WARNING Попытка повторного входа пользователя 'admin'";
            var tryParse = GetPrivateMethod("TryParseFormat1");

            object?[] args = new object?[] { line, null };
            var ok = (bool)tryParse.Invoke(null, args)!;
            ok.Should().BeTrue();

            var entry = args[1]!;
            GetProp(entry, "DateDDMMYYYY").Should().Be("11-03-2025");
            GetProp(entry, "LevelNormalized").Should().Be("WARN");
        }

        [Fact]
        public void Format2_With_Method_Parses()
        {
            var line = "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";
            var tryParse = GetPrivateMethod("TryParseFormat2");

            object?[] args = new object?[] { line, null };
            var ok = (bool)tryParse.Invoke(null, args)!;
            ok.Should().BeTrue();

            var entry = args[1]!;
            GetProp(entry, "DateDDMMYYYY").Should().Be("10-03-2025");
            GetProp(entry, "TimeRaw").Should().Be("15:14:51.5882");
            GetProp(entry, "LevelNormalized").Should().Be("INFO");
            GetProp(entry, "CallerMethod").Should().Be("MobileComputer.GetDeviceId");
            GetProp(entry, "Message").Should().Be("Код устройства: '@MINDEO-M40-D-410244015546'");
        }

        [Fact]
        public void Format2_Empty_Method_Becomes_DEFAULT()
        {
            var line = "2025-03-12 08:15:35.002| INFO|22|| Сообщение без метода";
            var tryParse = GetPrivateMethod("TryParseFormat2");

            object?[] args = new object?[] { line, null };
            var ok = (bool)tryParse.Invoke(null, args)!;
            ok.Should().BeTrue();

            var entry = args[1]!;
            GetProp(entry, "CallerMethod").Should().Be("DEFAULT");
            GetProp(entry, "Message").Should().Be("Сообщение без метода");
        }

        [Fact]
        public void Format2_Unknown_Level_Fails()
        {
            var line = "2025-03-14 11:22:33.777| UNKNOWN|11|SomeMethod| Что-то пошло не так";
            var tryParse = GetPrivateMethod("TryParseFormat2");

            object?[] args = new object?[] { line, null };
            var ok = (bool)tryParse.Invoke(null, args)!;
            ok.Should().BeFalse();
            args[1].Should().BeNull();
        }

        [Theory]
        [InlineData("INFORMATION", "INFO")]
        [InlineData("INFO", "INFO")]
        [InlineData("WARNING", "WARN")]
        [InlineData("WARN", "WARN")]
        [InlineData("ERROR", "ERROR")]
        [InlineData("DEBUG", "DEBUG")]
        public void NormalizeLevel_Maps_Correctly(string input, string expected)
        {
            var normalize = GetPrivateMethod("NormalizeLevel");
            var actual = (string)normalize.Invoke(null, new object?[] { input })!;
            actual.Should().Be(expected);
        }

        [Fact]
        public void Format1_Invalid_Date_Fails()
        {
            var line = "32.03.2025 10:00:00.000 INFO Невозможная дата";
            var tryParse = GetPrivateMethod("TryParseFormat1");

            object?[] args = new object?[] { line, null };
            var ok = (bool)tryParse.Invoke(null, args)!;
            ok.Should().BeFalse();
            args[1].Should().BeNull();
        }
    }
}
