using Microsoft.EntityFrameworkCore;
using Finance.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Finance.Api.Data;

public class FinanceDbContext : IdentityDbContext<ApplicationUser>
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; } = null!;
}