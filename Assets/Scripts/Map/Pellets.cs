using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pellets : MonoBehaviour
{
    [SerializeField()]
    int points;
    [SerializeField()]
    bool power;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PacMan"))
        {
            GameController.Controller.PlayPMMunch();
            if (power)
            {
                PacManScript.PacMan.pointMultiplier = 1;
                StopAllCoroutines();
                StartCoroutine(FrightenedMode1(6));
            }
            PacManScript.PacMan.points += points;
            PacManScript.PacMan.collectedPellets++;
            PacManScript.PacMan.totalCollectedPellets++;
            if (!power)
                Destroy(gameObject);
            else
            {
                GetComponent<SpriteRenderer>().enabled = false;
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision);
            }
        }
    }

    IEnumerator FrightenedMode1(int time)
    {
        BlinkyScript.Blinky.Frightened = true;
        PinkyScript.Pinky.Frightened = true;
        InkyScript.Inky.Frightened = true;
        ClydeScript.Clyde.Frightened = true;
        yield return new WaitForSeconds(time);
        BlinkyScript.Blinky.Frightened = false;
        PinkyScript.Pinky.Frightened = false;
        InkyScript.Inky.Frightened = false;
        ClydeScript.Clyde.Frightened = false;
        GameController.Controller.PlayGhostsSound(true);
        GameController.Controller.PlayGhostsSoundF(false);
        Destroy(gameObject);
    }
}
