using ApiCrud.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCrud.Data.Repositories
{
    public class CaioRepository : ICaioRepository
    {
        private readonly ApiCrudContext _context;

        public CaioRepository(ApiCrudContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Caio>> GetAllCaiosAsync()
        {
            return (IEnumerable<Caio>)await _context.Caios.ToListAsync();
        }
    }
}