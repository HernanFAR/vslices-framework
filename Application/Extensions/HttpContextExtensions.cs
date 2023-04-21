namespace Application.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetIdentityId(this IHttpContextAccessor contextAccessor) => new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
}
