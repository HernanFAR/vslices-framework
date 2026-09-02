namespace VSlices.Domain.Traits;

public interface AggregateRoot<SELF> : Entity<SELF>
    where SELF : AggregateRoot<SELF>;

public interface AggregateRoot<SELF, ID> : AggregateRoot<SELF>, Entity<SELF, ID>
    where SELF : AggregateRoot<SELF, ID>
    where ID : Identifier<ID>;
