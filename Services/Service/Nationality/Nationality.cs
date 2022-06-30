using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Nationality : BaseModel
{
    public string Name { get; set; }
  

    public List<User> User { get; set; } = new List<User>();

}
