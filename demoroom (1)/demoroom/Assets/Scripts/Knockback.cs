using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{

    public float thrust;
    public float knockTime;
    public float damage;
    private IsometricPlayerMovement player;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Boss"))
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (other.gameObject.CompareTag("Player"))
            {
                hit = other.transform.parent.GetComponent<Rigidbody2D>();
            }
            if (hit != null)
            {

                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
                {
                    hit.GetComponent<Enemy>().currentState = EnemyState.stagger;
                    other.GetComponent<Enemy>().Knock(hit, knockTime, damage);
                }
                if (other.gameObject.CompareTag("Player"))
                {
                    if(hit.GetComponent<IsometricPlayerMovement>().currentState != PlayerState.dead)
                    {
                        hit.GetComponent<IsometricPlayerMovement>().currentState = PlayerState.stagger;
                        other.transform.parent.GetComponent<IsometricPlayerMovement>().Knock(knockTime, damage);
                    }
                 
                   
                }
                


            }
        }
        
    }

  
    

}
