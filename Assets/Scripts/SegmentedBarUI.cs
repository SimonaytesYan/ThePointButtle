using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SegmentedBarUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform container;   
    [SerializeField] private Image segmentPrefab;      
    [SerializeField] private Sprite filledSprite;      
    [SerializeField] private Sprite emptySprite;       

    [Header("Scale (fixed UI)")]
    [SerializeField] private int segmentCount = 40;    
    
    [Header("Values (temporary static)")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int currentHP = 75;

    private readonly List<Image> segments = new();

    void Awake()
    {
        BuildSegments();
        Refresh();
    }

    
    public void SetValues(int current, int max)
    {
        maxHP = Mathf.Max(1, max);
        currentHP = Mathf.Clamp(current, 0, maxHP);
        Refresh();
    }

    private void BuildSegments()
    {
        for (int i = container.childCount - 1; i >= 0; i--) 
        {
            Destroy(container.GetChild(i).gameObject);
        }
        
        segments.Clear();

        for (int i = 0; i < segmentCount; i++)
        {
            Image seg = Instantiate(segmentPrefab, container);
            seg.sprite = emptySprite;
            segments.Add(seg);
        }
    }

    private void Refresh()
    {
        if (segments.Count == 0) 
        {
            return;
        }
        
        float t = (float)currentHP / maxHP;                
        int filled = Mathf.RoundToInt(t * segmentCount);    
        filled = Mathf.Clamp(filled, 0, segmentCount);

        for (int i = 0; i < segmentCount; i++)
            segments[i].sprite = (i < filled) ? filledSprite : emptySprite;
    }
}
