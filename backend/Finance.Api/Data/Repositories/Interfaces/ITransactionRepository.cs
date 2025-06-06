using Finance.Api.Models;

namespace Finance.Api.Data.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetByUserIdAsync(Guid userId);
}