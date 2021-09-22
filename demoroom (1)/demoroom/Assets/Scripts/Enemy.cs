using UnityEngine;
using System.Collections;


public enum EnemyState
{
    idle,
    walk,
    attack,
    stagger,
    boost,
    enraged,
    spinning
  
}
public class Enemy: MonoBehaviour
{
    [Header("State Machine")]
    public EnemyState currentState;
    [Header("Enemy Stats")]
    public float health;
    public string enemyName;
    public int baseAttack;
    public float maxHealth;
    public float moveSpeed;
    public float moveSpeedBuff;
    public float moveSpeedBuffDuration;
    public float moveSpeedIntervall;
    public float moveSpeedBuffStart;
    public float stage;
    public bool boss;
    public GameObject bossChar;
    public LootTable thisLoot;
    private void Awake()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (boss)
        {
            bossChar.SetActive(true);
            Debug.Log("spawn boss");
            boss = false;
          
        }
    }

    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = EnemyState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }
    public void Knock(Rigidbody2D myRigidbody, float knockTime, float damage)
    {
        StartCoroutine(KnockCo(myRigidbody, knockTime));
        if(health - damage <= 0)
            StartCoroutine(KnockCo(myRigidbody, 2f));
        TakeDamage(damage);
    }

    private void MakeLoot()
    {
        if (thisLoot != null)
        {
            GameObject current = thisLoot.LootPowerup();
            if (current != null)
            {
                Instantiate(current.gameObject, transform.position, Quaternion.identity);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
      
        if(health <= 0) {
            MakeLoot();
            this.gameObject.SetActive(false);
        }
    }

}