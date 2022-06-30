﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public partial class FormType : BaseModel
{
    public FormType()
    {
        Forms = new HashSet<Forms>();
        ContentPage = new HashSet<ContentPage>();
    }

    [DisplayName("Form Kodu")]
    public string FormCode { get; set; }

    [DisplayName("Ad")]
    [Required]
    public string Name { get; set; } 
    [DisplayName("Açıklama")]
    public string DescName { get; set; }
    [DisplayName("E-Posta")]
    public string Mail { get; set; }
    [DisplayName("E-Posta CC")]
    public string MailCC { get; set; }
    [DisplayName("Formlar")]
    public virtual ICollection<Forms> Forms { get; set; }
    public virtual ICollection<ContentPage> ContentPage { get; set; }


}

