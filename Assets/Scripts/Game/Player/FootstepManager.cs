using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isMoving = (movement.Movement.magnitude != 0);
        handleFootsteps();
    }

    private AudioSource audioSource;
    private Rigidbody2D rb;

    [SerializeField]
    private AudioClip[] footstepSounds;
    [SerializeField]
    private float walkStepInterval = 0.4f;
    [SerializeField]
    private float runStepInterval = 0.2f;
    private int lastPlayedIndex = -1;
    private bool isMoving;
    private float nextStepTime;


    [SerializeField]
    private float velocityThreshold = 0.2f;

    private PlayerMovement movement;

    private void handleFootsteps()
    {
        float currentStepInterval = movement.isRunning ? runStepInterval : walkStepInterval;
        if (isMoving && Time.time > nextStepTime)
        {
            playFootstepSounds();
            nextStepTime = Time.time + currentStepInterval;
        }
    }

    private void playFootstepSounds()
    {
        int randomIndex;
        if (footstepSounds.Length == 1)
        {
            randomIndex = 0;
        }
        else
        {
            randomIndex = Random.Range(0, footstepSounds.Length - 1);
            if (randomIndex >= lastPlayedIndex)
            {
                randomIndex++;
            }
        }

        //print(randomIndex);
        lastPlayedIndex = randomIndex;
        audioSource.clip = footstepSounds[randomIndex];
        audioSource.Play();
    }
}
