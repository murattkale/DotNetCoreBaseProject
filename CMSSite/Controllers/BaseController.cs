using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CMSSite.Controllers
{


    public class BaseController : Controller
    {
        ISendMail _ISendMail;
        IBaseModel _IBaseModel;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public BaseController(
            ISendMail _ISendMail,
            IBaseModel _IBaseModel,
            IHostingEnvironment _IHostingEnvironment,
            IHttpContextAccessor _IHttpContextAccessor,
            IHttpClientWrapper _client
            )
        {

            this._ISendMail = _ISendMail;
            this._IBaseModel = _IBaseModel;
            this._IHostingEnvironment = _IHostingEnvironment;
            this._IHttpContextAccessor = _IHttpContextAccessor;
            this._client = _client;

        }



        [Route("registration")]
        public async Task<IActionResult> registrationpersonalinfo()
        {
            if (SessionRequest.LoginUser == null)
            {
                return Redirect("/login");
            }

            if (SessionRequest.raceUser)
            {
                return Redirect("/registrationraceinfo");
            }



            var result = await setData();
            return View();
        }


        [Route("registrationraceinfo")]
        public async Task<IActionResult> registrationraceinfo()
        {
            if (SessionRequest.LoginUser == null)
                return Redirect("/login");

            var result = await setData();
            return View();
        }

        [Route("accountprofile")]
        public async Task<IActionResult> accountprofile()
        {
            if (SessionRequest.LoginUser == null)
                return Redirect("/login");

            var result = await setData();

            var _order = await _client.GetAsync<Order>($"Order/GetUserOrder?UserId={SessionRequest.LoginUser.Id}");
            if (_order.ResultRow != null)
            {
                foreach (var item in _order.ResultRow?.OrderDetail)
                {
                    item.Product = _client.Get<Product>(new Product().GetType().Name + $"/GetRow?id={item.ProductId}").ResultRow;
                }

                _IHttpContextAccessor.HttpContext.Session.Set("myOrder", _order.ResultRow);
            }


            return View();
        }

        [Route("accountraceinfo")]
        public async Task<IActionResult> accountraceinfo()
        {
            if (SessionRequest.LoginUser == null)
                return Redirect("/login");

            var result = await setData();

            var _order = await _client.GetAsync<Order>($"Order/GetUserOrder?UserId={SessionRequest.LoginUser.Id}");
            foreach (var item in _order.ResultRow?.OrderDetail)
            {
                item.Product = _client.Get<Product>(new Product().GetType().Name + $"/GetRow?id={item.ProductId}").ResultRow;
            }


            _IHttpContextAccessor.HttpContext.Session.Set("myOrder", _order.ResultRow);
            return View();
        }

        [Route("accountproducts")]
        public async Task<IActionResult> accountproducts()
        {
            if (SessionRequest.LoginUser == null)
                return Redirect("/login");

            var result = await setData();

            var _order = await _client.GetAsync<Order>($"Order/GetUserOrder?UserId={SessionRequest.LoginUser.Id}");
            foreach (var item in _order.ResultRow?.OrderDetail)
            {
                item.Product = _client.Get<Product>(new Product().GetType().Name + $"/GetRow?id={item.ProductId}").ResultRow;
            }


            _IHttpContextAccessor.HttpContext.Session.Set("myOrder", _order.ResultRow);
            return View();
        }


        [Route("registrationpurchase")]
        public async Task<IActionResult> registrationpurchase()
        {
            if (SessionRequest.LoginUser == null)
                return Redirect("/login");

            var result = await setData();
            PaymentViewModel model = new PaymentViewModel();
            return View(model);
        }


        public async Task<int?> setConf()
        {
            if (SessionRequest.config == null)
            {
                var config = await _client.GetAsync<SiteConfig>("SiteConfig/GetBy");
                _IHttpContextAccessor.HttpContext.Session.Set("config", config.ResultRow);
            }
            if (SessionRequest._User == null)
            {
                var _user = await _client.GetAsync<User>("User/GetRow?id=1");
                _IHttpContextAccessor.HttpContext.Session.Set("_user", _user.ResultRow);
                _IBaseModel.CreaUser = _user.ResultRow.Id;
            }
            if (SessionRequest._LangDisplay == null || SessionRequest._LangDisplay.Count < 1)
            {
                var _LangDisplay = await _client.GetAsync<LangDisplay>("LangDisplay/GetAll");
                _IHttpContextAccessor.HttpContext.Session.Set("_LangDisplay", _LangDisplay.ResultList);
            }

            if (SessionRequest.Languages == null || SessionRequest.Languages.Count < 1)
            {
                var Languages = await _client.PostAsync<Lang>($"Lang/GetPaging", new DTParameters<Lang>());
                _IHttpContextAccessor.HttpContext.Session.Set("Languages", Languages.ResultPaging.data);
            }


            _IBaseModel.LanguageId = SessionRequest.LanguageId;

            return _IBaseModel.LanguageId;
        }


        public async Task<int?> setData()
        {
            var conf = await setConf();

            var IsHeaderMenu = await _client.GetAsync<ContentPage>($"ContentPage/GetHeaderFooter?IsHeaderMenu=true&IsFooterMenu=false");
            ViewBag.IsHeaderMenu = IsHeaderMenu.ResultList;

            var IsFooterMenu = await _client.GetAsync<ContentPage>($"ContentPage/GetHeaderFooter?IsHeaderMenu=false&IsFooterMenu=true");
            ViewBag.IsFooterMenu = IsHeaderMenu.ResultList;


            var Slider = await _client.GetAsync<ContentPage>($"ContentPage/GetAllType?ContentTypes={ContentTypes.Slider}");
            ViewBag.Slider = Slider.ResultList;

            var MainPage = await _client.GetAsync<ContentPage>($"ContentPage/GetAllType?ContentTypes={ContentTypes.MainPage}");
            ViewBag.MainPage = MainPage.ResultList;



            return conf;
        }


        public async Task<IActionResult> BaseContent()
        {
            var link = HttpContext.Items["cmspage"].ToString();

            if (!string.IsNullOrEmpty(link))
            {
                await setConf();
                var resLink = await _client.GetAsync<ContentPage>($"ContentPage/GetLinkR?link={link}");
                var content = resLink.ResultRow;



                var langStatus = _IHttpContextAccessor.HttpContext.Session.GetInt32("LangStatus");

                if (content != null && content.LangId != SessionRequest.LanguageId)
                {

                    if (langStatus == 1)
                    {
                        _IHttpContextAccessor.HttpContext.Session.SetInt32("LangStatus", 0);

                        if (content.OrjId > 0)
                        {
                            var cOrj = await _client.GetAsync<ContentPage>($"ContentPage/GetOrj?OrjId={content.OrjId}");
                            var content1 = cOrj.ResultRow;

                            if (content1 == null)
                            {
                                var cOrjOrj = await _client.GetAsync<ContentPage>($"ContentPage/GetIdR?Id={content.OrjId}");
                                content = cOrjOrj.ResultRow;
                            }
                            else
                            {
                                content = content1;
                            }
                        }
                        else
                        {
                            var cOrj = await _client.GetAsync<ContentPage>($"ContentPage/GetOrj?OrjId={content.Id}");
                            content = cOrj.ResultRow;
                        }


                        return Redirect(content.Link);
                    }
                    else
                    {
                        _IHttpContextAccessor.HttpContext.Session.SetInt32("LanguageId", content.LangId);
                        _IBaseModel.LanguageId = content.LangId;
                        _IHttpContextAccessor.HttpContext.Session.SetInt32("LangStatus", 0);
                        return View(content);
                    }

                }

                if (content != null)
                {
                    return View(content);
                }
                else
                {
                    //}
                    return Redirect("/");
                }
            }
            else
            {
                return Redirect("/");
            }
        }


        public JsonResult SetLanguage(int id)
        {
            _IHttpContextAccessor.HttpContext.Session.SetInt32("LanguageId", id);
            _IHttpContextAccessor.HttpContext.Session.SetInt32("LangStatus", 1);
            _IBaseModel.LanguageId = SessionRequest.LanguageId;
            return Json("OK");
        }


        public async Task<IActionResult> Index()
        {
            var result = await setData();
            return View();
        }

        [SessionTimeout]
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            if (SessionRequest.LoginUser != null)
                return Redirect("/");

            var result = await setData();
            return View();
        }


        [Route("lostyourpass")]
        public async Task<IActionResult> lostyourpass()
        {
            var result = await setData();
            return View();
        }


        public async Task<IActionResult> Content()
        {
            var link = HttpContext.Items["cmspage"].ToString();


            if (!string.IsNullOrEmpty(link))
            {
                await setConf();
                var res = await _client.GetAsync<ContentPage>($"ContentPage/GetLinkR?link={link}");
                var menu = res.ResultRow;
                if (menu != null)
                {
                    _IHttpContextAccessor.HttpContext.Session.SetInt32("LanguageId", menu.LangId);
                    ViewBag.LanguageId = menu.LangId;
                    ViewBag.page = menu;
                    return View();
                }
                else
                {
                    int langID = 0;
                    if (_IHttpContextAccessor.HttpContext.Session.GetString("LanguageId") != null)
                    {
                        langID = _IHttpContextAccessor.HttpContext.Session.GetInt32("LanguageId") ?? 2;
                    }
                    if (_IHttpContextAccessor.HttpContext.Session.GetInt32("LanguageId") == null || langID == 0)
                    {
                        _IHttpContextAccessor.HttpContext.Session.SetInt32("LanguageId", 2);
                    }
                    ViewBag.LanguageId = _IHttpContextAccessor.HttpContext.Session.GetInt32("LanguageId");
                    return Redirect(SessionRequest.config.BaseUrl);
                }
            }
            else
            {
                return Redirect(SessionRequest.config.BaseUrl);
            }
        }


        public IActionResult Header()
        {
            return View();
        }

        public IActionResult Footer()
        {
            return View();
        }

        [Route("profil")]
        public IActionResult profil()
        {
            return View();
        }



        [Route("Logout")]
        public IActionResult Logout()
        {
            _IHttpContextAccessor.HttpContext.Session.Set("LoginUser", "");
            _IHttpContextAccessor.HttpContext.Session.Clear();
            return Redirect("/");
        }


        [Route("SendMail")]
        public async Task<IActionResult> SendMail(int ContentPageId)
        {
            var _contentRow = await _client.GetAsync<ContentPage>(new ContentPage().GetType().Name + $"/GetRow?id={ContentPageId}");
            var _content = _contentRow.ResultRow;

            var Icerik = "";

            Icerik = _content.ContentData.Replace("[NameSurname]", SessionRequest.LoginUser.NameSurname);
            Icerik = Icerik.Replace("[Mail]", SessionRequest.LoginUser.Mail);
            if (SessionRequest.myOrder != null)
            {
                Icerik = Icerik.Replace("[OrderId]", SessionRequest.myOrder.Id.ToStr());
                Icerik = Icerik.Replace("[OrderStatusName]", SessionRequest.myOrder.OrderStatusName);
                //Icerik = Icerik.Replace("[IsConfirm]", SessionRequest.myOrder.ConfirmStatusName == true ? "Confirmed" : "");
            }


            string result = await _ISendMail.Send(new MailModelCustom
            {
                Alicilar = new string[] { SessionRequest.LoginUser.Mail },
                bcc = new string[] { SessionRequest.config.SmtpMail },
                cc = null,
                Icerik = Icerik,
                Konu = _content.Description,
                MailGorunenAd = SessionRequest.config.MailGorunenAd,
                SmtpHost = SessionRequest.config.SmtpHost,
                SmtpMail = SessionRequest.config.SmtpMail,
                SmtpMailPass = SessionRequest.config.SmtpMailPass,
                SmtpPort = SessionRequest.config.SmtpPort,
                SmtpSSL = SessionRequest.config.SmtpSSL,
                SmtpUseDefaultCredentials = false,


            });


            return Json(result);
        }


        [Route("SendMailForm")]
        public async Task<IActionResult> SendMailForm(Forms postForm)
        {
            var _contentRow = await _client.GetAsync<ContentPage>(new ContentPage().GetType().Name + $"/GetRow?id={postForm.ContentPageId}");
            var _content = _contentRow.ResultRow;

            var Icerik = "";
            Icerik = _content == null ? "" : _content.ContentData;


            Icerik = Icerik.Replace("[NameSurname]", postForm.Name + " " + postForm.Surname);
            Icerik = Icerik.Replace("[Mail]", postForm.Email);
            Icerik = Icerik.Replace("[Phone]", postForm.Phone);
            Icerik = Icerik.Replace("[Message]", postForm.Message);
            Icerik = Icerik.Replace("[Hoca]", postForm.Custom1);


            var formType = await _client.GetAsync<FormType>(new FormType().GetType().Name + $"/GetRow?id={postForm.FormTypeId}");

            string result = await _ISendMail.Send(new MailModelCustom
            {
                Alicilar = formType.ResultRow.Mail.Split(';').ToArray(),
                bcc = new string[] { SessionRequest.config.SmtpMail },
                cc = formType.ResultRow.MailCC?.Split(';').ToArray(),
                Icerik = Icerik,
                Konu = postForm.Name + " " + postForm.Surname + " " + formType.ResultRow.DescName + " formunu doldurdu.",
                MailGorunenAd = SessionRequest.config.MailGorunenAd,
                SmtpHost = SessionRequest.config.SmtpHost,
                SmtpMail = SessionRequest.config.SmtpMail,
                SmtpMailPass = SessionRequest.config.SmtpMailPass,
                SmtpPort = SessionRequest.config.SmtpPort,
                SmtpSSL = SessionRequest.config.SmtpSSL,
                SmtpUseDefaultCredentials = false,

            });


            return Json(result);
        }


        [Route("passMail")]
        public async Task<IActionResult> passMail(string Mail, int ContentPageId)
        {
            var _contentRow = await _client.GetAsync<ContentPage>(new ContentPage().GetType().Name + $"/GetRow?id={ContentPageId}");
            var _content = _contentRow.ResultRow;

            var userMailControl = await _client.GetAsync<User>($"User/ValidateMailControl?Mail={Mail}");

            var Icerik = _content.ContentData.Replace("[NameSurname]", userMailControl.ResultRow.NameSurname);
            Icerik = Icerik.Replace("[Pass]", userMailControl.ResultRow.Pass);

            string result = await _ISendMail.Send(new MailModelCustom
            {
                Alicilar = new string[] { userMailControl.ResultRow.Mail },
                //bcc = new string[] { SessionRequest.config.SmtpMail },
                //cc = null,
                Icerik = Icerik,
                Konu = _content.Description,
                MailGorunenAd = SessionRequest.config.MailGorunenAd,
                SmtpHost = SessionRequest.config.SmtpHost,
                SmtpMail = SessionRequest.config.SmtpMail,
                SmtpMailPass = SessionRequest.config.SmtpMailPass,
                SmtpPort = SessionRequest.config.SmtpPort,
                SmtpSSL = SessionRequest.config.SmtpSSL,
                SmtpUseDefaultCredentials = false,


            });


            return Json(result);
        }





    }
}
