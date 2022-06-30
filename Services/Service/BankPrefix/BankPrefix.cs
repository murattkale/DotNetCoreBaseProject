using System;
using System.ComponentModel.DataAnnotations;

public class BankPrefix : BaseModel
{
    [Required]
    public string Prefix { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Organization { get; set; }

    public virtual Country Country { get; set; }
    public int? CountryId { get; set; }
}
