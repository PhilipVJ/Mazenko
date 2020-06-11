using System;
using System.Collections;
using System.Diagnostics;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Text pointsText;
    [SerializeField]
    private Text timeText;
    [SerializeField]
    private Text goalText;
    [SerializeField]
    private Button tryAgainButton;
    [SerializeField]
    private Button nextLevelButton;
    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private Texture bronzeMedal;
    [SerializeField]
    private Texture silverMedal;
    [SerializeField]
    private Texture goldMedal;
    [SerializeField]
    private RawImage medalArea;
    [SerializeField]
    private GameObject pauseMenu;

    private int points = 0;
    private Stopwatch timer;
    private static GameManager instance;
    private bool isDead;
    private bool reachedGoal = false;
    public static int currentLevel;
    private SystemManager systemManager;
    private bool startedSystems = false;
    public bool isPaused = false;
    private float defaultFixedDeltaTime;

    public int receivedMedal; // 1 for gold, 2 for silver, 3 for bronze

    // Combined score
    public static int bronzeMedals;
    public static int goldMedals;
    public static int silverMedals;
    public static int totalPoints;

    // Final level
    public const int lastLevel = 3;

    private PlayerMovementSystem movementSystem;
    private PausePlayerSystem pauseSystem;

    // Input
    public bool jumping;


    void Awake()
    {
        if (instance == null || instance != this)
            instance = this;
        systemManager = new SystemManager();
        movementSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PlayerMovementSystem>();
        pauseSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PausePlayerSystem>();
    }

    private void FixedUpdate()
    {
        if (startedSystems)
            movementSystem.Update();
    }

    private void Start()
    {
        Cursor.visible = false;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
        SetText();
        timer = new Stopwatch();
        timer.Start();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    void Update()
    {
        // Start the systems when the level and all its entities have been loaded
        if (!startedSystems)
        {
            var entities = World.DefaultGameObjectInjectionWorld.EntityManager.GetAllEntities(Unity.Collections.Allocator.Temp);
            if (entities.Length > 6)
            {
                systemManager.EnableAllSystems();
                startedSystems = true;
            }
            entities.Dispose();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !reachedGoal)
        {
            isPaused = !isPaused;
            if (isPaused)
                Pause();
            else
                Unpause();
        }

        if (isPaused && Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
            QuitGame();
            return;
        }

        if (isPaused)
            pauseSystem.Update();

        SetText();
        SetTime();
        if (!jumping)
            jumping = Input.GetKeyDown(KeyCode.Space);

        if (isDead)
        {
            ReloadLevel();
        }
    }

    private void Unpause()
    {
        pauseSystem.isPaused = false;
        pauseSystem.Update();

        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
        pauseMenu.gameObject.SetActive(false);
        timer.Start();
    }

    private void Pause()
    {
        pauseSystem.isPaused = true;
        timer.Stop();
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;
        pauseMenu.gameObject.SetActive(true);
    }

    public static void ResetStaticGameInformation()
    {
        bronzeMedals = 0;
        silverMedals = 0;
        goldMedals = 0;
        totalPoints = 0;
        currentLevel = 0;
    }

    public void QuitGame()
    {
        ResetStaticGameInformation();
        LoadScene("MainMenu");
    }


    private void LoadScene(String sceneName)
    {
        DeleteAllEntities();
        if (sceneName == "Level" + (lastLevel + 1))
            SceneManager.LoadScene("End");
        else
            SceneManager.LoadScene(sceneName);
    }

    public static void BeginGame()
    {
        currentLevel = 1;
        SceneManager.LoadScene("Level1");
    }

    public void HasReachedGoal()
    {
        if (!reachedGoal)
        {
            timer.Stop();
            reachedGoal = true;
            ShowGoalInformation();
            SoundManager.GetInstance().QueueAudio(Vector3.zero, "goal");
        }
    }

    public void ShowGoalInformation()
    {
        Cursor.visible = true;
        // Activate UI components
        tryAgainButton.gameObject.SetActive(true);
        goalText.gameObject.SetActive(true);
        medalArea.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        // Get and set medal information
        var info = MedalStorage.GetLevelInfo(GameManager.currentLevel);
        var goldMinimumTime = info[2];
        var silverMinimumTime = info[1];
        var bronzeMinimumTime = info[0];
        var time = timer.ElapsedMilliseconds / 1000;
        bool showNextLevelButton = true;
        if (time > bronzeMinimumTime)  // No medal
        {
            receivedMedal = 0;
            showNextLevelButton = false;
            medalArea.enabled = false;
            goalText.text = TextStorage.GetSlowText();
        }
        else if (time <= bronzeMinimumTime && time > silverMinimumTime)  // Bronze medal
        {
            receivedMedal = 3;
            medalArea.texture = bronzeMedal;
            goalText.text = TextStorage.GetBronzeText();
        }
        else if (time <= silverMinimumTime && time > goldMinimumTime) // Silver
        {
            receivedMedal = 2;
            medalArea.texture = silverMedal;
            goalText.text = TextStorage.GetSilverText();
        }
        else // Gold time
        {
            receivedMedal = 1;
            medalArea.texture = goldMedal;
            goalText.text = TextStorage.GetGoldText();
        }
        if (showNextLevelButton)
        {
            nextLevelButton.gameObject.SetActive(true);
        }
    }

    public void ReloadLevel()
    {
        tryAgainButton.interactable = false;
        DeleteAllEntities();
        SceneManager.LoadScene("Level" + currentLevel);
    }

    public static void DeleteAllEntities()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entities = manager.GetAllEntities();
        foreach (var entity in entities)
        {
            manager.DestroyEntity(entity);
        }
    }


    private void SetText()
    {
        pointsText.text = "Points: " + points;
    }

    public void NextLevel()
    {
        UpdateTotalPointsAndMedals();
        nextLevelButton.interactable = false;
        currentLevel++;
        LoadScene("Level" + currentLevel);
    }

    private void UpdateTotalPointsAndMedals()
    {
        switch (receivedMedal)
        {
            case 1:
                goldMedals++;
                break;
            case 2:
                silverMedals++;
                break;
            case 3:
                bronzeMedals++;
                break;
            default:
                break;
        }
        totalPoints += points;
    }

    private void SetTime()
    {
        timeText.text = " Time: " + (timer.ElapsedMilliseconds / 1000);
    }

    public void IncrementPoint()
    {
        points++;
    }

    public void Dead()
    {
        isDead = true;
    }

}
