namespace es_manage.api.Models {
    public class ItemSupplier_TransactionModel {
        public string ID { get; set; } = string.Empty;
        public string ItemSupplierId { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public DateTime? TransactionDate { get; set; }
        public decimal Quantity { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool Deleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
    }

    public class ItemSupplier_TransactionPlusItemSupplier
    {
        public string ID { get; set; } = string.Empty;
        public string ItemId { get; set;} = string.Empty;
        public string SupplierId { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public DateTime? TransactionDate { get; set; }
        public decimal Quantity { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool Deleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
    }
}