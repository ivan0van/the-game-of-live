using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    const int ScreenWeight = 240;
    const int ScreenHeight = 135;

    [SerializeField] public Slider speedSlider;
    
    Cell[,] cells = new Cell[ScreenWeight, ScreenHeight];
    float lastUpdateTime;
    float timeBetweenUpdates = 1f;
    bool isPause = true;
    int probability = 30;
    

    void Start() {
        InitiateAll();
        speedSlider.onValueChanged.AddListener(ChangeSpeed);
    }

    void ChangeSpeed(float newSpeed) {
        timeBetweenUpdates = 1 / newSpeed;
    }
    void InitiateAll() {
        for (int x = 0; x < ScreenWeight; ++x) {
            for (int y = 0; y < ScreenHeight; ++y) {
                Cell cell = Instantiate(
                    Resources.Load("Prefabs/Cell", typeof(Cell)), new Vector2 (x, y),  Quaternion.identity
                ) as Cell;
                cells[x, y] = cell;
                cell.UpdateAlive(false);
            }
        }
        lastUpdateTime = Time.time;
    }

    void MakeLiveByMap(bool[,] aliveMap) {
        for (int x = 0; x < ScreenWeight; ++x) {
            for (int y = 0; y < ScreenHeight; ++y) {
                cells[x, y].UpdateAlive(aliveMap[x, y]);
            }
        }
    }

    void UpdateByInput() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 clickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int clickX = Mathf.RoundToInt(clickPoint.x);
            int clickY = Mathf.RoundToInt(clickPoint.y);
            if (
                !(
                    0 <= clickX && clickX < ScreenWeight
                    && 0 <= clickY && clickY < ScreenHeight
                )
            ) return;
            cells[clickX, clickY].UpdateAlive(!cells[clickX, clickY].GetIsAlive());
        }
        if (Input.GetKeyUp(KeyCode.P)) isPause = !isPause;
        if (Input.GetKeyUp(KeyCode.R)) RandomGeneratation();
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            timeBetweenUpdates = Mathf.Min(1e9f, timeBetweenUpdates + 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            timeBetweenUpdates = Mathf.Max(0f, timeBetweenUpdates - 0.1f);
        }
    }

    void Update() {
        if (!isPause) {
            if (Time.time - lastUpdateTime > timeBetweenUpdates) {
                SimulationIteration();
                lastUpdateTime = Time.time;
            }
        }
        UpdateByInput();
    }

    void SimulationIteration() {
        ChangeAllNeighboors();
        changeAllAliveness();
    }

    void ChangeAllNeighboors() {
        for (int x = 0; x < ScreenWeight; ++x) {
            for (int y = 0; y < ScreenHeight; ++y) {
                for (int dx = -1; dx <= 1; ++dx) {
                    for (int dy = -1; dy <= 1; ++dy) {
                        if (dy == 0 && dx == 0) continue;
                        int neighboorX = (x + dx + ScreenWeight) % ScreenWeight;
                        int neighboorY = (y + dy + ScreenHeight) % ScreenHeight; 
                        if (cells[x, y].GetIsAlive()) cells[neighboorX, neighboorY].AddNeighboor();
                    }
                }
            }
        }
    }

    void changeAllAliveness() {
        for (int x = 0; x < ScreenWeight; ++x) {
            for (int y = 0; y < ScreenHeight; ++y) {
                cells[x, y].ChangeState();
            }
        }
    }

    bool RandomGenerateCellAliveness(int probability) {
        int rand = UnityEngine.Random.Range(0, 100);
        return rand > probability;
    }

    bool[,] GetRandomAlives(int probability) {
        bool[,] res = new bool[ScreenWeight, ScreenHeight];
        for (int x = 0; x < ScreenWeight; ++x) {
            for (int y = 0; y < ScreenHeight; ++y) {
                res[x, y] = RandomGenerateCellAliveness(probability);
            }
        }
        return res;
    }

    void RandomGeneratation() {
        bool[,] aliveMap = GetRandomAlives(probability);
        MakeLiveByMap(aliveMap);
    }
}
