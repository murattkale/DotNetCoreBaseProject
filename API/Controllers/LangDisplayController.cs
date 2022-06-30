using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class LangDisplayController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ILangDisplayService _ILangDisplayService;
        public LangDisplayController(IUnitOfWork<myDBContext> _uow, ILangDisplayService _ILangDisplayService)
        {
            this._uow = _uow;
            this._ILangDisplayService = _ILangDisplayService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _ILangDisplayService.WhereList();
            return Ok(result);
        }

        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _ILangDisplayService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name_1 }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<LangDisplay> param)
        {
            var result = _ILangDisplayService.GetPaging(null, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(LangDisplay postModel)
        {
            var result = _ILangDisplayService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ILangDisplayService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ILangDisplayService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }



    }
}
