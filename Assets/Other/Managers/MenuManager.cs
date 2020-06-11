using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject fader;
    [SerializeField]
    private GameObject startGameButton;
    [SerializeField]
    private GameObject exitButton;

    private bool fadeOutDone = false;
    private bool startGame = false;
    private bool exitGame = false;

    public AudioClip startGameSound;
    public AudioClip quitGameSound;
    private SystemManager systemManager;
    private AudioSource audio;

    private bool fadeInDone = false;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        systemManager.DisableAllSystems();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        audio = GetComponent<AudioSource>();
        StartCoroutine(FadeIn());
    }

    private void Awake()
    {
        systemManager = new SystemManager();
    }

    // Update is called once per frame
    void Update()
    {
        if (!fadeInDone) return;

        if (fadeOutDone && startGame)
            GameManager.BeginGame();

        if (fadeOutDone && exitGame)
            Application.Quit();
    }

    private IEnumerator FadeIn()
    {
        bool done = false;
        while (!done)
        {
            Image img = fader.GetComponent<Image>();
            Color c = fader.GetComponent<Image>().color;
            c.a -= 0.04f;
            img.color = c;
            yield return new WaitForSeconds(0.03f);
            if (c.a < 0f)
            {
                done = true;
            }
        }
        fader.SetActive(false);
        fadeInDone = true;
    }


    private IEnumerator FadeOut()
    {
        bool done = false;
        fader.SetActive(true);
        while (!done)
        {
            Image img = fader.GetComponent<Image>();
            Color c = fader.GetComponent<Image>().color;
            c.a += 0.02f;
            img.color = c;
            yield return new WaitForSeconds(0.03f);
            if (c.a >= 1f)
            {
                done = true;
            }
        }
        yield return new WaitForSeconds(1f);
        fadeOutDone = true;
    }

    public void StartGame()
    {
        audio.PlayOneShot(startGameSound);
        startGame = true;
        startGameButton.GetComponent<Button>().interactable = false;
        StartCoroutine(FadeOut());
        SoundManager.GetInstance().BeginFadeOutMusic();
    }

    public void ExitGame()
    {
        audio.PlayOneShot(quitGameSound);
        StartCoroutine(FadeOut());
        exitButton.GetComponent<Button>().interactable = false;
        exitGame = true;
    }
}
