using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc;
using TplDataflow.Common;
using TplDataflow.Dataflow;
using TplDataflow.Model;

namespace TplDataflow.Controllers
{
    [Produces("application/json")]
    [Route("tpldataflow-api")]
    [ApiController]
    public class TplDataflow9FullSampleController : ControllerBase
    {
        private const string SavedImageDirectory = @".\_dataflowImg\";
        private const string DefaultLink = "https://raw.githubusercontent.com/Dammouz/TplDataflow/master/WikimediaPicturesOfTheDayNovemberList.txt";

        /// <summary>
        /// Post method to illustrate a whole pipeline.
        /// Retrieves metadata from a list of image URL.
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
            Console.WriteLine($"Inside {nameof(TplDataflow9FullSampleController)} - {nameof(GetMetadatasFromAList)}");
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
            transformBlockStreamTextContent.LinkTo(transformBlockTranformContentIntoListOfUri, DataflowOptions.LinkOptions);
            transformBlockTranformContentIntoListOfUri.LinkTo(transformManyBlockTransformListIntoSeveralUris, DataflowOptions.LinkOptions);
            transformManyBlockTransformListIntoSeveralUris.LinkTo(transformBlockDownloadImageData, DataflowOptions.LinkOptions);
            transformBlockDownloadImageData.LinkTo(actionBlockSetStatusOfProcess, DataflowOptions.LinkOptions);

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
    }
}
