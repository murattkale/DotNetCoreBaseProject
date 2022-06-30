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
    public class OrderController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IOrderService _IOrderService;
        IOrderDetailService _IOrderDetailService;
        public OrderController(IUnitOfWork<myDBContext> _uow, IOrderService _IOrderService, IOrderDetailService _IOrderDetailService)
        {
            this._uow = _uow;
            this._IOrderService = _IOrderService;
            this._IOrderDetailService = _IOrderDetailService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IOrderService.WhereList();
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IOrderService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.TransactionID }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Order> param)
        {
            var result = _IOrderService.GetPaging(null, true, param, false, o => o.OrderDetail, o => o.User, o => o.Coupon);
           
            if (result.ResultPaging.data.Count > 0)
            {
                result.ResultPaging.data.ForEach(o =>
                {
                    var olist = o.OrderDetail.Select(o => o.Id).ToList();
                    if (olist.Count > 0)
                        o.OrderDetail = _IOrderDetailService.WhereList(oo => olist.Contains(oo.Id), true, false, oo => oo.Product).ResultList;
                });
            }

            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Order postModel)
        {
            postModel.PurchaseDate = DateTime.Now;
            var result = _IOrderService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            result.Message = saveResult.Message;
            result.RType = saveResult.RType;
            result.ResultRow = _IOrderService.Get(o => o.Id == result.ResultRow.Id, true, false, o => o.OrderDetail, o => o.Coupon).ResultRow;
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IOrderService.Get(o => o.Id == id, true, false, o => o.OrderDetail, o => o.Coupon);
            return Ok(result);
        }


        [HttpGet("GetUserOrder")]
        public IActionResult GetUserOrder(int UserId)
        {
            var result = _IOrderService.Get(o => o.UserId == UserId && o.OrderStatus == OrderStatus.Odendi, true, false, o => o.OrderDetail);
            return Ok(result);
        }


        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IOrderService.Delete(id);
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


        [HttpGet("GetConfirmStatus")]
        public IActionResult GetConfirmStatus()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(ConfirmStatus)).Cast<int>()
                .Select(x => new EnumModel { name = ((ConfirmStatus)x).ToStr(), value = x.ToString(), text = ((ConfirmStatus)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

    }
}
