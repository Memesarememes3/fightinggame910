using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Max Values")]
    public int maxHealth;
    public int maxJumps;
    public float curAttackTime;
    public float slowTime;
    public float maxSpeed;



    [Header("Cur Value")]
    public int curHP;
    public int curJumps;
    public int score;
    public float curMoveInput;
    public int dieCount;
    public float lastHit;
    public float lastHitice;
    public bool isSlowed;

    [Header("power-ups")]
    public bool amFireFast;
    public bool amFast;
    public bool amInvis;
    public float becameFast;
    public float becameInvis;
    public float becameFastFire;

    public float regAttackRate;
    public float fastTimer;
    public float InvisTimer;
    public float fireFastTimer;

    public Sprite[] Sprite_list;



    [Header("Attacking")]
    public PlayerController curAttacker;
    public float attackDmg;
    public float chargeDmg;
    public float attackSpeed;
    public float iceAttackSpeed;
    public float attackRate;
    public float lastAttackTime;
    public float lastCharge;
    public float chargeTime;
    public bool charging;
    public GameObject[] attackPrefabs;



    [Header("MODS")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Audio clips")]
    //jump snd 0
    //land snd 1
    //taunt_1 2
    //shoot snd 3
    //die snd 4
    public AudioClip[] playerfx_list;


    [Header("Components")]
    [SerializeField]
    private Rigidbody2D rig;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private AudioSource audio;
    private Transform muzzle;
    private GameManager gameManager;
    private PlayerContainerScript playerUI;
    public GameObject DeathEffectprefab;



    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        muzzle = GetComponentInChildren<Muzzle>().GetComponent<Transform>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }



    // Start is called before the first frame update
    void Start()
    {
        curHP = maxHealth;
        curJumps = maxJumps;
        score = 0;
        dieCount = 0;
        moveSpeed = maxSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -10 || curHP <= 0)
        {
            die();
        }
        if(dieCount >= 10)
        {
            die();
            dieCount = 0;
        }

        if(curAttacker)
        {
            if(Time.time - lastHit > curAttackTime)
            {
                curAttacker = null;
            }
        }

        if(isSlowed)
        {
            if (Time.time - lastHitice > slowTime)
            {
                isSlowed = false;
                moveSpeed = maxSpeed;
            }
        }

        if (Time.time - lastCharge > chargeTime)
        {
            if (charging)
            {
                chargeDmg += 1;
                lastCharge = Time.time;
                
            }
        }

        if (amFast)
        {
            if (Time.time - becameFast > fastTimer)
            {
                moveSpeed = maxSpeed;
                amFast = false;
            }
        }

        
        /*if (amInvis)
        {
            if (Time.time - becameInvis > InvisTimer)
            {
                amInvis = false;
                Sprite.sprite = SpriteList[1];
            }
        }*/


        if (amFireFast)
        {
            if (Time.time - becameFastFire > fireFastTimer)
            {
                amFireFast = false;
                attackRate = regAttackRate;
            }
        }


    }


    private void FixedUpdate()
    {
        move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        foreach(ContactPoint2D hit in collision.contacts)
        {
            if(hit.collider.CompareTag("Ground"))
            {
                audio.PlayOneShot(playerfx_list[1]);
                if (hit.point.y < transform.position.y)
                {
                    curJumps = maxJumps;
                }
            }
            if((hit.point.x > transform.position.x || hit.point.x < transform.position.x) && hit.point.y < transform.position.y)
            {
                curJumps++;
            }
        }
        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public void speedup(float speed)
    {
        moveSpeed += speed;
        becameFast = Time.time;
        amFast = true;
    }


    /*public void Invis()
    {
        Sprite.sprite = SpriteList[0];
        becameInvis = Time.time;
        amInvis = true;
    }*/


    public void FastFire(float fireRate)
    {
        attackRate -= fireRate;
        becameFastFire = Time.time;
        amFireFast = true;


    }


    private void move()
    {
        rig.velocity = new Vector2(curMoveInput * moveSpeed, rig.velocity.y);
        if(curMoveInput != 0)
        {
            transform.localScale = new Vector3(curMoveInput > 0 ? 1 : -1, 1, 1);
        }
    }

    private void jump()
    {
        
        rig.velocity = new Vector2(rig.velocity.x, 0);

        //add force up
        rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        audio.PlayOneShot(playerfx_list[0]);
    }

    public void die()
    {
        Destroy(Instantiate(DeathEffectprefab, transform.position, Quaternion.identity),1f);
        //play die snd
        audio.PlayOneShot(playerfx_list[4]);
        if(curAttacker != null)
        {
            curAttacker.addScore();
        }
        else
        {
            score--;
            if(score < 0)
            {
                score = 0;
            }

        }
        dieCount++;
        respawn();

    }

    public void dropOut()
    {
        Destroy(playerUI.gameObject);
        Destroy(gameObject);
    }

    public void addScore()
    {
        score++;
        playerUI.updateScoreText(score);
    }

    public void takeDamage(int ammount, PlayerController attacker)
    {
        curHP -= ammount;
        curAttacker = attacker;
        lastHit = Time.time;
        playerUI.updateHealthBar(curHP, maxHealth);
    }

    public void takeDamage(float ammount, PlayerController attacker)
    {
        curHP -= (int)ammount;
        curAttacker = attacker;
        lastHit = Time.time;
        playerUI.updateHealthBar(curHP, maxHealth);
    }

    public void takeIceDamage(float ammount, PlayerController attacker)
    {
        curHP -= (int)ammount;
        curAttacker = attacker;
        lastHit = Time.time;
        isSlowed = true;
        moveSpeed /= 2;
        lastHit = Time.time;
        lastHitice = Time.time;
        playerUI.updateHealthBar(curHP, maxHealth);
    }

    private void respawn()
    {
        curHP = maxHealth;
        curJumps = maxJumps;
        curAttacker = null;
        rig.velocity = Vector2.zero;
        moveSpeed = maxSpeed;
        transform.position = gameManager.spawn_points[Random.Range(0, gameManager.spawn_points.Length)].position;
        playerUI.updateHealthBar(curHP, maxHealth);
    }


    //move input method
    public void onMoveInput(InputAction.CallbackContext context)
    {
        
        float x = context.ReadValue<float>();
        if(x > 0)
        {
            curMoveInput = 1;
        }
        else if(x < 0)
        {
            curMoveInput = -1;
        }
        else
        {
            curMoveInput = 0;
        }
    }

    public void onJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if(curJumps > 0)
            {
                curJumps--;
                jump();
            }
        }


    }


    public void onBlockInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed block button");
        }

    }


    public void onAttackStandardInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            spawn_std_attack();
        }
    }

    public void spawn_std_attack()
    {
        GameObject fireBall = Instantiate(attackPrefabs[0], muzzle.position, Quaternion.identity);
        fireBall.GetComponent<Projectile>().onSpawn(attackDmg, attackSpeed, this, transform.localScale.x);
    }

    public void spawn_ice_attack()
    {
        GameObject iceBall = Instantiate(attackPrefabs[1], muzzle.position, Quaternion.identity);
        iceBall.GetComponent<Projectile>().onSpawn(attackDmg, iceAttackSpeed, this, transform.localScale.x);
    }


    public void onAttackChargeInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            charging = true;
        }


        if (context.phase == InputActionPhase.Canceled && Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            spawn_Charge_attack(attackDmg, attackSpeed);
            chargeDmg = attackDmg;
            charging = false;
        }


        

    }

    public void spawn_Charge_attack(float dmg, float speed)
    {
        print("spawn");
        GameObject fireBall = Instantiate(attackPrefabs[0], muzzle.position, Quaternion.identity);
        fireBall.GetComponent<Projectile>().onSpawn(chargeDmg, attackSpeed, this, transform.localScale.x);
    }

    public void setUI(PlayerContainerScript playerUI)
    {
        this.playerUI = playerUI;
    }

    public void onAttackIceInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate*2)
        {
            lastAttackTime = Time.time;
            spawn_ice_attack();
        }
    }


    public void onTaunt1Input(InputAction.CallbackContext context)
    {
        
        if (context.phase == InputActionPhase.Performed)
        {
            audio.PlayOneShot(playerfx_list[2]);
        }
    }


    public void onTaunt2Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt2 button");
        }
    }

    public void onTaunt3Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt3 button");
        }
    }

    public void onTaunt4Input(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed taunt4 button");
        }
    }

    public void onPauseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed pause button");
        }
    }
}
