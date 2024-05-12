using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerMomento : MonoBehaviour
{
    Rigidbody rb;
    Camera cam = null;

    float xRot = 0f;
    float yRot = 0f;
    float lookRot = 0f;

    float bobTimer = 0f;
    float fullBob = 0.15f;
    float bobExtra = 0f;

    float camInitY = 0f;

    public float rotSpeed;
    public float moveSpeed;
    public float runSpeed;
    public float lookSpeed;

    public AudioSource audioPlayer;
    AudioClip footstep;

    public AudioClip footstepNormal;
    public AudioClip footstepTest;

    float curSpeed;

    bool grounded = true;
    int groundedlastY = 0;

    float collideWallTime = 0.0f;
    bool collidingWithWalls = false;
    bool warping = false;

    public bool movedYet = false;
    public bool death = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        camInitY = cam.transform.localPosition.y;
        bobTimer = fullBob;
        curSpeed = moveSpeed;
        groundedlastY = roundForGrounded(transform.position.y);
        footstep = footstepNormal;
    }
    void Update()
    {
        grounded = (groundedlastY == roundForGrounded(transform.position.y));
        if (!grounded)
        {
            transform.position = transform.position + new Vector3(0, -0.5f * Time.deltaTime);
            rb.velocity = Vector3.zero;
        }
        groundedlastY = roundForGrounded(transform.position.y);

        if (collidingWithWalls)
        {
            collideWallTime += Time.deltaTime;
        }
        else
            collideWallTime = 0.0f;
        if (collideWallTime > 1f)
        {
            if (!warping)
            {
                GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().OhShitWereWarping();
            }
            warping = true;
        }
    }
 
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!warping && !death)
        {
            bool[] bools = look();
            bool preventRun = bools[0];
            ArrayUtility.RemoveAt(ref bools, 0);
            bool[] virtualInput = bools;

            turn();
            move(preventRun, virtualInput);
            
        }
        else if (!warping)
        {
            xRot = Mathf.MoveTowards(xRot, -90, rotSpeed * 1.5f);
            transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        }
    }


    void turn()
    {
        if (Input.GetKey(KeyCode.A))
        {
            yRot += -rotSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            yRot += rotSpeed;
        }
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    bool[] look()
    {
        bool[] returnValue = { false, false, false, false, false };

        if (Input.GetKey(KeyCode.Z))
        {
            xRot -= rotSpeed;
            // only allows running if going forwards
            returnValue[0] = !Input.GetKey(KeyCode.W);
            // backwards
            returnValue[2] = true;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            xRot += rotSpeed;
        }
        else
        {
            if (Mathf.Abs(xRot) < 1.5)
                xRot = 0f;
            if (xRot != 0)
            {
                if (xRot > 0)
                    xRot -= rotSpeed * 0.8f;
                else
                    xRot += rotSpeed * 0.8f;
            }
        }


        if (Input.GetKey(KeyCode.Q))
        {
            lookRot -= rotSpeed * lookSpeed;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            lookRot += rotSpeed * lookSpeed;
        }
        else
        {
            if (Mathf.Abs(lookRot) < 10)
                lookRot = 0f;
            if (lookRot != 0)
            {
                if (lookRot > 0)
                    lookRot -= rotSpeed * 0.8f * lookSpeed;
                else
                    lookRot += rotSpeed * 0.8f * lookSpeed;
            }
        }


        xRot = Mathf.Clamp(xRot, -50, 50);
        lookRot = Mathf.Clamp(lookRot, -175, 175);

        cam.transform.rotation = Quaternion.Euler(xRot, yRot + lookRot, 0);
        return returnValue;
    }

    void move(bool preventRun, bool[] virtualPresses)
    {
        if (Input.GetKey(KeyCode.LeftShift) && !preventRun)
            curSpeed = runSpeed;
        else
            curSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.W) || virtualPresses[0])
        {
            rb.velocity = transform.forward * curSpeed;
            bobing(true);
        }
        else if (Input.GetKey(KeyCode.S) || virtualPresses[1])
        {
            rb.velocity = transform.forward * -moveSpeed;
            bobing();
        }
        else if (Input.GetKey(KeyCode.Alpha3) || virtualPresses[2])
        {
            rb.velocity = transform.right * moveSpeed;
            bobing();
        }
        else if (Input.GetKey(KeyCode.Alpha1) || virtualPresses[3])
        {
            rb.velocity = transform.right * -moveSpeed;
            bobing();
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
        cam.transform.localPosition = new Vector3(0, camInitY + bobExtra, 0);

    }



    void bobing(bool movingForwards = false)
    {
        movedYet = true;
        var timeToAdd = Time.fixedDeltaTime / 2;
        if (curSpeed == runSpeed && movingForwards) // fuck!
            timeToAdd *= 1.5f;
        bobTimer += timeToAdd;
        if (bobTimer > fullBob / 2)
        {
            bobExtra -= timeToAdd;
            if (Mathf.Abs(bobExtra - (fullBob * 0.45f)) < 0.0125f)
            {
                audioPlayer.PlayOneShot(footstep);
            }
        }
        else
        {
            bobExtra += timeToAdd;
        }

        if (bobTimer > fullBob)
        {
            bobExtra = 0f;
            bobTimer = 0f;
        }
    }

    int roundForGrounded(float input)
    {
        return (int)Mathf.Floor((Mathf.Round(input) * 1000f) / 1000f);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pit")
        {
            var gc = GameObject.FindGameObjectWithTag("GameController").gameObject.GetComponent<GameController>();
            gc.deadLol = true;
            gc.OhShitWereWarping();
            death = true;
            GetComponent<CapsuleCollider>().enabled = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.tag == "Floor")
            {
                if (contact.otherCollider.transform.gameObject.GetComponent<Renderer>().material.name.ToLower().Contains("tet"))
                {
                    footstep = footstepTest;
                }
                else
                {
                    footstep = footstepNormal;
                }
            }
            if (contact.otherCollider.tag == "Wall")
            {
                collidingWithWalls = true;
                collideWallTime = 0.0f;
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Wall"))
        {
            if (Vector3.Distance(obj.transform.position, transform.position) > 1.05f)
            {
                collidingWithWalls = false;
            }
            else if (Vector3.Distance(obj.transform.position, transform.position) <= 1.05f)
            {
                collidingWithWalls = true;
                break;
            }
        }
    }
}

