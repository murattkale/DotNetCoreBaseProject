using System.Collections.Generic;


public partial class Country : BaseModel
{
    public Country()
    {
        BankPrefix = new HashSet<BankPrefix>();
        City = new HashSet<City>();
        UserAdress = new HashSet<UserAdress>();
    }
    public virtual ICollection<BankPrefix> BankPrefix { get; set; }
    public virtual ICollection<City> City { get; set; }
    public virtual ICollection<UserAdress> UserAdress { get; set; }


    public string Name { get; set; }
    public string DoubleCode { get; set; }
    public string ThreeCode { get; set; }
    public string PhoneCode { get; set; }



}

