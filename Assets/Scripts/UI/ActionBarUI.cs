using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarUI : MonoBehaviour
{
    [Header("Prefab Assignments")]
    public GameObject ActionItemPrefab;

    [Header("Assignments")]
    public GameObject animatedParent;

    public void ShowActionBar()
    {
        animatedParent.GetComponent<Animator>().SetBool("ShowingActionBar", true);
    }

    public void HideActionBar()
    {
        animatedParent.GetComponent<Animator>().SetBool("ShowingActionBar", false);
    }

    public bool ShowingActionBar
    {
        get
        {
            return animatedParent.GetComponent<Animator>().GetBool("ShowingActionBar");
        }
    }

    public void SetupActionList(List<EntityAction> actions)
    {
        if (actions.Count == 0) HideActionBar();
        else ShowActionBar();

        if (transform.childCount != actions.Count)
        {
            if (actions.Count > transform.childCount)
            {
                // Need to add children
                while (transform.childCount != actions.Count)
                {
                    Instantiate(ActionItemPrefab, transform);
                }
            }
            else
            {
                // Need to remove children
                int toRemove = transform.childCount - actions.Count;
                for (int i = 0; i < toRemove; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }

        for (int i = 0; i < actions.Count; i++)
        {
            transform.GetChild(i).GetComponent<ActionItemUI>().ChangeActionItem(actions[i]);
        }
    }
}
