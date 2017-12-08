using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClydeScript : GhostScript
{
    //Singleton
    static ClydeScript clyde;
    public static ClydeScript Clyde
    {
        get
        {
            if (clyde == null)
                clyde = FindObjectOfType<ClydeScript>();
            return clyde;
        }
    }
    //Singleton

    protected override void Start()
    {
        passDoor = true;
        gotOut = false;
        base.Start();
    }

    new void FixedUpdate()
    {
        bodyAnim.SetFloat("Multiplier", speedMultiplier);
        
        if (PacManScript.PacMan.collectedPellets >= 81 || GameController.level > 2)
        {
            float distance;
            Vector3 result = new Vector2(Mathf.Abs(PacManScript.PacMan.transform.position.x - transform.position.x), (Mathf.Abs(PacManScript.PacMan.transform.position.y - transform.position.y) - 2));

            distance = result.x + result.y;

            if (dead)
                target = deadTarget.position;
            else if (distance <= 4 || GameController.mode == GameController.Mode.Scatter)
                target = scatterTarget.position;
            else
                target = PacManScript.PacMan.transform.position;

            base.FixedUpdate();
        }
        else
            Direction = Vector3Int.left;

    }
}
