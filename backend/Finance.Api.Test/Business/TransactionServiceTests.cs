using Finance.Api.Business;
using Finance.Api.Data.Repositories.Interfaces;
using Finance.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Finance.Api.Test.Business;

public class TransactionServiceTests
{
    private Mock<ITransactionRepository> _mockRepo = null!;
    private Mock<ILogger<TransactionService>> _mockLogger = null!;
    private TransactionService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<ITransactionRepository>();
        _mockLogger = new Mock<ILogger<TransactionService>>();
        _service = new TransactionService(_mockRepo.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetUserTransactions_ValidUserId_ReturnsTransactions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactions = new List<Transaction>
        {
            new Transaction { Id = 1, UserId = userId, Amount = 100 },
            new Transaction { Id = 2, UserId = userId, Amount = 200 }
        };

        _mockRepo
            .Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(transactions);

        // Act
        var result = await _service.GetUserTransactions(userId);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Amount, Is.EqualTo(100));
    }
}