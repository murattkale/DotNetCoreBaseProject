using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;

namespace CMSSite.Components
{

    public class ContentView : ViewComponent
    {
        IBaseModel _IBaseModel;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public ContentView(
        IBaseModel _IBaseModel,
        IHostingEnvironment _IHostingEnvironment,
            IHttpContextAccessor _IHttpContextAccessor,
            IHttpClientWrapper _client
            )
        {

            this._IBaseModel = _IBaseModel;
            this._IHostingEnvironment = _IHostingEnvironment;
            this._IHttpContextAccessor = _IHttpContextAccessor;
            this._client = _client;
        }


        public IViewComponentResult Invoke(ContentPage _page)
        {
            var link = HttpContext.Request.Path.Value.Trim('/').ToStr();
            var datad = setData();
            return View(_page.TemplateType.ToStr(), _page);

        }


        public int? setConf()
        {
            if (SessionRequest.config == null)
            {
                var config = _client.Get<SiteConfig>("SiteConfig/GetBy");
                _IHttpContextAccessor.HttpContext.Session.Set("config", config.ResultRow);
            }
            if (SessionRequest._User == null)
            {
                var _user = _client.Get<User>("User/GetRow?id=1");
                _IHttpContextAccessor.HttpContext.Session.Set("_user", _user.ResultRow);
                _IBaseModel.CreaUser = _user.ResultRow.Id;
            }
            if (SessionRequest._LangDisplay == null || SessionRequest._LangDisplay.Count < 1)
            {
                var _LangDisplay = _client.Get<LangDisplay>("LangDisplay/GetAll");
                _IHttpContextAccessor.HttpContext.Session.Set("_LangDisplay", _LangDisplay.ResultList);
            }

            if (SessionRequest.Languages == null || SessionRequest.Languages.Count < 1)
            {
                var Languages = _client.Post<Lang>($"Lang/GetPaging", new DTParameters<Lang>());
                _IHttpContextAccessor.HttpContext.Session.Set("Languages", Languages.ResultPaging.data);
            }


            _IBaseModel.LanguageId = SessionRequest.LanguageId;

            return _IBaseModel.LanguageId;
        }


        public int? setData()
        {
            var conf = setConf();

            var IsHeaderMenu = _client.Get<ContentPage>($"ContentPage/GetHeaderFooter?IsHeaderMenu=true&IsFooterMenu=false");
            ViewBag.IsHeaderMenu = IsHeaderMenu.ResultList;

            var IsFooterMenu = _client.Get<ContentPage>($"ContentPage/GetHeaderFooter?IsHeaderMenu=false&IsFooterMenu=true");
            ViewBag.IsFooterMenu = IsHeaderMenu.ResultList;


            var Slider = _client.Get<ContentPage>($"ContentPage/GetAllType?ContentTypes={ContentTypes.Slider}");
            ViewBag.Slider = Slider.ResultList;

            var MainPage = _client.Get<ContentPage>($"ContentPage/GetAllType?ContentTypes={ContentTypes.MainPage}");
            ViewBag.MainPage = MainPage.ResultList;



            return conf;
        }


    }
}