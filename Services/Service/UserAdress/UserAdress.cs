using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public partial class UserAdress : BaseModel
{
    public UserAdress()
    {


    }
    [DisplayName("User")]
    [Required]
    public int UserId { get; set; }

    public virtual User User { get; set; }


    public string Def { get; set; }

    public int? CountryId { get; set; }
    public virtual Country Country { get; set; }

    public int? CityId { get; set; }
    public virtual City City { get; set; }

    //public int? TownId { get; set; }
    //public virtual Town Town { get; set; }

    //public int? DistrictId { get; set; }
    //public virtual District District { get; set; }

    public string Town { get; set; }


    public string District { get; set; }


    public string Name { get; set; }
    public string Adress { get; set; }
    public string Tax_Branch { get; set; }
    public string Tax_No { get; set; }

    public bool? IsDefault { get; set; }




}


public enum AddressType : int
{
    AddressType1 = 1,
    AddressType2 = 2,
}

