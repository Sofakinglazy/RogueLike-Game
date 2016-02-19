using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int max;
        public int min;

        public Count(int max, int min)
        {
            this.max = max;
            this.min = min;
        }
    }

    public int width = 8;
    public int height = 8;

    public GameObject exit;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject[] foodTiles;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] enemyTiles;

    private Transform boardHolder;
    private List<Vector3> gridHolder = new List<Vector3>();

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < width + 1; x++)
        {
            for (int y = -1; y < height + 1; y++)
            {
                GameObject toInstantiate = PickRandomTile( 
                    (x == -1 || x == width || y == -1 || y == height) ? outerWallTiles : floorTiles);
                
                GameObject instance =
                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);

            }
        }
    }

    GameObject PickRandomTile(GameObject[] tiles)
    {
        return tiles[Random.Range(0, tiles.Length)];
    }

    void InitialiseList()
    {
        gridHolder.Clear();
        
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                gridHolder.Add(new Vector3(x, y, 0f));
            }
        } 
    }

    void LayoutObjectAtRandomPos(GameObject[] tiles, int min, int max)
    {
        int objectCount = Random.Range(min, max);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPos = RandomPosition();
            GameObject pickedTile = tiles[Random.Range(0, tiles.Length)];
            Instantiate(pickedTile, randomPos, Quaternion.identity);
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridHolder.Count);
        Vector3 pickedPos = gridHolder[randomIndex];
        gridHolder.RemoveAt(randomIndex);
        return pickedPos;
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandomPos(foodTiles, foodCount.min, foodCount.max);
        LayoutObjectAtRandomPos(wallTiles, wallCount.min, wallCount.max);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandomPos(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(width - 1, height - 1, 0f), Quaternion.identity);

    }


}
