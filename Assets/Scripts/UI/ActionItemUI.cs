using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionItemUI : MonoBehaviour
{
    [Header("Assignments")]
    [SerializeField]
    private Image image;
    private EntityAction action;

    public EntityAction EntityAction
    {
        get
        {
            return action;
        }
    }

    public void ChangeActionItem(EntityAction _action)
    {
        action = _action;
        image.sprite = _action.picture;
    }

    public void OnButtonPressed()
    {
        InGameUIManager.Instance.ActionPressed(action);
    }
}
