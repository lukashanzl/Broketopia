using Microsoft.EntityFrameworkCore;
using Finance.Api.Models;

namespace Finance.Api.Data;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; } = null!;
}