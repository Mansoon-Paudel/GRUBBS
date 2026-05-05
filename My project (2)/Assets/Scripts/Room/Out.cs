using System;
using Unity.VisualScripting;
using UnityEngine;

public class Out : MonoBehaviour
{
    private UIManager UIManager;

    private void Start()
    {
        UIManager = FindObjectOfType<UIManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if  (collision.tag == "Player")
        {
            UIManager.GameOver();
        }
    }
}