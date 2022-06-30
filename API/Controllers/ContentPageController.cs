using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "api")]
    public class ContentPageController : ControllerBase
    {
        IUnitOfWork<myDBContext> _uow;
        IContentPageService _IContentPageService;
        IDocumentsService _IDocumentsService;
        IHostingEnvironment _IHostingEnvironment;

        public ContentPageController(IUnitOfWork<myDBContext> _uow, IContentPageService _IContentPageService, IDocumentsService _IDocumentsService, IHostingEnvironment _IHostingEnvironment)
        {
            this._uow = _uow;
            this._IContentPageService = _IContentPageService;
            this._IDocumentsService = _IDocumentsService;
            this._IHostingEnvironment = _IHostingEnvironment;

        }

        [HttpGet("GetId")]
        public IActionResult GetId(int Id)
        {
            var result = _IContentPageService.Get(o => o.Id == Id && o.IsPublish == true);
            return Ok(result);
        }

        [HttpGet("GetIdR")]
        public IActionResult GetIdR(int Id)
        {
            var result = _IContentPageService.Get(false,o => o.Id == Id && o.IsPublish == true, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                o => o.FormType
                );
            return Ok(result);
        }

        [HttpGet("GetOrj")]
        public IActionResult GetOrj(int OrjId)
        {
            var result = _IContentPageService.Get(o => o.OrjId == OrjId && o.IsPublish == true);
            return Ok(result);
        }

        [HttpGet("GetOrjR")]
        public IActionResult GetOrjR(int OrjId)
        {
            var result = _IContentPageService.Get(false, o => o.OrjId == OrjId && o.IsPublish == true, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                o => o.FormType,
                o => o.Orj
                );
            return Ok(result);
        }

        [HttpGet("GetLink")]
        public IActionResult GetLink(string link)
        {
            var result = _IContentPageService.Get(o => o.Link == link && o.IsPublish == true);
            return Ok(result);
        }

        [HttpGet("GetLinkR")]
        public IActionResult GetLinkR(string link)
        {
            var result = _IContentPageService.Get(false, o => o.Link == link && o.IsPublish == true, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                o => o.FormType,
                o => o.Orj
                );
            return Ok(result);
        }


        [HttpGet("GetHeaderFooter")]
        public IActionResult GetHeaderFooter(bool? IsHeaderMenu, bool? IsFooterMenu)
        {
            var result = _IContentPageService.WhereList(o => o.IsPublish == true
            && (IsHeaderMenu == true ? o.IsHeaderMenu == IsHeaderMenu : true)
            && (IsFooterMenu == true ? o.IsFooterMenu == IsFooterMenu : true)
            , true, false,
                o => o.Childs,
                o => o.Parent
                );
            return Ok(result);
        }


        [HttpGet("GetAllType")]
        public IActionResult GetAllType(ContentTypes ContentTypes)
        {
            var result = _IContentPageService.WhereList(o => o.IsPublish == true && o.ContentTypes == ContentTypes, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                 o => o.FormType
                );
            return Ok(result);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = _IContentPageService.WhereList(o => o.IsPublish == true, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                 o => o.FormType
                );
            return Ok(result);
        }

        [HttpGet("GetSelect")]
        public IActionResult GetSelect(ContentTypes? ContentTypes)
        {
            var rModel = new RModel<EnumModel>();
            var result = _IContentPageService.Where(o => o.IsPublish == true && (ContentTypes > 0 ? o.ContentTypes == ContentTypes : true)).Result.Select(o => new EnumModel { value = o.Id.ToStr(), text = o.Name }).ToList();
            rModel.ResultList = result;
            rModel.Result = null;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpPost("GetPaging")]
        public IActionResult GetPaging(DTParameters<ContentPage> param)
        {
            var type = param.selectid > 0 ? (ContentTypes)param.selectid : 0;
            var result = _IContentPageService.GetPaging(o => (type > 0 ? o.ContentTypes == type : true), true, param, false, o => o.Parent, o => o.Lang);
            return Ok(result);
        }

        [HttpPost("GetContentPage")]
        public IActionResult GetContentPage()
        {
            var result = _IContentPageService.Where(null, true, false, o => o.ContentTypes).Result.Select(o => new { value = o.Id, text = o.Name }).ToList();
            return Ok(result);
        }

        [HttpPost("InsertOrUpdate")]
        public IActionResult InsertOrUpdate(ContentPage postModel)
        {
            var result = _IContentPageService.InsertOrUpdate(postModel);
            var saveResult = _uow.SaveChanges();
            return Ok(result);
        }

        [HttpGet("GetRow")]
        public IActionResult GetRow(int? id)
        {
            var result = _IContentPageService.Get(o => o.Id == id, true, false,
                o => o.Childs,
                o => o.Parent,
                o => o.Gallery,
                o => o.Documents,
                o => o.ThumbImage,
                o => o.Picture,
                o => o.BannerImage,
                 o => o.FormType
                );
            return Ok(result);
        }



        [HttpGet("Delete")]
        public IActionResult Delete(int id)
        {
            var result = _IContentPageService.Delete(id);
            _uow.SaveChanges();
            return Ok(result);
        }

        [HttpPost("UpdateOrder")]
        public IActionResult UpdateOrder(List<OrderUpdateModel> postModel)
        {
            var rModel = new RModel<OrderUpdateModel>();
            if (postModel.Count > 0)
            {
                var type = (ContentTypes)postModel.FirstOrDefault().dataid;
                var rows = _IContentPageService.Where(o => o.ContentTypes == type, false, false).Result.ToList();
                postModel.ForEach(o =>
                {
                    var row = rows.FirstOrDefault(r => r.Id == o.Id);
                    if (row != null)
                    {
                        row.OrderNo = o.OrderNo;
                        _IContentPageService.Update(row);
                        _uow.SaveChanges();
                    }
                });

                rModel.ResultList = postModel;
                rModel.Result = null;
            }

            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpGet("GetTemplateType")]
        public IActionResult GetTemplateType()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(TemplateType)).Cast<int>()
                .Select(x => new EnumModel { name = ((TemplateType)x).ToStr(), value = x.ToString(), text = ((TemplateType)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }

        [HttpGet("GetContentTypes")]
        public IActionResult GetContentTypes()
        {
            var rModel = new RModel<EnumModel>();
            var list = Enum.GetValues(typeof(ContentTypes)).Cast<int>()
                .Select(x => new EnumModel { name = ((ContentTypes)x).ToStr(), value = x.ToString(), text = ((ContentTypes)x).ExGetDescription() }).ToList();
            rModel.ResultList = list;
            rModel.RType = RType.OK;
            return Ok(rModel);
        }




        [HttpPost("GetGallery")]
        public IActionResult GetGallery(DTParameters<Documents> param, int selectid)
        {
            var result = _IDocumentsService.GetPaging(o => o.GalleryId == selectid, true, param);
            result.ResultPaging.data = result.ResultPaging.data.OrderBy(o => o.OrderNo).ToList();
            return Ok(result);
        }

        [HttpPost("GetDocuments")]
        public IActionResult GetDocuments(DTParameters<Documents> param, int selectid)
        {
            var result = _IDocumentsService.GetPaging(o => o.DocumentId == selectid, true, param);
            result.ResultPaging.data = result.ResultPaging.data.OrderBy(o => o.OrderNo).ToList();
            return Ok(result);
        }



        [HttpPost]
        public IActionResult DeleteImage(int id)
        {
            var result = _IDocumentsService.Where(o => o.Id == id).Result.FirstOrDefault();
            //var path = this.GetPathAndFilename(result.Link);
            //if (System.IO.File.Exists(path))
            //    System.IO.File.Delete(path);
            _IDocumentsService.Delete(result.Id);
            var res = _uow.SaveChanges();
            return Ok(res);
        }


        string GetPathAndFilename(string filename)
        {
            string path = this._IHostingEnvironment.WebRootPath + "\\uploads\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }




    }
}
