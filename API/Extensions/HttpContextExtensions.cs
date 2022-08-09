namespace API.Extensions;

public static class HttpContextExtensions
{
    public static void AddCookieToResponse(this HttpContext httpContext, string key, string value, bool httpOnly, DateTime expires)
    {
        httpContext.Response.Cookies.Append(key, value, new CookieOptions() 
        { 
            HttpOnly = httpOnly,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = expires
        });
    }

    public static string GetValueFromCookie(this HttpContext httpContext, string key)
    {
        if(httpContext.Request.Cookies.ContainsKey(key))
            return httpContext.Request.Cookies[key];

        return string.Empty;
    }
}
