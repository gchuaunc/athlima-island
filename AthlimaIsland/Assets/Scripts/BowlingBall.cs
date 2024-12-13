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
        if (onFloor)
        {
            timeSinceLastOnFloor = 0;
        } else
        {
            timeSinceLastOnFloor += Time.fixedDeltaTime;
        }

        if (onFloor && GetComponent<Rigidbody>().velocity.sqrMagnitude > 1)
        {
            if (timeSinceLastOnFloor > 1f && !playing)
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
    }

    private bool onFloor = false;

    private void OnCollisionEnter(Collision collision)
    {
        onFloor = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        onFloor = false;
    }
}
