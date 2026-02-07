namespace RegistroEstudiantesH.Dominio.Entidades
{
    /// <summary>
    /// Modelo que representa una materia
    /// </summary>
    public class Materias
    {
        /// <summary>
        /// Identificador de una materia
        /// </summary>
        public int MateriaId { get; set; }

        /// <summary>
        /// Nombre de la materia
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Creditos de la materia
        /// </summary>
        public int Creditos { get; set; }

        /// <summary>
        /// Coleccion de relacion de ProfesorMateria
        /// </summary>
        public virtual ICollection<ProfesorMaterias>? ProfesorMaterias { get; set; }

        /// <summary>
        /// Coleccion de relacion de estudiantes con materias
        /// </summary>
        public virtual ICollection<EstudianteMaterias>? EstudianteMaterias { get; set; }
    }
}
