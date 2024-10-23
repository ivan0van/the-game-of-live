using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    int aliveNeighboors = 0;
    bool isAlive = false;

    public void UpdateAlive(bool alive) {
        isAlive = alive;
        GetComponent<SpriteRenderer>().enabled = alive;
    }

    public void AddNeighboor() { ++aliveNeighboors; }

    public bool GetIsAlive() { return isAlive; }

    public bool ChangeState() {
        bool res = false;
        if (!isAlive && aliveNeighboors == 3) {
            res = true;
        }
        if (isAlive && aliveNeighboors == 2 || aliveNeighboors == 3) {
            res = true;
        }
        aliveNeighboors = 0;
        UpdateAlive(res);
        return res;
    }
}
