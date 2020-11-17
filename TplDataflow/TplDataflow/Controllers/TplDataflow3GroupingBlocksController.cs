using Microsoft.AspNetCore.Mvc;

namespace TplDataflow.Controllers
{
    [Produces("application/json")]
    [Route("tpldataflow-api")]
    [ApiController]
    public class TplDataflow3GroupingBlocksController : ControllerBase
    {
        // C. Grouping Blocks
        // C.1. BatchBlock(T)
        // C.2. JoinBlock(T1, T2, ...)
        // C.3. BatchedJoinBlock(T1, T2, ...)
    }
}
