using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float runSpeed = 8f;

    private Vector2 movement;

    private Rigidbody2D rb;

    private bool isDashing;

    public bool IsDashing {
        get => isDashing;
    }

    [SerializeField]
    private int dashDamage;

    public int DashDamage {
        get => dashDamage;
    }

    [SerializeField]
    private float dashForce;

    [SerializeField]
    private float dashDuration;

    private bool isDashAble = true;
    
    private float coolDownDash = 0f;
    
    [SerializeField]
    private float CoolDown_DASH;

    private float originalSpeed;

    void Start()
    {
        originalSpeed = speed;
        isDashing = false;
        movement = new Vector2();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDashing) return;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            movement.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        if (Input.GetKey(KeyCode.LeftShift) && isDashAble) {
            StartCoroutine(SkillDash());
        }
        UpdateSkill();
    }

    private void FixedUpdate() {
        rb.velocity = movement * speed;
    }

    private void UpdateSkill() {
        updateDashCoolDown();
    }

    private void updateDashCoolDown() {
        if (!isDashAble) {
            coolDownDash += Time.deltaTime;
            if (coolDownDash > CoolDown_DASH) {
                isDashAble = true;
                coolDownDash = 0f;
            }
        }
    }

    private IEnumerator SkillDash() {
        if (movement.magnitude != 0) {
            isDashing = true;
            //Vector2 origin = transform.position;
            speed = dashForce;
            Dash(movement.normalized);

            yield return new WaitForSeconds(dashDuration);

            speed = originalSpeed;
            Dash(movement.normalized);
            isDashing = false;
            isDashAble = false;
        }
    }

    public void Dash(Vector2 dir) {
        movement = new Vector2(dir.x * speed, dir.y * speed);
    }
}
