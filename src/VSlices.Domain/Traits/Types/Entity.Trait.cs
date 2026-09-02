namespace VSlices.Domain.Traits;

public interface Entity<SELF> : DomainType<SELF>
    where SELF : Entity<SELF>;

public interface Entity<SELF, ID> : Entity<SELF>
    where SELF : Entity<SELF, ID>
    where ID : Identifier<ID>
{
    ID Id { get; }
}
