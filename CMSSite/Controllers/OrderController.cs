using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMSSite.Controllers
{
    public class OrderController : Controller
    {
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public OrderController(

            IHostingEnvironment _IHostingEnvironment,
            IHttpContextAccessor _IHttpContextAccessor,
            IHttpClientWrapper _client
            )
        {
            this._IHostingEnvironment = _IHostingEnvironment;
            this._IHttpContextAccessor = _IHttpContextAccessor;
            this._client = _client;
        }



        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetSelect(string name, string whereCase)
        {
            var result = await _client.GetAsync<EnumModel>(new Order().GetType().Name + $"/GetSelect?name={name}&whereCase={whereCase}");
            return Json(result.ResultList);

        }


        public async Task<IActionResult> GetAll()
        {
            var result = await _client.GetAsync<Order>(new Order().GetType().Name + "/GetAll");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<Order> param)
        {
            var result = await _client.PostAsync<Order>(new Order().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(Order postmodel)
        {
            if (postmodel.Id>0)
            {
                var result = await _client.GetAsync<Order>(new Order().GetType().Name + $"/GetRow?id={postmodel.Id}");
                return Json(result);
            }
            else
            {
                postmodel.BillingAdressId = SessionRequest.LoginUser.UserAdress.FirstOrDefault(o => o.IsDefault == true).Id;
                postmodel.ShippingAddId = postmodel.BillingAdressId;
                postmodel.RegistrationDate = DateTime.Now;
                postmodel.UserId = SessionRequest.LoginUser.Id;


                var resultRow = await _client.GetAsync<Product>(new Product().GetType().Name + $"/GetRow?id={postmodel.OrderDetail.FirstOrDefault().ProductId}");
                postmodel.Currency = resultRow.ResultRow.Currency;

                postmodel.TotalAmount = postmodel.OrderDetail.Sum(o=>o.PriceTotal);
                postmodel.OrderDetail = null;


                var result = await _client.PostAsync<Order>(new Order().GetType().Name + "/InsertOrUpdate", postmodel);

                _IHttpContextAccessor.HttpContext.Session.Set("myOrder", result.ResultRow);

                return Json(result);
            }
         

           
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<Order>(new Order().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            _IHttpContextAccessor.HttpContext.Session.Set("myOrder", result.ResultRow);
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<Order>(new Order().GetType().Name + "/GetRow" + $"?id={id}");
            return Json(result);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<Order>(new Order().GetType().Name + "/Delete" + $"?id={id}");
            return Json(result);
        }


    }
}
