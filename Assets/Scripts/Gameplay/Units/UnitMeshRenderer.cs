using System;
using UnityEngine;

public class UnitMeshRenderer : MonoBehaviour
{
    [SerializeField] private Renderer renderer;

    private MaterialPropertyBlock propertyBlock;
    private Color baseColor;
    private string colorPropertyName = "_Color";
    
    private void Awake()
    {
        baseColor = renderer.material.color;
        propertyBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propertyBlock);
    }

    public void SetColor(Color color)
    {
        propertyBlock.SetColor(colorPropertyName, color);
        renderer.SetPropertyBlock(propertyBlock);
    }

    public void ResetColor()
    {
        propertyBlock.SetColor(colorPropertyName, baseColor);
        renderer.SetPropertyBlock(propertyBlock);
    }
}
