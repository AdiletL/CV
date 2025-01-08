using UnityEngine;

public class UnitRenderer : MonoBehaviour
{
    [SerializeField] private Renderer[] baseRenderers;

    private MaterialPropertyBlock propertyBlock;
    private Color[] baseColors;
    private string colorPropertyName = "_BaseColor";
    
    private void Awake()
    {
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
}
