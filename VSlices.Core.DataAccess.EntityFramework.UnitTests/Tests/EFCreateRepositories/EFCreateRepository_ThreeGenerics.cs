using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.DataAccess.EntityFramework.UnitTests.Contexts;

namespace VSlices.Core.DataAccess.EntityFramework.UnitTests.Tests.EFCreateRepositories;

public class EFCreateRepository_ThreeGenerics : IClassFixture<Fixture>
{

    private readonly AppDbContext _context;
    private readonly Mock<ILogger<EFCreateRepository<AppDbContext, AppDbContext.Entity, AppDbContext.DbEntity>>> _mockedLogger;
    private readonly Mock<EFCreateRepository<AppDbContext, AppDbContext.Entity, AppDbContext.DbEntity>> _mockedRepository;

    public EFCreateRepository_ThreeGenerics(Fixture fixture)
    {
        _context = fixture.Context;
        _mockedLogger = new Mock<ILogger<EFCreateRepository<AppDbContext, AppDbContext.Entity, AppDbContext.DbEntity>>>();
        _mockedRepository = new Mock<EFCreateRepository<AppDbContext, AppDbContext.Entity, AppDbContext.DbEntity>>(_context, _mockedLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnBusinessFailure()
    {
        var entity = new AppDbContext.Entity(Guid.NewGuid(), null);
        var dbEntity = new AppDbContext.DbEntity
        {
            Id = entity.Id,
            Value = entity.Value
        };

        _context.ThrowOnSave = true;

        _mockedRepository.Setup(e => e.ConcurrencyMessageTemplate)
            .CallBase()
            .Verifiable();
        _mockedRepository.Setup(e => e.CreateAsync(entity, default))
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

        var response = await _mockedRepository.Object.CreateAsync(entity);

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
    public async Task CreateAsync_ShouldReturnSuccess()
    {
        _context.ChangeTracker.Clear();
        _context.ThrowOnSave = false;

        var entity = new AppDbContext.Entity(new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6), null);
        var dbEntity = new AppDbContext.DbEntity
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
        _mockedRepository.Setup(e => e.CreateAsync(entity, default))
            .CallBase()
            .Verifiable();

        var response = await _mockedRepository.Object.CreateAsync(entity);

        response.IsT0.Should().BeTrue();
        response.AsT0.Should().Be(entity);

        _mockedLogger.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();

        _context.Set<AppDbContext.DbEntity>()
            .Any(e => e.Id == dbEntity.Id)
            .Should().BeTrue();
    }
}
