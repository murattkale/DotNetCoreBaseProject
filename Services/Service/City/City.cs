using System.Collections.Generic;

public partial class City : BaseModel
{
    public City()
    {
        Town = new HashSet<Town>();
        UserAdress = new HashSet<UserAdress>();


    }


    public virtual ICollection<Town> Town { get; set; }
    public virtual ICollection<UserAdress> UserAdress { get; set; }

    public string Name { get; set; }
    public string Plate { get; set; }
    public string PhoneCode { get; set; }

    public int CountryId { get; set; }
    public virtual Country Country { get; set; }



}

