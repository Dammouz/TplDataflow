using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TplDataflow.Common;
using TplDataflow.Model;

namespace TplDataflow.Dataflow
{
    internal static class Functions
    {
        #region Functions used by GetMetadataFromFile

        internal static IEnumerable<IMetaData> ReturnOnlyOneMetadaInError(string error)
        {
            return new List<IMetaData>
            {
                new MetaData
                {
                    Error = error
                }
            };
        }

        internal async static Task<string> StreamTextContent(string uri)
        {
            Console.WriteLine($"{nameof(StreamTextContent)} - Downloading '{uri}'...");

            return await new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            })
            .GetStringAsync(uri);
        }

        internal static IList<string> TranformContentIntoListOfUri(string content, int numberOfLines)
        {
            Console.WriteLine($"{nameof(TranformContentIntoListOfUri)} - Tranforming content into list of URI...");

            var listOfUri = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                                   .Take(numberOfLines)
                                   .ToList();

            return listOfUri;
        }

        internal static IEnumerable<string> TransformListIntoSeveralUris(IList<string> listOfUri)
        {
            Console.WriteLine($"{nameof(TransformListIntoSeveralUris)} - Transforming list into several URIs...");

            foreach (var uri in listOfUri)
            {
                yield return uri;
            }
        }

        internal static IMetaData DownloadImageData(string url, string directoryPath)
        {
            Console.WriteLine($"{nameof(DownloadImageData)} - Downloading image data...");

            IMetaData metadata = null;

            try
            {
                var fileName = CommonHelpers.MakeValidFileName(Path.GetFileName(url));
                var imagePath = Path.Combine(directoryPath, fileName);

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, imagePath);
                }

                using (var image = Image.FromFile(imagePath))
                {
                    var width = image.Width;
                    var height = image.Height;
                    var creationTime = File.GetCreationTime(imagePath);
                    var lastModificationTime = File.GetLastWriteTime(imagePath);

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
        }

        internal static void SetStatusOfProcess(IList<IMetaData> listOfMetadata, IMetaData metadata)
        {
            Console.WriteLine($"{nameof(SetStatusOfProcess)} - Seting the status of each metadata");
            metadata.Status = string.IsNullOrEmpty(metadata.Error) ? 1 : -888;
            listOfMetadata.Add(metadata);
        }

        #endregion Functions used by GetMetadataFromFile

        #region Functions used by TransformBlockUsage

        internal static string[] SplitAnInputString(string input, char splitterSeparator)
        {
            Console.WriteLine($"{nameof(SplitAnInputString)} - Splitting {input} by the char '{splitterSeparator}'...");

            var splittedInput = input?.Split(splitterSeparator);
            if (splittedInput == null || splittedInput.Length < 1)
            {
                return new[]
                {
                    "-0",
                    string.Empty,
                    string.Empty
                };
            }

            if (splittedInput.Length == 1)
            {
                return new[]
                {
                    "-100",
                    splittedInput[0],
                    "Array contains only 1 element"
                };
            }

            if (splittedInput.Length == 2)
            {
                return new[]
                {
                    "-200",
                    splittedInput[0],
                    splittedInput[1]
                };
            }

            return splittedInput;
        }

        internal static IMetaData CreateASingleMedatadataFromStrings(string[] stringArray)
        {
            Console.WriteLine($"{nameof(CreateASingleMedatadataFromStrings)} - Creating the metadata based on value : '{string.Join(" - ", stringArray)}'...");

            var status = int.TryParse(stringArray[0], out var parsedInt) ? parsedInt : 1;
            return new MetaData
            {
                Name = stringArray[0],
                InitialUrl = stringArray[1],
                Folder = stringArray[2],
                Status = status
            };
        }

        #endregion Functions used by TransformBlockUsage
    }
}
