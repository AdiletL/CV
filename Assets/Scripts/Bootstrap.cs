using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameInstaller gameInstaller;
    [SerializeField] private Gameplay.Manager.GameManager gameManager;
    [SerializeField] private Laboratory.Manager.LaboratoryManager laboratoryManager;

    private GameInstaller currentGameInstaller;
    
    public void Initialize()
    {
        currentGameInstaller = Instantiate(gameInstaller).GetComponent<GameInstaller>();

    }

    
    public void TransitionToScene(string sceneName)
    {
        CreateManager(sceneName);
        Scenes.TransitionToScene(sceneName);
    }

    private void CreateManager(string sceneName)
    {
        switch (sceneName)
        {
            case Scenes.GAMEPLAY_NAME:
                currentGameInstaller.InstantiatePrefab(gameManager.gameObject).GetComponent<Gameplay.Manager.GameManager>();
                break;
            case Scenes.LABORATORY_NAME:
                currentGameInstaller.InstantiatePrefab(laboratoryManager.gameObject).GetComponent<Laboratory.Manager.LaboratoryManager>();
                break;
        }
    }
}
