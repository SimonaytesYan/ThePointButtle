using UnityEngine;
using System.Reflection;

public class FieldValueListenerMB : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private string componentName;
    [SerializeField] private string memberName;

    Component component;
    FieldInfo field;
    PropertyInfo property;

    void Start()
    {
        if (targetObject == null) 
        {
            return;
        }

        component = targetObject.GetComponent(componentName);
        if (component == null) 
        {
            Debug.Log($"Component {componentName} not found");
            return;
        }

        var type = component.GetType();

        field = type.GetField(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (field == null)
        {
            Debug.Log($"Member {memberName} not found");
            return;
        }
    }

    public object GetValue()
    {
        if (component == null) 
        {
            Debug.Log($"Component {componentName} not found");
            return null;
        }

        if (field != null) 
        {
            Debug.Log($"Member {memberName} not found");
            return field.GetValue(component);
        }

        return null;
    }

    public T GetValue<T>()
    {
        object v = GetValue();
        if (v == null) 
        {
            return default;
        }

        return (T)v;
    }
}
