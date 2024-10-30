using Cysharp.Threading.Tasks;

public interface ISpawner
{
    public void Initialize();
    public UniTask Execute();
}
