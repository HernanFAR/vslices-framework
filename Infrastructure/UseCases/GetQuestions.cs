using Application.UseCases;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UseCases;

public class GetAllQuestionsRepository : IGetAllQuestionsRepository
{
    private readonly ApplicationDbContext _context;

    public GetAllQuestionsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAllQuestionsDto[]> GetAllQuestionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .Select(e => new GetAllQuestionsDto(e.Id, e.Name, e.CreatedById, e.UpdatedById))
            .ToArrayAsync(cancellationToken);
    }
}