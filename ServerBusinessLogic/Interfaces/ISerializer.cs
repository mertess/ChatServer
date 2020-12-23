﻿namespace ServerBusinessLogic.Interfaces
{
    public interface ISerializer
    {
        string Serialize<T>(T obj) where T : class, new();
        T Deserialize<T>(string obj) where T : class, new();
    }
}
