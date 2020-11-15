﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TplDataflow.Model;

namespace TplDataflow.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FirstPipelineController : ControllerBase
    {
        private const string WorkingDirectory = @".\imgDataflow\";
        private readonly ILogger<FirstPipelineController> _logger;

        public FirstPipelineController(ILogger<FirstPipelineController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Post method to retrieve metadata from a list of image URL.
        /// </summary>
        /// <param name="numberOfLines">number of maximum files to retrieve</param>
        /// <param name="pathToFile">Link to text file ( https://raw.githubusercontent.com/Dammouz/TplDataflow/master/WikimediaPicturesOfTheDayNovemberList.txt )</param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<IMetaData> Get(int numberOfLines, string pathToFile)
        {
            return Get(numberOfLines, pathToFile, false);
        }

        /// <summary>
        /// Get method to retrieve metadata from a list of image URL.
        /// </summary>
        /// <param name="numberOfLines">number of maximum files to retrieve</param>
        /// <param name="pathToFile">Link to text file ( https://raw.githubusercontent.com/Dammouz/TplDataflow/master/WikimediaPicturesOfTheDayNovemberList.txt )</param>
        /// <param name="order">Choose if failed object are displayed only at the end</param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<IMetaData> Get(int numberOfLines, string pathToFile, bool order)
        {
            _logger.LogWarning($"Inside {nameof(FirstPipelineController)}-{nameof(Get)}");

            if (numberOfLines < 1)
            {
                return new List<IMetaData> 
                {
                    new MetaData
                    {
                        Error = "You aks for number of line lower than 1"
                    }
                };
            }

            if (string.IsNullOrWhiteSpace(pathToFile))
            {
                return new List<IMetaData>
                {
                    new MetaData
                    {
                        Error = "The path file is null or empty"
                    }
                };
            }

            CleanWorkingDirectory();
            var listOfMetaData = new List<IMetaData>();

            //
            // Create the members of the pipeline.
            //

            var streamTextContent = new TransformBlock<string, string>(async uri =>
            {
                Console.WriteLine($"Downloading '{uri}'...");

                return await new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip
                })
                .GetStringAsync(uri);
            });

            var tranformContentIntoListOfUri = new TransformBlock<string, IList<string>>(content =>
            {
                Console.WriteLine("Tranforming content into list of URI...");

                var listOfUri = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                                       .Take(numberOfLines)
                                       .ToList();

                return listOfUri;
            });

            var transformListIntoSeveralUris = new TransformManyBlock<IList<string>, string>(listOfUri =>
            {
                Console.WriteLine("Transforming list into several URIs...");

                return listOfUri;
            });

            var downloadImageData = new TransformBlock<string, IMetaData>(url =>
            {
                Console.WriteLine("Downloading image data...");


                IMetaData metadata = null;

                try
                {
                    var fileName = MakeValidFileName(Path.GetFileName(url));
                    var imagePath = Path.Combine(WorkingDirectory, fileName);

                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(new Uri(url), imagePath);
                    }

                    using (var image = Image.FromFile(imagePath))
                    {
                        var width = image.Width;
                        var height = image.Height;
                        var creationTime = System.IO.File.GetCreationTime(imagePath);
                        var lastModificationTime = System.IO.File.GetLastWriteTime(imagePath);

                        metadata = new MetaData
                        {
                            Name = fileName,
                            InitialUrl = url,
                            Width = width,
                            Height = height,
                            CreationTime = creationTime,
                            LastModificationTime = lastModificationTime,
                            Folder = imagePath
                        };
                    }
                }
                catch (Exception ex)
                {
                    metadata = new MetaData
                    {
                        InitialUrl = url,
                        Error = ex.Message
                    };
                }

                return metadata ?? new MetaData
                    {
                        Error = "Can't retrieve metadata from URL"
                    };
            });

            var setStatusOfProcess = new ActionBlock<IMetaData>(metadata =>
            {
                Console.WriteLine("Seting the status of each metadata");
                metadata.Status = string.IsNullOrEmpty(metadata.Error) ? 1 : -1;
                listOfMetaData.Add(metadata);
            });

            //
            // Connect the dataflow blocks to form a pipeline.
            //

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
                ? listOfMetaData.OrderByDescending(md => md.Status)
                : (IEnumerable<IMetaData>)listOfMetaData;
        }

        private static string MakeValidFileName(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        private void CleanWorkingDirectory()
        {
            if (Directory.Exists(WorkingDirectory))
            {
                foreach (var file in Directory.GetFiles(WorkingDirectory, "*.jpg"))
                {
                    System.IO.File.Delete(file);
                }
            }
            else
            {
                Directory.CreateDirectory(WorkingDirectory);
            }
        }
    }
}
