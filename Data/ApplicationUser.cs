using Microsoft.AspNetCore.Identity;

namespace SandStats.Data
{
    public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser
    {
        // Control de acceso
        public bool IsActive { get; set; } = true;     // si false, no puede loguear
        public bool MustResetPassword { get; set; }    // forzá cambio en primer login

        // Identidad y organización (útil para SandStats)
        public string? FullName { get; set; }
        public int? JugadorId { get; set; }            // si lo querés vincular a Jugador
        public Guid? TenantId { get; set; }            // multi-dupla/cliente a futuro

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
    }

}
