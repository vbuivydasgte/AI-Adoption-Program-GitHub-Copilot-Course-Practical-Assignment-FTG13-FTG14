using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}
