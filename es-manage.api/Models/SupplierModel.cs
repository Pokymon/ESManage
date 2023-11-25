using System;

namespace es_manage.api.Models{
    public class SupplierModel{
        public string ID { get; set; } = string.Empty;
        public string SupplierName { get; set; }
        public bool Deleted { get; set; } = false;
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
