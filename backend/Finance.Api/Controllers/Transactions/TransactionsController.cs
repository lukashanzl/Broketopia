using System.Security.Claims;
using Finance.Api.Business.Interfaces;
using Finance.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Finance.Api.Controllers.Transactions;

[ApiController]
[Route("api/[controller]")]
[Authorize] // 🔐 This secures all endpoints in this controller
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
    [HttpGet]
    public async Task<ActionResult<List<Transaction>>> GetUserTransactions()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            _logger.LogError("Invalid user ID in JWT.");
            return Unauthorized("Invalid user token.");
        }

        var transactions = await _transactionService.GetUserTransactions(parsedUserId);
        return Ok(transactions);
    }
}