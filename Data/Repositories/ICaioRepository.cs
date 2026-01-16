using ApiCrud.Models;

namespace ApiCrud.Data.Repositories
{
    public interface ICaioRepository
    {
        Task<IEnumerable<Caio>> GetAllCaiosAsync();
    }
}
