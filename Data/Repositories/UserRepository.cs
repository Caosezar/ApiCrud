using ApiCrud.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ApiCrud.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApiCrudContext _context;
        public UserRepository(ApiCrudContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
