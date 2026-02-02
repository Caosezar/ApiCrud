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
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
