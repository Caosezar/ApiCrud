using ApiCrud.Models;

namespace ApiCrud.Services
{
    public interface ICaioService
    {
        Task<IEnumerable<Caio>> GetAllCaiosAsync();
    }
}
