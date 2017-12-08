using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemScript : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    GameObject text;
    [SerializeField]
    AudioSource audio;
    [SerializeField]
    int[] points;
    [SerializeField]
    Sprite[] sprites;
    Sprite current;
    SpriteRenderer Renderer;
    Collider2D Collider;
    Text pointTxt;
    int point, pellets;
    bool scored, enabled;

	// Use this for initialization
	void Start ()
    {
        pellets = 70;
        scored = true;
        Renderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<Collider2D>();

        if (GameController.level == 1)
        {
            current = sprites[0];
            point = 100;
        }
        else if (GameController.level == 2)
        {
            current = sprites[1];
            point = 300;
        }
        else if (GameController.level == 3 || GameController.level == 4)
        {
            current = sprites[2];
            point = 500;
        }
        else if (GameController.level == 5 || GameController.level == 6)
        {
            current = sprites[3];
            point = 700;
        }
        else if (GameController.level == 7 || GameController.level == 8)
        {
            current = sprites[4];
            point = 1000;
        }
        else if (GameController.level == 9 || GameController.level == 10)
        {
            current = sprites[5];
            point = 2000;
        }
        else if (GameController.level == 11 || GameController.level == 12)
        {
            current = sprites[6];
            point = 3000;
        }
        else if (GameController.level > 12)
        {
            current = sprites[7];
            point = 5000;
        }
        Renderer.sprite = null;
        Collider.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (PacManScript.PacMan.totalCollectedPellets == pellets && !enabled)
        {
            pellets += 100;
            enabled = true;
            scored = false;
            Collider.enabled = true;
            Renderer.sprite = current;
            StartCoroutine(DestroyDelay(12));
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PacMan") && !scored)
        {
            StopAllCoroutines();
            pointTxt = Instantiate(text, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
            pointTxt.color = new Color(0.958f, 0.745f, 0.827f);
            pointTxt.text = point.ToString();
            Destroy(pointTxt, 1);
            enabled = false;
            audio.Play();
            scored = true;
            Collider.enabled = false;
            Renderer.sprite = null;
            PacManScript.PacMan.points += point;
        }
    }

    IEnumerator DestroyDelay(int time)
    {
        yield return new WaitForSeconds(time);

        enabled = false;
        scored = true;
        Collider.enabled = false;
        Renderer.sprite = null;
    }
}
