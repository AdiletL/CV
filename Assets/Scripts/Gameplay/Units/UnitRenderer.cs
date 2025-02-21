using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

public class UnitRenderer : MonoBehaviour
{
    [Inject] private SO_GameConfig so_GameConfig;
    
    [SerializeField] private Renderer[] baseRenderers;

    [Space]
    [SerializeField] private GameObject rangeVisual;

    [Space] 
    [SerializeField] private GameObject selectedObject;

    private Material highlightedMaterial;
    private MaterialPropertyBlock propertyBlock;
    private Color[] baseColors;
    private string colorPropertyName = "_BaseColor";
    
    public void Initialize()
    {
        highlightedMaterial = so_GameConfig.HighlightedMaterial;
        baseColors = new Color[baseRenderers.Length];
        for (int i = 0; i < baseRenderers.Length; i++)
        {
            baseColors[i] = baseRenderers[i].sharedMaterial.GetColor(colorPropertyName);
        }

        propertyBlock = new MaterialPropertyBlock();
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i < baseRenderers.Length; i++)
        {
            propertyBlock.SetColor(colorPropertyName, color);
            baseRenderers[i].SetPropertyBlock(propertyBlock);
        }
    }

    public void ResetColor()
    {
        for (int i = 0; i < baseRenderers.Length; i++)
        {
            propertyBlock.SetColor(colorPropertyName, baseColors[i]);
            baseRenderers[i].SetPropertyBlock(propertyBlock);
        }
    }
    
    
    public void SetRangeScale(float scale) => rangeVisual.transform.localScale = Vector3.one * (scale * 2);
    public void ShowRangeVisual() => rangeVisual.SetActive(true);
    public void HideRangeVisual() => rangeVisual.SetActive(false);

    public void HighlightedObject()
    {
        foreach (var renderer in baseRenderers)
        {
            var materials = renderer.sharedMaterials;
            int length = materials.Length;

            // Проверяем, есть ли уже highlightedMaterial
            for (int i = 0; i < length; i++)
            {
                if (materials[i] == highlightedMaterial)
                    return;
            }

            // Создаем новый массив с дополнительным слотом
            var newMaterials = new Material[length + 1];
            Array.Copy(materials, newMaterials, length);
            newMaterials[length] = highlightedMaterial;
        
            renderer.materials = newMaterials; // Устанавливаем новые материалы
        }
    }

    public void UnHighlightedObject()
    {
        foreach (var renderer in baseRenderers)
        {
            var materials = renderer.sharedMaterials;
            int length = materials.Length;

            int index = -1;
            for (int i = 0; i < length; i++)
            {
                if (materials[i] == highlightedMaterial)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1) return; // Если материала нет, выходим

            // Создаем новый массив на один элемент меньше
            var newMaterials = new Material[length - 1];
            for (int i = 0, j = 0; i < length; i++)
            {
                if (i == index) continue;
                newMaterials[j++] = materials[i];
            }

            renderer.materials = newMaterials;
        }
    }
    
    public void SelectedObject() => selectedObject.SetActive(true);
    public void UnSelectedObject() => selectedObject.SetActive(false);
}
