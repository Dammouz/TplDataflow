using System;

namespace TplDataflow.Model
{
    public interface IMetaData
    {
        string Name { get; set; }
        string InitialUrl { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        DateTime CreationTime { get; set; }
        DateTime LastModificationTime { get; set; }
        string Folder { get; set; }
        string Error { get; set; }
        int Status { get; set; }
    }
}
