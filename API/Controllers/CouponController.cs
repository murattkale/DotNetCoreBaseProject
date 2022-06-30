using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class CouponController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ICouponService _ICouponService;
        public CouponController(IUnitOfWork<myDBContext> _uow, ICouponService _ICouponService)
        {
            this._uow = _uow;
            this._ICouponService = _ICouponService;
        }


        [HttpPost("CouponControl")]
        public IActionResult CouponControl(Coupon postModel)
        {
            var rModel = new RModel<Coupon>();
            var res = _ICouponService.Where(o => o.Limit > 0 && (o.Limit - o.Used) > 0 && o.IsActive == true
            && o.StartDate <= DateTime.Now && o.EndDate >= DateTime.Now && o.Name == postModel.Name);
            rModel.ResultRow = res.Result.FirstOrDefault();
            rModel.RType = res.RType;
            rModel.Message = res.Message;
            return Ok(rModel);

        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _ICouponService.WhereList();
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _ICouponService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Coupon> param)
        {
            var result = _ICouponService.GetPaging(null, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Coupon postModel)
        {
            var result = _ICouponService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ICouponService.Get(o => o.Id == id, true, false);
            return Ok(result);
        }



        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ICouponService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
