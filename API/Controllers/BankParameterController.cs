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
    public class BankParameterController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IBankParameterService _IBankParameterService;
        public BankParameterController(IUnitOfWork<myDBContext> _uow, IBankParameterService _IBankParameterService)
        {
            this._uow = _uow;
            this._IBankParameterService = _IBankParameterService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IBankParameterService.WhereList();
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IBankParameterService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Value }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<BankParameter> param)
        {
            var result = _IBankParameterService.GetPaging(null, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(BankParameter postModel)
        {
            var result = _IBankParameterService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IBankParameterService.Get(o => o.Id == id, true, false);
            return Ok(result);
        }



        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IBankParameterService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }



    
    }
}
