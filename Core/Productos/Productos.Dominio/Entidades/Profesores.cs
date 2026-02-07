namespace RegistroEstudiantesH.Dominio.Entidades
{
    /// <summary>
    /// Modelo de la entidad Profesor
    /// </summary>
    public class Profesores
    {
        /// <summary>
        /// Identificador del profesor
        /// </summary>
        public int ProfesorId { get; set; }

        /// <summary>
        /// Nombre del profesor
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Coleccion de profesorMaterias
        /// </summary>
        public virtual ICollection<ProfesorMaterias>? ProfesorMaterias { get; set; }

        /// <summary>
        /// Coleccion de EstudianteMaterias
        /// </summary>
        public virtual ICollection<EstudianteMaterias>? EstudianteMaterias { get; set; }
    }
}
