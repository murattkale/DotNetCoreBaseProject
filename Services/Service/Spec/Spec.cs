using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Spec : BaseModel
{
    public Spec()
    {
        SpecChilds = new HashSet<Spec>();
        SpecAttrs = new HashSet<SpecAttr>();
        SpecContentValue = new HashSet<SpecContentValue>();

    }

    [ForeignKey("ParentId")]
    [DisplayName("Üst Spec")]
    public int? ParentId { get; set; }


    public virtual Spec Parent { get; set; }

    [DisplayName("TR")]
    public string Name { get; set; }

    [DisplayName("EN")]
    public string Name1 { get; set; }

    [DisplayName("DE")]
    public string Name2 { get; set; }

    [DisplayName("RU")]
    public string Name3 { get; set; }

    [DisplayName("FR")]
    public string Name4 { get; set; }


    [DisplayName("Tip")]
    public SpecType SpecType { get; set; }


    [NotMapped]
    public string SpecTypeName { get { return SpecType.ExGetDescription(); } }


    [DisplayName("Tanım")]
    public bool? IsTanim { get; set; }


    public virtual ICollection<Spec> SpecChilds { get; set; }
    public virtual ICollection<SpecAttr> SpecAttrs { get; set; }
    public virtual ICollection<SpecContentValue> SpecContentValue { get; set; }


}


public enum SpecType : int
{
    [Description("tanım")]
    tanim = 1,
    [Description("check")]
    check = 2,
    [Description("list")]
    list = 3,
    [Description("text")]
    text = 4,
    [Description("editor")]
    editor = 5,

}