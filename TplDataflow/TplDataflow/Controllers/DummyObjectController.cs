using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TplDataflow.Model;

namespace TplDataflow.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class DummyObjectController : ControllerBase
    {
        private readonly ILogger<DummyObjectController> _logger;

        public DummyObjectController(ILogger<DummyObjectController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<IDummyObject> Get(int numberOfElement)
        {
            _logger.LogWarning($"Inside {nameof(DummyObjectController)}-{nameof(Get)}");

            if (numberOfElement < 1)
            {
                return new List<IDummyObject>
                {
                    new DummyObject(numberOfElement)
                };
            }

            return Enumerable.Range(1, numberOfElement)
                .Select(index => new DummyObject(index))
                .ToArray();
        }
    }
}
