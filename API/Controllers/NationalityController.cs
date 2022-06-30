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
    public class NationalityController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        INationalityService _INationalityService;
        public NationalityController(IUnitOfWork<myDBContext> _uow, INationalityService _INationalityService)
        {
            this._uow = _uow;
            this._INationalityService = _INationalityService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _INationalityService.WhereList();
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _INationalityService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Nationality> param)
        {
            var result = _INationalityService.GetPaging(null, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Nationality postModel)
        {
            var result = _INationalityService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _INationalityService.Get(o => o.Id == id, true, false);
            return Ok(result);
        }


        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _INationalityService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
