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
    public class OrderDetailController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IOrderDetailService _IOrderDetailService;
        public OrderDetailController(IUnitOfWork<myDBContext> _uow, IOrderDetailService _IOrderDetailService)
        {
            this._uow = _uow;
            this._IOrderDetailService = _IOrderDetailService;
        }


        [HttpGet("GetAllByOrder")]
        public IActionResult GetAllByOrder(int OrderId)
        {
            var result = _IOrderDetailService.WhereList(o => o.OrderId == OrderId, true, false, o => o.Order, o => o.Product);
            return Ok(result);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IOrderDetailService.WhereList(null, true, false, o => o.Order, o => o.Product);
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IOrderDetailService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.OrderStatusName }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<OrderDetail> param)
        {
            var result = _IOrderDetailService.GetPaging(o => (param.selectid > 0 ? o.OrderId == param.selectid : true), true, param, false, o => o.Order, o => o.Product,o=>o.Product);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(OrderDetail postModel)
        {
            var result = _IOrderDetailService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IOrderDetailService.Get(o => o.Id == id, true, false, o => o.Order, o => o.Product);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IOrderDetailService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetOrderStatus")]
        public IActionResult GetOrderStatus()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(OrderStatus)).Cast<int>()
                .Select(x => new EnumModel { name = ((OrderStatus)x).ToStr(), value = x.ToString(), text = ((OrderStatus)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

    }
}
