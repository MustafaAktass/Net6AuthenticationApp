using System.ComponentModel.DataAnnotations;

namespace Net6WebApp.Models
{
    public class RegisterViewModel
    {
        //[Display(Name ="Kullanıcı Adı",Prompt ="Kullanıcı Adınız")]Name htmlde ki labeli prompt placeholderın yerini tutar
        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        [StringLength(30, ErrorMessage = "En fazla 30 karakter olabilir")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [MinLength(6, ErrorMessage = "En az 6 karakter olmalıdır.")]
        [MaxLength(15, ErrorMessage = "En fazla 15 karakter olmalıdır.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [MinLength(6, ErrorMessage = "En az 6 karakter olmalıdır.")]
        [MaxLength(15, ErrorMessage = "En fazla 15 karakter olmalıdır.")]
        [Compare(nameof(Password),ErrorMessage ="Şifreler uyuşmuyor")]
        public string RePassword { get; set; }
        [Required(ErrorMessage = "E-posta alanı boş bırakılamaz.")]
        public string Email { get; set; }
    }
}
