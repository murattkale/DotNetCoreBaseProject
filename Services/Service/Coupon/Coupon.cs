using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public partial class Coupon : BaseModel
{
    public Coupon()
    {
        Order = new HashSet<Order>();
    }
    public virtual ICollection<Order> Order { get; set; }


    [Required]
    public string Name { get; set; }

    public CouponType CouponType { get; set; }

    [NotMapped]
    public string CouponTypeName { get { return CouponType.ExGetDescription(); } }

    [NotMapped]
    public List<EnumModel> CouponTypeList = Enum.GetValues(typeof(CouponType)).Cast<int>().Select(x => new EnumModel { value = x.ToString(), name = ((CouponType)x).ToStr(), text = ((CouponType)x).ExGetDescription() }).ToList();


    public decimal CouponValue { get; set; }

    public decimal MinBasket { get; set; }


    public string Currency { get; set; }
    public int? Limit { get; set; }
    public int? Used { get; set; }


    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }



}

public enum CouponType : int
{
    [Description("Tutar")]
    Tutar = 0,
    [Description("Oran")]
    Oran = 1,
}

