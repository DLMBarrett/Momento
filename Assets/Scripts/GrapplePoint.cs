using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        player.GetComponentInChildren<Grapple>().currentGrapplePoint = null;
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, player.GetComponentInChildren<Grapple>().range);
        for(int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i] == player.GetComponent<Collider2D>())
            {
                if(player.GetComponentInChildren<Grapple>().currentGrapplePoint != null)
                {
                    if(player.GetComponentInChildren<Grapple>().GetDistanceToThis(player.GetComponentInChildren<Grapple>().currentGrapplePoint.position) > player.GetComponentInChildren<Grapple>().GetDistanceToThis(transform.position))
                    {
                        player.GetComponentInChildren<Grapple>().currentGrapplePoint = transform;
                    }
                }
                else
                {
                    player.GetComponentInChildren<Grapple>().currentGrapplePoint = transform;
                }
            }
        }
    }
}
