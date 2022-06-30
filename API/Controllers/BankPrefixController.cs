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
    public class BankPrefixController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IBankPrefixService _IBankPrefixService;
        public BankPrefixController(IUnitOfWork<myDBContext> _uow, IBankPrefixService _IBankPrefixService)
        {
            this._uow = _uow;
            this._IBankPrefixService = _IBankPrefixService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IBankPrefixService.WhereList(null, true, false);
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect()
        {
            var rModel = new RModel<EnumModel>();
            var result = _IBankPrefixService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<BankPrefix> param)
        {
            var result = _IBankPrefixService.GetPaging(null, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(BankPrefix postModel)
        {
            var result = _IBankPrefixService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IBankPrefixService.Get(o => o.Id == id, true, false);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IBankPrefixService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
