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
    public class BankController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IBankService _IBankService;
        public BankController(IUnitOfWork<myDBContext> _uow, IBankService _IBankService)
        {
            this._uow = _uow;
            this._IBankService = _IBankService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IBankService.WhereList();
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IBankService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Bank> param)
        {
            var result = _IBankService.GetPaging(null, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Bank postModel)
        {
            var result = _IBankService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IBankService.Get(o => o.Id == id, true, false);
            return Ok(result);
        }



        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IBankService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }



    
    }
}
