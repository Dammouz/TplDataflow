using System;
using System.Linq;

namespace TplDataflow.Model
{
    internal class DummyObject : IDummyObject
    {
        public DateTime DummyDate { get; set; }

        public int[] DummyTab { get; set; }

        public int? DummyTabLength => DummyTab?.Length;

        public string DummyString { get; }

        public string DummyExceptionMessage => DummyException?.Message;

        internal Exception DummyException { get; set; }

        private readonly Random _random = new Random();

        internal DummyObject(int index)
        {
            if (index < 0)
            {
                DummyException = new ArgumentOutOfRangeException($"{nameof(index)} must be positive but was : {index}");
                return;
            }

            var tabInt = new int[index * 3];
            var initializedTabInt = tabInt.Select(i => _random.Next(-1000, 10000)).ToArray();

            DummyDate = DateTime.Now.AddDays(index);
            DummyTab = initializedTabInt;
            DummyString = string.Join(" ; ", DummyTab);
        }
    }
}
