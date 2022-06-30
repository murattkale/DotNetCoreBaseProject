using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ThreeDPayment.Requests;
using ThreeDPayment.Results;


namespace CMSSite.Controllers
{
    public class PaymentController : Controller
    {
        IBaseModel _IBaseModel;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public PaymentController(


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



        //TL ISO code | EURO 978 | Dolar 840
        [HttpPost]
        public async Task<IActionResult> Index(PaymentViewModel model)
        {

            if (string.IsNullOrEmpty(model.CardNumber))
                return Json(new { error = "Please Card Number".Trans() });
            if (string.IsNullOrEmpty(model.CvvCode))
                return Json(new { error = "Please Cvv Code".Trans() });
            if (model.ExpireYear < 1)
                return Json(new { error = "Please Expire Year".Trans() });
            if (string.IsNullOrEmpty(model.ExpireMonth))
                return Json(new { error = "Please Expire Month".Trans() });


            var order = await _client.GetAsync<Order>(new Order().GetType().Name + $"/GetRow?id={SessionRequest.myOrder.Id}");
            var _order = order.ResultRow;

            var prefix = model.CardNumber.Trim().ReplaceIllegalCharacters();
            prefix = prefix.Substring(0, 6);

            decimal CouponDiscount = 0;
            decimal CouponDiscountTotal = SessionRequest.myOrder.TotalAmount;
            if (_order.Coupon != null)
            {
                switch (_order.Coupon.CouponType)
                {
                    case CouponType.Tutar:
                        {
                            CouponDiscount =  _order.Coupon.CouponValue;
                            CouponDiscountTotal = SessionRequest.myOrder.TotalAmount - _order.Coupon.CouponValue;
                        }
                        break;
                    case CouponType.Oran:
                        {
                            CouponDiscount = (SessionRequest.myOrder.TotalAmount * (_order.Coupon.CouponValue / 100));
                            CouponDiscountTotal = SessionRequest.myOrder.TotalAmount - (SessionRequest.myOrder.TotalAmount * (_order.Coupon.CouponValue / 100));
                        }
                        break;
                    default:
                        {
                            break;
                        }
                }
            }

            model.TotalAmount = Helpers.ToDecimal(CouponDiscountTotal.ToDouble().toFixed(2));
            _order.Discount = Helpers.ToDecimal(CouponDiscount.ToDouble().toFixed(2));
            _order.TotalAmount = model.TotalAmount;

            var bank = await _client.PostAsync<InstallmentViewModel>("Payment/GetInstallments", new InstallmentViewModel()
            {
                Prefix = prefix,
                TotalAmount = model.TotalAmount,
            });

            var bankRes = bank.ResultRow;


            //Türk kartı geldiğinde ve 3 ile başladığında
            if (model.CardNumber.StartsWith("3") || bankRes.BankId > 0)
            {
                //_order.Currency = "₺";
                model.CurrencyIsoCode = "949";
                //model.TotalAmount = Helpers.ToKur(SessionRequest.myOrder?.TotalAmount, "euro");
                //_order.ExchangeRate = SessionRequest.myOrder.Currency.GetKur();
                //model.TotalAmount = SessionRequest.myOrder.TotalAmount;

            }
            else
            {
                //_order.Currency = "€";
                //model.CurrencyIsoCode = "978";
                model.CurrencyIsoCode = "949";
                //model.TotalAmount = SessionRequest.myOrder.TotalAmount;
            }
            model.BankId = 5;

            var orderSave = await _client.PostAsync<Order>(new Order().GetType().Name + "/InsertOrUpdate", _order);
            _IHttpContextAccessor.HttpContext.Session.Set("myOrder", _order);


            var result = await _client.PostAsync<PaymentTransaction>($"Payment/Index", model);
            return Json(result);
        }


        [Route("payment/confirm/{paymentId?}")]
        public async Task<IActionResult> Confirm(string paymentId)
        {
            var result = await _client.GetStringAsync<string>($"Payment/Confirm?paymentId={paymentId}&Callback=https://{HttpContext.Request.Host}/");
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> GetInstallments([FromBody] InstallmentViewModel model)
        {
            var result = await _client.PostAsync<InstallmentViewModel>("Payment/GetInstallments", model);
            return Json(result);
        }


        [HttpPost("payment/callback/{paymentId?}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Callback(string paymentId, IFormCollection form)
        {
            var result = await _client.PostFormAsync<VerifyGatewayResult>($"Payment/Callback", form);

            if (result.ResultRow != null && result.ResultRow.Success)
            {
                return View("Success", result.ResultRow);
            }
            else
            {
                var _orderRow = await _client.GetAsync<Order>(new Order().GetType().Name + $"/GetRow?id={paymentId}");
                var _order = _orderRow.ResultRow;
                _order.OrderStatus = OrderStatus.Iptal;
                var UpdateOrder = await _client.PostAsync<Order>(new Order().GetType().Name + "/InsertOrUpdate", _order);

                return View("Fail", result.ResultRow);
            }
        }

        [HttpGet("payment/Completed/{orderNumber?}")]
        public async Task<IActionResult> Completed(string orderNumber)
        {
            var result = await _client.GetAsync<CompletedViewModel>($"Payment/Completed?orderNumber={orderNumber}");

            var _orderRow = await _client.GetAsync<Order>(new Order().GetType().Name + $"/GetRow?id={orderNumber.ToInt()}");
            var _order = _orderRow.ResultRow;
            if (string.IsNullOrEmpty(SessionRequest.LoginUser.HealtReportUrl))
                _order.ConfirmStatus = ConfirmStatus.Beklemede;
            else
                _order.ConfirmStatus = ConfirmStatus.Değerlendirmede;

            _order.IsSozlesme = DateTime.Now;
            _order.OrderStatus = OrderStatus.Odendi;

            var UpdateOrder = await _client.PostAsync<Order>(new Order().GetType().Name + "/InsertOrUpdate", _order);

            //var _orderRow2 = await _client.GetAsync<Order>(new Order().GetType().Name + $"/GetRow?id={orderNumber.ToInt()}");
            var _order2 = UpdateOrder.ResultRow;

            foreach (var item in _order2.OrderDetail)
            {
                item.Product = _client.Get<Product>(new Product().GetType().Name + $"/GetRow?id={item.ProductId}").ResultRow;
            }

            _IHttpContextAccessor.HttpContext.Session.Set("myOrder", _order2);

            if (_order2.CouponId>0)
            {
                var couponRow = await _client.GetAsync<Coupon>(new Coupon().GetType().Name + $"/GetRow?id={_order2.CouponId}");
                if (couponRow.ResultRow != null)
                {
                    couponRow.ResultRow.Used += 1;
                    var update_couponRow = await _client.PostAsync<Coupon>(new Coupon().GetType().Name + "/InsertOrUpdate", couponRow.ResultRow);
                }
            }
          
           


            await setData();
            return View(result.ResultRow);
        }

        public async Task<IActionResult> Success()
        {
            await setData();
            return View();
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


    }
}