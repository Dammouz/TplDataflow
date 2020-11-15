using System.IO;

namespace TplDataflow.Common
{
    internal static class CommonHelpers
    {
        internal static string MakeValidFileName(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        internal static void CleanWorkingDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.GetFiles(directory, "*.jpg"))
                {
                    File.Delete(file);
                }
            }
            else
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
