using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item Scriptable/New Item", order = 0)]
public class ItemScriptable : ScriptableObject
{
    [Header("Item Settings")]
    [SerializeField]
    new string name;
    [SerializeField]
    ushort size;
    [SerializeField]
    Texture2D itemPortrait;

    public string Name
    {
        get
        {
            return name;
        }
    }

    public ushort ItemSize
    {
        get
        {
            return size;
        }
    }

    public Texture2D ItemPortrait
    {
        get
        {
            return itemPortrait;
            ;
        }
    }
}
