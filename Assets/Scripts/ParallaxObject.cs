using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    // 0.0f (moves with foreground) to 1.0f (background, static)
    [Range(0.0f, 1.0f)] [SerializeField] private float intensity;
    private Vector3 parallaxObjectStartingPosition;
    
    // TODO remove this once we have a global camera variable
    private static GameObject mainCamera;
    private static Vector3 cameraStartingPosition;
    
    void Start()
    {
        // TODO remove this once we have a global camera variable
        if (!mainCamera)
        {
            mainCamera = GameObject.Find("Main Camera");

            cameraStartingPosition = mainCamera.transform.position;
        }
        
        parallaxObjectStartingPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraRelativePosition = mainCamera.transform.position - cameraStartingPosition;
        this.transform.position = parallaxObjectStartingPosition + cameraRelativePosition * intensity;
    }
}
