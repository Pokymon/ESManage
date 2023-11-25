using System.ComponentModel.DataAnnotations;

namespace es_manage.api.Models
{
    public class ItemModel
    {
        public string ID { get; set; }
        public string ItemName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BrandId { get; set; }
        public string? Uom { get; set; }
        public int? TaxType { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? MinimumRetailPrice { get; set; }
        public decimal? BalanceQty { get; set; }
        public decimal? AvgCostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }
}