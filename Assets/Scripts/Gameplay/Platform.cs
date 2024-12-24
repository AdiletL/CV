using System;
using System.Collections.Generic;
using Unit.Character.Player;
using TMPro;
using UnityEngine;

[Serializable]
public class Platform : MonoBehaviour
{
    [SerializeField] private TextMeshPro platformText;
    
    [HideInInspector] public bool IsBlocked;

    public Vector2Int CurrentCoordinates { get; private set; }
    

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
        GetComponent<UnitMeshRenderer>().ResetColor();
        platformText.enabled = false;
    }
    
    public void SetColor(Color color)
    {
        GetComponent<UnitMeshRenderer>().SetColor(color);
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
        return Physics.OverlapSphere(transform.position, 1, ~Layers.PLATFORM_LAYER).Length == 0;
    }
}