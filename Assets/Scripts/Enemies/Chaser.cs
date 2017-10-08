using UnityEngine;
using System.Collections;
using System;

public class Chaser : MonoBehaviour, IEnemy
{
    public int hitPoints;
    public bool ChasePlayer { get; set; }
    private int currentHp;

    private float moveSpeed = 1.0f;

    GameObject player;
    private Vector3 spawnLocation;
    Rigidbody rigidBody;
    private Timer timer;

    public int deathCount = 2;
    


    public void Awaken()
    {
        ChasePlayer = true;
    }

    public void Sleepen()
    {
        ChasePlayer = false;
        transform.position = spawnLocation;

    }

    private void Start() {
        currentHp = hitPoints;
        rigidBody = gameObject.GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        timer = player.GetComponent<Controls>().getTimer();
        spawnLocation = gameObject.transform.position;

        
    }

    public void SetDeathCount(int deathCount)
    {
        this.deathCount = deathCount;
    }

    private void Update()
    {
        if (ChasePlayer) {
            transform.LookAt(new Vector3(player.transform.position.x,
                                        transform.position.y,
                                        player.transform.position.z),
                                        Vector3.up);

            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    public void PerformAttack() {

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

    public void TakeDamage(int amount) {
        currentHp -= amount;
        StartCoroutine(damaged());
        if (currentHp <= 0)
            Die();
    }

    public void Die() {
//This is now only a boss so no need to add time
        if (deathCount >= 1)
        {
            GameObject cloneOne = Instantiate(gameObject, transform.position, transform.rotation, transform.parent);
            GameObject cloneTwo = Instantiate(gameObject, transform.position, transform.rotation, transform.parent);

            cloneOne.transform.localScale *= 0.5f;
            cloneTwo.transform.localScale *= 0.5f;

            cloneOne.transform.position = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z + 1f);
            cloneTwo.transform.position = new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z - 1f);

            cloneOne.GetComponent<Chaser>().Awaken();
            cloneTwo.GetComponent<Chaser>().Awaken();

            cloneOne.GetComponent<Chaser>().SetDeathCount(deathCount - 1);
            cloneTwo.GetComponent<Chaser>().SetDeathCount(deathCount - 1);

        }

        Destroy(gameObject);
    }
}