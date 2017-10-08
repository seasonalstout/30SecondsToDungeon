using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Arrow : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Can handle enemy arrows differently
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<IEnemy>().TakeDamage(1);
            
        }
        //remove no clip
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
            gameObject.GetComponent<AudioSource>().Play();
            return;
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        //remove no clip
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<IEnemy>().TakeDamage(1);
            }
        }
    }
}