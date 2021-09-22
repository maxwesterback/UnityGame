using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
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





    // Use this for initialization
    void Start()
    {
        currentState = EnemyState.idle;
        myRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
        tempSpeed = moveSpeed;
    

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckDistance();
        
      
    }

    public void CheckDistance()
    {
        if (Vector3.Distance(target.position,
                            transform.position) <= chaseRadius
             && Vector3.Distance(target.position,
                               transform.position) > attackRadius)
        {
            if (currentState != EnemyState.stagger)
            {
                if(Time.time > moveSpeedBuffDuration + moveSpeedBuffStart)
                {
                    moveSpeed = tempSpeed;

                }
                if (Time.time % moveSpeedIntervall == 0)
                {
                    moveSpeedBuffStart = Time.time;
                    moveSpeed = moveSpeedBuff;
                }              
              
                Vector3 temp = Vector3.MoveTowards(transform.position,
                                                         target.position,
                                                         moveSpeed * Time.deltaTime);
                changeAnim(temp - transform.position);
                anim.SetBool("moving", true);
                myRigidbody.MovePosition(temp);
                ChangeState(EnemyState.walk);
                elapsedTimeMovement = Time.time;
            }

        }    
        
        else if (Vector3.Distance(target.position,
                    transform.position) <= chaseRadius
                    && Vector3.Distance(target.position,
                    transform.position) <= attackRadius)
        {
            if (currentState == EnemyState.walk
                && currentState != EnemyState.stagger && Time.time > elapsedTimeAttack)
            {
                StartCoroutine(AttackCo());
                elapsedTimeAttack = Time.time + AttackSpeed;

            }
        }

    }

   
    public IEnumerator AttackCo()
    {
        currentState = EnemyState.attack;
        anim.SetBool("attack", true);
        yield return new WaitForSeconds(.5f);
        currentState = EnemyState.walk;
        anim.SetBool("attack", false);
        //player.TakeDamage(2);
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
