using System;
using System.Collections.Generic;
using Gameplay;
using Unit.Character.Player;
using TMPro;
using UnityEngine;

[SelectionBase]
public class Platform : MonoBehaviour
{
    [SerializeField] private TextMeshPro platformText;
    
    private UnitRenderer unitRenderer;
    private Collider[] colliders = new Collider[1];
    
    public Vector2Int CurrentCoordinates { get; private set; }
    

    private void OnEnable()
    {
        PlayerIdleState.ASD += ADS;
    }
    private void OnDisable()
    {
        PlayerIdleState.ASD -= ADS;
    }

    private void Start()
    {
        unitRenderer = GetComponent<UnitRenderer>();
    }
    
    private void ADS()
    {
        unitRenderer.ResetColor();
        platformText.enabled = false;
    }
    
    public void SetColor(Color color)
    {
        unitRenderer.SetColor(color);
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

    public bool IsBlocked()
    {
        var colliderCount = Physics.OverlapSphereNonAlloc(transform.position, .3f, this.colliders, ~Layers.PLATFORM_LAYER);
        if(colliderCount == 0) return false;
        
        for (int i = colliders.Length - 1; i >= 0; i--)
        {
            if (colliders[i].TryGetComponent(out BlockGameObject blockGameObject))
            {
                return blockGameObject.IsBlocked;
            }
        }
        return false;
    }
}