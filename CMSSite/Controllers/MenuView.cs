//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;

//namespace CMSSite.Components
//{
//    public class MenuView : ViewComponent
//    {
//        ILangDisplayService _ILangDisplayService;
//        IHttpContextAccessor _httpContextAccessor;
//        IContentPageService _IContentPageService;
//        IDocumentsService _IDocumentsService;
//        public MenuView(
//            ILangDisplayService _ILangDisplayService,
//            IContentPageService _IContentPageService,
//            IDocumentsService _IDocumentsService,
//        IHttpContextAccessor httpContextAccessor
//            )
//        {
//            this._ILangDisplayService = _ILangDisplayService;
//            this._IContentPageService = _IContentPageService;
//            this._httpContextAccessor = httpContextAccessor;
//            this._IDocumentsService = _IDocumentsService;
//        }

//        public IViewComponentResult Invoke(string type)
//        {
//            #region dynamicContent 
//            var link = HttpContext.Request.Path.Value.Trim().Trim('/');
//            ViewBag.LanguageId = SessionRequest.LanguageId;

//            var content = _IContentPageService.Where(o => o.IsActive == true && o.IsPublish == true && o.Link.ToLower() == link.ToLower(), true, false,
//                o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Picture, o => o.BannerImage, o => o.FormType).Result.FirstOrDefault();

//            if (content != null && content.LangId != SessionRequest.LanguageId)
//                content = _IContentPageService.Where(o => o.LangId == SessionRequest.LanguageId && o.OrjId == content.Id &&  o.IsActive == true && o.IsPublish == true , true, false,
//                o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Picture, o => o.BannerImage, o => o.FormType).Result.FirstOrDefault();

         
          
//            //}

//            List<ContentPage> contentPages = new List<ContentPage>();
//            if (_httpContextAccessor.HttpContext.Session.Get("contentPages") == null)
//            {
//                contentPages = _IContentPageService.Where(x => x.LangId == SessionRequest.LanguageId, true, false).Result.ToList();
//                _httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages);
//            }
//            else
//            {
//                contentPages =
//                _httpContextAccessor.HttpContext.Session.Get<List<ContentPage>>("contentPages");

//                if (content != null && content.LangId != SessionRequest.LanguageId)
//                {
//                    contentPages = _IContentPageService.Where(o => o.LangId == SessionRequest.LanguageId, true, false, 
//                        o => o.Childs, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Picture, o => o.BannerImage).Result.ToList();

//                }

//                _httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages);
//            }

//            ViewBag.contentPages = contentPages;
//            ViewBag.content = content;
   

//            if (SessionRequest._LangDisplay == null)
//            {
//                var _LangDisplay = _ILangDisplayService.Where().Result.ToList();
//                _httpContextAccessor.HttpContext.Session.Set("_LangDisplay", _LangDisplay);
//            }

//            if (type == "SideMenu")
//            {
//                int parentID = content.ParentId ?? 0;
//                ContentPage parentContent = new ContentPage();
//                if (parentID != 0)
//                {
//                    while (contentPages.Any(p => p.Id == parentID))
//                    {
//                        parentContent = contentPages.FirstOrDefault(p => p.Id == parentID);

//                        if (parentContent.ParentId == null)
//                        {
//                            parentID = parentContent.Id;
//                            break;
//                        }
//                        else
//                        {
//                            parentID = parentContent.ParentId ?? 0;
//                        }
//                    }
//                    ViewBag.SideMenu = contentPages.Where(x => x.ParentId == parentID && x.IsSideMenu == true && x.LangId == _httpContextAccessor.HttpContext.Session.GetInt32("LanguageId")).OrderBy(o => o.ContentOrderNo).ToList();
//                    List<ContentPage> sideMenus = new List<ContentPage>();
//                    List<ContentPage> tempList = contentPages.Where(x => x.ParentId == parentID).OrderBy(o => o.ContentOrderNo).ToList();
//                    while (true)
//                    {
//                        foreach (var item in tempList)
//                        {
//                            if (item.IsSideMenu == true)
//                            {
//                                sideMenus.Add(item);
//                            }
//                        }
//                        tempList = contentPages.Where(x => x.IsDeleted == null && x.LangId == content.LangId && tempList.Select(x => x.Id).ToList().Contains(x.ParentId ?? 0)).OrderBy(o => o.ContentOrderNo).ThenBy(m => m.Id).ToList();
//                        if (tempList.SelectMany(x => x.Childs).Count() == 0)
//                        {
//                            foreach (var item in tempList)
//                            {
//                                if (item.IsSideMenu == true)
//                                    sideMenus.Add(item);
//                            }
//                            break;
//                        }
//                    }
//                    if (sideMenus.Any(x => x.Id == (x.LangId == 1 ? 307 : 593)))
//                    {
//                        ContentPage itemTah = sideMenus.FirstOrDefault(x => x.Id == (x.LangId == 1 ? 307 : 593));
//                        sideMenus.Remove(itemTah);
//                        sideMenus.Add(itemTah);
//                        //sideMenus = Helpers.Swap(sideMenus, 18, 7).ToList();
//                    }
//                    ViewBag.SideMenu = sideMenus;
//                }
//                else
//                {
//                    ViewBag.SideMenu = content;
//                }
//            }
//            #endregion
//            return View(type);
//        }
//    }
//}