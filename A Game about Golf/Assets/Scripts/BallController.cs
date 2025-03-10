using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.CompilerServices;


public class BallController : MonoBehaviour
{
    [Header("References")]
    Camera mainCamera;
    public Rigidbody rb;
    private HoleTrigger holeTriggerScript;

    [Header("Ball settings")]
    public float startPositionY = 0.26f;
    public float maxStrength;
    public float minStrength;
    public float shotPower;
    public float maxSpeed;
    public float stopVelocity;
    public float slowVelocity;
    public float slowMultiplier;
    public float wallBounceDamping;
    public float wallBouncePower;
    public float wallSlowPower;
    public float collisionRadius;
    public float closeWallDistance;
    public float closeWallDamping;

    [Header("Aiming angle")]
    public int currentAngleIndex = 0; //0 = normal, 1 = 90°, 2 = 180°, 3 = 270°
    private Vector3 originalAimPos;
    private readonly float[] angles = { 0f, 90f, 180f, 270f };

    [Header("Line settings")]
    public LineRenderer lineRenderer;
    public float lineOffset;
    public float linePosY;

    [Header("Dashed line settings")]
    public LineRenderer dashedLineRenderer;

    [Header("Audio settings")]
    //[SerializeField] private AudioClip golfSwing;
    public AudioClip[] golfSwingArray;
    private float golfSwingVolume;
    public float golfSwingMinVol;
    public float golfSwingMaxVol;

    public AudioClip[] hitWallArray;
    private float hitWallVolume;
    public float hitWallMinVol;
    public float hitWallMaxVol; 

    [Header("Data")]
    public int shotCount;
    public float currentSpeed;

    //Extra collision coroutine
    private IEnumerator extraCollisionChecks;
    private bool extraCheckEnabled;

    bool isAiming;
    public bool isIdle;
    bool isShooting;

    //[Header("Outline settings")]
    //private RectTransform outlineTransform;
    //public float outlinePosY;
    //public float outlineScale;
    //public Image outline;

    private float colSphereRadius;
    private int wallsLayerMask;

    Vector3 lineBallOffset;
    Vector3 lineVector;
    Vector3 aimPos;
    Vector3 mousePos;

    private void Awake()
    {
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Walls"), true);

        mainCamera = Camera.main;
        shotCount = 0;

        rb.maxAngularVelocity = 1000; //fix rotation problems

        isAiming = false;

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        dashedLineRenderer.positionCount = 2;
        dashedLineRenderer.enabled = false;

        holeTriggerScript = GameObject.FindGameObjectWithTag("HoleTrigger").GetComponent<HoleTrigger>();
        if (holeTriggerScript == null) Debug.LogError("GameManager - holeTrigger not found!");
    }
    private void Start()
    {
        //extra check enable
        extraCheckEnabled = false;
        extraCollisionChecks = ExtraCollisionChecks();
        StartCoroutine(extraCollisionChecks);

        wallsLayerMask = LayerMask.GetMask("Walls");
        colSphereRadius = GetComponent<SphereCollider>().radius * transform.localScale.x * collisionRadius;

        GameObject startPosition = GameObject.FindGameObjectWithTag("StartPosition");
        if (startPosition == null)
        {
            Debug.LogError("ballController - startPosition not found!");
        }
        else
        {
            rb.position = startPosition.transform.position + new Vector3(0,startPositionY,0);
        }
        //outline
        //outlineTransform = GameObject.FindGameObjectWithTag("Outline").GetComponent<RectTransform>();
        //outlineTransform.sizeDelta = new Vector2(100f * outlineScale, 100f * outlineScale);
        //outline = GameObject.FindGameObjectWithTag("Outline").GetComponent<Image>();
        //outline.enabled = true;
    }
    private void Update()
    {
        //outline
        //Vector3 screenPos = mainCamera.WorldToScreenPoint(new Vector3(rb.position.x, outlinePosY, rb.position.z));
        //Vector2 anchoredPos;

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(outlineTransform.parent as RectTransform, screenPos, mainCamera, out anchoredPos);

        //outlineTransform.anchoredPosition = anchoredPos;

        if (isIdle && !holeTriggerScript.stageComplete)
        {
            isAiming = true;

            float width = lineRenderer.startWidth;
            lineRenderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);

            ProcessAiming();

            if (Input.GetMouseButtonDown(1))
            {
                ToggleAimAngle();
            }

            if (Input.GetMouseButtonUp(0))
            {
                isShooting = true;
            }
        }
    }
    void FixedUpdate()
    {
        OverrideWallCollision(); //override collisions with walls

        currentSpeed = rb.linearVelocity.magnitude;
        if (currentSpeed > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        if (currentSpeed < slowVelocity) //start slowing ball down
        {
            Slow();
        }
        if (currentSpeed < stopVelocity) //stop ball
        {
            Stop();
        }
        if (isShooting) //shoot ball
        {
            Shoot();
            isShooting=false;
        }
    }
    IEnumerator ExtraCollisionChecks()
    {

        while (extraCheckEnabled)
        {
            Debug.Log("Performing extra collision check");

            WaitForSeconds waitForHalfFixedUpdate = new WaitForSeconds(Time.fixedDeltaTime * 0.5f);
            yield return waitForHalfFixedUpdate;
            
            //if (rb.linearVelocity.magnitude > maxSpeed * 0.5f) OverrideWallCollision();

            OverrideWallCollision();
        }
    }

    void OverrideWallCollision()
    {
        Vector3 rayStartPos = rb.position;
        Vector3 rayDirection = rb.linearVelocity.normalized;
        float rayDistance = rb.linearVelocity.magnitude * Time.fixedDeltaTime;

        RaycastHit hit;

        int maxReflections = 3;

        for (int i = 0; i < maxReflections; i++)
        {
            if (!Physics.SphereCast(rayStartPos, colSphereRadius, rayDirection, out hit, rayDistance, wallsLayerMask))
            {
                break;
            }

            Debug.Log($"Wall hit! Reflection #{i + 1} at {hit.point}");

            float hitWallVolume = Mathf.Clamp(currentSpeed / maxSpeed, hitWallMinVol, hitWallMaxVol);
            SoundManager.instance.PlaySoundClip(hitWallArray[Random.Range(0, hitWallArray.Length)], transform, hitWallVolume);

            Vector3 reflectedVelocity = Vector3.Reflect(rb.linearVelocity, hit.normal);

            rb.position = hit.point + hit.normal * colSphereRadius;

            ApplyWallModifiers(hit.collider.tag, ref reflectedVelocity); 

            rb.linearVelocity = reflectedVelocity * wallBounceDamping;

            rayStartPos = rb.position;
            rayDirection = rb.linearVelocity.normalized;
            rayDistance = rb.linearVelocity.magnitude * Time.fixedDeltaTime;
        }

        /*
        RaycastHit hit;
        if (Physics.SphereCast(rayStartPos, colSphereRadius, rayDirection, out hit, rayDistance, wallsLayerMask))
            {
            float hitWallVolume = Mathf.Clamp(currentSpeed / maxSpeed, hitWallMinVol, hitWallMaxVol);
            SoundManager.instance.PlaySoundClip(hitWallArray[Random.Range(0, hitWallArray.Length)], transform, hitWallVolume);

            Vector3 reflectedVelocity = Vector3.Reflect(rb.linearVelocity, hit.normal);

            switch (hit.collider.tag)
            {
                case "Wall":
                case "WallBounce":
                case "WallSlow":
                    // Apply common logic for all wall types
                    rb.position = hit.point + hit.normal * colSphereRadius;

                    switch (hit.collider.tag)
                    {
                        case "WallBounce":
                            reflectedVelocity *= wallBouncePower;
                            break;
                        case "WallSlow":
                            reflectedVelocity *= wallSlowPower;
                            break;
                    }

                    rb.linearVelocity = reflectedVelocity * wallBounceDamping;
                    break;
            }
        }
        */
    }
    private void ApplyWallModifiers(string wallTag, ref Vector3 velocity)
    {
        switch (wallTag)
        {
            case "WallBounce":
                velocity *= wallBouncePower;
                break;
            case "WallSlow":
                velocity *= wallSlowPower;
                break;
        }
    }
    void ToggleAimAngle()
    {
        currentAngleIndex = (currentAngleIndex + 1) % angles.Length;
    }
    void ProcessAiming()
    {
        if (!isAiming) return; //check if aim allowed
        else
        {
            mousePos = Input.mousePosition;
            float zDistance = Mathf.Abs(mainCamera.transform.position.y - rb.position.y); //distance from camera to ball
            mousePos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zDistance)); //convert mouse pos to world data

            originalAimPos = mousePos - rb.position;
            originalAimPos.y = 0f;

            if (currentAngleIndex > 0)
            {
                dashedLineRenderer.enabled = true;

                float angleInRadians = angles[currentAngleIndex] * Mathf.Deg2Rad; //convert angle to radians
                float cos = Mathf.Cos(angleInRadians); //get angle cos
                float sin = Mathf.Sin(angleInRadians); //get angle sin

                aimPos = new Vector3
                    (
                    originalAimPos.x * cos - originalAimPos.z * sin,
                    0f,
                    originalAimPos.x * sin + originalAimPos.z * cos
                    );

                float originalStrength = Vector3.Distance(rb.position, mousePos);
                originalStrength = Mathf.Clamp(originalStrength, minStrength, maxStrength);

                Vector3 originalLineVector = originalAimPos.normalized * originalStrength;
                Vector3 originalLineBallOffset = originalAimPos.normalized * lineOffset;

                dashedLineRenderer.SetPosition(0, new Vector3(rb.position.x, linePosY, rb.position.z) + originalLineBallOffset);
                dashedLineRenderer.SetPosition(1, new Vector3(rb.position.x, linePosY, rb.position.z) + originalLineVector);
            }
            else
            {
                aimPos = originalAimPos;
                dashedLineRenderer.enabled = false;
            }

            float strength = Vector3.Distance(rb.position, mousePos);
            strength = Mathf.Clamp(strength, minStrength, maxStrength);

            lineVector = aimPos.normalized * strength;

            lineBallOffset = aimPos.normalized * lineOffset;
            lineRenderer.SetPosition(0, new Vector3(rb.position.x, linePosY, rb.position.z) + lineBallOffset);
            lineRenderer.SetPosition(1, new Vector3(rb.position.x, linePosY, rb.position.z) + lineVector);
        }
    }
    void Slow() //slow down ball
    {
        if (rb.linearVelocity != Vector3.zero)
        {
            rb.linearVelocity *= slowMultiplier;
        }
    }
    private void Stop() //stop ball
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isIdle = true;

        if (holeTriggerScript.stageComplete)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
        }
    }
    void Shoot() //shoot ball, disable line, add to shotcount
    {
        shotCount += 1;
        lineRenderer.enabled = false;
        dashedLineRenderer.enabled = false;
        isAiming = false;
        isIdle = false;

        Vector3 direction = aimPos.normalized;
        float strength = Vector3.Distance(rb.position, mousePos);
        strength = Mathf.Clamp(strength, minStrength, maxStrength);

        if (strength > maxStrength / 3)
        {
            CloseWallReflect(rb.position, ref direction, ref strength, 3);
        }

        rb.AddForce(shotPower * strength * direction);

        currentAngleIndex = 0;

        //play swing sound
        float normalizedStrength = (strength - minStrength) / (maxStrength - minStrength);
        golfSwingVolume = Mathf.Clamp(normalizedStrength, golfSwingMinVol, golfSwingMaxVol);
        int randomIndex = Random.Range(0, golfSwingArray.Length);
        SoundManager.instance.PlaySoundClip(golfSwingArray[randomIndex], transform, golfSwingVolume);
    }
    private void CloseWallReflect(Vector3 startPos, ref Vector3 direction, ref float strength, int maxReflections)
    {
        RaycastHit hit;
        for (int i = 0; i < maxReflections; i++)
        {
            if (!Physics.Raycast(startPos, direction, out hit, colSphereRadius * closeWallDistance, wallsLayerMask))
                break;

            Debug.Log($"Close wall hit! Reflection #{i + 1}");

            direction = Vector3.Reflect(direction, hit.normal);
            startPos = hit.point + hit.normal * colSphereRadius;

            CloseWallDamping(hit.collider.tag, ref strength);
        }
    }
    private void CloseWallDamping(string wallTag, ref float strength)
    {
        strength *= closeWallDamping;
        switch (wallTag)
        {
            case "WallBounce":
                strength *= wallBouncePower;
                break;
            case "WallSlow":
                strength *= wallSlowPower;
                break;
        }
    }
}

