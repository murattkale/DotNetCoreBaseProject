using System.Collections.Generic;

public partial class District : BaseModel
{
    public District()
    {
        UserAdress = new HashSet<UserAdress>();


    }

    public virtual ICollection<UserAdress> UserAdress { get; set; }


    public string Name { get; set; }

    public string NeighName { get; set; }

    public string PostaKodu { get; set; }
    public int? TownId { get; set; }

    public virtual Town Town { get; set; }


}

