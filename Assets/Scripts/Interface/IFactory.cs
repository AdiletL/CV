using System;

public interface IFactory
{
    public T Create<T>(Type type);
}
