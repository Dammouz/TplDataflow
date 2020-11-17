using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc;
using TplDataflow.Dataflow;

namespace TplDataflow.Controllers
{
    [Produces("application/json")]
    [Route("tpldataflow-api")]
    [ApiController]
    public class TplDataflow3GroupingBlocksController : ControllerBase
    {
        // C. Grouping Blocks
        // C.2. JoinBlock(T1, T2, ...)
        // C.3. BatchedJoinBlock(T1, T2, ...)

        #region BatchBlockUsage

        /// <summary>
        /// Post method to illustrate <see cref="BatchBlock{T}" />.
        /// Retrieves a dictionnary of batched strings.
        /// </summary>
        /// <param name="numberOfIteration">Number of gien input (by iteration)</param>
        /// <param name="batchsize">Size of the buffer of the <see cref="BatchBlock{T}" /></param>
        /// <returns>The dictionary of batched strings</returns>
        [HttpPost]
        [Route(nameof(BatchBlockUsage))]
        public IDictionary<string, string[]> BatchBlockUsage(int numberOfIteration, int batchsize)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow3GroupingBlocksController)} - {nameof(BatchBlockUsage)}");

            var ouputCollection = new Dictionary<string, string[]>();

            // Create the members of the pipeline.
            var batchBlockWithSizeGivenInInput = new BatchBlock<string>(batchsize);
            var actionBlockPerformActionOnBatchData = new ActionBlock<string[]>(batchedInput =>
                Functions.DisplayByGroups(ouputCollection, batchedInput)
            );

            // Connect the dataflow blocks to form a pipeline.
            batchBlockWithSizeGivenInInput.LinkTo(actionBlockPerformActionOnBatchData, DataflowOptions.LinkOptions);

            // Start BatchBlockUsage pipeline with the input values.
            for (var i = 0; i < numberOfIteration; i++)
            {
                batchBlockWithSizeGivenInInput.Post($"Value = {i}");
            }

            // Mark the head of the pipeline as complete.
            batchBlockWithSizeGivenInInput.Complete();

            // Wait for the last block in the pipeline to process all messages.
            actionBlockPerformActionOnBatchData.Completion.Wait();

            return ouputCollection;
        }

        #endregion BatchBlockUsage
    }
}
