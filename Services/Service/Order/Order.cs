using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Order : BaseModel
{
    public Order()
    {
        OrderDetail = new HashSet<OrderDetail>();
    }
    public virtual ICollection<OrderDetail> OrderDetail { get; set; }


    [Required]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    public string TransactionID { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }

    public decimal ExchangeRate { get; set; }

    //[NotMapped]
    //public decimal TotalAmountKur { get { return Helpers.ToKur(TotalAmount, "euro"); } }

    public string Currency { get; set; }

    public OrderStatus OrderStatus { get; set; }

    [NotMapped]
    public string OrderStatusName { get { return OrderStatus.ExGetDescription(); } }


    public int? ShippingAddId { get; set; }
    public int? BillingAdressId { get; set; }


    public string BillingAdressNote { get; set; }

    public DateTime? RegistrationDate { get; set; }
    public DateTime? PurchaseDate { get; set; }

    public ConfirmStatus? ConfirmStatus { get; set; }

    [NotMapped]
    public string ConfirmStatusName { get { return ConfirmStatus.ExGetDescription(); } }

    public DateTime? IsSozlesme { get; set; }

    public string ConfirmNote { get; set; }

    public int? MainProductId { get; set; }
    public virtual Product MainProduct { get; set; }


    public int? CouponId { get; set; }
    public virtual Coupon Coupon { get; set; }


}

public enum OrderStatus : int
{
    [Description("Sepette")]
    Sepette = 0,
    [Description("Ödendi")]
    Odendi = 1,
    [Description("İptal")]
    Iptal = 2,
    [Description("İade")]
    Iade = 3,
}


public enum ConfirmStatus : int
{
    [Display(Name = "Beklemede")]
    Beklemede = 1,

    [Display(Name = "Değerlendirmede")]
    Değerlendirmede = 2,

    [Display(Name = "Tamamlandı")]
    Tamamlandı = 3
}