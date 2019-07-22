using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HoldPoint
{
    public Transform transform;
    public ushort pointSize;
    public HoldableType type;
    public HoldableScriptable defaultItem;
}

[System.Serializable]
public struct MoveRules
{
    public int maxJump;
    public int maxDrop;
    public bool blocksMovement;
}

[RequireComponent(typeof(BoxCollider))]
public class Entity : MonoBehaviour, Targetable
{
    [Header("Assignments")]
    public Animator animator;
    public SelectionType selectionGroup = SelectionType.all;

    [Header("Basic Stats")]
    public int AP = 10;
    public int MAP = 10;
    public int health = 10;
    public int mana = 10;

    [Header("Actions")]
    public EntityAction[] actions;

    [Header("Movement Settings")]
    public MoveRules rules;

    [Header("Hold points")]
    [SerializeField]
    HoldPoint[] points;
    public Dictionary<HoldPoint, GameObject> holding = new Dictionary<HoldPoint, GameObject>();
    public static List<Entity> entities = new List<Entity>();
    new private BoxCollider collider;

    private NavTile originalTile;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        collider = GetComponent<BoxCollider>();
        entities.Add(this);
        foreach (HoldPoint p in points)
        {
            if (p.defaultItem != null)
            {
                AddItemToHoldpoint(p, p.defaultItem);
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public bool GetNavTile(out NavTile navTile)
    {
        if (LevelManager.GetExistingNavTile(this.GetTile(), out NavTile _target))
        {
            navTile = _target;
            return true;
        }
        navTile = new NavTile();
        return false;
    }

    public bool AddItemToHoldpoint(HoldPoint point, HoldableScriptable holdable)
    {
        RemoveItemInHoldpoint(point);
        if (point.pointSize > holdable.HoldPointSize)
        {
            Debug.Log("Holdable item too large for point.", this);
            return false;
        }
        var i = Instantiate(holdable.Prefab, point.transform);
        holding.Add(point, i);
        return true;
    } 

    public bool HasItemInHoldpoint(HoldPoint point)
    {
        return holding.ContainsKey(point);
    }

    public bool RemoveItemInHoldpoint(HoldPoint point)
    {
        if (HasItemInHoldpoint(point))
        {
            Destroy(holding[point]);
            holding.Remove(point);
            return true;
        }
        return false;
    }

    public void StartWalking(NavTile[] path)
    {
        StartCoroutine(WalkRoutine(path));
    }

    private Vector3 walkVelocity;
    private float time = 0.25f;
    private float directionVelocity;
    private float rotateTime = 0.05f;
    IEnumerator WalkRoutine(NavTile[] path)
    {
        if (animator.GetBool("Walking")) yield break;
        animator.SetBool("Walking", true);

        int index = 0;
        NavTile nextTarget = path[index];
        while (Mathf.Abs((transform.position - path[path.Length - 1].tile.MiddleBase).magnitude) > 0.15f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, nextTarget.tile.MiddleBase, ref walkVelocity, time);
            transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, Quaternion.LookRotation((nextTarget.tile.Middle - transform.position).RemoveAxisAndNormalize(false, true, false), Vector3.up).eulerAngles.y, ref directionVelocity, rotateTime), 0);

            TilePosition testPosition = new TilePosition(transform.position + new Vector3(0, 0.1f, 0));

            if (Mathf.Abs((transform.position - nextTarget.tile.MiddleBase).sqrMagnitude) < 0.15f && index < path.Length - 1) nextTarget = path[++index];

            yield return null;
        }

        TilePosition newPosition = new TilePosition(transform.position + new Vector3(0, 0.1f, 0));
        transform.position = newPosition.MiddleBase;
        transform.rotation = Quaternion.Euler(0, Mathf.Round(transform.rotation.eulerAngles.y / 90) * 90, 0);

        animator.SetBool("Walking", false);
        yield break;
    }

    public void OnTargetted()
    {

    }
}