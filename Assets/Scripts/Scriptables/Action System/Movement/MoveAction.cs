using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Entity Action", menuName = "New Entity Action/Move Action", order = 1)]
public class MoveAction : EntityAction
{
    public uint McostMultiplier = 1;
    private Entity entityToMove;

    public override void ActionSelected(Entity entity)
    {
        base.ActionSelected(entity);
        InGameUIManager.RequestInputType(ActionInputType.clickPosition);
    }

    public override void ProcessInput(Entity entity, ActionInput action)
    {
        base.ProcessInput(entity, action);

        if (action.ReturnAsType(out TilePosition tile))
        {
            entityToMove = entity;
            LevelManager.RequestPath(new PathRequest(entity.GetTile(), tile, entity.rules, OnPathFound));
            InGameUIManager.FinishUpAction();
        }
    }

    private void OnPathFound(PathResult results)
    {
        if (results.succeeded)
        {
            entityToMove.StartWalking(results.path);
        }
    }
}
