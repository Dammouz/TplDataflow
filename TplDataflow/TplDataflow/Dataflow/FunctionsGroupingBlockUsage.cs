using System.Collections.Generic;

namespace TplDataflow.Dataflow
{
    internal static partial class Functions
    {
        private static int _counter = 0;

        public static void DisplayByGroups(IDictionary<string, string[]> ouputCollection, string[] batchedInput)
        {
            ouputCollection.Add($"Batch number {_counter++}", batchedInput);
        }
    }
}
