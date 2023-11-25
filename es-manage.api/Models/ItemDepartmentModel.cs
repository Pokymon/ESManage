using System.ComponentModel.DataAnnotations;

namespace es_manage.api.Models {
    public class ItemDepartmentModel {
        public string ID { get; set; } = string.Empty;
        [Key]
        public string CategoryName { get; set; }
        public string? ItemDepartmentParentId { get; set; }
        public bool Deleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
