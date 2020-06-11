using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    [SerializeField]
    private Text resultText;
    private SystemManager manager;
    [SerializeField]
    private Button quit;
    [SerializeField]
    private GameObject fader;
    private bool fadeOutDone;

    private void Awake()
    {
        manager = new SystemManager();
        manager.DisableAllSystems();
    }

    public void Quit()
    {
        GameManager.ResetStaticGameInformation();
        quit.interactable = false;
        SoundManager.GetInstance().BeginFadeOutMusic();
        StartCoroutine(FadeOutToMainMenu());
    }

    void Start()
    {
        Cursor.visible = true;
        var score = CalculateScore();
        resultText.text = "" + score;
    }

    private int CalculateScore()
    {
        int initScore = (GameManager.bronzeMedals * 5) + (GameManager.silverMedals * 10) + 
            (GameManager.goldMedals * 15) + (GameManager.totalPoints * 2);
        return initScore * 10;
    }

    private IEnumerator FadeOutToMainMenu()
    {
        StartCoroutine(FadeOut());
        while (!fadeOutDone)
        {
            yield return new WaitForEndOfFrame();
        }
        GameManager.DeleteAllEntities();
        SceneManager.LoadScene("MainMenu");

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

}
