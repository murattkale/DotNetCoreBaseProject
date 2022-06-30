using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class SpecController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        ISpecService _ISpecService;
        ISpecContentTypeService _ISpecContentTypeService;
        public SpecController(IUnitOfWork<myDBContext> _uow, ISpecService _ISpecService, ISpecContentTypeService _ISpecContentTypeService)
        {
            this._uow = _uow;
            this._ISpecService = _ISpecService;
            this._ISpecContentTypeService = _ISpecContentTypeService;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _ISpecService.WhereList();
            return Ok(result);
        }


        [HttpGet("GetSpecType")]
        public IActionResult GetSpecType()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(SpecType)).Cast<int>()
                .Select(x => new EnumModel { name = ((SpecType)x).ToStr(), value = x.ToString(), text = ((SpecType)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<Spec> param)
        {
            var result = _ISpecService.GetPaging(null, true, param, false, o => o.Parent, o => o.SpecAttrs, o => o.SpecChilds);
            return Ok(result);
        }

        [HttpGet("GetSelect")]
        public IActionResult GetSelect(string name, string whereCase)
        {
            var rModel = new RModel<EnumModel>();
            if (!string.IsNullOrEmpty(whereCase))
            {
                if (whereCase == "IsTanim")
                {
                    var result = _ISpecService.Where(o => o.IsTanim == true).Result
                   .Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name + "(" + o.SpecTypeName + ")" }).ToList();
                    rModel.ResultList = result;
                }
                else if (whereCase.ToInt() > 0)
                {
                    var type = (whereCase.ToInt() > 0 ? (SpecType)whereCase.ToInt() : 0);

                    var result = _ISpecService.Where(o => (type > 0 ? o.SpecType == type : true)).Result
                  .Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name + "(" + o.SpecTypeName + ")" }).ToList();
                    rModel.ResultList = result;
                }
                else
                {
                    var result = _ISpecService.Where().Result
                    .Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name + "(" + o.SpecTypeName + ")" }).ToList();
                    rModel.ResultList = result;
                }
            }
            else
            {
                var result = _ISpecService.Where().Result
                  .Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name + "(" + o.SpecTypeName + ")" }).ToList();
                rModel.ResultList = result;
            }
            rModel.RType = RType.OK;

            return Ok(rModel);
        }



        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(Spec postModel)
        {
            var result = _ISpecService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _ISpecService.Get(o => o.Id == id);
            return Ok(result);
        }

        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _ISpecService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }


        [HttpGet("GetSpecValueAll")]
        public IActionResult GetSpecValueAll(ContentTypes ContentTypesId)
        {
            RModel<Spec> res = new RModel<Spec>();
            var specTypes = _ISpecContentTypeService.WhereList(o => o.ContentTypes == ContentTypesId, true, false).ResultList.ToList();
            var result = new List<Spec>();
            if (specTypes.Count > 0)
            {
                var specListIds = specTypes.Select(o => o.SpecId).ToList();
                //var list = new List<int> { 1, 2 };
                var spec = _ISpecService.WhereList(o => specListIds.Contains(o.Id), true, false, o => o.SpecChilds, o => o.SpecAttrs, o => o.SpecContentValue).Result.ToList();
                //result = spec.Where(o => ).ToList();
                result.ForEach(o =>
                {
                    o.SpecChilds = o.SpecChilds.Count > 0 ? spec.Where(oo => oo.ParentId == o.Id).ToList() : new List<Spec>();
                });

            }
            res.ResultList = result;
            return Ok(res);
        }





    }
}
