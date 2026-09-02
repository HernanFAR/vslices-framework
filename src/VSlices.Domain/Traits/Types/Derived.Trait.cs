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
public interface Derived<SELF, BASE> : DomainType<SELF>
    where SELF : Derived<SELF, BASE>
    where BASE : DomainType<BASE>
{
    /// <summary>
    ///
    /// </summary>
    /// <returns>
    ///
    /// </returns>
    BASE ToBase();
}
