using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarFillByPrefabs : MonoBehaviour
{
    [SerializeField] private RectTransform barRect;
    [SerializeField] private RectTransform container;

    [SerializeField] private Image fullPrefab;
    [SerializeField] private Image emptyPrefab;

    [SerializeField] private int segmentCount = 40;
    [SerializeField] private float paddingLeft = 10f;
    [SerializeField] private float paddingRight = 10f;
    [SerializeField] private float spacing = 2f;
    [SerializeField] private bool autoHeight = true;

    [SerializeField] private FieldValueListenerMB maxHplistener;
    [SerializeField] private FieldValueListenerMB currentHplistener;
    private int maxHP = 0;
    private int currentHP = 0;

    private readonly List<Image> segments = new();

    void Awake()
    {
        Rebuild();
        Refresh(currentHplistener.GetValue<int>(), maxHplistener.GetValue<int>());
    }

    void Update() 
    {
        Refresh(currentHplistener.GetValue<int>(), maxHplistener.GetValue<int>());
    }

    public void Refresh(int current, int max)
    {
        maxHP = Mathf.Max(1, max);
        currentHP = Mathf.Clamp(current, 0, maxHP);

        int filled = Mathf.RoundToInt((float)currentHP / maxHP * segmentCount);
        filled = Mathf.Clamp(filled, 0, segmentCount);

        for (int i = 0; i < segments.Count; i++)
        {
            Image source = i < filled ? fullPrefab : emptyPrefab;
            segments[i].sprite = source.sprite;
            segments[i].color = source.color;
            segments[i].type = source.type;
            segments[i].preserveAspect = source.preserveAspect;
            segments[i].material = source.material;
        }
    }

    public void Rebuild()
    {
        if (barRect == null) 
        {
            barRect = GetComponent<RectTransform>();
        }

        if (container == null)
        {
            container = barRect;
        } 

        Clear();

        float width = barRect.rect.width;
        float height = barRect.rect.height;

        float usable = width - paddingLeft - paddingRight - spacing * (segmentCount - 1);
        float segW = usable / segmentCount;
        if (segW < 1f) segW = 1f;

        float segH = autoHeight ? height : fullPrefab.rectTransform.rect.height;

        for (int i = 0; i < segmentCount; i++)
        {
            Image img = Instantiate(emptyPrefab, container);
            segments.Add(img);

            RectTransform rt = img.rectTransform;
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);

            float x = paddingLeft + i * (segW + spacing);
            rt.anchoredPosition = new Vector2(x, 0f);
            rt.sizeDelta = new Vector2(segW, segH);
        }
    }

    void Clear()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i] != null)
            {
                Destroy(segments[i].gameObject);
            }
        }
        segments.Clear();
    }
}
