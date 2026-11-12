namespace Carpo.Structure.Interfaces
{
    public interface IJLayoutCell
    {
        string Row { get; set; }
        string Col { get; set; }
        string Pos { get; set; }
        string Width { get; set; }
        string Height { get; set; }
    }
}