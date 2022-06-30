using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public partial class OrderDetail : BaseModel
{
    public OrderDetail()
    {
    }


    [Required]
    public int OrderId { get; set; }
    public virtual Order Order { get; set; }


    [Required]
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }


    [NotMapped]
    public int? Stock{ get; set; }
    public decimal PriceUnit { get; set; }
    public decimal PriceTotal { get; set; }
    public int OrderCount { get; set; }

    public string Currency { get; set; }

    public string Size { get; set; }
    public string Course { get; set; }
    public string Club { get; set; }
    public string RaceSize { get; set; }

    public string Hotel { get; set; }
    public string Accomodation { get; set; }

    public string RideBack { get; set; }



    [Required]
    public OrderStatus OrderStatus { get; set; }


    [NotMapped]
    public string OrderStatusName { get { return OrderStatus.ExGetDescription(); } }


}

