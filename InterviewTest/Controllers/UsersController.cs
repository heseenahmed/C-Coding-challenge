using InterviewTest.Entities;
using InterviewTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterviewTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService service) : ControllerBase
    {

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] User dto, CancellationToken ct)
        => Ok(await service.CreateUserAsync(dto, ct));

        [HttpPost("create-bulk-users")]
        public async Task<IActionResult> CreateBulk(CancellationToken ct)
            => Ok(new { inserted = await service.CreateBulkUsersAsync(ct) });

        [HttpGet("fetch-users")]
        public async Task<IActionResult> Fetch([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
       => Ok(await service.FetchUsersAsync(page, pageSize, ct));
    }
}
