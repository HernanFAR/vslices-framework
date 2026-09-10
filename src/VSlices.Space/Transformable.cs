using LanguageExt;
using VSlices.Arrows;

namespace VSlices.Space;

/// <summary>
/// Defines a semantic transformation from <typeparamref name="FROM"/> to
/// <typeparamref name="TO"/> whose rules are owned by <typeparamref name="CTX"/>.
/// </summary>
/// <typeparam name="CTX">The semantic authority that owns the transformation.</typeparam>
/// <typeparam name="FROM">The source space.</typeparam>
/// <typeparam name="TO">The target space.</typeparam>
public interface Transformable<CTX, FROM, TO>
    where CTX : Transformable<CTX, FROM, TO>
{
    /// <summary>
    /// Gets the rules that determine whether and how a source value can become a target value.
    /// </summary>
    static abstract Req<FROM, TO>.Full Transformation { get; }
}

/// <summary>
/// Defines a target-owned semantic transformation from <typeparamref name="FROM"/>
/// to <typeparamref name="TO"/>.
/// </summary>
/// <remarks>
/// This is equivalent to <c>Transformable&lt;TO, FROM, TO&gt;</c>: the target space
/// itself owns the transformation.
/// </remarks>
public interface Transformable<FROM, TO> : Transformable<TO, FROM, TO>
    where TO : Transformable<FROM, TO>;

/// <summary>
/// Operations for executing semantic transformations.
/// </summary>
public static class Transformable
{
    /// <summary>
    /// Attempts a transformation owned by an explicit semantic context.
    /// </summary>
    public static Fin<TO> Transform<CTX, FROM, TO>(FROM source)
        where CTX : Transformable<CTX, FROM, TO> =>
        CTX.Transformation.RunFin(source);

    /// <summary>
    /// Attempts a transformation owned by its target space.
    /// </summary>
    public static Fin<TO> Transform<FROM, TO>(FROM source)
        where TO : Transformable<FROM, TO> =>
        TO.Transformation.RunFin(source);
}
