using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionInputType
{
    clickPosition,
    entitySelect,
    entityAdjacentSelect,
    itemSelect,
    confirm,
    unknown
}

public class ActionInput
{
    public ActionInputType actionType;
    public object content;
}

public abstract class EntityAction : ScriptableObject
{

    public string title;
    public string tooltip;
    public uint APcost;
    public Sprite picture;
    private List<ActionInput> inputBuffer = new List<ActionInput>();
    private Dictionary<ActionInputType, int> counts = new Dictionary<ActionInputType, int>();

    public void ClearInputBuffer()
    {
        inputBuffer.Clear();
    }

    public virtual void ActionSelected(Entity entity)
    {
    }

    public virtual void ProcessInput(Entity entity, ActionInput action)
    {
        inputBuffer.Add(action);
    }

    public ActionInput GetNextInputOfType(ActionInputType type)
    {
        if (!counts.ContainsKey(type)) counts.Add(type, 0);
        var x = GetInputWithIndex(type, counts[type]);
        if (x != null) counts[type]++;
        return x;
    }

    public object GetInputOfType(ActionInputType type)
    {
        foreach (var i in inputBuffer)
        {
            if (i.actionType == type)
            {
                return i.content;
            }
        }
        return null;
    }

    public void ResetIterator()
    {
        counts.Clear();
    }

    public ActionInput GetInputWithIndex(ActionInputType type, int toFind)
    {
        int counter = 0;
        foreach (var i in inputBuffer)
        {
            if (i.actionType == type)
            {
                if (counter == toFind)
                {
                    counts[type]++;
                    return i;
                }
                counter++;
            }
        }
        return null;
    }

}

public static class ActionExtenstions
{
    public static bool ReturnAsType<T>(this ActionInput input, out T output)
    {
        if (input.content is T)
        {
            output = (T) input.content;
            return true;
        }
        output = default(T);
        return false;
    }
}
