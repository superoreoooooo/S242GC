using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    //변수 초기화 및 설정
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    //매 프레임 호출됨. 걷는지 검사하여 handleFootsteps 호출
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

    //걷는지 뛰는지 확인하여 발소리 간 간격 조정
    private void handleFootsteps()
    {
        float currentStepInterval = movement.isRunning ? runStepInterval : walkStepInterval;
        if (isMoving && Time.time > nextStepTime)
        {
            playFootstepSounds();
            nextStepTime = Time.time + currentStepInterval;
        }
    }

    //발소리 플레이. 1~4 중 랜덤하게 소리를 골라 무작위 소리 플레이
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
