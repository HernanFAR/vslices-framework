namespace Sample.Core.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetIdentityId(this IHttpContextAccessor _) => new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
}
