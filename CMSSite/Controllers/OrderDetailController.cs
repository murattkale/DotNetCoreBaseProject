using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMSSite.Controllers
{
    public class OrderDetailController : Controller
    {
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public OrderDetailController(

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
            var result = await _client.GetAsync<EnumModel>(new OrderDetail().GetType().Name + $"/GetSelect?name={name}&whereCase={whereCase}");
            return Json(result.ResultList);

        }


        public async Task<IActionResult> GetAllByOrder(int OrderId)
        {
            var result = await _client.GetAsync<OrderDetail>(new OrderDetail().GetType().Name + $"/GetAllByOrder?OrderId={OrderId}");
            return Json(result);
        }


        public async Task<IActionResult> GetAll()
        {
            var result = await _client.GetAsync<OrderDetail>(new OrderDetail().GetType().Name + "/GetAll");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<OrderDetail> param)
        {
            var result = await _client.PostAsync<OrderDetail>(new OrderDetail().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(List<OrderDetail> postModelDatas)
        {
            var OrderId = postModelDatas.FirstOrDefault().OrderId;
            var pr = new List<Product>();
            postModelDatas.ForEach(postmodel =>
            {

                if (postmodel.Id > 0)
                {
                    var orderdetail = _client.Get<OrderDetail>(new OrderDetail().GetType().Name + $"/GetRow?id={postmodel.Id}");

                    orderdetail.ResultRow.RaceSize = postmodel.Size;
                    orderdetail.ResultRow.Size = postmodel.Size;
                    orderdetail.ResultRow.Club = postmodel.Club;
                    orderdetail.ResultRow.Course = postmodel.Course;
                    orderdetail.ResultRow.Hotel = postmodel.Hotel;
                    orderdetail.ResultRow.Accomodation = postmodel.Accomodation;
                    orderdetail.ResultRow.RideBack = postmodel.RideBack;

                    var result = _client.Post<OrderDetail>(new OrderDetail().GetType().Name + "/InsertOrUpdate", orderdetail.ResultRow);

                    //var order = _client.Get<Order>(new Order().GetType().Name + $"/GetRow?id={orderdetail.ResultRow.OrderId}");
                    //_IHttpContextAccessor.HttpContext.Session.Set("myOrder", order.ResultRow);
                    //OrderId = orderdetail.ResultRow.OrderId;

                }
                else
                {
                    var resultRow = _client.Get<Product>(new Product().GetType().Name + $"/GetRow?id={postmodel.ProductId}");
                    resultRow.ResultRow.Stock = postmodel.Stock;
                    pr.Add(resultRow.ResultRow);
                    postmodel.PriceTotal = postmodel.Stock > 0 ? (postmodel.Stock * resultRow.ResultRow.PriceDiscount).ToDecimal() : resultRow.ResultRow.PriceDiscount;
                    postmodel.PriceUnit = resultRow.ResultRow.Price;

                    postmodel.Currency = resultRow.ResultRow.Currency;
                    postmodel.RaceSize = postmodel.Size;
                    postmodel.OrderCount = postmodel.Stock.Value;


                    var result = _client.Post<OrderDetail>(new OrderDetail().GetType().Name + "/InsertOrUpdate", postmodel);


                }

            });

            var order = _client.Get<Order>(new Order().GetType().Name + $"/GetRow?id={OrderId}");
            if (postModelDatas.Any(o => o.Id < 1))
            {
                order.ResultRow.TotalAmount = pr.Sum(o => o.Stock > 0 ? (o.Stock * o.PriceDiscount).ToDecimal() : o.PriceDiscount);

                var result = _client.Post<Order>(new Order().GetType().Name + "/InsertOrUpdate", order.ResultRow);

                foreach (var item in order.ResultRow.OrderDetail)
                {
                    item.Product = pr.FirstOrDefault(o => o.Id == item.ProductId);
                }
            }


            _IHttpContextAccessor.HttpContext.Session.Set("myOrder", order.ResultRow);

            return Json("ok");

        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<OrderDetail>(new OrderDetail().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<OrderDetail>(new OrderDetail().GetType().Name + "/GetRow" + $"?id={id}");
            return Json(result);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<OrderDetail>(new OrderDetail().GetType().Name + "/Delete" + $"?id={id}");
            return Json(result);
        }


    }
}
