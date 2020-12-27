namespace ServerBusinessLogic.Interfaces
{
    public interface ISerializer<G>
    {
        G Serialize<T>(T obj) where T : class, new();
        T Deserialize<T>(G obj) where T : class, new();
    }
}
