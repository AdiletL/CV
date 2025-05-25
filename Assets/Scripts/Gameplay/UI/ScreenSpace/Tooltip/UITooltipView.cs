using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Gameplay.UI.ScreenSpace
{
    public class UITooltipView : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject headerTxtPrefab;
        [SerializeField] private AssetReferenceGameObject descriptionTxtPrefab;
        
        [Space]
        [SerializeField] private AssetReferenceGameObject uiStatTxtPrefab;
        [SerializeField] private AssetReferenceGameObject statsContainerPrefab;

        [Space] 
        [SerializeField] private GameObject parent;
        [SerializeField] private RectTransform spawnParent;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        
        private TextMeshProUGUI headerTxt;
        private TextMeshProUGUI descriptionTxt;
        private Transform statsContainer;

        private Vector2 spawnSizeDelta;

        private Coroutine rebuildNextFrameCoroutine;
        
        private List<TextMeshProUGUI> uiStats;
        
        private TextMeshProUGUI CreateUiStatText()
        {
            var newGameObject = Addressables.InstantiateAsync(uiStatTxtPrefab, statsContainer).WaitForCompletion();
            return newGameObject.gameObject.GetComponent<TextMeshProUGUI>();
        }

        private void Awake()
        {
            spawnSizeDelta = new Vector2(spawnParent.GetComponent<RectTransform>().sizeDelta.x, 0);
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }
        private void SetPosition()
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 localPoint;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                mousePos,
                null,
                out localPoint);

            // Смещение вправо и вниз от курсора
            localPoint += new Vector2(180f, 100f);

            spawnParent.anchoredPosition = GetClampedPosition(localPoint);
        }
        private Vector2 GetClampedPosition(Vector2 position)
        {
            Vector2 size = spawnParent.sizeDelta;

            float padding = 10f;

            float maxX = rectTransform.rect.width / 2f - size.x / 2f - padding;
            float minX = -maxX;

            float maxY = rectTransform.rect.height / 2f - size.y / 2f - padding;
            float minY = -maxY;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            return position;
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 0f;
            RebuildLayoutDelayed();
        }
        private void RebuildLayoutDelayed()
        {
            if(rebuildNextFrameCoroutine != null) StopCoroutine(rebuildNextFrameCoroutine);
            rebuildNextFrameCoroutine = StartCoroutine(RebuildNextFrame());
        }

        private IEnumerator RebuildNextFrame()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(spawnParent);
            yield return null;
            SetPosition();
            canvasGroup.alpha = 1f;
        }
        public void Hide()
        {
            HideHeader();
            HideDescription();
            HideStats();
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            RemoveHeader();
            RemoveDescription();
            RemoveStats();
        }
        
        public void SetHeader(string header)
        {
            if (!headerTxt)
            {
                var result = Addressables.InstantiateAsync(headerTxtPrefab, spawnParent).WaitForCompletion();
                result.GetComponent<RectTransform>().sizeDelta = spawnSizeDelta;
                headerTxt = result.GetComponent<TextMeshProUGUI>();
            }
            headerTxt.text = header;
            ShowHeader();
        }
        private void ShowHeader()
        {
            headerTxt?.gameObject.SetActive(true);
        }
        private void HideHeader()
        {
            if(!headerTxt) return;
            headerTxt.gameObject.SetActive(false);
        }

        private void RemoveHeader()
        {
            if(!headerTxt) return;
            Destroy(headerTxt.gameObject);
        }

        public void SetDescription(string description)
        {
            if (!descriptionTxt)
            {
                var result = Addressables.InstantiateAsync(descriptionTxtPrefab, spawnParent).WaitForCompletion();
                result.GetComponent<RectTransform>().sizeDelta = spawnSizeDelta;
                descriptionTxt = result.GetComponent<TextMeshProUGUI>();
            }
            descriptionTxt.text = description;
            ShowDescription();
        }
        private void ShowDescription()
        {
            descriptionTxt?.gameObject.SetActive(true);
        }
        private void HideDescription()
        {
            if(!descriptionTxt) return;
            descriptionTxt.gameObject.SetActive(false);
        }

        private void RemoveDescription()
        {
            if(!descriptionTxt) return;
            Destroy(descriptionTxt.gameObject);
        }

        public void SetStats(List<string> stats)
        {
            if (!statsContainer)
            {
                var result = Addressables.InstantiateAsync(statsContainerPrefab, spawnParent).WaitForCompletion();
                result.GetComponent<RectTransform>().sizeDelta = spawnSizeDelta;
                statsContainer = result.GetComponent<Transform>();
            }
            
            uiStats ??= new();
            
            for (int i = 0; i < stats.Count; i++)
            {
                if (uiStats.Count <= i)
                    uiStats.Add(CreateUiStatText());
                
                uiStats[i].text = stats[i];
            }

            ShowStats();
        }
        private void ShowStats()
        {
            statsContainer?.gameObject.SetActive(true);
        }
        private void HideStats()
        {
            if(!statsContainer) return;
            statsContainer.gameObject.SetActive(false);
        }
        private void RemoveStats()
        {
            if(!statsContainer) return;
            Destroy(statsContainer.gameObject);
            uiStats?.Clear();
        }
    }
}