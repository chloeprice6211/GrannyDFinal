using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ObjectSpawnerManager : Mirror.NetworkBehaviour
{
    private static ObjectSpawnerManager _instance;

    public static ObjectSpawnerManager Instance
    {
        get
        {
            return _instance;
        }
    }
    [SyncVar] public int playersCount;

    [SerializeField] List<GameObject> objs;
    [SerializeField] List<GameObject> customGenerableObjs;

    [Header("Custom Generated Objects")]
    [SerializeField] List<GameObject> customObjects;

    [Header("Spawned network objects")]
    [Space]
    [SerializeField] List<Note> notes = new();
    [SerializeField] List<Transform> notesSpawnPoints = new();

    [Space]
    [SerializeField] List<Item> keys = new();
    [SerializeField] List<Transform> keysSpawnPoints = new();

    [Space]
    [SerializeField] List<Device> devices = new();
    [SerializeField] List<Transform> deviceSpawnPoints = new();

    [Space]
    [SerializeField] List<GeneratedNote> generatedNotes = new();
    [SerializeField] List<Transform> generatedNoteSpawnPoints = new();

    [Space]
    [SerializeField] List<Item> bigItems = new();
    [SerializeField] List<Transform> bigItemsSpawnPoints = new();

    [Space]
    [SerializeField] List<Transform> startedKeySpawnPoints = new();
    [SerializeField] List<Item> startedKeys = new();

    [Space]
    [SerializeField] List<Transform> miscSpawnPoints = new();
    [SerializeField] List<Item> miscItems = new();

    [Space]
    [SerializeField] List<Item> staticItems = new();

    [Space]
    [SerializeField] List<Note> allNotes = new();

    public List<Item> allItems;

    [Space]
    [SerializeField] List<Backpack> backpacks = new();

    [SerializeField] SwitchPanel switchPanel;
    [SerializeField] WalkieTalkie walkieTalkie;

    private GeneratedCode _generatedCode;
    private string _code;
    PlayerControls _controls;

    string switchers;

    float _highlightTimer = 0;
    bool disable;
    [SyncVar] public bool isOutlineAllowed;

    [Header("injector")]
    public Syringe injector;


    private void Awake()
    {
        _instance = this;
        _controls = new();
        _controls.Player.Highlight.performed += HighlightPerformed;

        allItems.AddRange(bigItems);
        allItems.AddRange(startedKeys);
        allItems.AddRange(devices);
        allItems.AddRange(keys);
        allItems.AddRange(miscItems);
        allItems.AddRange(staticItems);

        allNotes.AddRange(notes);
        allNotes.AddRange(generatedNotes);

        _highlightTimer = 0;
    }

    private void HighlightPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isOutlineAllowed = !isOutlineAllowed;
        foreach (Item item in allItems)
        {
            item.outlineShader.enabled = isOutlineAllowed;
        }
        foreach(Note note in allNotes)
        {
            note.outline.enabled = isOutlineAllowed;
        }
    }

    private void Update()
    {
        _highlightTimer += Time.deltaTime;

        if(_highlightTimer >= 1500 && !disable && !isOutlineAllowed)
        {
            UIManager.Instance.controlsMessageTip.ControlsMessage("highlightTip");
            _controls.Player.Highlight.Enable();
            disable = true;
        }
    }

    public override void OnStartServer()
    {
        SetupAllCodes();
        isOutlineAllowed = SetupPanel.isOutlineAllowed;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(WaitForPlayersRoutine());
    }

    IEnumerator WaitForPlayersRoutine()
    {
        if (CustomNetworkManager.Instance.LobbyPlayers.Count > 1)
        {
            UIManager.Instance.waitingForPlayersPanel.SetActive(true);
        }

        while (CustomNetworkManager.Instance.LobbyPlayers.Count != CustomNetworkManager.Instance.GamePlayers.Count)
        {
            yield return null;
        }

        UIManager.Instance.waitingForPlayersPanel.SetActive(false);

        RandomMoveObjects();

        if (SetupPanel.isOutlineAllowed)
        {
            HighlightItems();
        }
            
        yield return null;
    }

    [Server]
    private void SetupAllCodes()
    {
        foreach (GameObject obj in objs)
        {
            _code = string.Empty;
            _generatedCode = obj.GetComponent<GeneratedCode>();
            _code = GenerateCode(_generatedCode);

            obj.GetComponent<IGenerable>().ApplyGeneratedCode(_code);
        }

        //if (switchPanel != null)
        //{
        //    bool isTrue;

        //    for (int a = 0; a < 7; a++)
        //    {
        //        isTrue = Random.Range(0, 2) == 1 ? true : false;

        //        if (isTrue)
        //        {
        //            switchers += '1';
        //        }
        //        else
        //        {
        //            switchers += '0';
        //        }
        //    }

        //    switchPanel.ApplyGeneratedCode(switchers);
        //}
        if(walkieTalkie != null)
        {
            int randomIndex = Random.Range(0, 5);
            string code = string.Empty;

            switch (randomIndex)
            {
                case 0: code = "SNARGU"; break;
                case 1: code = "UNGARS"; break;
                case 2: code = "NURASG"; break;
                case 3: code = "AUGSNR"; break;
                case 4: code = "SURANG"; break;
            }

            walkieTalkie.ApplyGeneratedCode(code);
        }

        foreach(GameObject obj in customObjects)
        {
            obj.GetComponent<IGenerable>().ApplyGeneratedCode("");
        }

        foreach(GameObject cObj in customGenerableObjs){
            cObj.GetComponent<ICustomGenerable>().CustomGenerate();
        }


    }

    public void ShowCodes()
    {
        foreach (GameObject obj in objs)
        {
            obj.GetComponent<IGenerable>().ShowGeneratedCode();
        }

        if (switchPanel != null)
        {
            switchPanel.ShowCodes();
        }

        if(walkieTalkie != null)
        {
            walkieTalkie.ShowGeneratedCode();
        }

        foreach(GameObject obj in customObjects)
        {
            obj.GetComponent<IGenerable>().ShowGeneratedCode();
        }
    }

    public string GenerateCode(GeneratedCode codeProperties)
    {
        if (codeProperties.isNumeric)
        {
            for (int a = 0; a < codeProperties.charsAmount; a++)
            {
                int num = Random.Range(0, 9);
                _code += num;
            }

            return _code;
        }
        else
        {
            for (int a = 0; a < codeProperties.charsAmount; a++)
            {
                if (a < codeProperties.charsAmount / 2)
                {
                    switch (Random.Range(0, 5))
                    {
                        case 1: _code += 'a'; break;
                        case 2: _code += 't'; break;
                        case 3: _code += 'g'; break;
                        case 4: _code += 'k'; break;
                        case 5: _code += 'u'; break;
                    }
                }
                else
                {
                    _code += Random.Range(0, 9);
                }
            }

            return _code;
        }
    }

    [ServerCallback]
    public void RandomMoveObjects()
    {
        MoveItems(devices, deviceSpawnPoints);
        MoveItems(notes, notesSpawnPoints);
        MoveItems(generatedNotes, generatedNoteSpawnPoints);
        MoveItems(keys, keysSpawnPoints);
        MoveItems(bigItems, bigItemsSpawnPoints);
        MoveItems(startedKeys, startedKeySpawnPoints);
        MoveItems(miscItems, miscSpawnPoints);
    }

    void MoveItems<T>(List<T> items, List<Transform> spawnPoints)
    {
        int spawnPointIndex;

        foreach (T item in items)
        {
            spawnPointIndex = Random.Range(0, spawnPoints.Count);
            MoveItemRpc(item as NetworkBehaviour, spawnPoints[spawnPointIndex]);

            spawnPoints.RemoveAt(spawnPointIndex);
        }
    }


    [ClientRpc]
    void MoveItemRpc(NetworkBehaviour item, Transform position)
    {
        //Debug.Log(item.name);
        item.transform.parent.SetParent(position);
        item.transform.parent.localPosition = Vector3.zero;
        item.transform.parent.localRotation = Quaternion.Euler(Vector3.zero);
    }

    [ServerCallback]
    void HighlightItems()
    {
       

        HighlightRpc();
    }

    [ClientRpc]
    void HighlightRpc()
    {
        foreach(Item item in allItems)
        {
            item.highlightLight.enabled = true;
        }
        foreach (Note note in allNotes)
        {
            Debug.Log("highligted");
            note.highlightLight.enabled = true;
        }
    }

    private void OnDestroy()
    {
        _controls.Disable();
    }

}
