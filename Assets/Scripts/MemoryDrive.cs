using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryDrive : MonoBehaviour
{
    public int keyNumber;

    private void Start()
    {
        if (keyNumber == 0)
        {
            Debug.Log("ERROR: key number is unassigned.");
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // add memory drive
            collision.gameObject.GetComponent<MemoryStorage>().AddMemoryDrive(gameObject);
            // destroy this object
            Destroy(gameObject);
        }
    }
}
