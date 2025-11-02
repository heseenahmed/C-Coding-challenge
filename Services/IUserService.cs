using InterviewTest.Dtos;
using InterviewTest.Entities;

namespace InterviewTest.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User dto, CancellationToken ct);
        Task<int> CreateBulkUsersAsync(CancellationToken ct);
        Task<PagedResult<User>> FetchUsersAsync(int page, int pageSize, CancellationToken ct);
    }
}
