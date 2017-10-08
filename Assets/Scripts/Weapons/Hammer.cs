using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Hammer : MonoBehaviour, IWeapon
{
    public bool isEquiped{ get; set;}
    public bool isOffHand { get; set; }
    private Animator animator;
    public int damage { get; set;}

    public bool canHitPlayer { get; set; }

    private void Start()
    {
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
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<BoxCollider>().enabled = true;
        animator.SetTrigger("SwingHammer");
        gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.6f);
        gameObject.GetComponent<BoxCollider>().enabled = false;

    }

    //TODO Animation for adding time
    public void PerformAttack() {
        StartCoroutine(AttackTimer());

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Enemy")) {
            other.GetComponent<IEnemy>().TakeDamage(1);
        }
    }

    public void EquipWeapon(PlayerWeaponController weaponController)
    {
        isEquiped = true;
        //animator.SetTrigger("equip");
        weaponController.EquipWeapon(this);
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    public string GetName()
    {
        return name;
    }
}