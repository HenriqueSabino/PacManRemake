using System;
using System.Collections.Generic;
using UnityEngine;

public class TileDetector : MonoBehaviour, IComparable<TileDetector>
{
    [SerializeField]
    GameObject pellet, superPellet;

    public List<TileDetector> neighbors;
    public float F, H;
    const float G = 10;

    private void Start()
    {
        neighbors = new List<TileDetector>();
        if (gameObject.name.Contains("SuperPellet"))
            Instantiate(superPellet, transform.position, Quaternion.identity);
        else if (gameObject.name.Contains("Pellet"))
            Instantiate(pellet, transform.position, Quaternion.identity);
    }

    public void CalculateDistanceValue(Vector3 target)
    {
        H = Vector3.Distance(transform.position, target);

        F = G + H;
        F = (float)Math.Round(F, 3);
    }

    public List<TileDetector> RemoveInverseNeighbor(Vector3Int direction, Vector3 target, bool includeDoor)
    {
        List<TileDetector> tiles = new List<TileDetector>();
        
        foreach (var neighbor in neighbors)
        {
            if (direction != Vector3Int.zero)
            {
                Vector3Int result, absResult;
                absResult =  result = Vector3Int.RoundToInt(transform.localPosition - neighbor.transform.localPosition);
                //Getting the absolute value of the vector
                if (result.x < 0)
                    absResult *= new Vector3Int(-1, 1, 0);
                else if (result.y < 0)
                    absResult *= new Vector3Int(1, -1, 0);

                DivideVectors(ref result, absResult);

                if (result == direction)
                    continue;
            }
            neighbor.CalculateDistanceValue(target);
            if (neighbor.gameObject.CompareTag("Door") && includeDoor)
                tiles.Add(neighbor);
            else if (!neighbor.gameObject.CompareTag("Door"))
                tiles.Add(neighbor);
        }
        return tiles;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Path") || collision.gameObject.CompareTag("GhostHouse") || collision.gameObject.CompareTag("Door") || collision.gameObject.tag.Contains("Intersection")) && collision.bounds.size.x > 0.3f && neighbors.Find(x => x.transform == collision.transform) == null)
        {
            neighbors.Add(collision.gameObject.GetComponent<TileDetector>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Path") && neighbors.Contains(collision.gameObject.GetComponent<TileDetector>()))
        {
            neighbors.Remove(collision.gameObject.GetComponent<TileDetector>());
        }
    }

    public int CompareTo(TileDetector other)
    {
        if (this.F < other.F)
            return this.F.CompareTo(other.F);
        else
            return other.F.CompareTo(other.F);
    }

    /// <summary>
    /// Divide 2 vectors3Int
    /// </summary>
    /// <param name="a">The vector that will be divided</param>
    /// <param name="b">The divisor vector</param>
    public static void DivideVectors(ref Vector3Int a, Vector3Int b)
    {
        int x = 0, y = 0, z = 0;

        x = (b.x != 0) ? b.x : 1; y = (b.y != 0) ? b.y : 1; z = (b.z != 0) ? b.z : 1;

        b = new Vector3Int(x, y, z);

        a = new Vector3Int((a.x / b.x), (a.y / b.y), 0);
    }
}
