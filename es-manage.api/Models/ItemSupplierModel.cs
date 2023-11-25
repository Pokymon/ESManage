using System.ComponentModel.DataAnnotations;

namespace es_manage.api.Models {
    public class ItemSupplierModel {
        public string ID { get; set; } = string.Empty;
        public string ItemId { get; set; }
        public string? SupplierId { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }
}