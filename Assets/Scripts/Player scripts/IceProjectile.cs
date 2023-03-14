using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectile : Projectile
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        audio.PlayOneShot(clips[1]);
        if (collision.CompareTag("Player"))
        {
            print("this is working");
            collision.GetComponent<PlayerController>().takeIceDamage(damage, owner);
        }
        transform.position = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
        Destroy(gameObject);
    }









}
