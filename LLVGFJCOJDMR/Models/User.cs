using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LLVGFJCOJDMR.Models
{
    public class User
    {

        public byte[] Image { get; set; }
        [Key]
        public int Id { get; set; }
        public int RolId { get; set; }
        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El campo Contraseña es obligatorio.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "El campo Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El campo Email debe ser una dirección de correo electrónico válida.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo Estado es obligatorio.")]
        public int Status { get; set; }

        public virtual Rol Rol { get; set; }
    }
}
