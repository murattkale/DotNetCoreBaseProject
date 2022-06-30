using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Controllers
{
    public class CouponController : Controller
    {
        IBaseModel _IBaseModel;
        IConfiguration _IConfiguration;
        IHostingEnvironment _IHostingEnvironment;
        IHttpContextAccessor _IHttpContextAccessor;
        IHttpClientWrapper _client;

        public CouponController(
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> GetPaging(DTParameters<Coupon> param)
        {
            var result = await _client.PostAsync<Coupon>(new Coupon().GetType().Name + "/GetPaging", param);
            var rs = result.ResultPaging;
            return Json(rs);
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrUpdate(Coupon postmodel)
        {
            if (postmodel.Id < 1) postmodel.Used = 0;
            var result = await _client.PostAsync<Coupon>(new Coupon().GetType().Name + "/InsertOrUpdate", postmodel);
            return Json(result);
        }

        public async Task<IActionResult> InsertOrUpdatePage()
        {
            var result = await _client.GetAsync<Coupon>(new Coupon().GetType().Name + $"/GetRow?id={Request.Query["id"].ToInt()}");
            ViewBag.postModel = result.ResultRow;
            return View();
        }

        public async Task<IActionResult> GetRow(int? id)
        {
            var result = await _client.GetAsync<Coupon>(new Coupon().GetType().Name + $"/GetRow?id={id}");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _client.GetAsync<Coupon>(new Coupon().GetType().Name + $"/Delete?id={id}");
            return Json(result);
        }


        public async Task<IActionResult> GetCouponType()
        {
            var result = new Coupon().CouponTypeList;
            return Json(result);
        }



    }
}
