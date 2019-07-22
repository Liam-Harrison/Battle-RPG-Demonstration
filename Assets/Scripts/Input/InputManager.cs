using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [Header("Assignments")]
    [SerializeField]
    new private Camera camera;
    [SerializeField]
    private Transform gridTransform;
    [SerializeField]
    private GameObject gridSelectionPrefab;
    [SerializeField]
    private GameObject gridHighlightPrefab;
    [SerializeField]
    private GameObject gridOptionPrefab;

    [Header("Double Click Settings")]
    public float clickTime = 0.5f;
    public float cooldownTime = 0.75f;

    [Header("Movement Settings")]
    public Vector3 targetPosition;
    private Vector3 currentPosition;
    private Vector3 velcoityPosition;
    public float positionTime = 0.1f;

    [Header("Rotation Settings")]
    public float targetYaw;

    public float targetPitch;
    public float minPitch;
    public float maxPitch;
    private float yawVelocity;
    private float pitchVelocity;
    public float rotationTime = 0.1f;

    [Header("Zoom Settings")]
    [Range(5, 25)]
    public float targetZoom = 10;
    private float zoomVelocity;
    private float currentZoom;
    public float zoomTime = 0.15f;
    public float minZoom = 5;
    public float maxZoom = 25;

    private bool mouseScrollBegun = false;

    private GameObject gridObject;
    private TilePosition gridPosition;
    private GameObject selectedGridObject;
    private Entity selected;
    private int nextSelectedUnit = 0;
    private float lastClick, lastDoubleClick;
    
    private bool returnOnNext = false;

    public void ReturnNextEntity()
    {
        returnOnNext = true;
    }

    private bool returnNextOption = false;

    struct OptionTarget
    {
        public TilePosition option;
        public Targetable target;
    }

    public void PromptEntityAdjacentTileSelection(SelectionType type = SelectionType.all)
    {
        returnNextOption = true;
        List<OptionTarget> tiles = new List<OptionTarget>();
        foreach (var i in Entity.entities)
        {
            if (i.GetNavTile(out NavTile t)) {
                var x = LevelManager.GetAdjacentNavTiles(t, 0.5f);
                foreach (var j in x)
                {
                    foreach (var a in tiles)
                    {
                        if (a.option == j.tile) continue;
                    }
                    if ((i.selectionGroup == type || type == SelectionType.all) && j.tile.GetEntity() == null) tiles.Add(new OptionTarget { option = j.tile, target = i });
                }
            }
        }
        DrawOptionTiles(tiles);
    }

    public void CancelInputRequest()
    {
        returnNextOption = false;
        returnOnNext = false;
    }

    private static InputManager instance;
    public static InputManager Instance
    {
        get => instance;
    }

    public static void FocusTargetTile(TilePosition tile)
    {
        instance.FocusTile(tile);
    }

    public static void Select(Entity entity)
    {
        instance.selected = entity;
    }

    public static TilePosition GetSelectedTile()
    {
        return instance.selected.GetTile();
    }

    public static bool HasSelectedTile
    {
        get
        {
            return instance.selected != null;
        }
    }

    public static Entity GetSelectedEntity()
    {
        return instance.selected;
    }
    
    public static bool HasSelectedEntity
    {
        get
        {
            return instance.selected != null;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
    }
    
    void DrawHighlightTile()
    {
        if (selected != null) { 
            if (selectedGridObject == null) selectedGridObject = Instantiate(gridHighlightPrefab, gridTransform);
            selectedGridObject.SetActive(true);
            selectedGridObject.transform.position = selected.GetTile().MiddleBase;
        }
        else if (selectedGridObject)
        {
            selectedGridObject.SetActive(false);
        }
    }

    private struct GameObjectOptionTarget
    {
        public GameObject gameObject;
        public OptionTarget optionTarget;
    }

    private List<GameObjectOptionTarget> instantiatedOptionTiles = new List<GameObjectOptionTarget>();
    void DrawOptionTiles(List<OptionTarget> tiles)
    {
        if (tiles.Count == 0) ClearOptionTiles();

        if (instantiatedOptionTiles.Count != tiles.Count)
        {
            if (tiles.Count > instantiatedOptionTiles.Count)
            {
                // Need to add children
                while (instantiatedOptionTiles.Count != tiles.Count)
                {
                    var t = Instantiate(gridOptionPrefab, gridTransform);
                    instantiatedOptionTiles.Add(new GameObjectOptionTarget { gameObject = t });
                }
            }
            else
            {
                // Need to remove children
                int toRemove = transform.childCount - tiles.Count;
                for (int i = 0; i < toRemove; i++)
                {
                    Destroy(instantiatedOptionTiles[0].gameObject);
                    instantiatedOptionTiles.RemoveAt(0);
                }
            }
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            GameObjectOptionTarget target = new GameObjectOptionTarget { gameObject = instantiatedOptionTiles[i].gameObject, optionTarget = new OptionTarget { option = tiles[i].option, target = tiles[i].target } };
            instantiatedOptionTiles[i] = target;
            instantiatedOptionTiles[i].gameObject.transform.position = tiles[i].option.MiddleBase;
        }
    }

    public void ClearOptionTiles()
    {

        for (int i = 0; i < instantiatedOptionTiles.Count; i++)
        {
            Destroy(instantiatedOptionTiles[0].gameObject);
            instantiatedOptionTiles.RemoveAt(0);
        }
    }

    void DrawSelectionTile()
    {
        if (gridPosition != null)
        {
            if (gridObject == null) gridObject = Instantiate(gridSelectionPrefab, gridTransform);
            if (selected != null)
            {
                if (gridPosition == selected.GetTile())
                {
                    gridObject.SetActive(false);
                }
                else
                {
                    gridObject.SetActive(true);
                    gridObject.transform.position = gridPosition.MiddleBase;
                }
            }
            else
            {
                gridObject.SetActive(true);
                gridObject.transform.position = gridPosition.MiddleBase;
            }
        }
        else if (gridObject)
        {
            selectedGridObject.SetActive(false);
        }
    }

    void FocusTile(TilePosition tile)
    {
        targetPosition = tile.MiddleBase;
        targetZoom = minZoom;
        selected = tile.GetEntity();
    }

    void Update()
    {
        // Code if cursor is not on a UI element
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 75, LayerMask.GetMask("Entities")))
            {
                var entity = hit.transform.GetComponent<Entity>();
                if (entity != null)
                {
                    gridPosition = entity.GetTile();
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!returnOnNext)
                        {
                            selected = entity;
                            if (Time.time - lastDoubleClick > cooldownTime)
                            {
                                var t = Time.time - lastClick;
                                if (t <= clickTime)
                                {
                                    FocusTile(entity.GetTile());
                                    lastDoubleClick = Time.time;
                                }
                                lastClick = Time.time;
                            }
                        }
                        else
                        {
                            returnOnNext = false;
                            InGameUIManager.RegisterInput(new ActionInput { actionType = ActionInputType.entitySelect, content = entity });
                        }
                    }
                }
            }
            else if (Physics.Raycast(ray, out hit, 75, LayerMask.GetMask("Ground")))
            {
                if (Vector3.Dot(hit.normal, Vector3.up) == 1)
                {
                    var tile = Tile.GetTile(hit.point);
                    gridPosition = tile;

                    if (Input.GetMouseButtonDown(0))
                    {
                        foreach (var i in instantiatedOptionTiles.ToArray())
                        {
                            if (gridPosition == i.optionTarget.option)
                            {
                                InGameUIManager.RegisterInput(new ActionInput { actionType = ActionInputType.entityAdjacentSelect, content = gridPosition });
                            }
                        }
                        InGameUIManager.RegisterInput(new ActionInput { actionType = ActionInputType.clickPosition, content = gridPosition });
                    }

                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                selected = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (nextSelectedUnit == Player.Players.Count) nextSelectedUnit = 0;
            FocusTile(Player.Players[nextSelectedUnit].GetTile());
            selected = Player.Players[nextSelectedUnit];
            nextSelectedUnit++;
        }

        DrawSelectionTile();
        DrawHighlightTile();

        #region Camera Behaviour
        
        /////// Input

        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * 20f * Time.deltaTime;
        float zoomInput = -Input.GetAxis("Mouse ScrollWheel") * 350f * Time.deltaTime;
        float yawInput = 0;
        float pitchInput = 0;
        if (Input.GetMouseButton(2))
        {
            if (mouseScrollBegun)
            {
                var delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                pitchInput = -delta.y * 80 * Time.deltaTime;
                yawInput = delta.x * 80 * Time.deltaTime;
            }
            mouseScrollBegun = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else {
            mouseScrollBegun = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                pitchInput = Input.GetAxis("Yaw") * 250 * Time.deltaTime;
            }
            else
            {
                yawInput = -Input.GetAxis("Yaw") * 250 * Time.deltaTime;
            }
        }

        /////// Target updating

        targetPosition += camera.transform.forward.RemoveAxisAndNormalize(Y: true) * moveInput.z + camera.transform.right.RemoveAxisAndNormalize(Y: true) * moveInput.x;
        targetZoom = Mathf.Clamp(targetZoom + zoomInput, minZoom, maxZoom);
        targetPitch = ClampAngle(targetPitch + pitchInput, minPitch, maxPitch);
        targetYaw = Mathf.Repeat(targetYaw + yawInput, 360);
        
        /////// Calcuating new position & rotation

        currentPosition = Vector3.SmoothDamp(currentPosition, targetPosition, ref velcoityPosition, positionTime);
        currentZoom = Mathf.Clamp(Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVelocity, zoomTime), minZoom, maxZoom);

        Vector3 groundOffset = Vector3.zero;
        RaycastHit hit2;
        if (Physics.Raycast(camera.transform.position, Vector3.down, out hit2, 30, LayerMask.GetMask("Ground"))) {
            groundOffset = hit2.point.RemoveAxis(X: true, Z: true);
        }

        /////// Update position & rotation

        Quaternion newRotation = Quaternion.RotateTowards(camera.transform.rotation, Quaternion.Euler(targetPitch, targetYaw, 0), 360 * Time.deltaTime);
        Vector3 newPosition = newRotation * new Vector3(0, 0, -currentZoom) + currentPosition + groundOffset;

        camera.transform.position = newPosition;
        camera.transform.rotation = newRotation;

#endregion

    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle, 360);
        return Mathf.Clamp(angle, min, max);
    }
}
