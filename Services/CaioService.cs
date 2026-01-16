using ApiCrud.Data.Repositories;
using ApiCrud.Models;

namespace ApiCrud.Services
{
    public class CaioService : ICaioService
    {
        private readonly ICaioRepository _repository;

        public CaioService(ICaioRepository repository)
        {
            _repository = repository;
        }

        public async Task <IEnumerable<Caio>> GetAllCaiosAsync()
        {
            var caios = await _repository.GetAllCaiosAsync();
            return caios;
        }
    }
}
