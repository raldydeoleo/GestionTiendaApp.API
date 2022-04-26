using System;

namespace BoxTrackLabel.API.Repositories
{
    public interface IEntity
    {
        bool EstaBorrado { get; set; }
        DateTime? FechaHoraBorrado { get; set; }
        DateTime? FechaHoraModificacion {get; set;}
        string Codigo { get; set; }
        int Id { get; set; }
    }
}