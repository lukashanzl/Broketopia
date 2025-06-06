using Finance.Api.Data.Repositories.Interfaces;
using Finance.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<TransactionRepository> _logger;

    public TransactionRepository(FinanceDbContext context, ILogger<TransactionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public Task<List<Transaction>> GetByUserIdAsync(Guid userId)
    {
        return _context.Transactions.Where(c => c.UserId == userId).ToListAsync();
    }
}