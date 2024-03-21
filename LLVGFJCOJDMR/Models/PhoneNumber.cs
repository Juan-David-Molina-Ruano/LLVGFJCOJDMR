using System.ComponentModel.DataAnnotations;

namespace LLVGFJCOJDMR.Models
{
    public class PhoneNumber
    {
        [Key]
        public int Id { get; set; }
        //--------------customer

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;

        //--------------
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "El campo {0} solo puede contener números")]
        public string NumberPhone { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "El campo {0} solo puede contener letras")]
        public string Note { get; set; }

    }
}
