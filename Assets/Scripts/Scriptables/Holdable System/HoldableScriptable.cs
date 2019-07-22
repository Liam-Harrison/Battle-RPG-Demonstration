using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Holdable", menuName = "New Item Scriptable/New Holdable", order = 1)]
public class HoldableScriptable : ItemScriptable
{

    [Header("Holdable Settings")]
    [SerializeField]
    ushort holdSize;
    [SerializeField]
    HoldableType holdType;
    [SerializeField]
    GameObject prefab;

    public ushort HoldPointSize
    {
        get
        {
            return holdSize;
        }
    }

    public HoldableType HoldType
    {
        get
        {
            return holdType;
        }
    }

    public GameObject Prefab { get => prefab; }
}
