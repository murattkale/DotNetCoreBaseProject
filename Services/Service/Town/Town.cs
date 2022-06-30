using System.Collections.Generic;

public partial class Town : BaseModel
{
    public Town()
    {
        District = new HashSet<District>();
        UserAdress = new HashSet<UserAdress>();
    }


    public virtual ICollection<District> District { get; set; }
    public virtual ICollection<UserAdress> UserAdress { get; set; }

    public string Name { get; set; }
    public string CityName { get; set; }

    public int? CityId { get; set; }
    public virtual City City { get; set; }



}

