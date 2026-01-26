using ApiCrud.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCrud.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApiCrudContext _context;

        public UserRepository(ApiCrudContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}