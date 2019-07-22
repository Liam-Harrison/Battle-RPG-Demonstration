using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    [Header("Assignments")]
    public ActionBarUI ActionUIManager;

    private ActionInputType nextNeededInputType;
    private bool enableUI = true;
    private Entity selectedEntity = null;
    private EntityAction selectedAction = null;

    private static InGameUIManager instance;
    public static InGameUIManager Instance
    {
        get => instance;
    }

    public static void RequestInputType(ActionInputType type, params object[] args)
    {
        instance.nextNeededInputType = type;
        if (type == ActionInputType.entitySelect) InputManager.Instance.ReturnNextEntity();
        if (type == ActionInputType.entityAdjacentSelect) InputManager.Instance.PromptEntityAdjacentTileSelection((SelectionType) args[0]);
    }

    public static void RegisterInput(ActionInput input)
    {
        if (instance.selectedAction == null) return;
        if (input.actionType == instance.nextNeededInputType) instance.selectedAction.ProcessInput(instance.selectedEntity, input);
    }

    public static void FinishUpAction()
    {
        if (instance.selectedAction)
        {
            instance.selectedAction.ClearInputBuffer();
            instance.selectedAction = null;
        }
        InputManager.Instance.ClearOptionTiles();
    }

    private void Awake()
    {
        instance = this;
    }

    public bool UIEnabled
    {
        get
        {
            return enableUI;
        }
        set
        {
            enableUI = value;
        }
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if (InputManager.HasSelectedEntity)
        {
            if (selectedEntity == null)
            {
                selectedEntity = InputManager.GetSelectedEntity();
                SetupActionBar(InputManager.GetSelectedEntity());
            }
            else if (selectedEntity != InputManager.GetSelectedEntity())
            {
                selectedEntity = InputManager.GetSelectedEntity();
                SetupActionBar(InputManager.GetSelectedEntity());
            }
            else if (!ActionUIManager.ShowingActionBar)
            {
                SetupActionBar(InputManager.GetSelectedEntity());
            }
        }
        else
        {
            ActionUIManager.HideActionBar();
            FinishUpAction();
        }
    }

    private void SetupActionBar(Entity entity)
    {
        ActionUIManager.SetupActionList(new List<EntityAction>(entity.actions));
    }

    public void ActionHighlighted(EntityAction action)
    {

    }

    public void ActionPressed(EntityAction action)
    {
        action.ClearInputBuffer();
        selectedAction = action;
        selectedAction.ActionSelected(InputManager.GetSelectedEntity());
    }
}
