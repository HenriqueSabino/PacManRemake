using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyScript : GhostScript
{
    //Singleton
    static GhostScript blinky;
    public static GhostScript Blinky
    {
        get
        {
            if (blinky == null)
                blinky = FindObjectOfType<BlinkyScript>();
            return blinky;
        }
    }
    //Singleton

    new void Start()
    {
        passDoor = false;
        gotOut = true;
        base.Start();   
    }

    new void FixedUpdate()
    {
        bodyAnim.SetFloat("Multiplier", speedMultiplier);
        if (dead)
            target = deadTarget.position;
        else if (GameController.mode == GameController.Mode.Scatter)
            target = scatterTarget.position;
        else
            target = PacManScript.PacMan.transform.position;
        base.FixedUpdate();
    }
}
