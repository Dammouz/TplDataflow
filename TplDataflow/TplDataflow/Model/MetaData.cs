using System;

namespace TplDataflow.Model
{
    internal class MetaData : IMetaData
    {
        public string Name { get; set; }
        public string InitialUrl { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string Folder { get; set; }
        public string Error { get; set; }
        public int Status { get; set; }
    }
}
