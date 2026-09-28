public class ObjetoPerdido
{
    int id;
    private string descripcion;
    private string categoria;
    private string lugarEncontrado;
    private DateTime fechaEncontrado;
    private bool reclamado;
    private string nombrePersonaQueRetiro;

    public int Id { get => id; set => id = value; }
    public string Descripcion { get => descripcion; set => descripcion = value; }
    public string Categoria { get => categoria; set => categoria = value; }
    public string LugarEncontrado { get => lugarEncontrado; set => lugarEncontrado = value; }
    public DateTime FechaEncontrado { get => fechaEncontrado; set => fechaEncontrado = value; }
    public bool Reclamado { get => reclamado; set => reclamado = value; }
    public string NombrePersonaQueRetiro { get => nombrePersonaQueRetiro; set => nombrePersonaQueRetiro = value; }
}