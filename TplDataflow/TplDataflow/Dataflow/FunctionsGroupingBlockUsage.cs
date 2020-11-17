using System;
using System.Collections.Generic;

namespace TplDataflow.Dataflow
{
    internal static partial class Functions
    {
        private static int _counter = 0;

        internal static void ClearCounter()
        {
            _counter = 0;
        }

        internal static void DisplayByGroups(IDictionary<string, string[]> ouputCollection, string[] batchedInput)
        {
            ouputCollection.Add($"Batch number {_counter++}", batchedInput);
        }

        internal static void FormatTupleForTheOuputCollection(IDictionary<string, string[]> ouputCollection, Tuple<int, int, double> resultOfBothTransformBlock)
        {
            ouputCollection.Add($"Iteration number {_counter++}",
                new[]
                {
                    $"Item 1 - Noop   : {resultOfBothTransformBlock.Item1}",
                    $"Item 2 - Square : {resultOfBothTransformBlock.Item2}",
                    $"Item 3 - x * PI : {resultOfBothTransformBlock.Item3}"
                }
            );
        }
    }
}
