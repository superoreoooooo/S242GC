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

    private void initPlayerMoveRoom()
    {
        isInvincible = false;
        transform.position = new Vector2((gameData.SpawnX) * gameData.RoomSizeX + 18, (gameData.SpawnY) * gameData.RoomSizeY + 19);
        mapManager = FindObjectOfType<MapManager>();
    }

    private SpriteRenderer sr;

    private bool isInteractable;

    private IEnumerator fadeIn()
    {
        yield return new WaitForSeconds(0.5f);

        onPlayerTeleportComplete.Invoke();
    }

    private IEnumerator teleport(Vector3 pos)
    {
        yield return new WaitForSeconds(1f);

        transform.position = pos;

        movement.isMoveable = true;

        StartCoroutine(fadeIn());
    }

    private void updatePlayerMoveRoom()
    {
        if (mapManager == null) return;

        cellNow = mapManager.getCell((Vector2) transform.position);
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
                        case CellDirection.LEFT: //DOWN
                            position = cell.GetComponent<Cell>().DOOR_LEFT.transform.position;
                            break;
                        case CellDirection.RIGHT: //DOWN
                            position = cell.GetComponent<Cell>().DOOR_RIGHT.transform.position;
                            break;
                    }
                }

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

    private void dead()
    {
        onPlayerDead.Invoke();
        gameObject.SetActive(false);
        print("DIE!");
    }

    public void gainDamage(int amount)
    {
        health -= amount;
    }

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
            }
        }

        interacionUI.SetActive(isInteractable);
    }

    void Start()
    {
        //initPlayerMoveRoom();

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

    void Update()
    {
        if (health <= 0)
        {
            dead();
        }

        if (!movement.isMoveable) return;

        updateInteraction();
        updatePlayerMoveRoom();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - transform.position).normalized;

        /*
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + new Vector3(direction.x, direction.y, transform.position.z) * lrLen);
        */

        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(projectile, transform.position, Quaternion.identity);
            Projectile pj = obj.GetComponent<Projectile>();
            pj.direction = direction;
        }
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

            // 좌우로 60도 각도의 벡터 계산 (2D 기준으로)
            float halfAngle = knifeAngle / 2f;

            // 좌측 방향 (direction에서 -60도 회전)
            Vector2 leftDirection = Quaternion.Euler(0, 0, -halfAngle) * direction;
            // 우측 방향 (direction에서 +60도 회전)
            Vector2 rightDirection = Quaternion.Euler(0, 0, halfAngle) * direction;

            // 각 방향으로의 끝점 계산 (길이 5)
            Vector2 leftEndPoint = startPosition + leftDirection.normalized * knifeDistance;
            Vector2 rightEndPoint = startPosition + rightDirection.normalized * knifeDistance;

            // LineRenderer로 원뿔의 양쪽 끝과 시작점을 그리기
            lineRenderer.SetPosition(0, leftEndPoint);   // 좌측 끝점
            lineRenderer.SetPosition(1, startPosition);  // 시작점 (플레이어 위치)
            lineRenderer.SetPosition(2, rightEndPoint);  // 우측 끝점
        }
        */
    }

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

    private IEnumerator updateInvincible(LayerMask lm)
    {
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
        Physics2D.IgnoreLayerCollision(gameObject.layer, lm, false);
    }

    private void Fade()
    {
        Color fc = sr.color;
        fc.a = 0f;
        sr.color = fc;
    }

    private void Sharp()
    {
        Color fc = sr.color;
        fc.a = 255f;
        sr.color = fc;
    }

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

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.F))
        {
            nav.GetComponent<Nav>().buildNavMesh();
        }
        if (Input.GetKey(KeyCode.J))
        {
            //MAPMGR.GetComponent<MapManager>().genCell(get, 0, CellDirection.LEFT);
        }
    }


}
