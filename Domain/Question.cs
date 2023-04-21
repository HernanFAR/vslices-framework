namespace Domain;

public class Question
{
    public Question(string name, Guid createdById)
    {
        Id = Guid.NewGuid();
        Name = name;
        CreatedById = createdById;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid CreatedById { get; private set; }
    public Guid? UpdatedById { get; private set; }
    public Guid? DeleteById { get; private set; }

    public void UpdateState(string name, Guid updatedById)
    {
        Name = name;
        UpdatedById = updatedById;
    }

    public void Delete(Guid deleteById)
    {
        DeleteById = deleteById;
    }
}
