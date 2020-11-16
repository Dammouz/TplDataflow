using System.Collections.Concurrent;

namespace TplDataflow.Dataflow
{
    internal static partial class Functions
    {
        internal static void AddInputIntoTheGivenList(BlockingCollection<string> strings, string stringInput, string subscriberName)
        {
            strings.TryAdd($"{subscriberName} produce input : '{stringInput}'");
        }
    }
}
