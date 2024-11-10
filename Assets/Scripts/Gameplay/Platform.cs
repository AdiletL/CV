using System;
using System.Collections.Generic;
using Character.Player;
using TMPro;
using UnityEngine;

[Serializable]
public class Platform : MonoBehaviour
{
    [SerializeField] private MeshRenderer platformRenderer;
    [SerializeField] private TextMeshPro platformText;
    
    [HideInInspector] public bool IsBlocked;

    private List<GameObject> gameObjects = new();
    public Vector2Int CurrentCoordinates { get; private set; }
    
    private Color startColor;

    private void Awake()
    {
        startColor = platformRenderer.material.color;
    }

    private void OnEnable()
    {
        PlayerIdleState.ASD += ADS;
    }
    private void OnDisable()
    {
        PlayerIdleState.ASD -= ADS;
    }

    private void ADS()
    {
        SetColor(startColor);
        platformText.enabled = false;
    }

    private void Start()
    {
        startColor = platformRenderer.material.color;
    }
    
    
    public void SetColor(Color color)
    {
        platformRenderer.material.color = color;
    }

    public void SetText(string weight)
    {
        platformText.text = weight;
        platformText.enabled = true;
    }
    public void SetCoordinates(Vector2Int coordinates)
    {
        CurrentCoordinates = coordinates;
    }

    public bool IsFreeForSpawn()
    {
        return gameObjects.Count == 0;
    }


    public void AddGameObject(GameObject newGO)
    {
        if (!gameObjects.Contains(newGO))
            gameObjects.Add(newGO);
    }

    public void RemoveGameObject(GameObject main)
    {
        if (gameObjects.Contains(main))
            gameObjects.Remove(main);
    }
}