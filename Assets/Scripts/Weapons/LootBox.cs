using UnityEngine;
using System.Collections;
using System;

public class LootBox : MonoBehaviour
{

    GameObject player;
    private Timer timer;
    public GameObject[] weapons;
    private Vector3 spawnLocation;
    private bool isOpen = false;
    public bool isFinalChest = false;

    private Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        timer = player.GetComponent<Controls>().getTimer();
        spawnLocation = gameObject.transform.position;
    }


    private void SpitOutWeapon()
    {
        GameObject weapon = Instantiate(weapons[UnityEngine.Random.Range(0, weapons.Length)], transform.position, transform.rotation);
        weapon.name = weapon.name.Replace("(Clone)", "");
        weapon.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        StartCoroutine(ThrowWeapon(weapon));
    }

    private void SpitOutConfetti()
    {


        GameObject weapon = Instantiate(weapons[UnityEngine.Random.Range(0, weapons.Length)], transform.position, new Quaternion());
        weapon.name = weapon.name.Replace("(Clone)", "");
        weapon.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    public IEnumerator ThrowWeapon(GameObject weapon)
    {
        weapon.GetComponent<BoxCollider>().enabled = false;
        Vector3 position = weapon.transform.position;
        int lerpTime = 40;
        for (int i = 0; i < lerpTime; i++)
        {
            yield return new WaitForSeconds(0.001f);
            if (i < (Mathf.Abs(lerpTime / 2)))
                position.y += 0.1f;
            else
                position.y -= 0.08f;

            position.z -= 0.04f;
            weapon.transform.position = position;
        }
        weapon.GetComponent<BoxCollider>().enabled = true;
    }

    public void Update()
    {
        if (isFinalChest)
        {
            int nFound = 0;
            foreach (Transform child in transform.parent)
                if (child.CompareTag("Enemy"))
                {
                    nFound++;
                }
            if (nFound < 1)
            {
                if (!isOpen)
                {
                    isOpen = true;
                    timer.EndGame(true);
                    StartCoroutine(Opening());
                }
            }
        }
    }

    public void Open()
    {

    if (!isOpen && !isFinalChest)
    {
        isOpen = true;
        StartCoroutine(Opening());
    }
    }

    public IEnumerator Opening()
    {
        animator.SetTrigger("open");
        yield return new WaitForSeconds(0.8f);
        if (isFinalChest)
            SpitOutConfetti();
        else
            SpitOutWeapon();
    }


}