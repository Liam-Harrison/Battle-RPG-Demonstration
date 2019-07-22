using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Entity Action", menuName = "New Entity Action/Attack Action", order = 1)]
public class MeleeAction : EntityAction
{

    private Entity entityToMove;

    public override void ActionSelected(Entity entity)
    {
        base.ActionSelected(entity);
        if (entity.selectionGroup == SelectionType.friendlies) InGameUIManager.RequestInputType(ActionInputType.entityAdjacentSelect, SelectionType.enemy);
        else if (entity.selectionGroup == SelectionType.enemy) InGameUIManager.RequestInputType(ActionInputType.entityAdjacentSelect, SelectionType.friendlies);
        else InGameUIManager.RequestInputType(ActionInputType.entityAdjacentSelect, SelectionType.all);
    }

    public override void ProcessInput(Entity entity, ActionInput action)
    {
        base.ProcessInput(entity, action);

        if (action.ReturnAsType(out TilePosition target))
        {
            entityToMove = entity;
            LevelManager.RequestPath(new PathRequest(entity.GetTile(), target, entity.rules, OnPathFound));
            InGameUIManager.FinishUpAction();
        }
    }

    private void OnPathFound(PathResult results)
    {
        if (results.succeeded)
        {
            List<NavTile> path = new List<NavTile>(results.path);
            entityToMove.StartWalking(path.ToArray());
        }
    }
}
