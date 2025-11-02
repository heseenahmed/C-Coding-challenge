using InterviewTest.Data;
using InterviewTest.Dtos;
using InterviewTest.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InterviewTest.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IMemoryCache _cache;
        private const string FetchCacheKey = "all_users_cache_v1";

        public UserService(AppDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<User> CreateUserAsync(User dto, CancellationToken ct)
        {
            dto.Id = Guid.NewGuid();
            dto.TimeStamp = DateTime.UtcNow;

            await _db.Users.AddAsync(dto, ct);
            await _db.SaveChangesAsync(ct);
            _cache.Remove(FetchCacheKey);
            return dto;
        }

        public async Task<int> CreateBulkUsersAsync(CancellationToken ct)
        {
            const int total = 10_000;
            const int batchSize = 1_000;

            // Start index AFTER current count (so multiple runs won't collide)
            var offset = await _db.Users.CountAsync(ct);

            var faker = new Bogus.Faker<User>()
                .RuleFor(u => u.Id, _ => Guid.NewGuid())
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Age, f => f.Random.Int(18, 70))
                // Email is guaranteed unique: user{index}@demo.local
                // index = offset + i (we'll set it inside the loop)
                .RuleFor(u => u.Email, _ => "placeholder")   // will be overwritten per item
                .RuleFor(u => u.TimeStamp, _ => DateTime.UtcNow);

            int inserted = 0;
            for (int b = 0; b < total / batchSize; b++)
            {
                var batch = new List<User>(batchSize);
                for (int i = 0; i < batchSize; i++)
                {
                    var u = faker.Generate();
                    var index = offset + inserted + i + 1; // 1-based
                                                           // Avoid any dupes: stable, unique, and repeatable
                    u.Email = $"user{index}@demo.local";
                    batch.Add(u);
                }

                await _db.Users.AddRangeAsync(batch, ct);
                await _db.SaveChangesAsync(ct);
                inserted += batch.Count;
            }

            _cache.Remove(FetchCacheKey); // invalidate cache
            return  inserted ;
        }

        public async Task<PagedResult<User>> FetchUsersAsync(int page, int pageSize, CancellationToken ct)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 500) pageSize = 500; // cap to protect the API

            var query = _db.Users.AsNoTracking().OrderBy(u => u.Name);

            var total = await query.CountAsync(ct);
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<User>(items, page, pageSize, total, totalPages);
        }
    }
}
