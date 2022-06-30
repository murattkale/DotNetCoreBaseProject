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
    public class TownController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ITownService _ITownService;
        public TownController(IUnitOfWork<myDBContext> _uow, ITownService _ITownService)
        {
            this._uow = _uow;
            this._ITownService = _ITownService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll(int CityId)
        {
            var result = _ITownService.WhereList(o => o.CityId == CityId, true, false);
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(int CityId)
        {
            var rModel = new RModel<EnumModel>();
            var result = _ITownService.Where(o => o.CityId == CityId).Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Town> param)
        {
            var result = _ITownService.GetPaging(o => o.CityId == param.selectid, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Town postModel)
        {
            var result = _ITownService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ITownService.Get(o => o.Id == id, true, false);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ITownService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
