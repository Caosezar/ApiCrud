namespace ApiCrud.Data.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<Models.User>> GetAllUsersAsync();
    }
}
