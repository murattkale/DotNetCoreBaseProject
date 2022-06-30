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
    public class DistrictController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IDistrictService _IDistrictService;
        public DistrictController(IUnitOfWork<myDBContext> _uow, IDistrictService _IDistrictService)
        {
            this._uow = _uow;
            this._IDistrictService = _IDistrictService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll(int TownId)
        {
            var result = _IDistrictService.WhereList(o => o.TownId == TownId, true, false);
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(int TownId)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IDistrictService.Where(o => o.TownId == TownId).Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<District> param)
        {
            var result = _IDistrictService.GetPaging(o => o.TownId == param.selectid, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(District postModel)
        {
            var result = _IDistrictService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IDistrictService.Get(o => o.Id == id, true, false);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IDistrictService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }




    }
}
