using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public partial class ContentPage : BaseModel
{
    public ContentPage()
    {
        Documents = new HashSet<Documents>();
        Gallery = new HashSet<Documents>();
        Childs = new HashSet<ContentPage>();
        SpecContentValue = new HashSet<SpecContentValue>();
    }

    //1. Sayfa Yapısı
    
    [DisplayName("Başlık")]
    [Required()]
    public string Name { get; set; }

    
    [DisplayName("Üst Kategori")]
    public int? ParentId { get; set; }

    [DisplayName("Üst Liste")]
    public virtual ContentPage Parent { get; set; }


    [DisplayName("İçerik Tipi")]
    [Required()]
    public ContentTypes ContentTypes { get; set; }

    [NotMapped]
    public string ContentTypesName { get { return ContentTypes.ExGetDescription(); } }



    [DisplayName("Şablon Tipi")]
    [Required()]
    public TemplateType TemplateType { get; set; }


    [NotMapped]
    public string TemplateTypeName { get { return TemplateType.ExGetDescription(); } }



    [DisplayName("Sayfa Url")]
    [Required()]
    public string Link { get; set; }
   

    
    [DisplayName("Dış Url")]
    public string ExternalLink { get; set; }



    
    [DisplayName("Orjinal Id")]
    public int? OrjId { get; set; }

    
    [DisplayName("Orjinal")]
    public virtual ContentPage Orj { get; set; }


    
    [DisplayName("Dil")]
    [Required()] public int LangId { get; set; }
    public virtual Lang Lang { get; set; }




    [DataType("SingleDocument")]
    [DisplayName("Ön Görsel")]
    public Documents ThumbImage { get; set; }

    [DataType("SingleDocument")]
    [DisplayName("Görsel")]
    public Documents Picture { get; set; }

    [DataType("SingleDocument")]
    [DisplayName("Banner Görsel")]
    public Documents BannerImage { get; set; }

    
    [DisplayName("Banner Yazı")]
    public string BannerText { get; set; }

    
    [DisplayName("Banner Button Yazı")]
    public string BannerButtonText { get; set; }

    
    [DisplayName("Button Yazı")]
    public string ButtonText { get; set; }

    
    [DisplayName("Button Link")]
    public string ButtonLink { get; set; }
     
    
    [DataType("text")]
    [DisplayName("Açıklama")]
    public string Description { get; set; }

    
    [DataType("text")]
    [DisplayName("Kısa İçerik")]
    public string ContentShort { get; set; } 

    
    [DataType("text")]
    [DisplayName("İçerik")]
    public string ContentData { get; set; }
     
    
    [DisplayName("Video Link")]
    public string VideoLink { get; set; }
    
    [DisplayName("Form Tipi")]
    public int? FormTypeId { get; set; }

    [DisplayName("Form Tipi")]
    public virtual FormType FormType { get; set; }

    [DisplayName("Form Onayı")]
    public bool? IsForm { get; set; }


    [DisplayName("Galeri")]
    public bool? IsGallery { get; set; }

    
    [DisplayName("Harita")]
    public bool? IsMap { get; set; }

    [DataType("text")]
    [DisplayName("Harita")]
    public string Map { get; set; }

    //3. Sayfa Ayarları


    [DisplayName("Üst Menü")]
    public bool? IsHeaderMenu { get; set; }

    
    [DisplayName("Alt Menü")]
    public bool? IsFooterMenu { get; set; }

    
    [DisplayName("Hamburger Menü")]
    public bool? IsHamburgerMenu { get; set; }

    
    [DisplayName("Yan Menü")]
    public bool? IsSideMenu { get; set; }

    


    
    [DisplayName("Meta Title")]
    public string MetaTitle { get; set; }

    
    [DisplayName("Meta Keywords")]
    public string MetaKeyword { get; set; }

    
    [DisplayName("Meta Description")]
    public string MetaDescription { get; set; }

    
    [DisplayName("İçerik Sırası")]
    public int? ContentOrderNo { get; set; }

    
    [DisplayName("Yayına Alma Durumu")]
    public bool? IsPublish { get; set; }


    [DisplayName("Tıklanabilir")]
    public bool? IsClick { get; set; }



    [DisplayName("Başlangıç Tarihi")]
    public DateTime StartDate { get; set; }
    [DisplayName("Bitiş Tarihi")]
    public DateTime EndDate { get; set; }



    [DisplayName("Galeri")]
    public virtual ICollection<Documents> Gallery { get; set; }

    
    [DisplayName("Dokümanlar")]
    public virtual ICollection<Documents> Documents { get; set; }

         
    [DisplayName("Alt Liste")]
    public virtual ICollection<ContentPage> Childs { get; set; }


    public virtual ICollection<SpecContentValue> SpecContentValue { get; set; }




}






public enum TemplateType : int
{
    [Description("Boş Sayfa")]
    None = 0,
    [Description("Anasayfa")]
    Index = 1,
    [Description("Alt Sayfa - Yan Menü")]
    SideContent = 2,
    [Description("Alt Sayfa")]
    Content = 3,
    [Description("Blog Listeleme")]
    BlogList = 4,
    [Description("Blog Detay")]
    BlogDetail = 5,
    [Description("Etkinlik Listeleme")]
    EventList = 6,
    [Description("Etkinlik Detay")]
    EventDetail = 7,
    [Description("Empty")]
    Empty = 8,
    [Description("İletişim")]
    Contact = 9,
    [Description("HTML Raw")]
    HtmlRaw = 11,
    [Description("Ürün Detay")]
    UrunDetay = 12,
    [Description("Ürün Listeleme")]
    UrunList = 13,
    [Description("Ön Kayıt Formu")]
    PreForm = 14,
    [Description("Yarım Sayfa")]
    PageHalf = 15,
    [Description("Çapraz Liste")]
    CrossList = 16,
    [Description("Parçalı Sayfa")]
    PagePart = 17,
    [Description("Akordion")]
    Accordion = 18,
    [Description("Akordion Menü")]
    AccordionMenu = 20,
}



public enum ContentTypes : int
{
    [Description("Sayfa")]
    Page = 1,
    [Description("Blog")]
    Blog = 3,
    [Description("Slider")]
    Slider = 4,
    [Description("Etkinlik")]
    Event = 5,
    [Description("Anasayfa")]
    MainPage = 8,
    [Description("Footer")]
    Footer = 9,
    [Description("Mailing")]
    Mailing = 10,

}

