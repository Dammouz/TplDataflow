using System.Threading.Tasks.Dataflow;

namespace TplDataflow.Dataflow
{
    internal static class DataflowOptions
    {
        internal static DataflowLinkOptions LinkOptions => new DataflowLinkOptions
        {
            PropagateCompletion = true
        };

        internal static ExecutionDataflowBlockOptions CreateBlockOptionsWithBoundedBoundedCapacity(int capacity)
        {
            return new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = capacity
            };
        }
    }
}
