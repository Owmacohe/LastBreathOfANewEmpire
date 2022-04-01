using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellRinging : MonoBehaviour
{
    [SerializeField] AudioClip[] bigBells;
    [SerializeField] AudioClip[] midBells;
    [SerializeField] float ringChance = 1000;

    public enum BellSize { None, Mid, Big }
    public BellSize size;

    AudioSource sound;
    AudioClip[] bells;

    bool isPulsing;
    float startPulseTime;
    float pulseSpeed = 10;

    private void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (!size.Equals(BellSize.None))
        {
            if (size.Equals(BellSize.Big) && bells != bigBells)
            {
                bells = bigBells;
            }
            else if (size.Equals(BellSize.Mid) && bells != midBells)
            {
                bells = midBells;
            }

            if (!sound.isPlaying && Random.Range(0, ringChance) <= 0.75f)
            {
                sound.clip = bells[Random.Range(0, bells.Length - 1)];
                sound.Play();

                isPulsing = true;
                startPulseTime = Time.time;
            }
        }

        if (isPulsing)
        {
            float amplitude;

            if (size.Equals(BellSize.Big))
            {
                amplitude = 0.03f;
            }
            else
            {
                amplitude = 0.08f;
            }

            transform.localScale += Vector3.one * (amplitude * Mathf.Sin(pulseSpeed * (Time.time - startPulseTime)));

            if (Time.time - startPulseTime >= ((2 * Mathf.PI) / pulseSpeed))
            {
                isPulsing = false;
            }
        }
    }
}