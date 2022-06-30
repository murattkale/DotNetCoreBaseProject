using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public static class SessionExtensions
{
    public static void Set(this ISession session, string key, object value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
    }

    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);

        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}

public class SessionTimeout : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Path.Value.Contains("/login") && SessionRequest.LoginUser == null)
        {
            context.Result =
                new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Base",
                    action = "Login"
                }));
        }

        base.OnActionExecuting(context);
    }
}

