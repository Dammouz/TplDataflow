using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TplDataflow.Dataflow
{
    public class Dataflow
    {
        // Predefined Dataflow Block Types

        // A. Buffering Blocks
        // A.1. BufferBlock(T)
        // A.2. BroadcastBlock(T)
        // A.3. WriteOnceBlock(T)

        // B. Execution Blocks
        // B.1. ActionBlock(T)
        // B.2. TransformBlock(TInput, TOutput)
        // B.3. TransformManyBlock(TInput, TOutput)

        // C. Grouping Blocks
        // C.1. BatchBlock(T)
        // C.2. JoinBlock(T1, T2, ...)
        // C.3. BatchedJoinBlock(T1, T2, ...)
    }
}
