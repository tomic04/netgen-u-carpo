namespace Carpo.Structure.Interfaces
{
    public interface IJupiterStart
    {
        void Run(string server, string database, string username, string password, int operateriid, string menuID, int parent, short pTop, short pLeft, string path);
    }
}