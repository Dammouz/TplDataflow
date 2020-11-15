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
    [Route("api/tplDataflow")]
    [ApiController]
    public class TplDataflowController : ControllerBase
    {
        private readonly ILogger<TplDataflowController> _logger;
        private const string WorkingDirectory = @".\imgDataflow\";
        private const string DefaultLink = "https://raw.githubusercontent.com/Dammouz/TplDataflow/master/WikimediaPicturesOfTheDayNovemberList.txt";

        public TplDataflowController(ILogger<TplDataflowController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Post method to retrieve metadata from a list of image URL.
        /// </summary>
        /// <param name="pathToFile">Link to text file containing the list<br />
        /// https://raw.githubusercontent.com/Dammouz/TplDataflow/master/WikimediaPicturesOfTheDayNovemberList.txt </param>
        /// <param name="numberOfLines">Number of maximum files to retrieve</param>
        /// <param name="order"></param>
        /// <returns>Some metadata of downloaded images</returns>
        [HttpPost]
        [Route(nameof(GetMetadataFromFile))]
        public IEnumerable<IMetaData> GetMetadataFromFile(string pathToFile, int numberOfLines, bool order = false)
        {
            Console.WriteLine($"Inside {nameof(TplDataflowController)} - {nameof(GetMetadataFromFile)}");
            CommonHelpers.CleanWorkingDirectory(WorkingDirectory);
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

            var streamTextContent = new TransformBlock<string, string>(uri => Functions.StreamTextContent(uri));

            var tranformContentIntoListOfUri = new TransformBlock<string, IList<string>>(content => Functions.TranformContentIntoListOfUri(content, numberOfLines));

            var transformListIntoSeveralUris = new TransformManyBlock<IList<string>, string>(listOfUri => Functions.TransformListIntoSeveralUris(listOfUri));

            var downloadImageData = new TransformBlock<string, IMetaData>(url => Functions.DownloadImageData(url, WorkingDirectory));

            var setStatusOfProcess = new ActionBlock<IMetaData>(metadata => Functions.SetStatusOfProcess(listOfMetadata, metadata));


            // Connect the dataflow blocks to form a pipeline.

            var linkOptions = new DataflowLinkOptions
            {
                PropagateCompletion = true
            };

            streamTextContent.LinkTo(tranformContentIntoListOfUri, linkOptions);
            tranformContentIntoListOfUri.LinkTo(transformListIntoSeveralUris, linkOptions);
            transformListIntoSeveralUris.LinkTo(downloadImageData, linkOptions);
            downloadImageData.LinkTo(setStatusOfProcess, linkOptions);

            streamTextContent.Post(pathToFile);

            // Mark the head of the pipeline as complete.
            streamTextContent.Complete();

            // Wait for the last block in the pipeline to process all messages.
            setStatusOfProcess.Completion.Wait();

            return order
                ? listOfMetadata.OrderByDescending(metadata => metadata.Status)
                : (IEnumerable<IMetaData>)listOfMetadata;
        }
    }
}
