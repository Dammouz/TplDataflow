using System;
using System.Text;
using TplDataflow.Common;

namespace TplDataflow.Dataflow
{
    internal static partial class Functions
    {
        internal static string TransformIntIntoRepeatedLines(int input)
        {
            Console.WriteLine($"{nameof(TransformIntIntoRepeatedLines)} - Transforming the int {input} into several repeated lines...");

            var repeatedLines = new StringBuilder($"We should have {input} repeated lines :{Environment.NewLine}");
            repeatedLines.Insert(repeatedLines.Length, $"The line is repeated {input} times{Environment.NewLine}", input);
            repeatedLines.Append($"    ----------{Environment.NewLine}{Environment.NewLine}");

            return repeatedLines.ToString();
        }

        internal static void ModifyStringAndWriteInFile(string stringifiedInt, string savedTextDirectory, string basedFileName)
        {
            Console.WriteLine($"{nameof(ModifyStringAndWriteInFile)} - Modifying the string '{stringifiedInt}' and saving data on disk...");

            var modifiedString = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {stringifiedInt}";
            var fileName = $"{basedFileName}{DateTime.Now:yyyy-MM-dd}.txt";
            CommonHelpers.WriteContentInAFile(savedTextDirectory, modifiedString, fileName);
        }
    }
}
