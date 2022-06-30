using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

public static class SessionRequest
{
    static IHttpContextAccessor _IHttpContextAccessor;

    public static void Configure(IHttpContextAccessor __IHttpContextAccessor)
    {
        _IHttpContextAccessor = __IHttpContextAccessor;
    }

    public static HttpContext _HttpContext => _IHttpContextAccessor.HttpContext;

    public static List<Lang> Languages => _IHttpContextAccessor.HttpContext.Session.Get<List<Lang>>("Languages");

    public static User _User => _IHttpContextAccessor.HttpContext.Session.Get<User>("_user");
    public static User LoginUser => _IHttpContextAccessor.HttpContext.Session.Get<User>("LoginUser");

    public static SiteConfig config => _IHttpContextAccessor.HttpContext.Session.Get<SiteConfig>("config");

    public static int LanguageId => _IHttpContextAccessor.HttpContext.Session.GetInt32("LanguageId") ?? 1;

    public static List<LangDisplay> _LangDisplay => _IHttpContextAccessor.HttpContext.Session.Get<List<LangDisplay>>("_LangDisplay");

    public static List<EnumModel> ContentTypesList => Enum.GetValues(typeof(ContentTypes)).Cast<int>().Select(x => new EnumModel { name = ((ContentTypes)x).ToStr(), value = x.ToString(), text = ((ContentTypes)x).ExGetDescription() }).ToList();

    public static List<FormType> FormTypeList => _IHttpContextAccessor.HttpContext.Session.Get<List<FormType>>("FormType");


    public static string SetImage(this string ImageUrl)
    {
        return !string.IsNullOrEmpty(ImageUrl) ? SessionRequest.config.ImageUrl + "/fileupload/UserFiles/Folders/" + ImageUrl : "";
    }




}


