using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentLevel = 1;

    [Header("References")]
    public List<string> PaintingWorlds = new List<string>();
    public int currentPaintingIndex = 0;
    public Transition transitionEffect;
    public GameObject postProcessPrefab;

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

    private void InitializeTransition()
    {
        transitionEffect = FindAnyObjectByType<Transition>();

        if (transitionEffect == null && postProcessPrefab != null)
        {
            Instantiate(postProcessPrefab);
            transitionEffect = FindAnyObjectByType<Transition>();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this == null) return;
        StartCoroutine(FindTransitionAfterLoad());
    }


    private IEnumerator FindTransitionAfterLoad()
    {
        yield return new WaitForSeconds(0.1f); // wait one frame or so
        transitionEffect = FindAnyObjectByType<Transition>();

        if (transitionEffect != null)
        {
            yield return transitionEffect.ReverseTransition();
        }
        else
        {
            InitializeTransition();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && transitionEffect != null)
        {
            StartCoroutine(transitionEffect.DoTransition(PaintingWorlds[currentPaintingIndex]));
        }

        if (Input.GetKeyDown(KeyCode.R) && transitionEffect != null)
        {
            StartCoroutine(transitionEffect.ReverseTransition());
        }
    }
}
