using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    private BoardManager boardManager;

    public int playerFoodPoint = 100;
    [HideInInspector] public bool playerTurn = true;

    private List<Enemy> enemies;
    private bool enemiesMoving;
    private float turnUpDelay = 0.1f;

    private int level = 1;
   
    private Text levelText;
    private GameObject levelImage;
    private bool doingSetUp;
    private float levelStartDelay = 2f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        boardManager = GetComponent<BoardManager>();
        enemies = new List<Enemy>();
        InitGame();
    }

    void InitGame()
    {
        doingSetUp = true;

        levelImage = GameObject.Find("LevelScreen");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardManager.SetupScene(level);
    }

    private void HideLevelImage()
    {
        doingSetUp = false;
        levelImage.SetActive(false);
    }

    IEnumerator MovingEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnUpDelay);
        if (enemies.Count == 0)
            yield return new WaitForSeconds(turnUpDelay);
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playerTurn = true;
        enemiesMoving = false;
    }

    void Update()
    {
        if (playerTurn || enemiesMoving || doingSetUp)
            return;
        StartCoroutine(MovingEnemies());
    }

    private void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }

    public void GameOver()
    {
        levelImage.SetActive(true);
        levelText.text = "You die!";
        enabled = false;
    }

    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }
}
