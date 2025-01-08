using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameInstaller gameInstaller;
    [SerializeField] private Gameplay.Manager.GameManager gameManager;
    [SerializeField] private string sceneName;
    
    private void Awake()
    {
        var installer = Instantiate(gameInstaller).GetComponent<GameInstaller>();
        installer.InstantiatePrefab(gameManager.gameObject).GetComponent<Gameplay.Manager.GameManager>();
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}
