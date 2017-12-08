using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    #region variables
    public static int level;
    public static Mode mode;
    public enum Mode
    {
        Chase, Scatter
    }
    int index = 0, lifeIndex = 1;
    bool won = false, lifeUp;
    Color defaultColor;

    public static bool restartGame;
    public enum Layers
    {
        PacMan = 0, Ghosts = 10, Wall = 9
    }

    [Header("Ghosts AI")]
    [SerializeField()]
    int[] modeTimes;
    [SerializeField]
    AudioSource ghost, scaredGhost, startGame, pacmanMunch, extraMan;
    [Space(10)]

    [Header("UI")]
    [SerializeField()]
    Text scoreTxt, GameOverTxt, highScoreTxt, Lifes, Ready;
    [SerializeField]
    Material mapMaterial;

    //Singleton
    static GameController controller;
    public static GameController Controller
    {
        get
        {
            if (controller == null)
                controller = FindObjectOfType<GameController>();
            return controller;
        }
    }
    #endregion

    // Use this for initialization
    void Start ()
    {
        level = PlayerPrefs.GetInt("Level");
        lifeIndex = (PlayerPrefs.GetInt("LifeIndex") != 0)? PlayerPrefs.GetInt("LifeIndex") : 1;
        mode = Mode.Scatter;
        lifeUp = false;
        Ready.enabled = true;
        highScoreTxt.text = "";
        if (PlayerPrefs.GetInt("Score") == 0)
            scoreTxt.text = "00";
        else
            scoreTxt.text = scoreTxt.text.Replace("X", PlayerPrefs.GetInt("Score").ToString());
        Lifes.text = Lifes.text.Replace("N", (PacManScript.PacMan.Lifes + 1).ToString());
        StartCoroutine(StartDelay());
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Add Lifes to PacMan
        if (PacManScript.PacMan.points >= 10000 * lifeIndex && !lifeUp)
        {
            extraMan.Play();
            lifeIndex++;
            PacManScript.PacMan.Lifes++;
            lifeUp = true;
            Lifes.text = "LIFES:  x " + (PacManScript.PacMan.Lifes + 1).ToString();
        }
        //Handles when PacMan collects all pellets
        if (PacManScript.PacMan.totalCollectedPellets == 244 && !won)
        {
            PlayerPrefs.SetInt("Level", ++level);
            PlayGhostsSound(false);
            PlayGhostsSoundF(false);
            PacManScript.PacMan.Multiplier = 0;
            PacManScript.PacMan.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            PacManScript.PacMan.enabled = false;
            PlayerPrefs.SetInt("Score", PacManScript.PacMan.points);
            PlayerPrefs.SetInt("Lifes", PacManScript.PacMan.Lifes);
            PlayerPrefs.SetInt("LifeIndex", lifeIndex);
            won = true;
            BlinkyScript.Blinky.speedMultiplier = 0;
            PinkyScript.Pinky.speedMultiplier = 0;
            ClydeScript.Clyde.speedMultiplier = 0;
            InkyScript.Inky.speedMultiplier = 0;
            PlayGhostsSound(false);
            index = 0;
            StartCoroutine(Blink());
        }
        //Handles when PacMan has no lifes and dies
        if (restartGame && PacManScript.PacMan.Lifes < 0)
        {
            PlayGhostsSound(false);
            PlayGhostsSoundF(false);
            Lifes.text = "LIFES:  x " + (PacManScript.PacMan.Lifes + 1).ToString();
            PlayerPrefs.SetInt("Lifes", -1);
            PlayerPrefs.SetInt("LifeIndex", 0);
            PacManScript.PacMan.Multiplier = 0;
            BlinkyScript.Blinky.speedMultiplier = 0;
            PinkyScript.Pinky.speedMultiplier = 0;
            ClydeScript.Clyde.speedMultiplier = 0;
            InkyScript.Inky.speedMultiplier = 0;
            PlayGhostsSound(false);
            PlayerPrefs.SetInt("Score", 0);

            if (PacManScript.PacMan.points > PlayerPrefs.GetInt("HighScore") || !PlayerPrefs.HasKey("HighScore"))
                PlayerPrefs.SetInt("HighScore", PacManScript.PacMan.points);
            GameOverTxt.gameObject.SetActive(true);
            StartCoroutine(BackToMenu(1f));
        }
        //Just restart the game when PacMan dies
        else if (restartGame)
        {
            restartGame = false;
            Lifes.text = "LIFES:  x " + (PacManScript.PacMan.Lifes + 1).ToString();
            PlayerPrefs.SetInt("LifeIndex", lifeIndex);
            PacManScript.PacMan.Multiplier = 0;
            BlinkyScript.Blinky.speedMultiplier = 0;
            PinkyScript.Pinky.speedMultiplier = 0;
            ClydeScript.Clyde.speedMultiplier = 0;
            InkyScript.Inky.speedMultiplier = 0;
            PlayGhostsSound(false);
            StopAllCoroutines();
            index = 0;
            PacManScript.PacMan.Restart();
            BlinkyScript.Blinky.Restart();
            PinkyScript.Pinky.Restart();
            ClydeScript.Clyde.Restart();
            InkyScript.Inky.Restart();
            Start();
        }

        if (PacManScript.PacMan.points == 0)
            scoreTxt.text = "00";
        else
            scoreTxt.text = PacManScript.PacMan.points.ToString();
        if (PacManScript.PacMan.points > PlayerPrefs.GetInt("HighScore"))
            highScoreTxt.text = PacManScript.PacMan.points.ToString();
        if (lifeUp && PacManScript.PacMan.points % 10000 != 0)
            lifeUp = false;
    }

    public void PlayGhostsSound(bool play)
    {
        if (play)
            ghost.Play();
        else
            ghost.Stop();
    }

    public void PlayGhostsSoundF(bool play)
    {
        if (play)
            scaredGhost.Play();
        else
            scaredGhost.Stop();
    }

    public void PlayPMMunch()
    {
        pacmanMunch.Play();
    }


    IEnumerator BackToMenu(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("Menu");
    }

    IEnumerator ChangeMode(int time)
    {
        yield return new WaitForSeconds(time);
        mode = (mode == Mode.Scatter) ? Mode.Chase : Mode.Scatter;
        BlinkyScript.Blinky.InvertDirection();
        PinkyScript.Pinky.InvertDirection();
        ClydeScript.Clyde.InvertDirection();
        InkyScript.Inky.InvertDirection();
        index++;
        try
        {
            StartCoroutine(ChangeMode(modeTimes[index]));
        }
        catch (IndexOutOfRangeException)
        {
            mode = Mode.Chase;
        }
        
    }

    IEnumerator Blink()
    {
        if (index < 5)
        {
            mapMaterial.color = (mapMaterial.color == defaultColor) ? Color.white : defaultColor;
            yield return new WaitForSeconds(0.5f);
            index++;
            StartCoroutine(Blink());
        }
        else
        {
            mapMaterial.color = defaultColor;
            SceneManager.LoadScene("Map - Copia");
        }
    }

    IEnumerator StartDelay()
    {
        PacManScript.PacMan.enabled = false;
        BlinkyScript.Blinky.speedMultiplier = 0;
        PinkyScript.Pinky.speedMultiplier = 0;
        ClydeScript.Clyde.speedMultiplier = 0;
        InkyScript.Inky.speedMultiplier = 0;
        PlayGhostsSound(false);

        startGame.Play();
        yield return new WaitForSeconds(3.717f);
        Ready.text = "Go!!!";
        yield return new WaitForSeconds(0.5f);
        Ready.enabled = false;
        Ready.text = "Ready?";
        startGame.Stop();

        highScoreTxt.text = PlayerPrefs.GetInt("HighScore").ToString();
        PacManScript.PacMan.enabled = true;
        BlinkyScript.Blinky.speedMultiplier = 1;
        PinkyScript.Pinky.speedMultiplier = 1;
        ClydeScript.Clyde.speedMultiplier = 1;
        InkyScript.Inky.speedMultiplier = 1;
        PlayGhostsSound(true);

        defaultColor = mapMaterial.color;

        restartGame = false;
        mode = Mode.Scatter;

        highScoreTxt.text = highScoreTxt.text.Replace("X", PlayerPrefs.GetInt("HighScore").ToString());

        try
        {
            StartCoroutine(ChangeMode(modeTimes[index]));
        }
        catch (IndexOutOfRangeException)
        {
            mode = Mode.Chase;
            BlinkyScript.Blinky.InvertDirection();
            PinkyScript.Pinky.InvertDirection();
            ClydeScript.Clyde.InvertDirection();
            InkyScript.Inky.InvertDirection();
        }
    }
}
