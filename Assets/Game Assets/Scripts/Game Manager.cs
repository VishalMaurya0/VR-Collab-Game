using Autohand;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentLevel = 1;

    [Header("References")]
    public List<string> PaintingWorlds = new List<string>();
    public Transition transitionEffect;
    public GameObject postProcessPrefab;
    public GameObject CanvasForTimeTOStay;
    public GameObject timeOfStayText;   
    public Camera mainCamera;

    [Header("Inital Parameters")]
    public string baseSceneName = "Base Scene";
    public int currentPaintingIndex = 0;

    [Header("Across Data")]
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

        // Subscribe only once
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Prevent ghost events from destroyed instances
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Delay to let Unity finish loading everything
        StartCoroutine(ReinitializeSceneReferences());
    }

    private IEnumerator ReinitializeSceneReferences()
    {
        yield return new WaitForSeconds(0.1f);

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
        timeOfStayText = GameObject.FindWithTag("TimeOfStay");
        mainCamera = GameObject.FindAnyObjectByType<Camera>();

        if (timeOfStayText == null && CanvasForTimeTOStay != null)
        {
            Instantiate(CanvasForTimeTOStay,
                mainCamera.transform);
            timeOfStayText = GameObject.FindWithTag("TimeOfStay");
        }

        if (timeOfStayText != null)
            timeOfStayText.SetActive(false);
    }


    private void Start()
    {
        mainCamera = GameObject.FindAnyObjectByType<Camera>();
        InitializeTransition();
        if (timeOfStayText == null)
        {
            timeOfStayText = GameObject.FindWithTag("TimeOfStay");
        }
    }

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
                timeOfStayText.SetActive(true);
                TMP_Text text = timeOfStayText.GetComponent<TMP_Text>();
                text.text = "Time Of Stay: " + timeLeft.ToString("F2") + " secs";
            }
            else
            {
                timeOfStayText.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && transitionEffect != null)
        {
            baseSceneName = SceneManager.GetActiveScene().name;
            StartCoroutine(transitionEffect.DoTransition(PaintingWorlds[currentPaintingIndex]));
            insidePainting = true;
        }

        if (Input.GetKeyDown(KeyCode.R) && transitionEffect != null)
        {
            StartCoroutine(transitionEffect.ReverseTransition());
        }

        if (insidePainting)
        {
            timer_InsidePainting += Time.deltaTime;
            if (timer_InsidePainting >= totalTimeInsidePainting)
            {
                // Time's up, exit painting
                StartCoroutine(transitionEffect.DoTransition(baseSceneName));
                insidePainting = false;
                timer_InsidePainting = 0f;
            }
        }


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
    }
}
