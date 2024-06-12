using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //! Stun
            var player = other.GetComponent<PlayerManager>();
            player.StunPlayer();
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "UtilityFunction")
        {
            Destroy(this.gameObject);
        }
    }
}
