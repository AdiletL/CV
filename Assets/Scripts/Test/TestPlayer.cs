using UnityEngine;
using Zenject;

public class TestPlayer : MonoBehaviour, IUnit
{
    [Inject] private GameUnits _gameUnits;
    public bool IsActive;

    public void Appear()
    {
        var players = _gameUnits.GetUnits<TestPlayer>();

        Debug.Log(players.Count);
        foreach (var VARIABLE in players)
        {
            if (VARIABLE != this)
            {
                Debug.Log("!=");
            }
            else
            {
                Debug.Log("==");
            }
        }
    }

    public void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public void Show()
    {
        throw new System.NotImplementedException();
    }

    public void Hide()
    {
        throw new System.NotImplementedException();
    }
}
