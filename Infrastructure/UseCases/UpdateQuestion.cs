using Application.UseCases;
using Domain;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UseCases;

public class UpdateQuestionRepository : IUpdateQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public UpdateQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetQuestion(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .FirstAsync(e => e.Id == id, cancellationToken);
    }

    public async Task UpdateQuestion(Question question, CancellationToken cancellationToken = default)
    {
        _context.Update(question);

        await _context.SaveChangesAsync(cancellationToken);
    }
}