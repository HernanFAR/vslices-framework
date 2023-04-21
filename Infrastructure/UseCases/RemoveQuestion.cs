using Application.UseCases;
using Domain;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UseCases;

public class RemoveQuestionRepository : IRemoveQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public RemoveQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetQuestion(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task RemoveQuestion(Question question, CancellationToken cancellationToken = default)
    {
        _context.Remove(question);

        await _context.SaveChangesAsync(cancellationToken);
    }
}