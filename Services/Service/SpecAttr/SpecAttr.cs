using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public partial class SpecAttr : BaseModel
{
    public SpecAttr()
    {
    }

    [ForeignKey("SpecId")]
    [Required]
    [DisplayName("Spec")]
    public int SpecId { get; set; }
    public virtual Spec Spec { get; set; }

    [Required]
    [DisplayName("TR")]
    public string AttrValue { get; set; }


    //[DisplayName("EN")]
    //public string AttrValue1 { get; set; }



}

