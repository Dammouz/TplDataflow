using System;
using System.Collections.Generic;

namespace TplDataflow.Dataflow
{
    internal static partial class Functions
    {
        private static int _counterForJoinBlockUsage = 0;

        internal static void ClearCounterForJoinBlockUsage()
        {
            _counterForJoinBlockUsage = 0;
        }

        internal static int Noop(int i)
        {
            return i;
        }

        internal static int Square(int i)
        {
            return i * i;
        }

        internal static double MultiplyByPi(int i)
        {
            return i * Math.PI;
        }

        internal static void FormatTupleForTheOuputCollection(IDictionary<string, string[]> ouputCollection, Tuple<int, int, double> resultOfBothTransformBlock)
        {
            ouputCollection.Add($"Iteration number {_counterForJoinBlockUsage++}",
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
