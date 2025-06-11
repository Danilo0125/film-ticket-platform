using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_MVC_Prueba.Models
{
    public class Amistad
    {
        [Key]
        public int AmistadId { get; set; }

        [Required]
        public int UsuarioSolicitanteId { get; set; }

        [Required]
        public int UsuarioReceptorId { get; set; }

        [Required]
        public EstadoAmistad Estado { get; set; } = EstadoAmistad.Pendiente;

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        public DateTime? FechaAceptacion { get; set; }

        // Navigation properties
        [ForeignKey("UsuarioSolicitanteId")]
        public virtual Usuarios UsuarioSolicitante { get; set; }

        [ForeignKey("UsuarioReceptorId")]
        public virtual Usuarios UsuarioReceptor { get; set; }
    }

    public enum EstadoAmistad
    {
        Pendiente,
        Aceptada,
        Rechazada,
        Bloqueada
    }
}
