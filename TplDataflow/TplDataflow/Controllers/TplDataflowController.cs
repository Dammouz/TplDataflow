using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TplDataflow.Common;
using TplDataflow.Dataflow;
using TplDataflow.Model;

namespace TplDataflow.Controllers
{
    [Produces("application/json")]
    [Route("tpldataflow-api")]
    [ApiController]
    public class TplDataflowController : ControllerBase
    {
        #region .ctor & private properties

        private readonly ILogger<TplDataflowController> _logger;

        private readonly DataflowLinkOptions LinkOptions = new DataflowLinkOptions
        {
            PropagateCompletion = true
        };

        public TplDataflowController(ILogger<TplDataflowController> logger)
        {
            _logger = logger;
        }

        #endregion .ctor & private properties

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
        /// <returns>Returns a <see cref="IMetaData" /></returns>
        [HttpPost]
        [Route(nameof(TransformBlockUsage))]
        public IMetaData TransformBlockUsage(string stringToSplit)
        {
            Console.WriteLine($"Inside {nameof(TplDataflowController)} - {nameof(TransformBlockUsage)}");

            // Create the members of the pipeline.
            var transformBlockSplitAnInputStringIntoArray = new TransformBlock<string, string[]>(input =>
                Functions.SplitAnInputStringIntoArray(input, SplitterSeparator)
            );
            var transformBlockCreateASingleMedatadataFromStrings = new TransformBlock<string[], IMetaData>(stringArray =>
                Functions.CreateASingleMedatadataFromStrings(stringArray)
            );

            // Connect the dataflow blocks to form a pipeline.
            transformBlockSplitAnInputStringIntoArray.LinkTo(transformBlockCreateASingleMedatadataFromStrings, LinkOptions);

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
            Console.WriteLine($"Inside {nameof(TplDataflowController)} - {nameof(ActionBlockUsage)}");

            // Create the members of the pipeline.
            var transformBlockTransformIntIntoRepeatedLines = new TransformBlock<int, string>(anInt =>
                Functions.TransformIntIntoRepeatedLines(anInt)
            );
            var actionBlockModifyStringAndWriteInFile = new ActionBlock<string>(stringifiedInt =>
                Functions.ModifyStringAndWriteInFile(stringifiedInt, SavedTextDirectory, BasedFileName)
            );

            // Connect the dataflow blocks to form a pipeline.
            transformBlockTransformIntIntoRepeatedLines.LinkTo(actionBlockModifyStringAndWriteInFile, LinkOptions);

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
        /// <returns>Returns an enumeration of <see cref="IMetaData" /></returns>
        [HttpPost]
        [Route(nameof(TransformManyBlockUsage))]
        public IEnumerable<IMetaData> TransformManyBlockUsage(string stringToSplit)
        {
            Console.WriteLine($"Inside {nameof(TplDataflowController)} - {nameof(TransformManyBlockUsage)}");

            // Create the members of the pipeline.
            var transformManyBlockSplitAnInputStringIntoArray = new TransformManyBlock<string, string>(input =>
                Functions.SplitAnInputStringIntoArray(input, SplitterSeparator)
            );
            var transformBlockCreateASingleMedatadataFromAString = new TransformBlock<string, IMetaData>(stringInput =>
                Functions.CreateASingleMedatadataFromAString(stringInput)
            );

            // Connect the dataflow blocks to form a pipeline.
            transformManyBlockSplitAnInputStringIntoArray.LinkTo(transformBlockCreateASingleMedatadataFromAString, LinkOptions);

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

        #region GetMetadatasFromAList

        private const string SavedImageDirectory = @".\_dataflowImg\";
        private const string DefaultLink = "https://raw.githubusercontent.com/Dammouz/TplDataflow/master/WikimediaPicturesOfTheDayNovemberList.txt";

        /// <summary>
        /// Post method to illustrate a whole pipeline. Retrieves metadata from a list of image URL.
        /// </summary>
        /// <param name="pathToFile">Link to text file containing the list<br />
        ///   <code>
        ///   https://raw.githubusercontent.com/Dammouz/TplDataflow/master/WikimediaPicturesOfTheDayNovemberList.txt
        ///   </code>
        /// </param>
        /// <param name="numberOfLines">Number of maximum files to retrieve</param>
        /// <param name="order">Order ouput by <see cref="IMetaData.Status" /> value</param>
        /// <returns>Some <see cref="IMetaData" /> of downloaded images</returns>
        [HttpPost]
        [Route(nameof(GetMetadatasFromAList))]
        public IEnumerable<IMetaData> GetMetadatasFromAList(string pathToFile, int numberOfLines, bool order = false)
        {
            Console.WriteLine($"Inside {nameof(TplDataflowController)} - {nameof(GetMetadatasFromAList)}");
            CommonHelpers.CleanWorkingDirectory(SavedImageDirectory);
            var listOfMetadata = new List<IMetaData>();

            if (string.IsNullOrWhiteSpace(pathToFile))
            {
                return Functions.ReturnOnlyOneMetadaInError("The path file is null or empty");
            }

            if (numberOfLines < 1)
            {
                return Functions.ReturnOnlyOneMetadaInError("You aks for number of line lower than 1");
            }


            // Create the members of the pipeline.
            var transformBlockStreamTextContent = new TransformBlock<string, string>(uri =>
                Functions.StreamTextContent(uri)
            );
            var transformBlockTranformContentIntoListOfUri = new TransformBlock<string, IList<string>>(content =>
                Functions.TranformContentIntoListOfUri(content, numberOfLines)
            );
            var transformManyBlockTransformListIntoSeveralUris = new TransformManyBlock<IList<string>, string>(listOfUri =>
                Functions.TransformListIntoSeveralUris(listOfUri)
            );
            var transformBlockDownloadImageData = new TransformBlock<string, IMetaData>(url =>
                Functions.DownloadImageData(url, SavedImageDirectory)
            );
            var actionBlockSetStatusOfProcess = new ActionBlock<IMetaData>(metadata =>
                Functions.SetStatusOfProcess(listOfMetadata, metadata)
            );


            // Connect the dataflow blocks to form a pipeline.
            transformBlockStreamTextContent.LinkTo(transformBlockTranformContentIntoListOfUri, LinkOptions);
            transformBlockTranformContentIntoListOfUri.LinkTo(transformManyBlockTransformListIntoSeveralUris, LinkOptions);
            transformManyBlockTransformListIntoSeveralUris.LinkTo(transformBlockDownloadImageData, LinkOptions);
            transformBlockDownloadImageData.LinkTo(actionBlockSetStatusOfProcess, LinkOptions);

            // Start GetMetadatasFromAList pipeline with the input values.
            transformBlockStreamTextContent.Post(pathToFile);

            // Mark the head of the pipeline as complete.
            transformBlockStreamTextContent.Complete();

            // Wait for the last block in the pipeline to process all messages.
            actionBlockSetStatusOfProcess.Completion.Wait();

            return order
                ? listOfMetadata.OrderByDescending(metadata => metadata.Status)
                : (IEnumerable<IMetaData>)listOfMetadata;
        }

        #endregion GetMetadatasFromAList
    }
}
