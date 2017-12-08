using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructor : MonoBehaviour
{
    public static int destoyeds = 0;
    //private void Start()
    //{
    //    Destroy(this.gameObject);
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Pellets"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            destoyeds++;
            print(destoyeds);
        }
    }
}
