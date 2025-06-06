using Finance.Api.Business.Interfaces;
using Finance.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Transactions;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{   
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all transactions for a given user.
    /// </summary>
    /// <param name="userId">The user ID as a string (GUID format)</param>
    /// <returns>A list of transactions</returns>
    [HttpGet("{userId}")]
    public async Task<ActionResult<List<Transaction>>> GetUserTransactions(string userId)
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            _logger.LogError("Invalid input for user id: {UserId}", userId);
            return BadRequest("Invalid user ID format.");
        }

        var transactions = await _transactionService.GetUserTransactions(parsedUserId);
        return Ok(transactions);
    }
}