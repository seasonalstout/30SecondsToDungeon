using UnityEngine;
using System.Collections;
using System;

public class SkeletonHammer : MonoBehaviour, IEnemy
{
    public int hitPoints = 2;
    public bool ChasePlayer { get; set; }
    private int currentHp;

    private float moveSpeed = 2f;

    GameObject player;
    private Vector3 spawnLocation;
    Rigidbody rigidBody;
    private Timer timer;
    bool hasAwoken = false;

    private Animator animator;


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
            var distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < 5.0f) {
                PerformAttack();
            }
             else if (distance < 15.0f) {
                transform.LookAt(new Vector3(player.transform.position.x,
                                        transform.position.y,
                                        player.transform.position.z),
                                        Vector3.up);
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
             }
        }
    }

    public IEnumerator Attacking()
    {
        ChasePlayer = false;
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(7.8f);
        ChasePlayer = true;
        //immune = true;
    }

    public void PerformAttack()
    {
        StartCoroutine(Attacking());
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
        //if (!immune)
        // {
        StartCoroutine(damaged());
        currentHp -= amount;
        if (currentHp <= 0)
            StartCoroutine(Die());
        //   }
    }

    public IEnumerator Die()
    {
        animator.SetTrigger("die");
        Utils.playEnemyAudioClip(Utils.enemyName.Hammer, Utils.clipType.die);

        ChasePlayer = false;
        BoxCollider[] colliders = gameObject.GetComponentsInChildren<BoxCollider>();
        foreach( BoxCollider col in colliders)
            col.enabled = false;
        timer.AddTime(3.0f, gameObject);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}