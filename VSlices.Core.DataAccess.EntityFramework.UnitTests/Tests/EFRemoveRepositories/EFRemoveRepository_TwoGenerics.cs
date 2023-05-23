using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.DataAccess.EntityFramework.UnitTests.Contexts;

namespace VSlices.Core.DataAccess.EntityFramework.UnitTests.Tests.EFRemoveRepositories;

public class EFRemoveRepository_TwoGenerics : IClassFixture<Fixture>
{
    private readonly AppDbContext _context;
    private readonly Mock<ILogger<EFRemoveRepository<AppDbContext, AppDbContext.Entity>>> _mockedLogger;
    private readonly Mock<EFRemoveRepository<AppDbContext, AppDbContext.Entity>> _mockedRepository;

    public EFRemoveRepository_TwoGenerics(Fixture fixture)
    {
        _context = fixture.Context;
        _mockedLogger = new Mock<ILogger<EFRemoveRepository<AppDbContext, AppDbContext.Entity>>>();
        _mockedRepository = new Mock<EFRemoveRepository<AppDbContext, AppDbContext.Entity>>(_context, _mockedLogger.Object);
    }

    [Fact]
    public async Task RemoveAsync_ShouldReturnBusinessFailure()
    {
        _context.ThrowOnSave = true;

        var entity = new AppDbContext.Entity(Guid.NewGuid(), null);

        _mockedRepository.Setup(e => e.ConcurrencyMessageTemplate)
            .CallBase()
            .Verifiable();
        _mockedRepository.Setup(e => e.RemoveAsync(entity, default))
            .CallBase()
            .Verifiable();
        _mockedRepository.Setup(e => e.ProcessConcurrencyExceptionAsync(It.IsAny<DbUpdateConcurrencyException>(), default))
            .CallBase()
            .Verifiable();

        var expMessage = _mockedRepository.Object
            .ConcurrencyMessageTemplate
            .Replace("{EntityType}", typeof(AppDbContext.Entity).Namespace)
            .Replace("{EntityJson}", JsonSerializer.Serialize(entity));

        var response = await _mockedRepository.Object.RemoveAsync(entity);

        response.IsT1.Should().BeTrue();
        response.AsT1.Errors.Should().BeEmpty();
        response.AsT1.Kind.Should().Be(FailureKind.ConcurrencyError);

        _mockedLogger.Verify(e =>
            e.Log(It.Is<LogLevel>(l => l == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == expMessage),
                It.IsAny<DbUpdateConcurrencyException>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
        _mockedLogger.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveAsync_ShouldReturnSuccess()
    {
        _context.ChangeTracker.Clear();
        _context.ThrowOnSave = false;

        var entity = new AppDbContext.Entity(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3), null);

        _context.Set<AppDbContext.Entity>()
            .Add(entity);

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        entity = new AppDbContext.Entity(entity.Id, "AddedValue");

        _mockedRepository.Setup(e => e.RemoveAsync(entity, default))
            .CallBase()
            .Verifiable();

        var response = await _mockedRepository.Object.RemoveAsync(entity);

        response.IsT0.Should().BeTrue();
        response.AsT0.Should().Be(entity);

        _mockedLogger.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();

        _context.ChangeTracker.Clear();

        _context.Set<AppDbContext.Entity>()
            .Any(e => e.Id == entity.Id && e.Value == entity.Value)
            .Should().BeFalse();
    }
}
