using System;
using System.Collections.Generic;

namespace TplDataflow.Dataflow
{
    internal static partial class Functions
    {
        private static int _counterForBatchBlockUsage = 0;

        internal static void ClearCounterForBatchBlockUsage()
        {
            _counterForBatchBlockUsage = 0;
        }

        internal static void DisplayByGroups(IDictionary<string, string[]> ouputCollection, string[] batchedInput)
        {
            ouputCollection.Add($"Batch number {_counterForBatchBlockUsage++}", batchedInput);
        }
    }
}
