using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class DebugLabel : MonoBehaviour
{
    [Header("Debug Label Settings")]
    [SerializeField]
    protected string label;
    private TMPro.TextMeshProUGUI text;

    private static List<DebugLabel> labels = new List<DebugLabel>();

    public static DebugLabel GetLabel(string name)
    {
        foreach (var i in labels)
        {
            if (i.label == name) return i;
        }
        throw new System.Exception("Cannot find debug label with designation \"" + name + "\".\n" + UnityEngine.StackTraceUtility.ExtractStackTrace());
    }

    /// <summary>
    /// Shorthand for GetLabel(name).SetText(text);
    /// </summary>
    /// <param name="name">The title of the debug label.</param>
    /// <param name="text">The text you want to set.</param>
    public static void SetLabelText(string name, string text)
    {
        GetLabel(name).SetText(text);
    }

    /// <summary>
    /// Shorthand for GetLabel(name).AppendText(text);
    /// </summary>
    /// <param name="name">The title of the debug label.</param>
    /// <param name="text">The text you want to set.</param>
    public static void AppendLabelText(string name, string text)
    {
        GetLabel(name).AppendText(text);
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }

    public void AppendText(string newText)
    {
        text.text += newText;
    }

    public TMPro.TextMeshProUGUI GetTMProComponent()
    {
        return text;
    }
    
    void Awake()
    {
        if (label == "") Debug.LogWarning("Debug text has no label.", this);
        text = GetComponent<TMPro.TextMeshProUGUI>();
        text.text = "";
        labels.Add(this);
    }
    
    void Update()
    {
        
    }
}
