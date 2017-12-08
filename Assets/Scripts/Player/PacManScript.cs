using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PacManScript : MonoBehaviour
{
    #region variables
    [SerializeField]
    SpriteRenderer PacmanRenderer;
    [SerializeField()]
    float velocity;
    [SerializeField()]
    float distance;
    [SerializeField()]
    float time;
    [SerializeField()]
    float teleportTime;
    [SerializeField()]
    Transform start;
    [SerializeField()]
    List<GameObject> teleporters;
    [SerializeField]
    SpriteRenderer Renderer;
    [SerializeField]
    Sprite initialSprite;
    [SerializeField]
    Animator animator;
    public Collider2D Collider;
    public Vector3 direction, rotation;
    Vector3 startPosition;
    Rigidbody2D rb;
    [SerializeField()]
    bool Hit;
    bool canTeleport = true;
    bool hasHit, canInput = true;
    bool dead;
    RaycastHit2D hit;
    ContactFilter2D filter;
    [SerializeField()]
    LayerMask layer;
    public int points;
    public int collectedPellets;
    int lifes;
    public int Lifes
    {
        get
        {
            return lifes;
        }
        set
        {
            int temp = lifes;
            lifes = value;
            if (temp > lifes)
                Die();
        }
    }
    public bool Dead
    {
        get
        {
            return dead;
        }
        set
        {
            dead = value;
            animator.SetBool("Dead", value);
        }
    }
    public int pointMultiplier;
    int multiplier = 1;
    public int totalCollectedPellets;
    AudioSource Audio;
    public int Multiplier
    {
        get
        {
            return multiplier;
        }
        set
        {
            multiplier = value;
            animator.enabled = true;
            animator.SetFloat("Multiplier", multiplier);
        }
    }

    //Singleton
    static PacManScript pacman;
    public static PacManScript PacMan
    {
        get
        {
            if (pacman == null)
                pacman = FindObjectOfType<PacManScript>();
            return pacman;
        }
    }
    //Singleton
    #endregion

    private void Start()
    {
        Dead = false;
        Audio = GetComponent<AudioSource>();
        Multiplier = 0;
        hasHit = true;
        rb = GetComponent<Rigidbody2D>();
        filter = new ContactFilter2D();
        filter.SetLayerMask(layer);
        filter.useLayerMask = true;
        Lifes = (PlayerPrefs.GetInt("Lifes") == -1)? 2 : PlayerPrefs.GetInt("Lifes");
        startPosition = transform.position;
        points = PlayerPrefs.GetInt("Score");
                                      ;
        PlayerPrefs.SetInt("Score", 0);
    }

    private void Update()
    {
        if (direction == Vector3.zero)
            start.localPosition = new Vector3(0, 0, 0);
        else
            start.localPosition = new Vector3(0.5f, 0, 0);
        if (Multiplier == 1 || hasHit && canInput)
            InputCapture();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void InputCapture()
    {
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1)
        {
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                hit = Physics2D.Linecast(start.position, start.position + Vector3.left * distance, layer.value);
                Debug.DrawLine(start.position, start.position + Vector3.left * distance, Color.red);
                Hit = (hit.collider != null);
                if (!Hit)
                {
                    direction = Vector3.left;
                    rotation = new Vector3(0, 0, 0);
                    Hit = false;
                    Multiplier = 1;
                    hasHit = false;
                }
            }
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                hit = Physics2D.Linecast(start.position, start.position + Vector3.right * distance, layer.value);
                Debug.DrawLine(start.position, start.position + Vector3.right * distance, Color.red);
                Hit = hit.collider != null;
                if (!Hit)
                {
                    direction = Vector3.right;
                    rotation = new Vector3(0, 0, 180);
                    Hit = false;
                    Multiplier = 1;
                    hasHit = false;
                }
            }
            
        }
        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1)
        {
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                hit = Physics2D.Linecast(start.position, start.position + Vector3.down * distance, layer.value);
                Debug.DrawLine(start.position, start.position + Vector3.down * distance, Color.red);
                Hit = (hit.collider != null);
                if (!Hit)
                {
                    direction = Vector3.down;
                    rotation = new Vector3(0, 0, 90);
                    Hit = false;
                    Multiplier = 1;
                    hasHit = false;
                }
            }
            else if (Input.GetAxisRaw("Vertical") > 0)
            {
                hit = Physics2D.Linecast(start.position, start.position + Vector3.up * distance, layer.value);
                Debug.DrawLine(start.position, start.position + Vector3.up * distance, Color.red);
                Hit = hit.collider != null;
                if (!Hit)
                {
                    direction = Vector3.up;
                    rotation = new Vector3(0, 0, -90);
                    Hit = false;
                    Multiplier = 1;
                    hasHit = false;
                }
            }
        }
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void Move()
    {
        hit = Physics2D.Linecast(start.position, start.position + direction * distance, layer.value);
        Debug.DrawLine(start.position, start.position + direction * distance, Color.red);

        if (hit.collider != null)
        {
            direction = Vector3.zero;
            Multiplier = 0;
            hasHit = true;
        }

        rb.velocity = direction * velocity * Multiplier;
        //transform.position += direction * velocity * Time.fixedDeltaTime * Multiplier;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Teleporter") && canTeleport)
        {
            canTeleport = false;
            if (collision.gameObject == teleporters[0])
            {
                StartCoroutine(Teleport(teleportTime));
                transform.position = teleporters[1].transform.position;
            }
            else
            {
                StartCoroutine(Teleport(teleportTime));
                transform.position = teleporters[0].transform.position;
            }
        }
    }

    public void ActivateSprite(bool activate)
    {
        if (activate)
        {
            animator.enabled = true;
            PacmanRenderer.enabled = true;
        }
        else
        {
            animator.enabled = false;
            PacmanRenderer.enabled = false;
        }
    }

    public void Restart()
    {
        Dead = false;
        canInput = true;
        direction = Vector3.zero;
        Multiplier = 0;
        collectedPellets = 0;
        rotation = Vector3.zero;
        transform.rotation = Quaternion.Euler(rotation);
        animator.enabled = false;
        Renderer.sprite = initialSprite;
        transform.position = startPosition;
        hasHit = true;
    }

    public void Die()
    {
        canInput = false;
        Multiplier = 0;
        StartCoroutine(DieDelay(2.034f));
    }

    IEnumerator DieDelay(float time)
    {
        BlinkyScript.Blinky.speedMultiplier = 0;
        PinkyScript.Pinky.speedMultiplier = 0;
        ClydeScript.Clyde.speedMultiplier = 0;
        InkyScript.Inky.speedMultiplier = 0;
        yield return new WaitForSeconds(0.5f);
        BlinkyScript.Blinky.gameObject.SetActive(false);
        PinkyScript.Pinky.gameObject.SetActive(false);
        ClydeScript.Clyde.gameObject.SetActive(false);
        InkyScript.Inky.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Dead = true;
        Audio.Play();        
        yield return new WaitForSeconds(time);
        Audio.Stop();
        GameController.restartGame = true;
    }
    IEnumerator Teleport(float time)
    {
        yield return new WaitForSeconds(time);
        canTeleport = true;
    }
}
