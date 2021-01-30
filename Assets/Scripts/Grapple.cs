using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D player;
    [HideInInspector] public DistanceJoint2D distJoint;
    [HideInInspector] public TargetJoint2D targetJoint;
    public LayerMask grappleable, playerLayer;
    public LineRenderer line;

    [Header("Stats")]
    public float range = 20f;
    public float launchForce = 2f;

    [Header("Input")]
    public int grappleMouseButton = 0;
    [HideInInspector] public bool grappleMouseButtonDown, grappleMouseButtonUp;
    public int launchMouseButton = 1;
    [HideInInspector] public bool launchMouseButtonHeld;

    [Header("Animation")]
    public int resolution = 40, waveCount = 4;
    public float waveSize = 1;
    [Range(0, 1)] public float animSpeed = 0.5f;
    [Range(0, 1)] public float ropeSpeed = 0.75f;
    [HideInInspector] public float currentWaveSize;
    [HideInInspector] public bool lineStraight;
    [HideInInspector] public Vector2 ropeEnd;

    [Header("Sounds")]
    public AudioClip grappleSound;

    [Header("Other")]
    public float assistAngle = 22.5f;
    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public bool hasGrapplePoint;
    [HideInInspector] public Vector2 right, angleVector;
    [HideInInspector] public Transform currentGrapplePoint;
    [HideInInspector] public bool paused;

    // Start is called before the first frame update
    void Start()
    {
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            right = transform.right;
            angleVector = Quaternion.AngleAxis(0, Vector3.forward) * transform.right;
            transform.rotation = LookAtMouse();
            grappleMouseButtonDown = Input.GetMouseButtonDown(grappleMouseButton);
            grappleMouseButtonUp = Input.GetMouseButtonUp(grappleMouseButton);
            if (grappleMouseButtonDown)
            {
                StartGrapple();
            }
            if (grappleMouseButtonUp || GrappleDisrupted())
            {
                StopGrapple();
            }
            if (hasGrapplePoint)
            {
                targetJoint.enabled = launchMouseButtonHeld;
                ropeEnd = Vector2.Lerp(ropeEnd, grapplePoint, ropeSpeed);
                transform.rotation = LookAtRopeEnd();
                if (Input.GetMouseButtonDown(launchMouseButton))
                {
                    launchMouseButtonHeld = true;
                }
            }
            else
            {
                transform.rotation = LookAtMouse();
            }
            if (Input.GetMouseButtonUp(launchMouseButton) || grappleMouseButtonUp)
            {
                launchMouseButtonHeld = false;
            }
            AnimateLine();
        }
        else
        {
            StopGrapple();
        }
    }

    void StartGrapple()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, range, grappleable);
        if(hit)
        {
            hasGrapplePoint = true;
            grapplePoint = hit.point;
        }
        else if(currentGrapplePoint != null && Vector2.Angle(transform.right, LookAtTarget(currentGrapplePoint.position) * Vector2.right) <= assistAngle)
        {
            hasGrapplePoint = true;
            grapplePoint = currentGrapplePoint.position;
        }
        if (hasGrapplePoint)
        {
            distJoint = player.gameObject.AddComponent<DistanceJoint2D>();
            distJoint.autoConfigureConnectedAnchor = false;
            distJoint.autoConfigureDistance = false;
            distJoint.maxDistanceOnly = true;
            distJoint.enableCollision = true;
            distJoint.connectedAnchor = grapplePoint;
            distJoint.distance = GetDistanceToThis(grapplePoint);
            targetJoint = player.gameObject.AddComponent<TargetJoint2D>();
            targetJoint.autoConfigureTarget = false;
            targetJoint.target = grapplePoint;
            targetJoint.frequency = launchForce;
            targetJoint.enabled = false;
            ropeEnd = (Vector2)transform.position + Vector2.Distance(transform.position, grapplePoint) / 5 * (Vector2)transform.right;
            StartAnimation();
            if(grappleSound != null)
            {
                AudioSource.PlayClipAtPoint(grappleSound, transform.position);
            }
        }
    }

    void StopGrapple()
    {
        hasGrapplePoint = false;
        Destroy(distJoint);
        Destroy(targetJoint);
        HideLine(line);
    }

    float LookRotation(Vector2 v)
    {
        Vector2 difference = v - (Vector2)transform.position ;
        return Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
    }

    Vector2 GetMousePosition()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(mouse.x, mouse.y);
    }

    Quaternion LookAtMouse()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotation = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        return Rotation(rotation - 90);
    }

    //Camera.main.ScreenToWorldPoint(Input.mousePosition)

    Quaternion LookAtTarget(Vector2 target)
    {
        Vector3 difference = target - (Vector2)transform.position;
        float rotation = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        return Rotation(rotation - 90);
    }

    public float LookAtGrapplePoint()
    {
        Vector3 difference = (Vector3)grapplePoint - transform.position;
        float rotation = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        return rotation;
    }

    Quaternion LookAtRopeEnd()
    {
        Vector3 difference = (Vector3)ropeEnd - transform.position;
        float rotation = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        return Rotation(rotation - 90);
    }

    Quaternion Rotation(float rotZ)
    {
        return Quaternion.Euler(0, 0, -rotZ);
    }

    public float GetDistanceToThis(Vector2 v)
    {
        return Vector2.Distance(transform.position, v);
    }

    bool GrappleDisrupted()
    {
        RaycastHit2D hit = Physics2D.Raycast(grapplePoint, transform.position, Mathf.Infinity, playerLayer);
        return hit.transform == player.transform;
    }

    Vector2 GetAngleVector(float angle)
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    Vector2 GetNearerPoint(Vector2 a, Vector2 b)
    {
        if(GetDistanceToThis(a) > GetDistanceToThis(b))
        {
            return b;
        }
        else
        {
            return a;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GetMousePosition(), 0.5f);
        Gizmos.DrawWireSphere(transform.position, range);
        if (hasGrapplePoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, grapplePoint);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(grapplePoint, 0.5f);
        }
        else
        {
            Vector2 angleVectorPos = Quaternion.AngleAxis(assistAngle, Vector3.forward) * transform.right;
            Vector2 angleVectorNeg = Quaternion.AngleAxis(-assistAngle, Vector3.forward) * transform.right;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + (angleVectorPos * range));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + (angleVectorNeg * range));
        }
    }

    //Animation
    void StartAnimation()
    {
        currentWaveSize = waveSize;
        lineStraight = false;
    }
    
    void AnimateLine()
    {
        if(hasGrapplePoint && currentWaveSize > 0)
        {
            DrawWavedLine(line, ropeEnd, currentWaveSize, waveCount, resolution);
            currentWaveSize = Mathf.Lerp(currentWaveSize, 0, animSpeed);
        }
        else if(hasGrapplePoint && currentWaveSize <= 0.01f && !lineStraight)
        {
            lineStraight = true;
        }
        if(hasGrapplePoint && lineStraight)
        {
            DrawStraightLine(line, grapplePoint);
        }
    }

    void DrawWavedLine(LineRenderer lr, Vector2 target, float waveSize, int waveCount, int resolution)
    {
        int pointsPerWave = resolution / waveCount;
        float distanceBetweenPoints = Mathf.PI / pointsPerWave;
        lr.positionCount = resolution + 1;
        for(int i = 0; i < lr.positionCount; i++)
        {
            float x = Vector2.Distance(transform.position, target) / lr.positionCount * i;
            float y = Mathf.Sin(distanceBetweenPoints * i) * waveSize;
            Vector2 v = transform.position + transform.right * x + transform.up * y;
            lr.SetPosition(i, v);
        }
    }

    void DrawStraightLine(LineRenderer lr, Vector2 target)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, target);
    }

    void HideLine(LineRenderer lr)
    {
        lr.positionCount = 0;
    }

    float LookAtAngle(Vector2 target)
    {
        return Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
    }
}
