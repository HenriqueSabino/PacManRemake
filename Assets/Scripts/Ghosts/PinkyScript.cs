using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkyScript : GhostScript
{
    //Singleton
    static PinkyScript pinky;
    public static PinkyScript Pinky
    {
        get
        {
            if (pinky == null)
                pinky = FindObjectOfType<PinkyScript>();
            return pinky;
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
        if (dead)
            target = deadTarget.position;
        else if (GameController.mode == GameController.Mode.Scatter)
            target = scatterTarget.position;
        else
        {
            if (PacManScript.PacMan.direction != Vector3Int.up)
                target = PacManScript.PacMan.transform.position + PacManScript.PacMan.direction * 1.28f * 2f;
            else
                target = PacManScript.PacMan.transform.position + new Vector3(-1, 1, 0) * 1.28f * 2f;
            
        }
        base.FixedUpdate();
    }
}
