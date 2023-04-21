using Core.UseCases;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UseCases;

public class GetQuestionRepository : IGetQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public GetQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetQuestionDto?> GetQuestionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .Where(e => e.Id == id)
            .Select(e => new GetQuestionDto(e.Id, e.Name, e.CreatedById, e.UpdatedById))
            .FirstOrDefaultAsync(cancellationToken);
    }
}