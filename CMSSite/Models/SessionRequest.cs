
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;

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

    public static int LanguageId => _IHttpContextAccessor.HttpContext.Session.GetInt32("LanguageId") ?? 2;

    public static bool raceUser => _IHttpContextAccessor.HttpContext.Session.GetInt32("raceUser") == 1 ? true : false;


    public static List<LangDisplay> _LangDisplay => _IHttpContextAccessor.HttpContext.Session.Get<List<LangDisplay>>("_LangDisplay");

    public static List<EnumModel> ContentTypesList => Enum.GetValues(typeof(ContentTypes)).Cast<int>().Select(x => new EnumModel { name = ((ContentTypes)x).ToStr(), value = x.ToString(), text = ((ContentTypes)x).ExGetDescription() }).ToList();


    public static Order myOrder => _IHttpContextAccessor.HttpContext.Session.Get<Order>("myOrder");



    public static string TransSpec(this Spec _spec)
    {
        var text = _spec.Name;
        if (_spec != null)
        {
            switch (LanguageId)
            {
                case 1:
                    {
                        text = _spec.Name;
                    }
                    break;
                case 2:
                    {
                        text = _spec.Name1;
                    }
                    break;
                case 3:
                    {
                        text = _spec.Name2;
                    }
                    break;
                case 4:
                    {
                        text = _spec.Name3;
                    }
                    break;
                case 5:
                    {
                        text = _spec.Name4;
                    }
                    break;

            }
        }
        else
        {

        }
        return text;
    }

    public static string Trans(this string ParamName)
    {
        var lang = _LangDisplay.FirstOrDefault(o => o.ParamName == ParamName);
        var text = ParamName;
        if (lang != null)
        {
            switch (LanguageId)
            {
                case 1:
                    {
                        text = lang.Name_1;
                    }
                    break;
                case 2:
                    {
                        text = lang.Name_2;
                    }
                    break;
                case 3:
                    {
                        text = lang.Name_3;
                    }
                    break;
                case 4:
                    {
                        text = lang.Name_4;
                    }
                    break;
                case 5:
                    {
                        text = lang.Name_5;
                    }
                    break;
            }
        }
        else
        {

        }
        return text;
    }

    public static string Trans(this string ParamName, string Description)
    {
        var lang = _LangDisplay.FirstOrDefault(o => o.ParamName == ParamName);
        var text = ParamName;
        if (lang != null)
        {
            switch (LanguageId)
            {
                case 1:
                    {
                        text = lang.Name_1;
                    }
                    break;
                case 2:
                    {
                        text = lang.Name_2;
                    }
                    break;
                case 3:
                    {
                        text = lang.Name_3;
                    }
                    break;
                case 4:
                    {
                        text = lang.Name_4;
                    }
                    break;
                case 5:
                    {
                        text = lang.Name_5;
                    }
                    break;
            }
        }
        else
        {
            //_LangDisplay.Add(new LangDisplay()
            //{
            //    ParamName = text,
            //    Description = Description,
            //});
        }
        return text;
    }


    public static string SetImage(this string ImageUrl)
    {
        return !string.IsNullOrEmpty(ImageUrl) ? SessionRequest.config.ImageUrl + "/fileupload/UserFiles/Folders/" + ImageUrl : "";
    }


}




