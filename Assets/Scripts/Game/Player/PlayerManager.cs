using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerManager : MonoBehaviour
{
    /*
    private LineRenderer lr;
    private float lrLen = 3f;
    */
    public GameData gameData;

    public UnityEvent onPlayerDead;
    public UnityEvent onPlayerInteract; // -> 안쓸듯?
    public UnityEvent onPlayerMoveRoom;
    public UnityEvent onPlayerTeleportComplete;

    public Vector2Int playerRoomPos;
    public GameObject cellNow;

    private PlayerMovement movement;

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private int health;

    public int Health
    {
        get => health;
        set => health = Math.Clamp(value, 0, health);
    }

    [SerializeField]
    private float invincibleTime;

    private bool isInvincible;

    public bool Invincible
    {
        get => isInvincible;
        set { isInvincible = value; }
    }

    [SerializeField]
    private int knifeDamage;

    [SerializeField]
    private float knifeDistance;

    [SerializeField]
    private float knifeAngle;

    [SerializeField]
    private GameObject interacionUI;

    [SerializeField]
    private float interactionDistance;

    private MapManager mapManager;

    [SerializeField]
    private float exitDistance;

    [SerializeField]
    private Transform fovLight;

    [SerializeField]
    private UnityEngine.UI.Slider hpBar;

    [SerializeField]
    private Animator animator;

    public bool isDead = false;

    //체력바 관련 초기화 함수 호출
    private void Awake()
    {
        setMaxHealth(health);
    }

    //체력바 설정
    private void setMaxHealth(int health)
    {
        hpBar.maxValue = health;
        hpBar.value = health;
    }

    //초기 위치 계산 및 이동
    private void initPlayerMoveRoom()
    {
        mapManager = FindObjectOfType<MapManager>();

        if (mapManager != null )
        {
            isInvincible = false;
            transform.position = new Vector2((gameData.SpawnX) * gameData.RoomSizeX + 18, (gameData.SpawnY) * gameData.RoomSizeY + 19);
        }
    }

    private SpriteRenderer sr;

    private bool isInteractable;
    
    //페이드 인이라 써있지만 플레이어 텔레포트 이벤트 호출 관련 코루틴 구현부 
    private IEnumerator fadeIn()
    {
        yield return new WaitForSeconds(0.5f);

        onPlayerTeleportComplete.Invoke();
    }

    //플레이어 텔레포트 코루틴 구현부
    private IEnumerator teleport(Vector3 pos)
    {
        yield return new WaitForSeconds(1f);

        transform.position = pos;

        movement.isMoveable = true;

        StartCoroutine(fadeIn());
    }

    public UnityEvent onBossClear;

    private bool isInBoss = false;

    //보스방으로 이동. 여건 상 하드코딩되어있음
    public void moveToBossRoom()
    {
        isInBoss = true;
        StartCoroutine(teleport(new Vector2(0 * mapManager.data.RoomSizeX + 2.78f, 0 * mapManager.data.RoomSizeY + 25.21f)));
    }

    [SerializeField]
    private UITextFade UITextFadeWarning;

    [SerializeField]
    private UITextFade UITextFadeDone;

    private bool isMsgShown = false;
    
    //보스 클리어시 호출 (이벤트). 텍스트 페이드 아웃
    public void bossClear()
    {
        UITextFadeDone.startFadeOut();
        movement.isMoveable = false;
        StartCoroutine(a());
    }
    
    //방 이동 시 화면 흐려지는것 관련 함수 호출하는 코루틴 구현부
    private IEnumerator a()
    {
        yield return new WaitForSeconds(0.5f);
        onPlayerMoveRoom.Invoke();
    }

    //플레이어가 다른 방 이동하려는지 검사 및 방 생성. 이후 텔레포트
    private void updatePlayerMoveRoom()
    {
        if (isInBoss)
        {
            return;
        }
        if (mapManager == null) return;

        cellNow = mapManager.getCell((Vector2) transform.position);

        bool isRoomCleared = true;

        for (int i = 0; i < cellNow.transform.childCount; i++)
        {
            if (cellNow.transform.GetChild(i).name == "enemy")
            {
                for (int j = 0; j < cellNow.transform.GetChild(i).childCount; j++)
                {
                    if (cellNow.transform.GetChild(i).GetChild(j).gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        isRoomCleared = false;
                        break;
                    }
                }
            }
        }

        if (isRoomCleared && !cellNow.name.Contains("Cell_13") && !isMsgShown)
        {
            isMsgShown = true;
            UITextFadeDone.startFadeOut();
        }

        Vector2Int posNow = mapManager.getCellPos((Vector2) transform.position);

        if (cellNow != null)
        {
            CellDirection CD = CellDirection.VOID;
            bool a = false;
            Vector2Int roomDir = new Vector2Int();

            if (cellNow.GetComponent<Cell>().DOOR_UP != null && Vector2.Distance(transform.position, cellNow.GetComponent<Cell>().DOOR_UP.transform.position) <= exitDistance && (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.W)))
            {
                CD = CellDirection.UP;
                a = true;
                roomDir = Vector2Int.up;
            } 
            if (cellNow.GetComponent<Cell>().DOOR_DOWN != null && Vector2.Distance(transform.position, cellNow.GetComponent<Cell>().DOOR_DOWN.transform.position) <= exitDistance && (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.S)))
            {
                CD = CellDirection.DOWN;
                a = true;
                roomDir = Vector2Int.down;
            }
            if (cellNow.GetComponent<Cell>().DOOR_LEFT != null && Vector2.Distance(transform.position, cellNow.GetComponent<Cell>().DOOR_LEFT.transform.position) <= exitDistance && (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A)))
            {
                CD = CellDirection.LEFT;
                a = true;
                roomDir = Vector2Int.left;
            }
            if (cellNow.GetComponent<Cell>().DOOR_RIGHT != null && Vector2.Distance(transform.position, cellNow.GetComponent<Cell>().DOOR_RIGHT.transform.position) <= exitDistance && (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D)))
            {
                CD = CellDirection.RIGHT;
                a = true;
                roomDir = Vector2Int.right;
            }

            if (a)
            {
                if (!isRoomCleared)
                {
                    switch (CD)
                    {
                        case CellDirection.UP:
                            transform.position = transform.position - new Vector3(0, 3);
                            break;
                        case CellDirection.DOWN:
                            transform.position = transform.position + new Vector3(0, 3);
                            break;
                        case CellDirection.LEFT:
                            transform.position = transform.position + new Vector3(3, 0);
                            break;
                        case CellDirection.RIGHT:
                            transform.position = transform.position - new Vector3(3, 0);
                            break;
                    }

                    UITextFadeWarning.startFadeOut();

                    print("room not cleared!");

                    return;
                }

                onPlayerMoveRoom.Invoke();
                Vector3 position = new Vector3();
                GameObject cell;

                if (mapManager.getCell(mapManager.getCellPos(transform.position) + roomDir) != null)
                {
                    cell = mapManager.getCell(mapManager.getCellPos(transform.position) + roomDir);
                } 
                else
                {
                    cell = mapManager.genCell(posNow.x, posNow.y, CD);
                }

                if (cell != null)
                {
                    switch (mapManager.getOppositeDir(CD))
                    {
                        case CellDirection.UP: //DOWN
                            position = cell.GetComponent<Cell>().DOOR_UP.transform.position;
                            break;
                        case CellDirection.DOWN: //UP
                            position = cell.GetComponent<Cell>().DOOR_DOWN.transform.position;
                            break;
                        case CellDirection.LEFT: //RIGHT
                            position = cell.GetComponent<Cell>().DOOR_LEFT.transform.position;
                            break;
                        case CellDirection.RIGHT: //LEFT
                            position = cell.GetComponent<Cell>().DOOR_RIGHT.transform.position;
                            break;
                    }
                }

                isMsgShown = false;
                movement.isMoveable = false;
                movement.Movement = Vector2.zero;
                StartCoroutine(teleport(position));
            }
        }
    }


    /**
    public GameObject MAPMGR;

    private void interact(GameObject obj)
    {
        switch (obj.GetComponent<Interactable>().iType)
        {
            case "Door":
                int[] posC = MAPMGR.GetComponent<MapManager>().getCellDir(gameObject);
                CellDirection CD = CellDirection.VOID;

                switch (obj.GetComponent<Interactable>().iData[0])
                {
                    case "UP":
                        CD = CellDirection.UP;
                        break;
                    case "DOWN":
                        CD = CellDirection.DOWN;
                        break;
                    case "LEFT":
                        CD = CellDirection.LEFT;
                        break;
                    case "RIGHT":
                        CD = CellDirection.RIGHT;
                        break;
                    default:
                        break;
                }
                MAPMGR.GetComponent<MapManager>().genCell(posC[0], posC[1], CD);
                break;
            default:
                print("null");
                break;
        }
    } */
    
    //플레이어 사망 함수. onPlayerDead 이벤트 호출
    private void dead()
    {
        if (mz) return;
        onPlayerDead.Invoke();
        //gameObject.SetActive(false);
        isDead = true;
        /*
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name != "backLight") Destroy(transform.GetChild(i).gameObject);
        }*/

        //StopAllCoroutines();
        
        GetComponent<FootstepManager>().enabled = false;
        GetComponent<AudioSource>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //print("DIE!");

        StartCoroutine(re());
    }
    
    //인트로 씬 (맨 첫번째)로 씬 전환하는 코루틴 구현부.
    private IEnumerator re()
    {
        yield return new WaitForSeconds(3f);

        FindAnyObjectByType<SceneLoader>().LoadScene("IntroScene");

        Destroy(this);
    }

    //데미지 처리 및 체력바 동기화 (피해 입는거)
    public void gainDamage(int amount)
    {
        if (mz) return;
        health -= amount;
        hpBar.value = health;
    }
    
    //상호작용 (무기 교체) 함수
    private void updateInteraction()
    {
        isInteractable = false;

        GameObject iobj = null;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Interactable"))
        {
            if (((Vector2)obj.transform.position - (Vector2)transform.position).magnitude <= interactionDistance)
            {
                isInteractable = true;

                if (iobj != null)
                {
                    if ((iobj.transform.position - transform.position).magnitude > (obj.transform.position - transform.position).magnitude)
                    {
                        iobj = obj;
                    }
                }
                else
                {
                    iobj = obj;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && isInteractable)
        {
            if (iobj != null)
            {
                print(iobj.name);
                if (iobj.GetComponent<WeaponData>() != null)
                {
                    GameObject inst = Instantiate(GetComponentInChildren<WeaponManager>().currentWeapon.prefab);
                    inst.transform.position = transform.position;
                    GetComponentInChildren<WeaponManager>().currentWeapon = iobj.GetComponent<WeaponData>().weapon;
                    GetComponentInChildren<WeaponManager>().swapWeapon();

                    Destroy(iobj);
                }
            }
        }

        interacionUI.SetActive(isInteractable);
    }

    //변수 설정 및 플레이어 위치 이동 (initPlayerMoveRoom)
    void Start()  
    {
        initPlayerMoveRoom();

        movement = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();

        /*
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.blue;
        lr.positionCount = 2; 
        */
    }

    /**
    * 매 프레임 호출. 
    * 1. 플레이어 사망 시 사망 애니메이션 실행
    * 2. 상호작용 (updateInteraction)과 방 이동 관련 검사 (updatePlayerMoveRoom) 호출
    * 3. fovLight (시야 라이트) 보는 방향 마우스쪽으로 설정
    */

    void Update()
    {

        if (health <= 0)
        {
            animator.Play("player_die");
            dead();
        }

        if (isDead) return;
        if (!movement.isMoveable) return;

        updateInteraction();
        updatePlayerMoveRoom();

        /*
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + new Vector3(direction.x, direction.y, transform.position.z) * lrLen);
        */

        /**
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(projectile, transform.position, Quaternion.identity);
            Projectile pj = obj.GetComponent<Projectile>();
            pj.direction = direction;
        }
        */
        /**
        else if (Input.GetMouseButtonDown(1))
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(LayerMask.GetMask("Enemy"));

            Collider2D[] hitColliders = new Collider2D[10];
            int numHits = Physics2D.OverlapCollider(GetComponent<Collider2D>(), contactFilter, hitColliders);

            for (int i = 0; i < numHits; i++)
            {
                Collider2D collider = hitColliders[i];
                if (collider != null && collider.CompareTag("Enemy"))
                {
                    Vector3 dir = (collider.transform.position - transform.position).normalized;

                    float angle = Vector3.Angle(direction, dir);
                    if (angle <= knifeAngle)
                    {
                        Debug.Log("HIT " + collider.name);
                        collider.gameObject.GetComponent<EnemyManager>().gainDamage(knifeDamage);
                    }
                }
            }

            LineRenderer lineRenderer = GetComponent<LineRenderer>();


            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.cyan;

            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;

            lineRenderer.positionCount = 3;

            Vector2 startPosition = transform.position;

            float halfAngle = knifeAngle / 2f;

            Vector2 leftDirection = Quaternion.Euler(0, 0, -halfAngle) * direction;
            Vector2 rightDirection = Quaternion.Euler(0, 0, halfAngle) * direction;

            Vector2 leftEndPoint = startPosition + leftDirection.normalized * knifeDistance;
            Vector2 rightEndPoint = startPosition + rightDirection.normalized * knifeDistance;

            lineRenderer.SetPosition(0, leftEndPoint);   // 좌측 끝점
            lineRenderer.SetPosition(1, startPosition);  // 시작점 (플레이어 위치)
            lineRenderer.SetPosition(2, rightEndPoint);  // 우측 끝점
        }
        */


        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 direction = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        fovLight.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInvincible)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                if (!movement.IsDashing)
                {
                    health -= 1;
                    isInvincible = true;
                    LayerMask lm = collision.gameObject.layer;
                    Physics2D.IgnoreLayerCollision(gameObject.layer, lm, ignore: true);
                    //Physics2D.IgnoreCollision(coll, GetComponent<Collider2D>(), true);
                    hpBar.value = health;
                    StartCoroutine(updateInvincible(lm));
                    StartCoroutine(updateSpriteBlur());
                }
                else
                {
                    //collision.gameObject.GetComponent<EnemyManager>().gainDamage(movement.DashDamage);
                }
            }
        } 
    }
    */

    //충돌 (물리) 처리. 보스가 던지는 돌덩이만 적용되어있음.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mz) return;
        if (collision.gameObject.CompareTag("Rock"))
        {
            health -= 10;
            hpBar.value = health;
            animator.Play("Hit");
            //StartCoroutine(updateSpriteBlur());
        }
    }

    //충돌 시작 시 (비-물리) 처리. 보스 스킬 (레이저), 적 충돌 (부딪힘)에 대한 처리. 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mz) return;
        if (collision.gameObject.tag == "Laser")
        {
            health -= (int) (health / 2);
            hpBar.value = health;
            animator.Play("Hit");
        }
        if (!isInvincible)
        {
            if (collision.gameObject.tag == "Enemy")
            {   
                if (collision.gameObject.GetComponent<EnemyManager>() != null)
                {
                    if (collision.gameObject.GetComponent<EnemyManager>().isDead) return;
                    health -= 1;
                    isInvincible = true;
                    LayerMask lm = collision.gameObject.layer;
                    Physics2D.IgnoreLayerCollision(gameObject.layer, lm, ignore: true);
                    //Physics2D.IgnoreCollision(coll, GetComponent<Collider2D>(), true);
                    hpBar.value = health;
                    animator.Play("Hit");
                    StartCoroutine(updateInvincible(lm));
                    StartCoroutine(updateSpriteBlur());
                }
                else if (collision.gameObject.GetComponent<EnemyBoss>() != null)
                {
                    if (collision.gameObject.GetComponent<EnemyBoss>().isDead) return;
                    health -= 5;
                    isInvincible = true;
                    LayerMask lm = collision.gameObject.layer;
                    Physics2D.IgnoreLayerCollision(gameObject.layer, lm, ignore: true);
                    hpBar.value = health;
                    animator.Play("Hit");
                    StartCoroutine(updateInvincible(lm));
                    StartCoroutine(updateSpriteBlur());
                }
            }
        }
    }

    //충돌 유지 시 (비-물리) 처리. 보스 스킬 (레이저), 적 충돌 (부딪힘)에 대한 처리. 
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (mz) return;
        if (!isInvincible)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                if (collision.gameObject.GetComponent<EnemyManager>() != null)
                {
                    if (collision.gameObject.GetComponent<EnemyManager>().isDead) return;
                    health -= 1;
                    isInvincible = true;
                    LayerMask lm = collision.gameObject.layer;
                    Physics2D.IgnoreLayerCollision(gameObject.layer, lm, ignore: true);
                    //Physics2D.IgnoreCollision(coll, GetComponent<Collider2D>(), true);
                    hpBar.value = health;
                    animator.Play("Hit");
                    StartCoroutine(updateInvincible(lm));
                    StartCoroutine(updateSpriteBlur());
                }
                else if (collision.gameObject.GetComponent<EnemyBoss>() != null)
                {
                    if (collision.gameObject.GetComponent<EnemyBoss>().isDead) return;
                    health -= 5;
                    isInvincible = true;
                    LayerMask lm = collision.gameObject.layer;
                    Physics2D.IgnoreLayerCollision(gameObject.layer, lm, ignore: true);
                    hpBar.value = health;
                    animator.Play("Hit");
                    StartCoroutine(updateInvincible(lm));
                    StartCoroutine(updateSpriteBlur());
                }
            }
        }
    }

    //무적 (충돌 시 3초) 설정 코루틴 구현부. 레이어 마스크를 이용하여 구현
    private IEnumerator updateInvincible(LayerMask lm)
    {
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
        Physics2D.IgnoreLayerCollision(gameObject.layer, lm, false);
    }

    //어둡게 설정 (스프라이트)
    private void Fade()
    {
        Color fc = sr.color;
        fc.a = 0f;
        sr.color = fc;
    }

    //밝게 설정 (스프라이트)
    private void Sharp()
    {
        Color fc = sr.color;
        fc.a = 255f;
        sr.color = fc;
    }
    
    //피해 입을 시 0.2초마다 반짝이게 함
    private IEnumerator updateSpriteBlur()
    {
        for (int i = 0; i < 5; i++)
        {
            Fade();
            yield return new WaitForSeconds(0.2f);

            Sharp();
            yield return new WaitForSeconds(0.2f);
        }
    }

    //TEST ZONE

    public GameObject nav;

    public bool mz = false;
    
    /**
    * 1. NavMesh 관련 오류 시 f키로 강제 계산하게 하여 해결
    * 2. 강제 무적 설정 (J키)
    */
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.F))
        {
            nav.GetComponent<Nav>().buildNavMesh();
        }
        if (Input.GetKey(KeyCode.J))
        {
            mz = !mz;
        }
    }
}
