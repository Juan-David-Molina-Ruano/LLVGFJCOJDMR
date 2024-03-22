using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Timers;

namespace LLVGFJCOJDMR.Models
{
    public class Customer
    {
        public Customer()
        {
            PhoneNumbers = new List<PhoneNumber>();
        }

        [Key]
        public int Id { get; set;}
      
        public virtual List<PhoneNumber> PhoneNumbers { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "El campo {0} solo puede contener letras")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "El campo {0} solo puede contener letras")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [EmailAddress(ErrorMessage = "El campo {0} no es una dirección de correo válida")]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [NotMapped]//propiedadad para filtro
        public int Take { get; set; }

    }
}
