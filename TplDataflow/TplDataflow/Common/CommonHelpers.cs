using System.IO;
using System.Text;

namespace TplDataflow.Common
{
    internal static class CommonHelpers
    {
        internal static string MakeValidFileName(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        internal static void WriteContentInAFile(string savedTextDirectory, string modifiedString, string fileName)
        {
            using (var swFile = new StreamWriter(Path.Combine(savedTextDirectory, fileName), true, Encoding.UTF8))
            {
                swFile.WriteLine(modifiedString);
            }
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
