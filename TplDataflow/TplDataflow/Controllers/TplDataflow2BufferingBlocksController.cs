using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc;
using TplDataflow.Dataflow;

namespace TplDataflow.Controllers
{
    [Produces("application/json")]
    [Route("tpldataflow-api")]
    [ApiController]
    public class TplDataflow2BufferingBlocksController : ControllerBase
    {
        #region BroadcastBlockUsage

        /// <summary>
        /// Post method to illustrate <see cref="BroadcastBlock{T}" />.
        /// Display a sequence of strings broadcasted.
        /// </summary>
        /// <param name="numberOfIteration">Number of iterations</param>
        /// <returns>A List of 'numberOfIteration' strings</returns>
        [HttpPost]
        [Route(nameof(BroadcastBlockUsage))]
        public IEnumerable<string> BroadcastBlockUsage(int numberOfIteration)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow2BufferingBlocksController)} - {nameof(BroadcastBlockUsage)}");

            var strings = new BlockingCollection<string>();

            // Create the members of the pipeline.
            var broadcastBlockGivenInputToSubscribers = new BroadcastBlock<string>(input => input);
            var actionBlockSubscriber1 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 1")
            );
            var actionBlockSubscriber2 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 2")
            );
            var actionBlockSubscriber3 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 3")
            );

            // Connect the dataflow blocks to form a pipeline.
            broadcastBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber1, DataflowOptions.LinkOptions);
            broadcastBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber2, DataflowOptions.LinkOptions);
            broadcastBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber3, DataflowOptions.LinkOptions);

            // Start BroadcastBlockUsage pipeline with the input values.
            for (var i = 1; i <= numberOfIteration; i++)
            {
                broadcastBlockGivenInputToSubscribers.Post($"Value = {i}");
            }

            // Mark the head of the pipeline as complete.
            broadcastBlockGivenInputToSubscribers.Complete();

            // Waiting block to receive all post input.
            Task.WaitAll(actionBlockSubscriber1.Completion,
                         actionBlockSubscriber2.Completion,
                         actionBlockSubscriber3.Completion);

            return strings;
        }

        #endregion BroadcastBlockUsage

        #region BufferBlockUsage

        /// <summary>
        /// Post method to illustrate <see cref="BufferBlock{T}" />.
        /// Display a sequence of strings buffered.
        /// </summary>
        /// <param name="numberOfIteration">Number of iterations</param>
        /// <returns>A List of 'numberOfIteration' strings</returns>
        [HttpPost]
        [Route(nameof(BufferBlockUsage))]
        public IEnumerable<string> BufferBlockUsage(int numberOfIteration)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow2BufferingBlocksController)} - {nameof(BufferBlockUsage)}");

            var strings = new BlockingCollection<string>();

            // Create the members of the pipeline.
            var bufferBlockGivenInputToSubscribers = new BufferBlock<string>();
            var actionBlockSubscriber1 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 1")
            );
            var actionBlockSubscriber2 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 2")
            );
            var actionBlockSubscriber3 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 3")
            );

            // Connect the dataflow blocks to form a pipeline.
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber1, DataflowOptions.LinkOptions);
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber2, DataflowOptions.LinkOptions);
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber3, DataflowOptions.LinkOptions);

            // Start BufferBlockUsage pipeline with the input values.
            for (var i = 1; i <= numberOfIteration; i++)
            {
                bufferBlockGivenInputToSubscribers.Post($"Value = {i}");
            }

            // Mark the head of the pipeline as complete.
            bufferBlockGivenInputToSubscribers.Complete();

            // Waiting block to receive all post input.
            Task.WaitAll(actionBlockSubscriber1.Completion,
                         actionBlockSubscriber2.Completion,
                         actionBlockSubscriber3.Completion);

            return strings;
        }

        #endregion BufferBlockUsage

        #region BufferBlockUsageWithBoundedCapacity

        /// <summary>
        /// Post method to illustrate <see cref="BufferBlock{T}" /> and <see cref="DataflowBlockOptions.BoundedCapacity" />.
        /// Display a sequence of strings buffered.
        /// </summary>
        /// <param name="numberOfIteration">Number of iterations</param>
        /// <param name="capacity">Capacity of the bounded <see cref="ActionBlock{T}" /></param>
        /// <returns>A List of 'numberOfIteration' strings</returns>
        [HttpPost]
        [Route(nameof(BufferBlockUsageWithBoundedCapacity))]
        public IEnumerable<string> BufferBlockUsageWithBoundedCapacity(int numberOfIteration, int capacity = 1000)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow2BufferingBlocksController)} - {nameof(BufferBlockUsageWithBoundedCapacity)}");

            var strings = new BlockingCollection<string>();

            // Create the members of the pipeline.
            var bufferBlockGivenInputToSubscribers = new BufferBlock<string>();
            var actionBlockSubscriber1 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 1")
                , DataflowOptions.CreateBlockOptionsWithBoundedBoundedCapacity(capacity)
            );
            var actionBlockSubscriber2 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 2")
                , DataflowOptions.CreateBlockOptionsWithBoundedBoundedCapacity(capacity)
            );
            var actionBlockSubscriber3 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 3")
                , DataflowOptions.CreateBlockOptionsWithBoundedBoundedCapacity(capacity)
            );

            // Connect the dataflow blocks to form a pipeline.
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber1, DataflowOptions.LinkOptions);
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber2, DataflowOptions.LinkOptions);
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber3, DataflowOptions.LinkOptions);

            // Start BufferBlockUsageWithBoundedCapacity pipeline with the input values.
            for (var i = 1; i <= numberOfIteration; i++)
            {
                bufferBlockGivenInputToSubscribers.Post($"Value = {i}");
            }

            // Mark the head of the pipeline as complete.
            bufferBlockGivenInputToSubscribers.Complete();

            // Waiting block to receive all post input.
            Task.WaitAll(actionBlockSubscriber1.Completion,
                         actionBlockSubscriber2.Completion,
                         actionBlockSubscriber3.Completion);

            return strings;
        }

        #endregion BufferBlockUsageWithBoundedCapacity

        #region BufferBlockUsageWithFilters

        /// <summary>
        /// Post method to illustrate <see cref="BufferBlock{T}" />.
        /// Display a sequence of strings buffered with filters.
        /// </summary>
        /// <param name="numberOfIteration">Number of iterations</param>
        /// <param name="valueToFilterForSub1">The value that Subscriber 1 must filfter</param>
        /// <param name="valueToFilterForSub2">The value that Subscriber 2 must filfter</param>
        /// <returns>A List of 'numberOfIteration' strings</returns>
        [HttpPost]
        [Route(nameof(BufferBlockUsageWithFilters))]
        public IEnumerable<string> BufferBlockUsageWithFilters(int numberOfIteration, int valueToFilterForSub1, int valueToFilterForSub2)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow2BufferingBlocksController)} - {nameof(BufferBlockUsageWithFilters)}");

            // Convert input int into char
            var charToFilterForSub1 = (char)(valueToFilterForSub1 % 10 + 0x30);
            var char1ToFilterForSub2 = (char)(valueToFilterForSub2 % 10 + 0x30);
            var char2ToFilterForSub2 = (char)(valueToFilterForSub2 % 10 + 0x31);
            var char3ToFilterForSub2 = (char)(valueToFilterForSub2 % 10 + 0x32);

            var strings = new BlockingCollection<string>
            {
                $"Sub 1 - Will filter and keep only input containing '{charToFilterForSub1}'",
                $"Sub 2 - Will filter and keep only input containing '{char1ToFilterForSub2}' and '{char2ToFilterForSub2}' and '{char3ToFilterForSub2}'",
                $"Sub 3 - Will keep all other values, without filters"
            };

            // Create the members of the pipeline.
            var bufferBlockGivenInputToSubscribers = new BufferBlock<string>();
            var actionBlockSubscriber1 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 1 abc")
            );
            var actionBlockSubscriber2 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 2 def")
            );
            var actionBlockSubscriber3 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 3 ghi")
            );

            // Connect the dataflow blocks to form a pipeline.
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber1, DataflowOptions.LinkOptions,
                s => s.Contains(charToFilterForSub1)
            );
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber2, DataflowOptions.LinkOptions,
                s => s.IndexOfAny(new[] { char1ToFilterForSub2, char2ToFilterForSub2, char3ToFilterForSub2 }) > 0
            );
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber3, DataflowOptions.LinkOptions);

            // Start BufferBlockUsageWithFilters pipeline with the input values.
            for (var i = 1; i <= numberOfIteration; i++)
            {
                bufferBlockGivenInputToSubscribers.Post($"Value = {i}");
            }

            // Mark the head of the pipeline as complete.
            bufferBlockGivenInputToSubscribers.Complete();

            // Waiting block to receive all post input.
            Task.WaitAll(actionBlockSubscriber1.Completion,
                         actionBlockSubscriber2.Completion,
                         actionBlockSubscriber3.Completion);

            return strings.OrderBy(s => s);
        }

        #endregion BufferBlockUsageWithFilters

        #region BufferBlockUsageWithFiltersAndNullTarget

        /// <summary>
        /// Post method to illustrate <see cref="BufferBlock{T}" />.
        /// Display a sequence of strings buffered with filters and <see cref="DataflowBlock.NullTarget{T}" /> block.
        /// </summary>
        /// <param name="numberOfIteration">Number of iterations</param>
        /// <param name="valueToFilterForSub1">The value that Subscriber 1 must filfter</param>
        /// <param name="valueToFilterForSub2">The value that Subscriber 2 must filfter</param>
        /// <returns>A List of 'numberOfIteration' strings</returns>
        [HttpPost]
        [Route(nameof(BufferBlockUsageWithFiltersAndNullTarget))]
        public IEnumerable<string> BufferBlockUsageWithFiltersAndNullTarget(int numberOfIteration, int valueToFilterForSub1, int valueToFilterForSub2)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow2BufferingBlocksController)} - {nameof(BufferBlockUsageWithFiltersAndNullTarget)}");

            // Convert input int into char
            var charToFilterForSub1 = (char)(valueToFilterForSub1 % 10 + 0x30);
            var char1ToFilterForSub2 = (char)(valueToFilterForSub2 % 10 + 0x30);
            var char2ToFilterForSub2 = (char)(valueToFilterForSub2 % 10 + 0x31);
            var char3ToFilterForSub2 = (char)(valueToFilterForSub2 % 10 + 0x32);

            var strings = new BlockingCollection<string>
            {
                $"Sub 1 - Will filter and keep only input containing '{charToFilterForSub1}'",
                $"Sub 2 - Will filter and keep only input containing '{char1ToFilterForSub2}' and '{char2ToFilterForSub2}' and '{char3ToFilterForSub2}'",
            };

            // Create the members of the pipeline.
            var bufferBlockGivenInputToSubscribers = new BufferBlock<string>();
            var actionBlockSubscriber1 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 1 abc")
            );
            var actionBlockSubscriber2 = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Sub 2 def")
            );

            // Connect the dataflow blocks to form a pipeline.
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber1, DataflowOptions.LinkOptions,
                s => s.Contains(charToFilterForSub1)
            );
            bufferBlockGivenInputToSubscribers.LinkTo(actionBlockSubscriber2, DataflowOptions.LinkOptions,
                s => s.IndexOfAny(new[] { char1ToFilterForSub2, char2ToFilterForSub2, char3ToFilterForSub2 }) > 0
            );
            // All non filter items are not processed anymore.
            bufferBlockGivenInputToSubscribers.LinkTo(DataflowBlock.NullTarget<string>());

            // Start BufferBlockUsageWithFiltersAndNullTarget pipeline with the input values.
            for (var i = 1; i <= numberOfIteration; i++)
            {
                bufferBlockGivenInputToSubscribers.Post($"Value = {i}");
            }

            // Mark the head of the pipeline as complete.
            bufferBlockGivenInputToSubscribers.Complete();

            // Waiting block to receive all post input.
            Task.WaitAll(actionBlockSubscriber1.Completion,
                         actionBlockSubscriber2.Completion);

            return strings.OrderBy(s => s);
        }

        #endregion BufferBlockUsageWithFiltersAndNullTarget

        #region WriteOnceBlockUsage

        /// <summary>
        /// Post method to illustrate <see cref="WriteOnceBlock{T}" />.
        /// Display a sequence of strings going throught WriteOnceBlock.
        /// </summary>
        /// <param name="numberOfIteration">Number of iterations</param>
        /// <param name="minValue">First value of iteration</param>
        /// <returns>The only string that is consume by the block</returns>
        [HttpPost]
        [Route(nameof(WriteOnceBlockUsage))]
        public IEnumerable<string> WriteOnceBlockUsage(int numberOfIteration, int minValue)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow2BufferingBlocksController)} - {nameof(WriteOnceBlockUsage)}");

            var strings = new BlockingCollection<string>();

            // Create the members of the pipeline.
            var WriteOnceBlockGivenInputToASubscriber = new WriteOnceBlock<string>(null);
            var actionBlockSubscriber = new ActionBlock<string>(stringInput =>
                Functions.AddInputIntoTheGivenList(strings, stringInput, "Subscriber")
            );

            // Connect the dataflow blocks to form a pipeline.
            WriteOnceBlockGivenInputToASubscriber.LinkTo(actionBlockSubscriber, DataflowOptions.LinkOptions);

            // Start WriteOnceBlockUsage pipeline with the input values.
            for (var i = minValue; i <= minValue + numberOfIteration; i++)
            {
                WriteOnceBlockGivenInputToASubscriber.Post($"Value = {i}");
            }

            // Mark the head of the pipeline as complete.
            WriteOnceBlockGivenInputToASubscriber.Complete();

            // Wait for the last block in the pipeline to process all messages.
            actionBlockSubscriber.Completion.Wait();

            return strings;
        }

        #endregion WriteOnceBlockUsage
    }
}
