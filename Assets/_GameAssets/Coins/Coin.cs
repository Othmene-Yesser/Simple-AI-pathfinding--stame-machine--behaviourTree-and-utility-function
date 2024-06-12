using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    GameManager manager;
    public bool Collected;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !Collected)
        {
            Collected= true;
            var meshRenderer = this.GetComponent<MeshRenderer>();
            meshRenderer.material.color = Color.green;
            manager.CheckIfFinishedGame();
        }
    }
}
