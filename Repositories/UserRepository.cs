using Microsoft.Extensions.Hosting;
using Reddit.Models;
using System.Linq.Expressions;

namespace Reddit.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext applcationDBContext)
        {
            _context = applcationDBContext;
        }
        public async Task<PagedList<User>> GetUsers(int pageNumber, int pageSize, string? searchKey, string? sortKey = null, bool isAscending = true)
        {
            var users = _context.Users.AsQueryable();

            if (searchKey != null)
            {
                users = users.Where(p => p.Name.Contains(searchKey) || p.Email.Contains(searchKey));
            }

            if (isAscending)
            {
                users = users.OrderBy(GetSortExpression(sortKey));
            }
            else
            {
                users = users.OrderByDescending(GetSortExpression(sortKey));
            }

            return await PagedList<User>.CreateAsync(users, pageNumber, pageSize);
        }

        private Expression<Func<User, object>> GetSortExpression(string? sortKey)
        {
            sortKey = sortKey?.ToLower();
            return sortKey switch
            {
                "numberOfPosts" => users => users.Posts.Count,
                "Id" => users => users.Id,
                _ => users => users.Id
            };
        }
    }
} 
