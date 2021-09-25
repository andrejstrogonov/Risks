using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLiteLibrary.DataAccess;

namespace SQLiteLibrary.Model
{

    public class Product
    {
        public ProductNullable ProductNullable { get; set; }
        public Product() {
            ProductNullable = new ProductNullable();
        }
        public Product(ProductNullable productNulluble)
        {
            ProductNullable = productNulluble;
        }
        public long ID {
            get
            {
                return ProductNullable.ID;
            }
            set
            {
                ProductNullable.ID = value;
            }
        }

        public string Name {
            get
            {
                return ProductNullable.Name;
            }
            set
            {
                ProductNullable.Name = value;
            }
        }

        public bool IsInteger {
            get
            {
                if (ProductNullable.IsInteger is null)
                    return false;
                return (bool)ProductNullable.IsInteger;
            }
            set
            {
                ProductNullable.IsInteger = value;
            }
        }
        public double Minimum {
            get
            {
                if (ProductNullable.Minimum is null)
                    return 0;
                return (double)ProductNullable.Minimum;
            }
            set
            {
                ProductNullable.Minimum = value;
            }
        }

        public long UnitOfMimimumID {
            get
            {
                if (ProductNullable.UnitOfMimimumID is null)
                    return UnitDAL.GetUnitsNotMoney().First().ID;
                return (long)ProductNullable.UnitOfMimimumID;
            }
            set
            {
                ProductNullable.UnitOfMimimumID = value;
            }
        }
        public Unit UnitOfMinimum{
            get
            {
                return ProductNullable.UnitOfMinimum;
            }
            set
            { ProductNullable.UnitOfMinimum = value; }
        }

        public double Maximum {
            get
            {
                if (ProductNullable.Maximum is null)
                    return 0;
                return (double)ProductNullable.Maximum;
            }
            set
            {
                ProductNullable.Maximum = value;
            }
        }

        public long UnitOfMaximumID {
            get
            {
                if (ProductNullable.UnitOfMaximumID is null)
                    return UnitDAL.GetUnitsNotMoney().First().ID;
                return (long)ProductNullable.UnitOfMaximumID;
            }
            set
            {
                ProductNullable.UnitOfMaximumID = value;
            }
        }
        public Unit UnitOfMaximum {
            get
            {
                return ProductNullable.UnitOfMaximum;
            }
            set
            { ProductNullable.UnitOfMaximum = value; }
        }

        public double Stock
        {
            get
            {
                if (ProductNullable.Stock is null)
                    return 0;
                return (double)ProductNullable.Stock;
            }
            set
            {
                ProductNullable.Stock = value;
            }
        }

        public long UnitOfStockID {
            get
            {
                if (ProductNullable.UnitOfStockID is null)
                    return UnitDAL.GetUnitsNotMoney().First().ID;
                return (long)ProductNullable.UnitOfStockID;
            }
            set
            {
                ProductNullable.UnitOfStockID = value;
            }
        }
        public Unit UnitOfStock {
            get
            {
                return ProductNullable.UnitOfStock;
            }
            set
            { ProductNullable.UnitOfStock = value; }
        }

        public bool Perishable {
            get
            {
                if (ProductNullable.Perishable is null)
                    return false;
                return (bool)ProductNullable.Perishable;
            }
            set
            {
                ProductNullable.Perishable = value;
            }
        }//скоропортящийся

        public double Rate {
            get
            {
                if (ProductNullable.Rate is null)
                    return 0;
                return (double)ProductNullable.Rate;
            }
            set
            {
                ProductNullable.Rate = value;
            }
        }//ставка, для налогов

        public double OtherCosts {
            get
            {
                if (ProductNullable.OtherCosts is null)
                    return 0;
                return (double)ProductNullable.OtherCosts;
            }
            set
            {
                ProductNullable.OtherCosts = value;
            }
        }//прочие расходы


        public long UnitOfOtherID {
            get
            {
                if (ProductNullable.UnitOfOtherID is null)
                    return UnitDAL.GetUnitsNotMoney().First().ID;
                return (long)ProductNullable.UnitOfOtherID;
            }
            set
            {
                ProductNullable.UnitOfOtherID = value;
            }
        }
        public Unit UnitOfOther {
            get
            {
                return ProductNullable.UnitOfOther;
            }
            set
            {
                ProductNullable.UnitOfOther = value;
            }
        }



    }

    public class ProductNullable
    {
        public ProductNullable() { }
        [Key]
        public long ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public bool? IsInteger { get; set; }
        public double? Minimum { get; set; }

        [ForeignKey("UnitOfMinimum")]
        public long? UnitOfMimimumID { get; set; }
        public Unit UnitOfMinimum { get; set; }

        public double? Maximum { get; set; }

        [ForeignKey("UnitOfMaximum")]
        public long? UnitOfMaximumID { get; set; }
        public Unit UnitOfMaximum { get; set; }

        public double? Stock { get; set; }

        [ForeignKey("UnitOfStock")]
        public long? UnitOfStockID { get; set; }
        public Unit UnitOfStock { get; set; }

        public bool? Perishable { get; set; }//скоропортящийся

        public double? Rate { get; set; }//ставка, для налогов

        public double? OtherCosts { get; set; }//прочие расходы

        [ForeignKey("UnitOfOther")]
        public long? UnitOfOtherID { get; set; }
        public Unit UnitOfOther { get; set; }
        public List<ResourceOfProductNullable> ResourcesOfProduct { get; set; }
        public List<RiskOfProductNullable> RisksOfProduct { get; set; }
        public List<MemberOfGroupNullable> MembersOfGroup { get; set; }
    }
    
    public enum TypeOfUnit
    {
        Money,
        Mass,
        Volume,
        Unit,
        Time,
        Human,
        Human_Hour,
        Custom
    }

    public class UnitSequence
    {
        [Key]
        public long ID { get; set; }

        public string Name { get; set; }
        public bool IsUsing { get; set; }

        public TypeOfUnit TypeOfUnit { get; set; }

        [InverseProperty("UnitSequence")]
        public List<Unit> Units { get; set; }
    }

    public class Unit
    {
        [Key]
        public long ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Symbol { get; set; }
        public bool IsBasis { get; set; }

        [ForeignKey("UnitSequence")]
        public long UnitSequence_ID { get; set; }
        public UnitSequence UnitSequence { get; set; }

        [InverseProperty("UnitOfMaximum")]
        public List<ResourceNullable> ResourcesMaximum { get; set; }
        [InverseProperty("UnitOfStock")]
        public List<ResourceNullable> ResourcesStock { get; set; }
        [InverseProperty("UnitOfPrice")]
        public List<ResourceNullable> ResourcesPrice { get; set; }
        [InverseProperty("UnitOfMaximum")]
        public List<ProductNullable> ProductsMaximum { get; set; }
        [InverseProperty("UnitOfStock")]
        public List<ProductNullable> ProductsStock { get; set; }
        [InverseProperty("UnitOfOther")]
        public List<ProductNullable> ProductsOther { get; set; }
        [InverseProperty("UnitOfMinimum")]
        public List<ProductNullable> ProductsMinimum { get; set; }
        [InverseProperty("UnitFrom")]
        public List<TransferOfUnits> TransferOfUnitsFrom { get; set; }
        [InverseProperty("UnitTo")]
        public List<TransferOfUnits> TransferOfUnitsTo { get; set; }
        [InverseProperty("UnitOfMinimum")]
        public List<GroupOfInterchangableNullable> GroupOfInterchangableMinimum { get; set; }
        [InverseProperty("UnitOfMaximum")]
        public List<GroupOfInterchangableNullable> GroupOfInterchangableMaximum { get; set; }
        [InverseProperty("UnitOfPrice")]
        public List<RiskOfProductNullable> RiskOfProductsPrice { get; set; }
        [InverseProperty("UnitOfAmount")]
        public List<RiskOfProductNullable> RiskOfProductsAmount { get; set; }

        [InverseProperty("PeriodUnit")]
        public List<Settings> PeriodUnits { get; set; }

        [InverseProperty("BudgetUnit")]
        public List<Settings> BudgetUnits { get; set; }

        public List<ResourceOfProductNullable> ResourceOfProducts { get; set; }
        public List<FixedCost> FixedCosts { get; set; }
        public List<GroupOfInterchangableNullable> GroupOfInterchangables { get; set; }
    }

    public class TransferOfUnits
    {
        [Key, Column(Order =0), ForeignKey("UnitFrom")]
        public long ID_UnitFrom { get; set; }
        public Unit UnitFrom { get; set; }

        [Key, Column(Order =1), ForeignKey("UnitTo")]
        public long ID_UnitTo { get; set; }
        public Unit UnitTo { get; set; }

        public double Coefficient { get; set; }
    }

    public class Resource
    {

        public ResourceNullable ResourceNullable { get; set; }
        public Resource()
        {
            ResourceNullable = new ResourceNullable();
        }

        public Resource(ResourceNullable resourceNullable)
        {
            ResourceNullable = resourceNullable;
        }

        public long ID { get
            {
                return ResourceNullable.ID;
            }
            set
            {
                ResourceNullable.ID = value;
            }
        }

        public string Name { get
            {
                return ResourceNullable.Name;
            }
            set
            {
                ResourceNullable.Name = value;
            }
        }


        public long UnitOfMaximumID { get
            {
                if (ResourceNullable.UnitOfMaximumID is null)
                    return UnitDAL.GetUnitsNotMoney().First().ID;
                return (long)ResourceNullable.UnitOfMaximumID;
            }
            set
            {
                ResourceNullable.UnitOfMaximumID = value;
            }
        }

        public Unit UnitOfMaximum { get { return ResourceNullable.UnitOfMaximum; } set { ResourceNullable.UnitOfMaximum = value; } }

        public double Maximum {
            get
            {
                if (ResourceNullable.Maximum is null)
                    return 0;
                return (double)ResourceNullable.Maximum;
            }
            set
            {
                ResourceNullable.Maximum = value;
            }
        }
     
        public double Price {
            get
            {
                if (ResourceNullable.Price is null)
                    return 0;
                return (double)ResourceNullable.Price;
            }
            set
            {
                ResourceNullable.Price = value;
            }
        }
        

        public long UnitOfPriceID { 
            get
            {
                    if (ResourceNullable.UnitOfPriceID is null)
                        return UnitDAL.GetUnitsMoney().First().ID;
                    return (long)ResourceNullable.UnitOfPriceID;
                }
                set
            {
                    ResourceNullable.UnitOfPriceID = value;
                }
            }
        public Unit UnitOfPrice
        {
            get
            {
                return ResourceNullable.UnitOfPrice;
            }
            set
            {
                ResourceNullable.UnitOfPrice = value;
            }
        }
       
        public double Stock {
            get
            {
                if (ResourceNullable.Stock is null)
                    return 0;
                return (double)ResourceNullable.Stock;
            }
            set
            {
                ResourceNullable.Stock = value;
            }
        }

        public long UnitOfStockID {
            get
            {
                if (ResourceNullable.UnitOfStockID is null)
                    return UnitDAL.GetUnitsNotMoney().First().ID;
                return (long)ResourceNullable.UnitOfStockID;
            }
            set
            {
                ResourceNullable.UnitOfStockID = value;
            }
        }
        public Unit UnitOfStock {
            get
            {
                return ResourceNullable.UnitOfStock;
            }
            set
            {
                ResourceNullable.UnitOfStock = value;
            }
        }
  
        public bool IsInteger {
            get
            {
                if (ResourceNullable.IsInteger is null)
                    return false;
                return (bool)ResourceNullable.IsInteger;
            }
            set
            {
                ResourceNullable.IsInteger = value;
            }
        }

  
        public bool Perishable {
            get
            {
                if (ResourceNullable.Perishable is null)
                    return false;
                return (bool)ResourceNullable.Perishable;
            }
            set
            {
                ResourceNullable.Perishable = value;
            }
        }

        public double InputTax {
            get
            {
                if (ResourceNullable.InputTax is null)
                    return 0;
                return (double)ResourceNullable.InputTax;
            }
            set
            {
                ResourceNullable.InputTax = value;
            }
        }//входной ндс
    }

    public class ResourceNullable
    {
        public ResourceNullable() { }

        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [ForeignKey("UnitOfMaximum")]
        public long? UnitOfMaximumID { get; set; }

        public Unit UnitOfMaximum { get; set; }

        public double? Maximum { get; set; }

        public double? Price { get; set; }

        [ForeignKey("UnitOfPrice")]
        public long? UnitOfPriceID { get; set; }
        public Unit UnitOfPrice { get; set; }

        public double? Stock { get; set; }

        [ForeignKey("UnitOfStock")]
        public long? UnitOfStockID { get; set; }
        public Unit UnitOfStock { get; set; }

        public bool? IsInteger { get; set; }


        public bool? Perishable { get; set; }

        public double? InputTax { get; set; }//входной ндс
        public List<ResourceOfProductNullable> ResourceOfProducts { get; set; }
    }

    public class ResourceOfProduct
    {
        public ResourceOfProduct()
        {
            ResourceOfProductNullable = new ResourceOfProductNullable();
        }
        public ResourceOfProduct(ResourceOfProductNullable resourceOfProductNullable)
        {
            ResourceOfProductNullable = resourceOfProductNullable;
        }
        public ResourceOfProductNullable ResourceOfProductNullable { get; set; }

        public long ID
        {
            get
            {
                return ResourceOfProductNullable.ID;
            }
            set
            {
                ResourceOfProductNullable.ID = value;
            }
        }

        public long ID_Product {
            get
            {
                return ResourceOfProductNullable.ID_Product;
            }
            set
            {
                ResourceOfProductNullable.ID_Product = value;
            }
        }
        public Product Product {
            get
            {
                if (ResourceOfProductNullable.ProductNullable is null)
                    return null;
                return new Product(ResourceOfProductNullable.ProductNullable);
            }
            set
            {
                if(value is null)
                {
                    ResourceOfProductNullable.ProductNullable = null;
                }
                else
                ResourceOfProductNullable.ProductNullable = value.ProductNullable;
            }
        }

        public long ID_Resource {
            get
            {
                return ResourceOfProductNullable.ID_Resource;
            }
            set
            {
                ResourceOfProductNullable.ID_Resource = value;
            }
        }
        public Resource Resource {
            get
            {
                if (ResourceOfProductNullable.ResourceNullable is null)
                    return null;
                return new Resource(ResourceOfProductNullable.ResourceNullable);
            }
            set
            {
                if (value is null)
                {
                    ResourceOfProductNullable.ResourceNullable = null;
                }
                else
                ResourceOfProductNullable.ResourceNullable = value.ResourceNullable;
            }
        }

        public double AmountOfResource {
            get
            {
                if (ResourceOfProductNullable.AmountOfResource is null)
                    return 0;
                return (double)ResourceOfProductNullable.AmountOfResource;
            }
            set
            {
                ResourceOfProductNullable.AmountOfResource = value;
            }
        }

        public long UnitOfResourceID {
            get
            {
                if (ResourceOfProductNullable.UnitOfResourceID is null)
                    return UnitDAL.GetUnitsNotMoney().First().ID;
                return (long)ResourceOfProductNullable.UnitOfResourceID;
            }
            set
            {
                ResourceOfProductNullable.UnitOfResourceID = value;
            }
        }
        public Unit UnitOfResource {
            get
            {
                return ResourceOfProductNullable.UnitOfResource;
            }
            set
            {
                ResourceOfProductNullable.UnitOfResource = value;
            }
        }
    }

    public class ResourceOfProductNullable
    {

        [Key]
        public long ID { get; set; }
        [ForeignKey("ProductNullable")]
        public long ID_Product { get; set; }
        public ProductNullable ProductNullable { get; set; }

        [ForeignKey("ResourceNullable")]
        public long ID_Resource { get; set; }
        public ResourceNullable ResourceNullable { get; set; }

        public double? AmountOfResource { get; set; }

        [ForeignKey("UnitOfResource")]
        public long? UnitOfResourceID { get; set; }
        public Unit UnitOfResource { get; set; }

        public ResourceOfProductNullable() { }
    }

    public class RiskOfProduct
    {
        public RiskOfProduct()
        {
            RiskOfProductNullable = new RiskOfProductNullable();
        }
        public RiskOfProduct(RiskOfProductNullable riskOfProductNullable)
        {
            RiskOfProductNullable = riskOfProductNullable;
        }
        public RiskOfProductNullable RiskOfProductNullable { get; set; }

        public long ID {
            get
            {
                return RiskOfProductNullable.ID; }
            set
            {
                RiskOfProductNullable.ID = value;
            }
        }


        public long ID_Product { get
            {
                return RiskOfProductNullable.ID_Product;
            }
            set
            {
                RiskOfProductNullable.ID_Product = value;
            }
        }

        public Product Product { get
            {
                if (RiskOfProductNullable.ProductNullable is null)
                    return null;
                return new Product(RiskOfProductNullable.ProductNullable);
            }
            set
            {
                RiskOfProductNullable.ProductNullable = value.ProductNullable;
            }
        }

        public double Price { get
            {
                if (RiskOfProductNullable.Price is null)
                    return 0;
                return (double)RiskOfProductNullable.Price;
            }
            set
            {
                RiskOfProductNullable.Price = value;
            }
        }

        public long UnitOfPriceID {
            get
            {
                if (RiskOfProductNullable.UnitOfPriceID is null)
                    return UnitDAL.GetUnitsMoney().First().ID;
                return (long)RiskOfProductNullable.UnitOfPriceID;
            }
            set
            {
                RiskOfProductNullable.UnitOfPriceID = value;
            }
        }
        public Unit UnitOfPrice { get
            {
                return RiskOfProductNullable.UnitOfPrice;
            }
            set
            {
                RiskOfProductNullable.UnitOfPrice = value;
            }
        }

        public double GarantedAmount { get
            {
                if (RiskOfProductNullable.GarantedAmount is null)
                    return 0;
                return (double)RiskOfProductNullable.GarantedAmount;
            }
            set
            {
                RiskOfProductNullable.GarantedAmount = value;
            }
        }
        public double MaximumAmount { get
            {
                if (RiskOfProductNullable.MaximumAmount is null)
                    return 0;
                return (double)RiskOfProductNullable.MaximumAmount;
            }
            set
            {
                RiskOfProductNullable.MaximumAmount = value;
            }
        }
        public long UnitOfAmountID {
            get
            {
                if (RiskOfProductNullable.UnitOfAmountID is null)
                    return UnitDAL.GetUnitsNotMoney().First().ID;
                return (long)RiskOfProductNullable.UnitOfAmountID;
            }
            set
            {
                RiskOfProductNullable.UnitOfAmountID = value;
            }
        }
        public Unit UnitOfAmount {
            get
            {
                return RiskOfProductNullable.UnitOfAmount;
            }
            set
            {
                RiskOfProductNullable.UnitOfAmount = value;
            }
        }
    }

    public class RiskOfProductNullable
    {
        [Key]
        public long ID { get; set; }

        [ForeignKey("ProductNullable")]
        public long ID_Product { get; set; }

        public ProductNullable ProductNullable { get; set; }

        public double? Price { get; set; }

        [ForeignKey("UnitOfPrice")]
        public long? UnitOfPriceID { get; set; }
        public Unit UnitOfPrice { get; set; }

        public double? GarantedAmount { get; set; }
        public double? MaximumAmount { get; set; }
        [ForeignKey("UnitOfAmount")]
        public long? UnitOfAmountID { get; set; }
        public Unit UnitOfAmount { get; set; }

        public RiskOfProductNullable() { }
    }

    public enum TypeOfFixedCosts
    {
        incoming,
        outcoming,
        admortization
    }


    public class FixedCost
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }

        [ForeignKey("UnitOfPrice")]
        public long? UnitOfPriceID { get; set; }
        public virtual Unit UnitOfPrice { get; set; }

        public bool? IsDecrease { get; set; }

        public TypeOfFixedCosts TypeOfFixedCosts { get; set; }
    }

    public class GroupOfInterchangable
    {
        public GroupOfInterchangableNullable GroupOfInterchangableNullable { get; set; }
        public GroupOfInterchangable()
        {
            GroupOfInterchangableNullable = new GroupOfInterchangableNullable();
        }
        public GroupOfInterchangable(GroupOfInterchangableNullable groupOfInterchangableNullable)
        {
            GroupOfInterchangableNullable = groupOfInterchangableNullable;
        }

        public long ID {
            get
            {
                return GroupOfInterchangableNullable.ID;
            }
            set
            {
                GroupOfInterchangableNullable.ID = value;
            }
        }
     
        public string Name
        {
            get
            {
                return GroupOfInterchangableNullable.Name;
            }
            set
            {
                GroupOfInterchangableNullable.Name = value;
            }
        }

        public long UnitOfMaximumID {
            get
            {
                if (GroupOfInterchangableNullable.UnitOfMaximumID is null)
                    return UnitDAL.GetUnitsNotMoney().First().ID;
                return (long)GroupOfInterchangableNullable.UnitOfMaximumID;
            }
            set
            {
                GroupOfInterchangableNullable.UnitOfMaximumID = value;
            }
        }

        public Unit UnitOfMaximum { get
            {
                return GroupOfInterchangableNullable.UnitOfMaximum;
            }
            set
            {
                GroupOfInterchangableNullable.UnitOfMaximum = value;
            }
        }
     
        public double Maximum {
            get
            {
                if (GroupOfInterchangableNullable.Maximum is null)
                    return 0;
                return (double)GroupOfInterchangableNullable.Maximum;
            }
            set
            {
                GroupOfInterchangableNullable.Maximum = value;
            }
        }

    
        public long UnitOfMinimumID {
            get
            {
                if (GroupOfInterchangableNullable.UnitOfMinimumID is null)
                    return UnitDAL.GetUnitsNotMoney().First().ID;
                return (long)GroupOfInterchangableNullable.UnitOfMinimumID;
            }
            set
            {
                GroupOfInterchangableNullable.UnitOfMinimumID = value;
            }
        }
     
        public Unit UnitOfMinimum {
            get
            {
                return GroupOfInterchangableNullable.UnitOfMinimum;
            }
            set
            {
                GroupOfInterchangableNullable.UnitOfMinimum = value;
            }
        }
     
        public double Minimum {
            get
            {
                if (GroupOfInterchangableNullable.Minimum is null)
                    return 0;
                return (double)GroupOfInterchangableNullable.Minimum;
            }
            set
            {
                GroupOfInterchangableNullable.Minimum = value;
            }
        }
    }

    public class GroupOfInterchangableNullable
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(50)]

        public string Name { get; set; }

        [ForeignKey("UnitOfMaximum")]
        public long? UnitOfMaximumID { get; set; }

        public Unit UnitOfMaximum { get; set; }

        public double? Maximum { get; set; }

        [ForeignKey("UnitOfMinimum")]
        public long? UnitOfMinimumID { get; set; }

        public Unit UnitOfMinimum { get; set; }

        public double? Minimum { get; set; }

        public List<MemberOfGroupNullable> MembersOfGroup { get; set; }
    }


    public class MemberOfGroup
    {
        public MemberOfGroupNullable MemberOfGroupNullable { get; set; }
        public MemberOfGroup()
        {
            MemberOfGroupNullable = new MemberOfGroupNullable();
        }
        public MemberOfGroup(MemberOfGroupNullable memberOfGroupNullable)
        {
            MemberOfGroupNullable = memberOfGroupNullable;
        }
        public long ID_Product {
            get
            {
                if (MemberOfGroupNullable.ID_Product is null)
                    return ProductDAL.GetProducts().First().ID;
                return (long)MemberOfGroupNullable.ID_Product;
            }
            set
            {
                MemberOfGroupNullable.ID_Product = value;
            }
        }
        public Product Product {
            get
            {
                if (MemberOfGroupNullable.ProductNullable is null)
                    return null;
                return new Product(MemberOfGroupNullable.ProductNullable);
            }
            set
            {
                if(value is null)
                {
                    MemberOfGroupNullable.ProductNullable = null;
                }
                else
                MemberOfGroupNullable.ProductNullable = value.ProductNullable;
            }
        }


        public long ID_GroupOfInterchangable {
            get
            {
                if (MemberOfGroupNullable.ID_GroupOfInterchangable is null)
                    return GroupDAL.GetGroups().First().ID;
                return (long)MemberOfGroupNullable.ID_GroupOfInterchangable;
            }
            set
            {
                MemberOfGroupNullable.ID_GroupOfInterchangable = value;
            }
        }
        public GroupOfInterchangable GroupOfInterchangable {
            get
            {
                return new GroupOfInterchangable(MemberOfGroupNullable.GroupOfInterchangableNullable);
            }
            set
            {
                if(value is null)
                {
                    MemberOfGroupNullable.GroupOfInterchangableNullable = null;
                }
                else
                MemberOfGroupNullable.GroupOfInterchangableNullable = value.GroupOfInterchangableNullable;
            }
        }
    }
    public class MemberOfGroupNullable
    {
        [Key, Column(Order = 0), ForeignKey("ProductNullable")]
        public long? ID_Product { get; set; }
        public ProductNullable ProductNullable { get; set; }

        [Key, Column(Order = 1), ForeignKey("GroupOfInterchangableNullable")]
        public long? ID_GroupOfInterchangable { get; set; }
        public GroupOfInterchangableNullable GroupOfInterchangableNullable { get; set; }
    }



    public enum VATType
    {
        main_6,
        main_15,
        full,
        None
    }


    public class Settings
    {
        [Key]
        public int ID { get; set; }
        public VATType VAT_type { get; set; }
        public double Budget { get; set; }

        public DateTime DateOfCreation { get; set; }

        [ForeignKey("BudgetUnit")]
        public long? BudgetUnitID { get; set; }
        public virtual Unit BudgetUnit { get; set; }


        [ForeignKey("PeriodUnit")]
        public long? PeriodUnitID { get; set; }
        public virtual Unit PeriodUnit { get; set; }
        
        public double Period { get; set; }

        public int Level { get; set; }
    }



}
