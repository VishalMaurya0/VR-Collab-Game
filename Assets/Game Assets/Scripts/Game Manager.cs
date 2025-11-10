using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentLevel = 1;
    public float timeSpeed = 1f;

    [Header("References")]
    public List<string> PaintingWorlds = new List<string>();
    public Transition transitionEffect;
    public GameObject postProcessPrefab;
    public GameObject CanvasForTimeTOStay;
    public TMP_Text timeOfStayText;   
    public TMP_Text helpText;   
    public Camera mainCamera;
    public GameObject playerPrefab;
    public GameObject playerGO;

    [Header("Inital Parameters")]
    public string baseSceneName = "Base Scene";
    public int currentPaintingIndex = 0;

    [Header("Across Data")]
    public bool teleportable = false;
    public bool unlockGate1 = false;
    public bool unlockGate2 = false;
    public bool unlockGate3 = false;
    public bool placePointPlaced2 = false;
    public List<GameObject> teleportObjects = new List<GameObject>();

    [Header("Gate Refe")]
    public GameObject gate1;
    public GameObject gate2;
    public GameObject gate3;


    [Header("Properties")]
    public float totalTimeInsidePainting = 10f;
    public float timer_InsidePainting = 0f;
    bool insidePainting = false;
    bool rightTriggerPressed = false;
    public float timeToshowHelpText = 3f;


    private void Awake()
    {
        // Proper singleton guard
        if (Instance != null && Instance != this)
        {
            // Unsubscribe old one from event before destroying
            SceneManager.sceneLoaded -= Instance.OnSceneLoaded;
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeTransition();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnEnable()
    {
        // ensure only one subscription
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        Debug.Log("[GameManager] Subscribed to scene events");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        Debug.Log("[GameManager] Unsubscribed from scene events");
    }

    private void OnActiveSceneChanged(Scene prev, Scene next)
    {
        Debug.Log($"[GameManager] ActiveSceneChanged: {prev.name} -> {next.name}");
        // sometimes activeSceneChanged gets called when sceneLoaded doesn't in editor/build; run reinit too
        StartCoroutine(ReinitializeSceneReferences());

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Delay to let Unity finish loading everything
        StartCoroutine(ReinitializeSceneReferences());
    }

    private IEnumerator ReinitializeSceneReferences()
    {
        yield return new WaitForSeconds(0.2f);

        INstantiatePLayer();
        InitializeTransition();
        TryFindOrCreateTimeUI();

        if (transitionEffect != null)
            yield return transitionEffect.ReverseTransition();
    }

    private void InitializeTransition()
    {
        transitionEffect = FindAnyObjectByType<Transition>();

        if (transitionEffect == null && postProcessPrefab != null)
        {
            Instantiate(postProcessPrefab);
            transitionEffect = FindAnyObjectByType<Transition>();
        }
    }

    private void TryFindOrCreateTimeUI()
    {
        if (timeOfStayText != null && timeOfStayText.gameObject != null && timeOfStayText.gameObject.scene.name != null)
        {
            return;
        }

        timeOfStayText = GameObject.FindWithTag("TimeOfStay")?.GetComponent<TMP_Text>();
        helpText = GameObject.FindWithTag("HelpText")?.GetComponent<TMP_Text>();
        mainCamera = GameObject.FindAnyObjectByType<Camera>();

        if (timeOfStayText == null && CanvasForTimeTOStay != null)
        {
            var canvasInstance = Instantiate(CanvasForTimeTOStay, mainCamera.transform);
            DontDestroyOnLoad(canvasInstance); // Keep same canvas across scenes
            timeOfStayText = GameObject.FindWithTag("TimeOfStay")?.GetComponent<TMP_Text>();
        }

        if (timeOfStayText != null)
            timeOfStayText.gameObject.SetActive(false);

        if (helpText == null && timeOfStayText != null)
        {
            helpText = GameObject.FindWithTag("HelpText")?.GetComponent<TMP_Text>();
        }
    }



    private void Start()
    {
        mainCamera = GameObject.FindAnyObjectByType<Camera>();
        InitializeTransition();
        if (timeOfStayText == null)
        {
            timeOfStayText = GameObject.FindWithTag("TimeOfStay")?.GetComponent<TMP_Text>();
        }
        if (helpText == null && timeOfStayText != null)
        {
            helpText = GameObject.FindWithTag("HelpText")?.GetComponent<TMP_Text>();
            helpText.gameObject.SetActive(false);
        }
    }


    bool runOnce = false;
    private void Update()
    {
        if (transitionEffect == null)
            InitializeTransition();
        if (timeOfStayText == null)
            TryFindOrCreateTimeUI();


        if (timeOfStayText != null)
        {
            
            if (insidePainting)
            {
                float timeLeft = totalTimeInsidePainting - timer_InsidePainting;
                timeOfStayText.gameObject.SetActive(true);
                TMP_Text text = timeOfStayText.GetComponent<TMP_Text>();
                text.text = "Time Of Stay: " + timeLeft.ToString("F2") + " secs";
                runOnce = true;
            }
            else if (!teleportable && runOnce)
            {
                runOnce = false;
                timeOfStayText.gameObject.SetActive(false);
            }
        }

        if (helpText != null)
        {
            if (timeToshowHelpText > 0)
            {
                helpText.gameObject.SetActive(true);
                timeToshowHelpText -= Time.deltaTime;
            }
            if (timeToshowHelpText <= 0)
            {
                helpText.gameObject.SetActive(false);
            }
        }

        {
            InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            if (rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool rightTriggerValue))
            {
                if (rightTriggerValue && !rightTriggerPressed)
                {
                    rightTriggerPressed = true;
                    Debug.Log("Right Trigger Pressed");
                }
                else if (!rightTriggerValue && rightTriggerPressed)
                {
                    rightTriggerPressed = false;
                    Debug.Log("Right Trigger Released");
                }
            }
        }   // Right Trigger


        if ((Input.GetKeyDown(KeyCode.E) || rightTriggerPressed) && transitionEffect != null && teleportable)
        {
            baseSceneName = SceneManager.GetActiveScene().name;
            StartCoroutine(transitionEffect.DoTransition(PaintingWorlds[currentPaintingIndex]));
            insidePainting = true;
        }

        if (Input.GetKeyDown(KeyCode.R) && transitionEffect != null)
        {
            //StartCoroutine(transitionEffect.ReverseTransition());
        }

        if (insidePainting)
        {
            timer_InsidePainting += Time.deltaTime * timeSpeed;
            if (timer_InsidePainting >= totalTimeInsidePainting)
            {
                // Time's up, exit painting
                StartCoroutine(transitionEffect.DoTransition(baseSceneName));
                insidePainting = false;
                timer_InsidePainting = 0f;
            }
        }

        HandleTeleport();


    }

    private void HandleTeleport()
    {
        

        if (unlockGate1)
        {
            if (gate1 == null)
            {
                gate1 = GameObject.FindWithTag("Gate1");
            }
            gate1.GetComponent<Grabbable>().enabled = true;
        }
        if (unlockGate2)
        {
            if (gate2 == null)
            {
                gate2 = GameObject.FindWithTag("Gate2");
            }
            gate2.GetComponent<Grabbable>().enabled = true;
        }
        if (unlockGate3)
        {
            if (gate3 == null)
            {
                gate3 = GameObject.FindWithTag("Gate3");
            }
            gate3.GetComponent<Grabbable>().enabled = true;
        }


        if (teleportObjects.Count > 0)
        {
            if (SceneManager.GetActiveScene().name != baseSceneName)
                return;

            if (playerGO == null)
            {
                playerGO = GameObject.FindWithTag("Player");
                return;
            }

            foreach (GameObject obj in teleportObjects)
            {
                if (obj != null)
                {
                    obj.transform.position = playerGO.transform.position;
                    obj.SetActive(true);
                    SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
                    //Instantiate(obj, playerGO.transform.position, Quaternion.identity).SetActive(true);
                }
            }
            teleportObjects.Clear();
        }
    }

    private void INstantiatePLayer()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[GameManager] Checking player spawn for scene: {currentScene}");

        // If we are NOT in the base scene
        if (currentScene != baseSceneName)
        {
            Debug.Log("[GameManager] Current scene is NOT the base scene — checking if player exists...");

            if (playerGO == null)
            {
                playerGO = Instantiate(playerPrefab);
                Debug.Log($"[GameManager] Player instantiated successfully in scene: {currentScene}");
            }
            else
            {
                Debug.Log("[GameManager] Player already exists — no new instance created.");
            }
        }
        else
        {
            Debug.Log("[GameManager] Current scene is the base scene — checking for existing player...");

            if (playerGO == null)
            {
                playerGO = GameObject.FindWithTag("Player")?.gameObject;

                if (playerGO != null)
                    Debug.Log("[GameManager] Found existing player in base scene and assigned reference.");
                else
                    Debug.LogWarning("[GameManager] No player found in base scene! Make sure one exists with the 'Player' tag.");
            }
            else
            {
                Debug.Log("[GameManager] Player reference already assigned in base scene — skipping search.");
            }
        }
    }


}
