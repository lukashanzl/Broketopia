using Finance.Api.Business.Interfaces;
using Finance.Api.Data.Repositories.Interfaces;
using Finance.Api.Models;

namespace Finance.Api.Business;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionRepository repository, ILogger<TransactionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public Task<List<Transaction>> GetUserTransactions(Guid userId)
    {
        var result = _repository.GetByUserIdAsync(userId);
        _logger.LogInformation("Fetching {count} transactions for user {userId}", result.Result.Count, userId);

        return result;
    }
}