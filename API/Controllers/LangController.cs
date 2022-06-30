using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class LangController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ILangService _ILangService;
        public LangController(IUnitOfWork<myDBContext> _uow, ILangService _ILangService)
        {
            this._uow = _uow;
            this._ILangService = _ILangService;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _ILangService.WhereList();
            return Ok(result);
        }


        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            var result = _ILangService.Where().Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Lang> param)
        {
            var result = _ILangService.GetPaging(null, true, param, false);
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Lang postModel)
        {
            var result = _ILangService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ILangService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ILangService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }



    }
}
