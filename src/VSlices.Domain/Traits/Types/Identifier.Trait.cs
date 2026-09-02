namespace VSlices.Domain.Traits;

/// <summary>
///
/// </summary>
/// <typeparam name="SELF">
///
/// </typeparam>
public interface Identifier<SELF> :
    DomainType<SELF>,
    DiscreteSpace<SELF>
    where SELF : Identifier<SELF>;
