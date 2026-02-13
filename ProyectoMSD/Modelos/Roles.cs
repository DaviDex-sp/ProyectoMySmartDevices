using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";

        // Relación inversa: un rol puede tener varios usuarios
        public ICollection<Usuario> usuarios { get; set; } = new List<Usuario>();
    }
}
