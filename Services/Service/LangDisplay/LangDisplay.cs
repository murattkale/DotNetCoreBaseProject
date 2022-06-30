using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


public partial class LangDisplay : BaseModel
{
    public LangDisplay()
    {

    }


    [DisplayName("Paramater")]
    [Required]
    public string ParamName { get; set; }

    [DisplayName("TR")]
    public string Name_1 { get; set; }


    [DisplayName("EN")]
    public string Name_2 { get; set; }


    [DisplayName("DE")]
    public string Name_3 { get; set; }


    [DisplayName("FR")]
    public string Name_4 { get; set; }


    [DisplayName("RU")]
    public string Name_5 { get; set; }


    [DisplayName("Description")]
    public string Description { get; set; }

}

