using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Forms : BaseModel
{
    public Forms()
    {
        FormDocument = new HashSet<FormDocument>();
    }
    public virtual ICollection<FormDocument> FormDocument { get; set; }


    [NotMapped]
    public int? ContentPageId { get; set; }

    [DisplayName("Form Tipi")]
    public int FormTypeId { get; set; }

    [DisplayName("Form Tipi")]
    public virtual FormType FormType { get; set; }



    [DisplayName("Name")]
    public string Name { get; set; }

    [DisplayName("Surname")]
    public string Surname { get; set; }

    [Required]
    [DisplayName("Mail")]
    public string Email { get; set; }


    [DisplayName("Phone")]
    public string Phone { get; set; }

    [DisplayName("Adress")]
    public string Adress { get; set; }


    [DisplayName("Subject")]
    public string Subject { get; set; }

    [DisplayName("Mesaj")]
    public string Message { get; set; }

    [DisplayName("Okundu Bilgisi")]
    public string IsRead { get; set; }


    [DisplayName("Custom1")]
    public string Custom1 { get; set; }

    [DisplayName("Custom2")]
    public string Custom2 { get; set; }

    [DisplayName("Custom3")]
    public string Custom3 { get; set; }

    public string MainList{ get; set; }

}
