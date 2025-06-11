using ASP_MVC_Prueba.Models;

namespace ASP_MVC_Prueba.Services.Interface
{
    public interface IAmistadService
    {
        Task<bool> EnviarSolicitudAmistadAsync(int usuarioSolicitanteId, int usuarioReceptorId);
        Task<bool> AceptarSolicitudAmistadAsync(int amistadId);
        Task<bool> RechazarSolicitudAmistadAsync(int amistadId);
        Task<bool> BloquearUsuarioAsync(int usuarioId, int usuarioABloquearId);
        Task<List<Usuarios>> ObtenerAmigosAsync(int usuarioId);
        Task<List<Amistad>> ObtenerSolicitudesPendientesAsync(int usuarioId);
        Task<bool> SonAmigosAsync(int usuarioId1, int usuarioId2);
        Task<bool> EliminarAmistadAsync(int usuarioId1, int usuarioId2);
        Task<List<Usuarios>> BuscarUsuariosParaAgregarAsync(int usuarioId, string termino);
    }
}
