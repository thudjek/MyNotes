namespace API.Extensions;

public static class HttpContextExtensions
{
    public static void AddCookieToResponse(this HttpContext httpContext, string key, string value, bool httpOnly)
    {
        httpContext.Response.Cookies.Append(key, value, new CookieOptions() { HttpOnly = httpOnly });
    }
}
