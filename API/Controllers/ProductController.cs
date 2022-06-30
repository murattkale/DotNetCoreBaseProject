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
    public class ProductController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IProductService _IProductService;
        public ProductController(IUnitOfWork<myDBContext> _uow, IProductService _IProductService)
        {
            this._uow = _uow;
            this._IProductService = _IProductService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll(ProductType ProductType)
        {
            var result = _IProductService.WhereList(o=>o.ProductType == ProductType, true, false, o => o.MainProduct);
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(ProductType ProductType)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IProductService.Where(o=>o.ProductType == ProductType).Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Product> param)
        {
            var result = _IProductService.GetPaging(null, true, param, false, o => o.MainProduct);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Product postModel)
        {
            var result = _IProductService.InsertOrUpdate(postModel);
            var rs = _uow.SaveChanges();
            result.RType = rs.RType;
            result.Message = rs.Message;
            result.MessageList = rs.MessageList;
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IProductService.Get(o => o.Id == id, true, false, o => o.MainProduct);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IProductService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }



        [HttpGet("GetProductType")]
        public IActionResult GetProductType()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(ProductType)).Cast<int>()
                .Select(x => new EnumModel { name = ((ProductType)x).ToStr(), value = x.ToString(), text = ((ProductType)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }



    }
}
