using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Components
{

    public class DynamicInput : ViewComponent
    {

        IHttpContextAccessor _httpContextAccessor;
        IHttpClientWrapper _client;

        public DynamicInput(
        IHttpContextAccessor httpContextAccessor,
          IHttpClientWrapper _client
            )
        {
            this._httpContextAccessor = httpContextAccessor;
            this._client = _client;
        }


        public IViewComponentResult Invoke(DynamicModel postModel)
        {
            if (postModel.PageType == "Documents")
            {
                return View("DynamicInput_Documents", postModel);
            }
            else if (postModel.PageType == "ContentPage")
            {
                return View("DynamicInput_ContentPage", postModel);
            }
            else if (postModel.PageType == "SpecDynamic")
            {
                var ct = postModel.model.GetPropValue("ContentTypes");
                var result =  _client.Get<Spec>(new Spec().GetType().Name + $"/GetSpecValueAll?ContentTypesId={(int)ct}");
                
                ViewBag.spec = result.ResultList;

                return View("DynamicInput_Spec", postModel);
            }
            else if (postModel.PageType == "DynamicInput2")
            {
                return View("DynamicInput2", postModel);
            }
            else
            {
                return View("DynamicInput", postModel);
            }


        }




    }
}