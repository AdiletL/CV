using UnityEngine;

public interface IFactory
{
    public GameObject Create<T>();
}
