using Meta.XR.ImmersiveDebugger.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingPin : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == null) return;
        if (collision.collider.tag == "BowlingBall" || collision.collider.tag == "Pin")
        {
            AudioSource source = GetComponent<AudioSource>();
            int index = Random.Range(0, clips.Length);

            source.clip = clips[index];
            source.Play();
        }
    }
}
