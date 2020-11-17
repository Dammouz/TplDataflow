using System;
using TplDataflow.Model;

namespace TplDataflow.Dataflow
{
    internal static partial class Functions
    {
        internal static string[] SplitAnInputStringIntoArray(string input, char splitterSeparator)
        {
            Console.WriteLine($"{nameof(SplitAnInputStringIntoArray)} - Splitting {input} by the char '{splitterSeparator}' into an array...");

            var splittedInput = input?.Split(splitterSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (splittedInput == null || splittedInput.Length < 1)
            {
                return new[]
                {
                    "-0",
                    string.Empty,
                    string.Empty
                };
            }

            if (splittedInput.Length == 1)
            {
                return new[]
                {
                    "-100",
                    splittedInput[0],
                    "Array contains only 1 element"
                };
            }

            if (splittedInput.Length == 2)
            {
                return new[]
                {
                    "-200",
                    splittedInput[0],
                    splittedInput[1]
                };
            }

            return splittedInput;
        }

        internal static IMetaData CreateASingleMedatadataFromStrings(string[] stringArray)
        {
            Console.WriteLine($"{nameof(CreateASingleMedatadataFromStrings)} - Creating the metadata based on value : '{string.Join(" - ", stringArray)}'...");

            var status = int.TryParse(stringArray[0], out var parsedInt) ? parsedInt : 1;
            return new MetaData
            {
                Name = stringArray[0],
                InitialUrl = stringArray[1],
                Folder = stringArray[2],
                Status = status
            };
        }
    }
}
