using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Boss : Enemy
{
    public Animator anim;

    public Rigidbody2D myRigidbody;
    [Header("Target Variables")]
    public Transform target;
    public float chaseRadius;
    public float attackRadius;
    public float AttackSpeed = 3;
    private float elapsedTimeAttack = 0f;
    private float elapsedTimeMovement = 0f;
    private float tempSpeed;
    public float time = 0;
    public float stamina;
    public bool canSpin = false;
    public float maxStamina;
    PopUpDialogue pop;
    public Slider healthSlider;
    private float playerSighted = 0;
    public Transform leftWayPointDown, rightWayPointDown, leftWayPointMiddle, rightWayPointmiddle, leftWayPointUp, rightWayPointUp, walkToStage1, walkToStage2;
    Vector3 nextPos;
    public Transform startPos;
    public GameObject arrow;
    private float fireDelaySeconds;
    public float fireDelay;
    public bool canFire = true;
    private bool enraged = false;



    [SerializeField]
    Transform rotationCenter;

    [SerializeField]
    float rotationRadius = 2f, angularSpeed = 2f;

    float posX, posY, angle = 0f;

    // Update is called once per frame
   
// Use this for initialization
void Start()
    {
        currentState = EnemyState.idle;
        myRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
        tempSpeed = moveSpeed;
        this.stamina = this.maxStamina;
        pop = GameObject.FindGameObjectWithTag("Interact").GetComponent<PopUpDialogue>();
        //healthSlider.value = maxHealth;
        nextPos = startPos.position;


    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (health <= (maxHealth * 0.8) && stage != 4)
        {
            stage = 1;
            moveSpeed = 4;
            fireDelay = 2.5f;
            anim.SetFloat("moveX", 1);
            anim.SetFloat("moveY", 0);

        }
        if (health <= (maxHealth * 0.6) && stage != 4 )
        {
            stage = 2;
            moveSpeed = 5;
            fireDelay = 2f;
            anim.SetFloat("moveX", 0);
            anim.SetFloat("moveY", -1);

        }
        if (health <= (maxHealth * 0.4) && stage != 4)
        {
            stage = 3;
            moveSpeed = 6;
            fireDelay = 1.5f;
            anim.SetFloat("moveX", -1);
            anim.SetFloat("moveY", 0);

        }
        
        fireDelaySeconds -= Time.deltaTime;
        if(stage == 0)
        {
            if (transform.position == leftWayPointDown.position)
            {
                nextPos = rightWayPointDown.position;
            }
            if (transform.position == rightWayPointDown.position)
            {
                nextPos = leftWayPointDown.position;
            }
        }
        if (stage == 1)
        {
            if (transform.position == rightWayPointDown.position)
                nextPos = leftWayPointDown.position;
            if (transform.position == leftWayPointDown.position)
                nextPos = leftWayPointMiddle.position;
            if (transform.position == leftWayPointMiddle.position)
                nextPos = rightWayPointmiddle.position;            
            if (transform.position == rightWayPointmiddle.position)            
                nextPos = leftWayPointMiddle.position;          
            
        }
        if (stage == 2)
        {
            if (transform.position == leftWayPointMiddle.position)
                nextPos = rightWayPointmiddle.position;
            if (transform.position == rightWayPointmiddle.position)
                nextPos = leftWayPointUp.position;
            if (transform.position == leftWayPointUp.position)
                nextPos = rightWayPointUp.position;
            if (transform.position == rightWayPointUp.position)
                nextPos = leftWayPointUp.position;
        }
        
        if (stage == 3)
        {
            if (transform.position == leftWayPointUp.position)
                nextPos = rightWayPointUp.position;
            if (transform.position == rightWayPointUp.position)
            {
                nextPos = walkToStage1.position;
            }
            if (transform.position == walkToStage1.position)
            {
                nextPos = walkToStage2.position;
            }
            if (transform.position == walkToStage2.position)
            {
                stage = 4;
            }
       
        }

        if (stage != 4)
            transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);

        if(fireDelaySeconds <= 0 && stage != 4)
        {
            canFire = true;
            fireDelaySeconds = fireDelay;

        }
        if (canFire && stage != 4)
        {
            Debug.Log("fireattack");
            FireRangedAttack();
            canFire = false;
        }
        //healthSlider.value = health / maxHealth;
        
        if(stage == 4 && currentState != EnemyState.enraged && !enraged)
        {
            if (health < (maxHealth / 2.0))
            {
                enraged = true;
                currentState = EnemyState.enraged;

            }
            if (currentState == EnemyState.enraged && !anim.GetBool("enraged"))
            {
                Debug.Log("enraged now");
                anim.SetBool("enraged", true);
                moveSpeed = 2;
                AttackSpeed = 1;

                attackRadius = 3;
                health += 50;
                canSpin = true;
            }
            if (canSpin && stamina > 50)
            {
                stamina -= 50;
                StartCoroutine(SpinAttack());

            }
            
        }
        if(stage == 4)
        {
            CheckDistance();
            if (canSpin && stamina > 50)
            {
                stamina -= 50;
                StartCoroutine(SpinAttack());

            }

        }

        if (stamina < maxStamina)
            stamina += (3 * Time.deltaTime);





    }



    public void FireRangedAttack()
    {
        anim.SetTrigger("rangedAttack");
        Vector3 tempVector = target.transform.position - transform.position;
        GameObject current = Instantiate(arrow , transform.position, Quaternion.identity);
        if (stage == 1)
            current.GetComponent<Projectile>().moveSpeed = 2f;
        if (stage == 2)
            current.GetComponent<Projectile>().moveSpeed = 2.5f;
        if (stage == 3)
            current.GetComponent<Projectile>().moveSpeed = 3f;

        current.GetComponent<Projectile>().Launch(tempVector);
        canFire = false;


    }
   
    public void CheckDistance()
    {

        if (Vector3.Distance(target.position,
                            transform.position) <= chaseRadius
             && Vector3.Distance(target.position,
                               transform.position) > attackRadius)
        {

            if (currentState != EnemyState.stagger && currentState != EnemyState.spinning)
            {

                Vector3 temp = Vector3.MoveTowards(transform.position,
                                                         target.position,
                                                         moveSpeed * Time.deltaTime);
                changeAnim(temp - transform.position);
                //anim.SetBool("moving", true);
                myRigidbody.MovePosition(temp);
                elapsedTimeMovement = Time.time;
                playerSighted += 1;

            }
        }
        else if (Vector3.Distance(target.position,
                    transform.position) <= chaseRadius
                    && Vector3.Distance(target.position,
                    transform.position) <= attackRadius)
        {
            if (currentState != EnemyState.stagger && Time.time > elapsedTimeAttack)
            {
                Debug.Log("attack");
                StartCoroutine(AttackCo());
                elapsedTimeAttack = Time.time + AttackSpeed;

            }
        }

    }

    public IEnumerator AttackCo()
    {
        currentState = EnemyState.attack;
        anim.SetBool("attack", true);
        yield return new WaitForSeconds(.3f);
        anim.SetBool("attack", false);
    }
    private IEnumerator SpinAttack()
    {
        Debug.Log("spinning");
        currentState = EnemyState.spinning;
        anim.SetBool("spin", true);
        yield return null;
        //yield return new WaitForSeconds(10f);
        anim.SetBool("spin", false);
        currentState = EnemyState.enraged;
    }

    public void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
    }

    public void changeAnim(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                SetAnimFloat(Vector2.right);
            }
            else if (direction.x < 0)
            {
                SetAnimFloat(Vector2.left);
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0)
            {
                SetAnimFloat(Vector2.up);
            }
            else if (direction.y < 0)
            {
                SetAnimFloat(Vector2.down);
            }
        }
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }
}
