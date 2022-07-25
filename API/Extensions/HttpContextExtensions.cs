namespace API.Extensions;

public static class HttpContextExtensions
{
    public static void AddCookieToResponse(this HttpContext httpContext, string key, string value, bool httpOnly)
    {
        httpContext.Response.Cookies.Append(key, value, new CookieOptions() 
        { 
            HttpOnly = httpOnly,
            SameSite = SameSiteMode.None,
            Secure = true
        });
    }

    public static string GetValueFromCookie(this HttpContext httpContext, string key)
    {
        if(httpContext.Request.Cookies.ContainsKey(key))
            return httpContext.Request.Cookies[key];

        return string.Empty;
    }
}
