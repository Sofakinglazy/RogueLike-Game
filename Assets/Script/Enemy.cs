using UnityEngine;
using System.Collections;
using System;

public class Enemy : MovingObject {

    public int playerDamage = 20;

    private Animator enemyAnimator;
    private Transform target;
    private bool skipMove;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        enemyAnimator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();

    }

    protected override void AttemptMove<T>(int xDis, int yDis)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDis, yDis);
        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDis = 0;
        int yDis = 0;

        if (Mathf.Abs(transform.position.x - target.position.x) < float.Epsilon)
            yDis = transform.position.y > target.position.y ? -1 : 1;
        else
            xDis = transform.position.x > target.position.x ? -1 : 1;
        AttemptMove<Player>(xDis, yDis);
    }


    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        hitPlayer.LossFood(playerDamage);
        enemyAnimator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomiseSfx(enemyAttack1, enemyAttack2);
    }
}
