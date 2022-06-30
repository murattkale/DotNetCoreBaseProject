using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class FormDocument : BaseModel
{
    public FormDocument()
    {

    }

    public string Types { get; set; }

    public string Name { get; set; }

    public string Link { get; set; }

    public string Guid { get; set; }

    public string Alt { get; set; }

    public string Title { get; set; }

    public string data_class { get; set; }



    public int? FormsId { get; set; }
    public virtual Forms Forms { get; set; }


}

