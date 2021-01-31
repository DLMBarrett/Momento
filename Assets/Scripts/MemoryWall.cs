using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryWall : MonoBehaviour
{
    public int areaNumber;
    
    // Start is called before the first frame update
    void Start()
    {
        if (areaNumber == 0)
        {
            Debug.Log("ERROR: area number is unassigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "Player")
        {
            // destroy wall if player can access this area
            if (other.gameObject.GetComponent<MemoryStorage>().CanAccessArea(areaNumber))
            {
                Debug.Log("area " + areaNumber + " unlocked");
                Destroy(gameObject);
            }
        }
    }
}
