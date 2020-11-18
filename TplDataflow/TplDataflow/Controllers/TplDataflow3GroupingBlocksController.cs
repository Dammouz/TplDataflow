using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            Functions.ClearCounterForBatchBlockUsage();

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

        #region JoinBlockUsage

        /// <summary>
        /// Post method to illustrate <see cref="JoinBlock{T1,T2,T3}" />.
        /// Retrieves a collection of transformed and joined data.
        /// </summary>
        /// <param name="numberOfIteration">Number of iterations to produce</param>
        /// <returns>A formatted collection containing output of the <see cref="JoinBlock{T1,T2,T3}" /></returns>
        [HttpPost]
        [Route(nameof(JoinBlockUsage))]
        public IDictionary<string, string[]> JoinBlockUsage(int numberOfIteration)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow3GroupingBlocksController)} - {nameof(JoinBlockUsage)}");

            var ouputCollection = new Dictionary<string, string[]>();
            Functions.ClearCounterForJoinBlockUsage();

            // Create the members of the pipeline.
            var broadCastBlock = new BroadcastBlock<int>(i => i);
            var transformBlockDoNothing = new TransformBlock<int, int>(i =>
                Functions.Noop(i)
            );
            var transformBlockSquare = new TransformBlock<int, int>(i =>
                Functions.Square(i)
            );
            var transformBlockMultipleByPi = new TransformBlock<int, double>(i =>
                Functions.MultiplyByPi(i)
            );
            var joinBlock = new JoinBlock<int, int, double>();
            var processorBlock = new ActionBlock<Tuple<int, int, double>>(tuple =>
                Functions.FormatTupleForTheOuputCollection(ouputCollection, tuple)
            );

            // Connect the dataflow blocks to form a pipeline.
            broadCastBlock.LinkTo(transformBlockDoNothing, DataflowOptions.LinkOptions);
            broadCastBlock.LinkTo(transformBlockSquare, DataflowOptions.LinkOptions);
            broadCastBlock.LinkTo(transformBlockMultipleByPi, DataflowOptions.LinkOptions);
            transformBlockDoNothing.LinkTo(joinBlock.Target1);
            transformBlockSquare.LinkTo(joinBlock.Target2);
            transformBlockMultipleByPi.LinkTo(joinBlock.Target3);
            joinBlock.LinkTo(processorBlock, DataflowOptions.LinkOptions);

            // Start JoinBlockUsage pipeline with the input values.
            for (var i = 0; i <= numberOfIteration; i++)
            {
                broadCastBlock.Post(i);
            }

            // Mark the head of the pipeline as complete.
            broadCastBlock.Complete();

            // Wait for the last block in the pipeline to process all messages.
            Task.WhenAll(transformBlockDoNothing.Completion,
                         transformBlockSquare.Completion,
                         transformBlockMultipleByPi.Completion)
                .ContinueWith(_ => joinBlock.Complete());
            processorBlock.Completion.Wait();

            return ouputCollection;
        }

        #endregion JoinBlockUsage

        #region BatchedJoinBlockUsage

        /// <summary>
        /// Post method to illustrate <see cref="BatchedJoinBlock{T1,T2,T3}" />.
        /// Retrieves a collection of transformed batched and joined data.
        /// </summary>
        /// <param name="numberOfIteration">Number of iterations to produce</param>
        /// <param name="batchsize">Size of the buffer of the <see cref="BatchedJoinBlock{T1,T2,T3}" /></param>
        /// <returns>A collection produces</returns>
        [HttpPost]
        [Route(nameof(BatchedJoinBlockUsage))]
        public IDictionary<string, string[]> BatchedJoinBlockUsage(int numberOfIteration, int batchsize)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow3GroupingBlocksController)} - {nameof(BatchedJoinBlockUsage)}");

            var ouputCollection = new Dictionary<string, string[]>();
            Functions.ClearCounterForBatchedJoinBlockUsage();

            // Create the members of the pipeline.
            var broadCastBlock = new BroadcastBlock<int>(i => i);
            var transformBlockDoNothing = new TransformBlock<int, int>(i => i);
            var transformBlockSquare = new TransformBlock<int, int>(i => i * i);
            var transformBlockMultipleByPi = new TransformBlock<int, double>(i => i * Math.PI);
            var batchedJoinBlock = new BatchedJoinBlock<int, int, double>(batchsize);
            var processorBlock = new ActionBlock<Tuple<IList<int>, IList<int>, IList<double>>>(tuple =>
                Functions.FormatTupleForTheOuputCollection(ouputCollection, tuple)
            );

            // Connect the dataflow blocks to form a pipeline.
            broadCastBlock.LinkTo(transformBlockDoNothing, DataflowOptions.LinkOptions);
            broadCastBlock.LinkTo(transformBlockSquare, DataflowOptions.LinkOptions);
            broadCastBlock.LinkTo(transformBlockMultipleByPi, DataflowOptions.LinkOptions);
            transformBlockDoNothing.LinkTo(batchedJoinBlock.Target1);
            transformBlockSquare.LinkTo(batchedJoinBlock.Target2);
            transformBlockMultipleByPi.LinkTo(batchedJoinBlock.Target3);
            batchedJoinBlock.LinkTo(processorBlock, DataflowOptions.LinkOptions);

            // Start BatchedJoinBlockUsage pipeline with the input values.
            for (var i = 0; i < numberOfIteration; i++)
            {
                broadCastBlock.Post(i);
            }

            // Mark the head of the pipeline as complete.
            broadCastBlock.Complete();

            // Wait for the last block in the pipeline to process all messages.
            Task.WhenAll(transformBlockDoNothing.Completion,
                         transformBlockSquare.Completion,
                         transformBlockMultipleByPi.Completion)
                .ContinueWith(_ => batchedJoinBlock.Complete());
            processorBlock.Completion.Wait();

            return ouputCollection;
        }

        #endregion BatchedJoinBlockUsage
    }
}
