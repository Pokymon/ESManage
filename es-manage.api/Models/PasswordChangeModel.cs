using System.ComponentModel.DataAnnotations;

namespace es_manage.api.Models {
    public class PasswordChangeRequestModel {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string? OldPassword { get; set; }
    }
}