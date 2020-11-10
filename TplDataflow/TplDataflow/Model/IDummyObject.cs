using System;

namespace TplDataflow.Model
{
    public interface IDummyObject
    {
        DateTime DummyDate { get; set; }
        int[] DummyTab { get; set; }
        int? DummyTabLength { get; }
        string DummyString { get; }
        string DummyExceptionMessage { get; }
    }
}
