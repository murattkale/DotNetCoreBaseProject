using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public partial class User : BaseModel
{
    public User()
    {
        ParentUsers = new HashSet<User>();
        UserRoles = new HashSet<UserRole>();
        UserAdress = new HashSet<UserAdress>();
        Order = new HashSet<Order>();

    }
    public virtual ICollection<Order> Order { get; set; }
    public virtual ICollection<UserAdress> UserAdress { get; set; }
    public virtual ICollection<User> ParentUsers { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }

    [DataType("SingleDocument")]
    [DisplayName("Profile Image")]
    public string ProfileImage { get; set; }

    [DisplayName("Parent")]
    public int? ParentUserId { get; set; }

    public virtual User ParentUser { get; set; }

    [DisplayName("UserName")]
    public string UserName { get; set; }

    public string Mail { get; set; }

    [Required]
    public string Pass { get; set; }



    [Required]
    public string Name { get; set; }

    [Required]
    public string Surname { get; set; }

    [NotMapped]
    [DisplayName("Name Surname")]
    public string NameSurname { get { return Name + " " + Surname; } }



    //[Required]
    public Gender Gender { get; set; }

    [NotMapped]
    public string GenderName { get { return Gender.ExGetDescription(); } }

    public string Phone { get; set; }


    [DisplayName("Nationality")]
    public int? NationalityId { get; set; }
    public virtual Nationality Nationality { get; set; }

    public string TCKNPassport { get; set; }



    public DateTime? DateofBirth { get; set; }


    public string ContactEmergency { get; set; }

    public string ContactPhone { get; set; }

    public string LicenceNo { get; set; }



    public UserStatusType? UserStatusType { get; set; }

    [NotMapped]
    public string UserStatusTypeName { get { return UserStatusType.ExGetDescription(); } }

    [DataType("doc")]
    public string HealtReportUrl { get; set; }

    [DisplayName("Tercih Edilen Dil")]
    public int? PreferredLang { get; set; }

    [DisplayName("KVKK Onay")]
    public bool? IsKVKK { get; set; }
}


public enum UserStatusType : int
{
    Active = 1,
    Passive = 2,
    Deleted = 3,
}


public enum Gender : int
{
    [Description("Male")]
    Male = 1,
    [Description("Female")]
    Female = 2,
}