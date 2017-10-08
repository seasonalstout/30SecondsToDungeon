using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Crossbow : MonoBehaviour, IWeapon
{
    public bool isEquiped { get; set; }
    public bool isOffHand { get; set; }
    private Animator animator;
    public int damage { get; set; }
    private GameObject player;
    bool isAttacking = false;

    public GameObject arrow;

    public bool canHitPlayer { get; set; }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();


    }

    private void Update()
    {
        if (!isEquiped)
        {
            transform.Rotate(0f, 1f, 1f, Space.Self);
        }
    }

    public IEnumerator AttackTimer()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.1f);

        animator.SetTrigger("ShootCrossbow");
        yield return new WaitForSeconds(1.0f);
        gameObject.GetComponent<AudioSource>().Play();
        Vector3 spawn = gameObject.transform.position;
        spawn.y += 1.5f;
        

        GameObject projectile = Instantiate(arrow, spawn, player.transform.rotation);
        projectile.GetComponent<Rigidbody>().AddForce(player.transform.forward * 0.00008f);

        Destroy(projectile.gameObject, 1);
        isAttacking = false;

    }

    public void PerformAttack()
    {
        if (!isAttacking)
            StartCoroutine(AttackTimer());

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Enemy"))
        {
            other.GetComponent<IEnemy>().TakeDamage(1);
        }
    }

    public void EquipWeapon(PlayerWeaponController weaponController)
    {
        isEquiped = true;
        transform.localScale -= new Vector3(0.5F, 0.5F, 0.5F);
        transform.Rotate(0, 0, 0);
        weaponController.EquipWeapon(this);
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    public string GetName()
    {
        return name;
    }
}