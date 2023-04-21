using Application.UseCases;
using Domain;
using Infrastructure.EntityFramework;

namespace Infrastructure.UseCases;

public class CreateQuestionRepository : ICreateQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public CreateQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveQuestion(Question question, CancellationToken cancellationToken = default)
    {
        _context.Add(question);

        await _context.SaveChangesAsync(cancellationToken);
    }
}