using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostScript : MonoBehaviour
{
    #region variables
    public float speedMultiplier = 0;
    public Vector3Int direction;
    protected Vector3Int Direction
    {
        get
        {
            return direction;
        }
        set
        {
            int x, y, z;

            x = (value.x != 0) ? value.x / Mathf.Abs(value.x) : 0;
            y = (value.y != 0) ? value.y / Mathf.Abs(value.y) : 0;
            z = (value.z != 0) ? value.z / Mathf.Abs(value.z) : 0;

            direction = new Vector3Int(x, y, z);

            if (Eyes.active)
                UpdateEyes(direction);
        }
    }
    public bool Frightened
    {
        get
        {
            return frightened;
        }
        set
        {
            if (value == true)
            {
                frightened = (!Dead) ? true : false;
                if (frightened == true)
                {
                    speedMultiplier = 0.5f;
                    Eyes.SetActive(false);
                    bodyAnim.SetBool("Frightened", true);
                    GameController.Controller.PlayGhostsSound(false);
                    GameController.Controller.PlayGhostsSoundF(true);
                }
            } 
            else
            {
                Eyes.SetActive(true);
                speedMultiplier = (Dead)? 2.5f : 1;
                frightened = false;
            }
            bodyAnim.SetBool("Frightened", frightened);
        }
    }
    protected bool passDoor;
    protected Vector3 target;
    protected bool dead;
    protected bool Dead
    {
        get
        {
            return dead;
        }
        set
        {
            dead = value;
            if (dead == true)
            {
                Physics2D.IgnoreCollision(coll, PacManScript.PacMan.Collider, true);
                speedMultiplier = 2.5f;
                Frightened = false;
                passDoor = true;
            }
            else
            {
                Body.SetActive(true);
                Physics2D.IgnoreCollision(coll, PacManScript.PacMan.Collider, false);
                speedMultiplier = 1;
            }
        }
    }
    bool canTeleport = true;
    protected bool gotOut = false;
    bool firstTileB;
    bool frightened, counter;
    float time;
    TileDetector currentTile, firstTile;
    Vector3 startPosition;
    Vector3Int startDirection;
    List<TileDetector> tiles;
    Rigidbody2D rb;

    [SerializeField]
    Collider2D coll;
    [SerializeField]
    float velocity;
    [SerializeField]
    float teleportTime;
    [SerializeField]
    protected Transform scatterTarget, deadTarget;
    [SerializeField]
    List<GameObject> teleporters;
    [SerializeField]
    protected Animator bodyAnim;
    [SerializeField]
    Animator eyesAnim;
    [SerializeField]
    GameObject Eyes, Body;
    [SerializeField]
    AudioSource eatGhostAudio;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    GameObject text;
    Text points;
    #endregion

    protected virtual void Start()
    {
        Physics2D.IgnoreLayerCollision((int)GameController.Layers.Ghosts, (int)GameController.Layers.PacMan, false);
        gameObject.SetActive(true);
        gotOut = false;
        tiles = new List<TileDetector>();
        rb = GetComponent<Rigidbody2D>();
        Eyes.SetActive(true);
        Body.SetActive(true);
        startPosition = transform.position;
        startDirection = Direction;
    }

    private void Update()
    {
        if (counter)
        {
            if (Time.realtimeSinceStartup - time >= 0.5f)
            {
                Destroy(points);
                PacManScript.PacMan.ActivateSprite(true);
                PacManScript.PacMan.Multiplier = 1;

                Eyes.SetActive(true);

                Time.timeScale = 1;
                counter = false;
            }
        }
    }
    
    protected void FixedUpdate()
    {
        if (Dead)
            passDoor = true;
        else if (!Dead && gotOut)
            passDoor = false;
        if (Dead && Vector3.Distance(transform.position, deadTarget.position) <= 0.1f)
        {
            passDoor = true;
            frightened = false;
            Dead = false;
            Eyes.SetActive(true);
            Body.SetActive(true);
            Direction = Vector3Int.up;
        }
        if (frightened)
        {
            Eyes.SetActive(false);
        }
        else if (!Dead)
        {
            Eyes.SetActive(true);
        }

        Walk();
    }

    void ChangeDirection()
    {
        tiles.Clear();
        tiles.AddRange(currentTile.RemoveInverseNeighbor(Direction, target, passDoor));
        if (tiles.Count == 0)
        {
            currentTile = null;
            return;
        }

        if (!frightened)
        {
            tiles.Sort();
            if (tiles.Count > 1 && tiles[0].F == tiles[1].F)
            {
                //Chooses the closest way to the target when intercepts a intersection
                if (CalculateDirection(tiles[1]) == Vector3Int.up)
                    ChangeSpots(tiles);
                else if (CalculateDirection(tiles[1]) == Vector3Int.left && CalculateDirection(tiles[0]) != Vector3Int.up)
                    ChangeSpots(tiles);
                else if (CalculateDirection(tiles[1]) == Vector3Int.down && CalculateDirection(tiles[0]) != Vector3Int.left)
                    ChangeSpots(tiles);
            }
            Direction = CalculateDirection(tiles[0]);
        }
        else
        {
            //Chooses randomly a direction when intercepts a intersection
            int index = new System.Random().Next(0, tiles.Count);
            Direction = CalculateDirection(tiles[index]);
            //it doesn't need to update its eyes because it doesn't have 'em when frightened
        }
        
    }

    Vector3Int CalculateDirection(TileDetector tile)
    {
        Vector3Int direction = Vector3Int.RoundToInt(tile.transform.position - currentTile.transform.position);
        return direction;
    }

    void Walk()
    {
        //transform.position += (Vector3)Direction * velocity * Time.fixedDeltaTime * speedMultiplier;
        rb.velocity = (Vector3)Direction * speedMultiplier * velocity;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("IntersectionADoor") && Vector3.Distance(transform.position, collision.transform.position) <= 0.08f && Direction == Vector3.up)
        {
            if (GameController.mode == GameController.Mode.Scatter)
                Direction = Vector3Int.left;
            else if (GameController.mode == GameController.Mode.Chase)
                Direction = Vector3Int.right;
            passDoor = false;
            gotOut = true;
            return;
        }
        if (collision.CompareTag("IntersectionADoor") && !Dead && Vector3.Distance(transform.position, collision.transform.position) <= 0.08f && !gotOut)
        {
            if (GameController.mode == GameController.Mode.Scatter)
                Direction = Vector3Int.left;
            else if (GameController.mode == GameController.Mode.Chase)
                Direction = Vector3Int.right;
            passDoor = false;
            gotOut = true;
            return;
        }
        else if (collision.CompareTag("IntersectionBDoor") && Dead && Vector3.Distance(transform.position, collision.transform.position) <= 0.08f)
        {
            Direction = Vector3Int.up;
            Dead = false;
            passDoor = true;
            gotOut = false;
            return;
        }
        else if (collision.CompareTag("IntersectionBDoor") && !Dead && Vector3.Distance(transform.position, collision.transform.position) <= 0.08f)
        {
            Direction = Vector3Int.up;
            passDoor = true;
            gotOut = false;
            return;
        }
        if (collision.CompareTag("Teleporter") && canTeleport)
        {
            canTeleport = false;

            if (collision.gameObject == teleporters[0])
                transform.position = teleporters[1].transform.position;
            else
                transform.position = teleporters[0].transform.position;

            StartCoroutine(TeleportTime(teleportTime));
        }

        TileDetector tile = collision.GetComponent<TileDetector>();

        if (tile != null && tile != currentTile && tile.neighbors.Count > 0 && Vector3.Distance(transform.position, collision.transform.position) <= 0.08f)
        {
            if (collision.tag.Contains("Intersection") || collision.CompareTag("GhostHouse"))
            {
                if (Mathf.Abs(Direction.x) == 1)
                    transform.position = new Vector3(tile.transform.position.x, transform.position.y);
                else if (Mathf.Abs(Direction.y) == 1)
                    transform.position = new Vector3(transform.position.x, tile.transform.position.y);
                currentTile = tile;
                if (firstTileB)
                {
                    firstTile = currentTile;
                    firstTileB = false;
                }
                ChangeDirection();
            }
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject. CompareTag("PacMan") && frightened)
        {
            eatGhostAudio.Play();
            passDoor = true;
            frightened = false;
            Dead = true;
            int scoreBefore = PacManScript.PacMan.points;
            PacManScript.PacMan.points += 200 * PacManScript.PacMan.pointMultiplier;
            int givenPoints = PacManScript.PacMan.points - scoreBefore;
            PacManScript.PacMan.pointMultiplier *= 2;
            Eyes.SetActive(false);
            Body.SetActive(false);

            PacManScript.PacMan.ActivateSprite(false);
            Time.timeScale = 0;
            time = Time.realtimeSinceStartup;

            points = Instantiate(text, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
            points.text = givenPoints.ToString();
            counter = true;
        }
        else if (collision.gameObject.CompareTag("PacMan") && !frightened && !Dead)
        {
            eatGhostAudio.Play();
            PacManScript.PacMan.Lifes--;
            GameController.Controller.PlayGhostsSound(false);
            GameController.Controller.PlayGhostsSoundF(false);
            Physics2D.IgnoreLayerCollision((int)GameController.Layers.Ghosts, (int)GameController.Layers.PacMan, true);
        }
    }

    public void InvertDirection()
    {
        if (gotOut && !Dead)
        {
            currentTile = null;
            Direction *= -1;
        }     
    }

    public void Restart()
    {
        speedMultiplier = 0;
        Direction = startDirection;
        currentTile = firstTile;
        transform.position = startPosition;
        speedMultiplier = 1;
        Dead = false;
        Frightened = false;
        passDoor = false;
        gotOut = false;
        StopAllCoroutines();
        Start();
    }

    private void UpdateEyes(Vector3Int direction)
    {
        eyesAnim.SetInteger("XAxis", direction.x);
        eyesAnim.SetInteger("YAxis", direction.y);
    }

    void ChangeSpots<T>(List<T> list)
    {
        T temp = list[0];
        list[0] = list[1];
        list[1] = temp;
    }

    IEnumerator TeleportTime(float time)
    {
        yield return new WaitForSeconds(time);
        canTeleport = true;
    }

    IEnumerator DeathDelay(int time)
    {
        eatGhostAudio.Play();
        passDoor = true;
        frightened = false;
        Dead = true;
        int scoreBefore = PacManScript.PacMan.points;
        PacManScript.PacMan.points += 200 * PacManScript.PacMan.pointMultiplier;
        int givenPoints = PacManScript.PacMan.points - scoreBefore;
        PacManScript.PacMan.pointMultiplier *= 2;
        Eyes.SetActive(false);
        Body.SetActive(false);

        //BlinkyScript.Blinky.speedMultiplier = 0;
        //PinkyScript.Pinky.speedMultiplier = 0;
        //ClydeScript.Clyde.speedMultiplier = 0;
        //InkyScript.Inky.speedMultiplier = 0;
        //PacManScript.PacMan.Multiplier = 0;
        PacManScript.PacMan.ActivateSprite(false);
        Time.timeScale = 0;

        points = Instantiate(text, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
        points.text = givenPoints.ToString();
        yield return new WaitForSeconds(time);
        Destroy(points);
        PacManScript.PacMan.ActivateSprite(true);
        PacManScript.PacMan.Multiplier = 1;

        Eyes.SetActive(true);

        Time.timeScale = 1;
        //BlinkyScript.Blinky.speedMultiplier = 1;
        //PinkyScript.Pinky.speedMultiplier = 1;
        //ClydeScript.Clyde.speedMultiplier = 1;
        //InkyScript.Inky.speedMultiplier = 1;
    }
}