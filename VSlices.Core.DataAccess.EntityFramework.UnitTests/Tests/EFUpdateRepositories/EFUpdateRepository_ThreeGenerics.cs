using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.DataAccess.EntityFramework.UnitTests.Contexts;

namespace VSlices.Core.DataAccess.EntityFramework.UnitTests.Tests.EFUpdateRepositories;

public class EFUpdateRepository_ThreeGenerics : IClassFixture<Fixture>
{

    private readonly AppDbContext _context;
    private readonly Mock<ILogger<EFUpdateRepository<AppDbContext, AppDbContext.Entity, AppDbContext.DbEntity>>> _mockedLogger;
    private readonly Mock<EFUpdateRepository<AppDbContext, AppDbContext.Entity, AppDbContext.DbEntity>> _mockedRepository;

    public EFUpdateRepository_ThreeGenerics(Fixture fixture)
    {
        _context = fixture.Context;
        _mockedLogger = new Mock<ILogger<EFUpdateRepository<AppDbContext, AppDbContext.Entity, AppDbContext.DbEntity>>>();
        _mockedRepository = new Mock<EFUpdateRepository<AppDbContext, AppDbContext.Entity, AppDbContext.DbEntity>>(_context, _mockedLogger.Object);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnBusinessFailure()
    {
        _context.ThrowOnSave = true;

        var entity = new AppDbContext.Entity(Guid.NewGuid(), null);
        var dbEntity = new AppDbContext.DbEntity
        {
            Id = entity.Id,
            Value = entity.Value
        };

        _mockedRepository.Setup(e => e.ConcurrencyMessageTemplate)
            .CallBase()
            .Verifiable();
        _mockedRepository.Setup(e => e.UpdateAsync(entity, default))
            .CallBase()
            .Verifiable();
        _mockedRepository.Setup(e => e.ToDatabaseEntity(entity))
            .Returns(dbEntity)
            .Verifiable();
        _mockedRepository.Setup(e => e.ProcessConcurrencyExceptionAsync(It.IsAny<DbUpdateConcurrencyException>(), default))
            .CallBase()
            .Verifiable();

        var expMessage = _mockedRepository.Object
            .ConcurrencyMessageTemplate
            .Replace("{EntityType}", typeof(AppDbContext.DbEntity).Namespace)
            .Replace("{EntityJson}", JsonSerializer.Serialize(entity));

        var response = await _mockedRepository.Object.UpdateAsync(entity);

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
    public async Task UpdateAsync_ShouldReturnSuccess()
    {
        _context.ChangeTracker.Clear();
        _context.ThrowOnSave = false;

        var entity = new AppDbContext.Entity(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2), null);
        var dbEntity = new AppDbContext.DbEntity
        {
            Id = entity.Id,
            Value = entity.Value
        };

        _context.Set<AppDbContext.DbEntity>()
            .Add(dbEntity);

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        entity = new AppDbContext.Entity(Guid.NewGuid(), "AddedValue");
        dbEntity = new AppDbContext.DbEntity
        {
            Id = entity.Id,
            Value = entity.Value
        };

        _mockedRepository.Setup(e => e.ToDatabaseEntity(entity))
            .Returns(dbEntity)
            .Verifiable();
        _mockedRepository.Setup(e => e.ToEntity(dbEntity))
            .Returns(entity)
            .Verifiable();
        _mockedRepository.Setup(e => e.UpdateAsync(entity, default))
            .CallBase()
            .Verifiable();

        var response = await _mockedRepository.Object.UpdateAsync(entity);

        response.IsT0.Should().BeTrue();
        response.AsT0.Should().Be(entity);

        _mockedLogger.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();

        _context.Set<AppDbContext.DbEntity>()
            .Any(e => e.Id == dbEntity.Id && e.Value == dbEntity.Value)
            .Should().BeTrue();
    }
}
