using System;
using System.Collections.Generic;
using System.Linq;

namespace TplDataflow.Dataflow
{
    internal static partial class Functions
    {
        private static int _counterForBatchedJoinBlockUsage = 0;

        internal static void ClearCounterForBatchedJoinBlockUsage()
        {
            _counterForBatchedJoinBlockUsage = 0;
        }

        internal static void FormatTupleForTheOuputCollection(IDictionary<string, string[]> ouputCollection, Tuple<IList<int>, IList<int>, IList<double>> resultOfBothTransformBlock)
        {
            ouputCollection.Add($"Iteration number {_counterForBatchedJoinBlockUsage++}",
                new[]
                {
                    $"Item 1 - Noop   : {string.Join(" - ", resultOfBothTransformBlock.Item1)}",
                    $"Item 2 - Square : {string.Join(" - ", resultOfBothTransformBlock.Item2)}",
                    $"Item 3 - x * PI : {string.Join(" - ", resultOfBothTransformBlock.Item3.Select(d => $"{d:F3}"))}"
                }
            );
        }
    }
}
