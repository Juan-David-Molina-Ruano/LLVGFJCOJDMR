using System.ComponentModel.DataAnnotations;

namespace LLVGFJCOJDMR.Models
{
    public class Rol
    {
        public Rol()
        {
            User = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [Display(Name="Nombre")]
        public string Name { get; set; }
        [Required(ErrorMessage = "El campo Descripcion es obligatorio.")]
        [Display(Name ="Descripción")]
        public string Description { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
