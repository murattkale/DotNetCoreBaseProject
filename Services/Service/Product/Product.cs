using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Product : BaseModel
{
    public Product()
    {
        MainProducts = new HashSet<Product>();
        OrderDetail = new HashSet<OrderDetail>();
    }
    public virtual ICollection<Product> MainProducts { get; set; }
    public virtual ICollection<OrderDetail> OrderDetail { get; set; }


    public int? MainProductId { get; set; }
    public virtual Product MainProduct { get; set; }

    [DisplayName("Name")]
    public string Name { get; set; }


    [Required]
    public ProductType ProductType { get; set; }

    [NotMapped]
    public string ProductTypeName { get { return ProductType.ExGetDescription(); } }

    [NotMapped]
    public int? Stock { get; set; }

    public string EventPackage { get; set; }
    public string Description { get; set; }

    [DataType("SingleDocument")]
    [DisplayName("Default Image")]
    public string DefaultImage { get; set; }



    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }


    [Required]
    public decimal Price { get; set; }
    public decimal PriceDiscount { get; set; }
    public decimal PriceChip { get; set; }

    [Required]
    public string Currency { get; set; }


    public string BarcodeCode { get; set; }

    public DateTime? CancellationDate { get; set; }
    public DateTime? EditDate { get; set; }


}

public enum ProductType : int
{
    [Description("Event")]
    Event = 1,
    [Description("Product")]
    Product = 2,
    [Description("Workshop")]
    Workshop = 3,
    [Description("Training")]
    Training = 4,
}