using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public partial class BankParameter : BaseModel
{
    public BankParameter(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public int BankId { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }

    public Bank Bank { get; set; }

}
