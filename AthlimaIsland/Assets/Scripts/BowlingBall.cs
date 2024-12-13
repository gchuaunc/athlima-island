using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingBall : MonoBehaviour
{
    float timeSinceLastOnFloor = 100.0f;

    private void Start()
    {
        GetComponents<AudioSource>()[0].Play();
        GetComponents<AudioSource>()[1].PlayDelayed(GetComponents<AudioSource>()[0].clip.length);
    }
    private bool playing = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(GetComponent<Rigidbody>().velocity.magnitude);

        if (onFloor && GetComponent<Rigidbody>().velocity.sqrMagnitude > .1)
        {
            if (timeSinceLastOnFloor > .3f && !playing)
            {
                playing = true;
                // Play audio
                GetComponents<AudioSource>()[0].Play();
                GetComponents<AudioSource>()[1].PlayDelayed(GetComponents<AudioSource>()[0].clip.length);
            }
        } else {
            if (playing)
            {
                playing = false;
                // stop audio
                GetComponents<AudioSource>()[0].Stop();
                GetComponents<AudioSource>()[1].Stop();
            }
        }

        if (onFloor)
        {
            timeSinceLastOnFloor = 0;
        }
        else
        {
            timeSinceLastOnFloor += Time.fixedDeltaTime;
        }
    }

    private bool onFloor = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            onFloor = true;
        }
        if (collision.collider.CompareTag("Guard"))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(
                -GetComponent<Rigidbody>().velocity.x, 
                GetComponent<Rigidbody>().velocity.y, 
                GetComponent<Rigidbody>().velocity.z);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            onFloor = true;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            onFloor = false;
        }
    }
}
