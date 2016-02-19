using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public int wallDamage = 1;

    private Animator playerAnimator;
    private int food;

    private Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip gameOverSound;
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    private Vector2 touchOrigin = -Vector2.one;

    protected override void Start()
    {
        playerAnimator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoint;
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        foodText.text = "Food: " + food;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoint = food;
    }

    private void Update()
    {
        if (!GameManager.instance.playerTurn) return;

        int horizontal = 0;
        int vertical = 0;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (vertical != 0) horizontal = 0;
        if (horizontal != 0) vertical = 0;
#else
        if (Input.touchCount > 1)
        {
            Touch myTouch = Input.touches[0];
            if (myTouch.phase == TouchPhase.Began)
                touchOrigin = myTouch.position;
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin = -Vector2.one;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    horizontal = x > 0 ? 1 : -1;
                else
                    vertical = y > 0 ? 1 : -1;
            }
        }
#endif

        if (vertical != 0 || horizontal != 0)
            AttemptMove<Wall>(horizontal, vertical);

    }


    protected override void AttemptMove<T>(int xDis, int yDis)
    {
        food--;
        foodText.text = "Food: " + food;
        base.AttemptMove<T>(xDis, yDis);
        RaycastHit2D hit;
        if (Move(xDis, yDis, out hit))
            SoundManager.instance.RandomiseSfx(moveSound1, moveSound2);
        CheckIfGameOver();
        GameManager.instance.playerTurn = false;

    }

    public void LossFood(int loss)
    {
        playerAnimator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "Food: " + food;
        CheckIfGameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "Food: " + food;
            other.gameObject.SetActive(false);
            SoundManager.instance.RandomiseSfx(eatSound1, eatSound2);
        }
        if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "Food: " + food;
            other.gameObject.SetActive(false);
            SoundManager.instance.RandomiseSfx(drinkSound1, drinkSound2);
        }
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false; // disable player object
        }
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            GameManager.instance.GameOver();
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        playerAnimator.SetTrigger("playerAttack");
        SoundManager.instance.RandomiseSfx(chopSound1, chopSound2);
    }

    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
        Debug.Log("Invoke Restart");
    }
}
