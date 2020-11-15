using System;
using System.Collections.Generic;
using System.Linq;
using TplDataflow.Model;

namespace TplDataflow.Dataflow
{
    internal static partial class Functions
    {
        internal static IEnumerable<string> SplitAnInputString(string input, char splitterSeparator)
        {
            Console.WriteLine($"{nameof(SplitAnInputString)} - Splitting {input} by the char '{splitterSeparator}'...");

            var splittedInput = input?.Split(splitterSeparator);
            if (splittedInput == null || splittedInput.Count() < 1)
            {
                return new[]
                {
                    "-0",
                    string.Empty,
                    string.Empty
                };
            }

            if (splittedInput.Count() == 1)
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

        internal static IMetaData CreateASingleMedatadataFromAString(string stringInput)
        {
            Console.WriteLine($"{nameof(CreateASingleMedatadataFromAString)} - Creating the metadata based on value : '{stringInput}'...");

            return new MetaData
            {
                Name = stringInput,
                Status = 1
            };
        }
    }
}
