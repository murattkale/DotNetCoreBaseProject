using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public partial class Test : BaseModel
{


    [Required]
    public string Aile { get; set; }


    [Required]
    public int Sayi { get; set; }


}


