using Finance.Api.Models;

namespace Finance.Api.Business.Interfaces;

public interface ITransactionService
{
    Task<List<Transaction>> GetUserTransactions(Guid userId);
}