using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc;
using TplDataflow.Dataflow;
using TplDataflow.Model;

namespace TplDataflow.Controllers
{
    [Produces("application/json")]
    [Route("tpldataflow-api")]
    [ApiController]
    public class TplDataflow1ExecutionBlocksController : ControllerBase
    {
        #region TransformBlockUsage

        private const char SplitterSeparator = '#';

        /// <summary>
        /// Post method to illustrate <see cref="TransformBlock{TInput, TOutput}" />. Retrieves a metadata from an input string to split.
        /// </summary>
        /// <param name="stringToSplit">Input string to split by the char '#'<br />
        ///   <code>
        ///   NameOfTheImage#InitialUrl#Folder path with spaces
        ///   </code>
        /// </param>
        /// <returns>A <see cref="IMetaData" /></returns>
        [HttpPost]
        [Route(nameof(TransformBlockUsage))]
        public IMetaData TransformBlockUsage(string stringToSplit)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow1ExecutionBlocksController)} - {nameof(TransformBlockUsage)}");

            // Create the members of the pipeline.
            var transformBlockSplitAnInputStringIntoArray = new TransformBlock<string, string[]>(input =>
                Functions.SplitAnInputStringIntoArray(input, SplitterSeparator)
            );
            var transformBlockCreateASingleMedatadataFromStrings = new TransformBlock<string[], IMetaData>(stringArray =>
                Functions.CreateASingleMedatadataFromStrings(stringArray)
            );

            // Connect the dataflow blocks to form a pipeline.
            transformBlockSplitAnInputStringIntoArray.LinkTo(transformBlockCreateASingleMedatadataFromStrings, DataflowOptions.LinkOptions);

            // Start TransformBlockUsage pipeline with the input values.
            transformBlockSplitAnInputStringIntoArray.Post(stringToSplit);

            // Mark the head of the pipeline as complete.
            transformBlockSplitAnInputStringIntoArray.Complete();

            // Return the value transformed by the last TransformBlock.
            return transformBlockCreateASingleMedatadataFromStrings.Receive();
        }

        #endregion TransformBlockUsage

        #region ActionBlockUsage

        private const string SavedTextDirectory = @".\_dataflowTxt\";
        private readonly string BasedFileName = $"{nameof(ActionBlockUsage)}-";

        /// <summary>
        /// Post method to illustrate <see cref="ActionBlock{TInput}" />. Generates a text file.
        /// </summary>
        /// <param name="aNumber">A number to determine how many times lines will be repeated</param>
        [HttpPost]
        [Route(nameof(ActionBlockUsage))]
        public void ActionBlockUsage(int aNumber)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow1ExecutionBlocksController)} - {nameof(ActionBlockUsage)}");

            // Create the members of the pipeline.
            var transformBlockTransformIntIntoRepeatedLines = new TransformBlock<int, string>(anInt =>
                Functions.TransformIntIntoRepeatedLines(anInt)
            );
            var actionBlockModifyStringAndWriteInFile = new ActionBlock<string>(stringifiedInt =>
                Functions.ModifyStringAndWriteInFile(stringifiedInt, SavedTextDirectory, BasedFileName)
            );

            // Connect the dataflow blocks to form a pipeline.
            transformBlockTransformIntIntoRepeatedLines.LinkTo(actionBlockModifyStringAndWriteInFile, DataflowOptions.LinkOptions);

            // Start ActionBlockUsage pipeline with the input values.
            transformBlockTransformIntIntoRepeatedLines.Post(aNumber);

            // Mark the head of the pipeline as complete.
            transformBlockTransformIntIntoRepeatedLines.Complete();

            // Wait for the last block in the pipeline to process all messages.
            actionBlockModifyStringAndWriteInFile.Completion.Wait();
        }

        #endregion ActionBlockUsage

        #region TransformManyBlockUsage

        /// <summary>
        /// Post method to illustrate <see cref="TransformManyBlock{TInput, TOutput}" />. Retrieves some metadatas from an input string to split.
        /// </summary>
        /// <param name="stringToSplit">Input string to split by the char '#'<br />
        ///   <code>
        ///   NameOfTheImage#InitialUrl#Folder path with spaces
        ///   </code>
        /// </param>
        /// <returns>An enumeration of <see cref="IMetaData" /></returns>
        [HttpPost]
        [Route(nameof(TransformManyBlockUsage))]
        public IEnumerable<IMetaData> TransformManyBlockUsage(string stringToSplit)
        {
            Console.WriteLine($"Inside {nameof(TplDataflow1ExecutionBlocksController)} - {nameof(TransformManyBlockUsage)}");

            // Create the members of the pipeline.
            var transformManyBlockSplitAnInputStringIntoArray = new TransformManyBlock<string, string>(input =>
                Functions.SplitAnInputStringIntoArray(input, SplitterSeparator)
            );
            var transformBlockCreateASingleMedatadataFromAString = new TransformBlock<string, IMetaData>(stringInput =>
                Functions.CreateASingleMedatadataFromAString(stringInput)
            );

            // Connect the dataflow blocks to form a pipeline.
            transformManyBlockSplitAnInputStringIntoArray.LinkTo(transformBlockCreateASingleMedatadataFromAString, DataflowOptions.LinkOptions);

            // Start TransformManyBlockUsage pipeline with the input values.
            transformManyBlockSplitAnInputStringIntoArray.Post(stringToSplit);

            // Mark the head of the pipeline as complete.
            transformManyBlockSplitAnInputStringIntoArray.Complete();

            // Equivalent of transformManyBlockSplitAnInputStringIntoArray.OutputCount
            var ouputCount = stringToSplit?.Split(SplitterSeparator, StringSplitOptions.RemoveEmptyEntries).Length ?? 0;
            for (var i = 0; i < ouputCount; i++)
            {
                yield return transformBlockCreateASingleMedatadataFromAString.Receive();
            }
        }

        #endregion TransformManyBlockUsage
    }
}
