namespace VSlices.Domain.Traits;

/// <summary>
///
/// </summary>
/// <typeparam name="SELF">
///
/// </typeparam>
/// <typeparam name="BASE">
///
/// </typeparam>
public interface Refined<SELF, BASE> : Derived<SELF, BASE>
    where SELF : Refined<SELF, BASE>
    where BASE : DomainType<BASE>;
