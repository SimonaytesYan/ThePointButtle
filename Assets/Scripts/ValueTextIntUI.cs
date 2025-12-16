using UnityEngine;
using TMPro;

public class ValueTexInttUI : MonoBehaviour
{
    [SerializeField] FieldValueListenerMB listener;
    [SerializeField] TMP_Text text;

    void Update()
    {
        if (listener == null || text == null) 
        {
            return;
        }

        int value = listener.GetValue<int>();
        text.text = value.ToString();
    }
}
