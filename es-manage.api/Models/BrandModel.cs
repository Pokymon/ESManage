using System.ComponentModel.DataAnnotations;

namespace es_manage.api.Models {
    public class BrandModel {
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
    }
}