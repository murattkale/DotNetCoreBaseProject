using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


public partial class Lang : BaseModel
{
    public Lang()
    {
        ContentPage = new HashSet<ContentPage>();
    }


    [DisplayName("Dil Adı")]
    [Required]
    public string Name { get; set; }

    [DisplayName("Dil Kodu")]
    [Required]
    public string Code { get; set; }

    public bool IsDefault { get; set; }


    [DataType("SingleDocument")]
    [DisplayName("Lang Logo")]
    public string Logo { get; set; }

    [DisplayName("Varsayılan Dil")]

    public virtual ICollection<ContentPage> ContentPage { get; set; }
}

