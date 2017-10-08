using UnityEngine;
using System.Collections;
using System;

public class SkeletonArcher : MonoBehaviour, IEnemy
{
    public int hitPoints = 2;
    public bool ChasePlayer { get; set; }
    private int currentHp;

    bool isAttacking, isDead = false;
    bool hasAwoken = false;

    GameObject player;
    private Vector3 spawnLocation;
    Rigidbody rigidBody;
    private Timer timer;

    private float attackTimer = 1.0f;
    private Animator animator;

    public Rigidbody arrow;


    public void Awaken()
    {
        if (!hasAwoken)
            Utils.playEnemyAudioClip(Utils.enemyName.Hammer, Utils.clipType.awake);
        hasAwoken = true;
        ChasePlayer = true;
    }

    public void Sleepen()
    {
        ChasePlayer = false;
        transform.position = spawnLocation;

    }

    private void Start()
    {
        ChasePlayer = false;
        currentHp = hitPoints;
        animator = gameObject.GetComponent<Animator>();
        rigidBody = gameObject.GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        timer = player.GetComponent<Controls>().getTimer();
        spawnLocation = gameObject.transform.position;


    }


    private void Update()
    {
        if (ChasePlayer)
        {
            transform.LookAt(new Vector3(player.transform.position.x,
                                        transform.position.y,
                                        player.transform.position.z),
                                        Vector3.up);
            attackTimer -= Time.deltaTime;

            var distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < 15.0f)
            {
                if (attackTimer <= 0.0f)
                {
                    attackTimer = 1.0f;
                    ShootBow();
                }
            }
        }
    }

    public IEnumerator WaitForAttack()
    {
        ChasePlayer = false;
        yield return new WaitForSeconds(2.0f);
        if (!isDead)
        {
            Utils.playEnemyAudioClip(Utils.enemyName.Archer, Utils.clipType.attack);
            Vector3 spawn = gameObject.transform.position;
            spawn.y += 1f;
            Rigidbody projectile = Instantiate(arrow, spawn, gameObject.transform.rotation);
            projectile.AddForce(gameObject.transform.forward * 0.00008f);
            yield return new WaitForSeconds(0.2f);
            Destroy(projectile.gameObject, 1);
        }
        ChasePlayer = true;
    }

    public void ShootBow()
    {
        if (!isAttacking)
        {
            animator.SetTrigger("attack");
            StartCoroutine(WaitForAttack());
        }
    }

    

    public void PerformAttack()
    {
    }

    public IEnumerator damaged()
    {
        Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
        Color normalColor = renderer.material.color;
        for (int i = 0; i < 2; i++)
        {
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(.1f);
            renderer.material.color = normalColor;
            yield return new WaitForSeconds(.1f);
        }
        renderer.material.color = normalColor;
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        StartCoroutine(damaged());
        if (currentHp <= 0)
            StartCoroutine(Die());
    }



    public IEnumerator Die()
    {
        Utils.playEnemyAudioClip(Utils.enemyName.Hammer, Utils.clipType.die);

        isDead = true;
        animator.SetTrigger("die");
        ChasePlayer = false;
        BoxCollider[] colliders = gameObject.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider col in colliders)
            col.enabled = false;
        timer.AddTime(3.0f, gameObject);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}