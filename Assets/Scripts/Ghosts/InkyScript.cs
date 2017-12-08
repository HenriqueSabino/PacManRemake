using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkyScript : GhostScript
{
    //Singleton
    static InkyScript inky;
    public static InkyScript Inky
    {
        get
        {
            if (inky == null)
                inky = FindObjectOfType<InkyScript>();
            return inky;
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
        if (PacManScript.PacMan.collectedPellets >= 30 || GameController.level > 1)
        {
            if (dead)
                target = deadTarget.position;
            else if (GameController.mode == GameController.Mode.Scatter)
                target = scatterTarget.position;
            else
                target = (2 * PacManScript.PacMan.transform.position) - BlinkyScript.Blinky.transform.position;
            base.FixedUpdate();
        }
        else
            Direction = Vector3Int.right;
    }
}
