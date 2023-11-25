using System.ComponentModel.DataAnnotations;

namespace es_manage.api.Models {
    // Membuat class LoginRequestModel untuk menampung request dari login
    public class LoginRequestModel {
        // Username dengan tipe data string, not null, required
        [Required(ErrorMessage = "Username tidak boleh kosong")]
        public string UserName { get; set; }

        // Password dengan tipe data string, not null, required
        [Required(ErrorMessage = "Password tidak boleh kosong")]
        public string Password { get; set; }
    }

    // Membuat class LoginResponseModel untuk menampung response dari login
    public class LoginResponseModel {
        public UserDetail User { get; set; }
        public TokenInfo Token { get; set; }
    }

    public class UserDetail {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
    }

    // Membuat class TokenInfo untuk menampung token
    public class TokenInfo {

        public string AccessToken { get; set; }
    }
}