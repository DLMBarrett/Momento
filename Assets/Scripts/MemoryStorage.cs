using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryStorage : MonoBehaviour
{
    private List<MemoryDrive> memoryDrives;

    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        memoryDrives = new List<MemoryDrive>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public bool CanAccessArea(int areaKey)
    {
        foreach (MemoryDrive drive in memoryDrives)
        {
            if (drive.keyNumber == areaKey) return true;
        }

        return false;
    }
    
    public void AddMemoryDrive(GameObject driveObject)
    {
        MemoryDrive drive = driveObject.GetComponent<MemoryDrive>();
        memoryDrives.Add(drive);
        Debug.Log("added drive with key " + drive.keyNumber);
        audioSource.Play();
    }
}
