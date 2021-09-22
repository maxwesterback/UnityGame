using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public enum PlayerState
{
    run,
    attack,
    interact,
    stagger,
    idle,
    dead,
    dashAttack,
    spinAttack,
    dodge,
    shielding
}
public class IsometricPlayerMovement : MonoBehaviour
{
    public Slider healthBar;
    public Slider staminaBar;
    public float movementSpeed;
    public GameObject inventoryScreen;
    public bool hasSword = false;
    public GameObject shortcutSword;
    public GameObject shortcutShield;
    public Transform target;
    public bool canFire = false;
    public bool interacting = false;

    [SerializeField] private InventoryManager inventoryManager;

    public PlayerState currentState;
    private Animator animator;
    private Vector3 change;
    public float currentHealth;
    public float maxStamina;
    public float currentStamina;
    public int maxHealth;

    public Message playerHit;
    public GameObject currentInterObj = null;
    public GameObject sword;
    public GameObject shield;
    public GameObject shoes;
    public GameObject belt;
    public GameObject gloves;
    public GameObject sword_pickup;
    public GameObject chest;
    public GameObject arrow;
    public GameObject lastEquipped;
    public GameObject lastEquippedShortcut;
    public GameObject lastEquippedInventory;


    public float AttackSpeed = 3;
    private float elapsedTime = 0;
    public float channelTime;
    private float tempTime;
    private Vector2 tempMovement = Vector2.down;
    public GameObject currentlyEquipped = null;
    PopUpDialogue pop;
    Rigidbody2D rbody;
    Enemy boss;

    private void Start()
    {
        currentState = PlayerState.idle;
        animator = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
        this.currentHealth = this.maxHealth;
        this.currentStamina = this.maxStamina;
        healthBar.value = 1;
        staminaBar.value = 1;
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
        tempTime = channelTime;
        pop = GameObject.FindGameObjectWithTag("Interact").GetComponent<PopUpDialogue>();
        
    }
    private void Awake()
    {
    }




    // Update is called once per frame
    void Update()
    {
            
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        GetInput();


        if (currentState == PlayerState.idle)
        {
            if (channelTime > 0)
            {
                channelTime -= Time.deltaTime;

            }
            else
            {
                if (currentStamina < maxStamina)
                    currentStamina += (30 * Time.deltaTime);

                if (currentHealth < maxHealth && chest.activeSelf)
                    currentHealth += (20 * Time.deltaTime);
                
            }

        }

        else if (currentState != PlayerState.stagger)
        {
            channelTime = tempTime;
        }
        healthBar.value = currentHealth / maxHealth;
        staminaBar.value = currentStamina / maxStamina;


    }

    void GetInput()
    {
       
        if(Input.GetKey(KeyCode.E) && currentState == PlayerState.shielding && currentState != PlayerState.dead && currentStamina > 20)
        {
            currentStamina -= (30 * Time.deltaTime);
            Time.timeScale = 0.5f;
        }
       
        if(Input.GetKeyUp(KeyCode.E) &&  currentState == PlayerState.shielding && currentState != PlayerState.dead)
            FireRangedAttack();
        if (Input.GetButton("attack") && currentlyEquipped == shield && currentState != PlayerState.dead)
        {
            Shield();
        }
        else if (currentState != PlayerState.dead)
        {
            currentState = PlayerState.run;
            animator.SetBool("Shielding", false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && sword.activeSelf && currentState != PlayerState.dead)
        {
            currentlyEquipped = sword;
            Debug.Log(currentlyEquipped.name + " is now equipped");

        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && shield.activeSelf && currentState != PlayerState.dead)
        {

            currentlyEquipped = shield;
            Debug.Log(currentlyEquipped.name + " is now equipped");
        }

        if (Input.GetButtonDown("attack") && currentState != PlayerState.attack && currentState != PlayerState.stagger && currentState != PlayerState.dead)
        {
            
           
                if (currentlyEquipped == sword && Time.time > elapsedTime)
                {
                    StartCoroutine(AttackCo());
                    elapsedTime = Time.time + AttackSpeed;


                }

            

        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && belt.activeSelf && currentState != PlayerState.dead)
        {
            StartCoroutine(Dodge());
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && channelTime <= 0 && shoes.activeSelf && sword.activeSelf && currentState != PlayerState.dead)
        {
            StartCoroutine((DashAttack()));

        }
        if (Input.GetKeyDown(KeyCode.F) && gloves.activeSelf && sword.activeSelf && currentState != PlayerState.dead)
        {
            StartCoroutine(SpinAttack());
        }
        
        
        if (Input.GetButtonDown("Interact") && !interacting)
        {

            EquipDialogue(currentInterObj);
        }

        else if (Input.GetButtonDown("Interact") && interacting)
        {

            pop.anim.SetTrigger("close");
            pop.Resume();
            currentState = PlayerState.idle;
            interacting = false;
        }
        else if (Input.GetKeyDown(KeyCode.X) && interacting)
        {
            UnEquip();
            interacting = false;
           
        }
        
        if (Input.GetKeyDown(KeyCode.R) && currentState == PlayerState.dead)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
            //SceneManager.LoadScene("demoroom");
            Time.timeScale = 1;

        }
        if (Input.GetButtonDown("Inventory") && !inventoryScreen.activeSelf && currentState != PlayerState.dead)
        {
            inventoryManager.Opna();
            inventoryScreen.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (Input.GetButtonDown("Inventory") && inventoryScreen.activeSelf && currentState != PlayerState.dead)
        {
            Debug.Log("Closed Inventory");
            inventoryScreen.SetActive(false);
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
            currentHealth = maxHealth;

       if (currentState != PlayerState.attack && currentState != PlayerState.shielding && currentState != PlayerState.dead && currentState != PlayerState.dodge && currentState != PlayerState.dashAttack && currentState != PlayerState.interact)
            {
                tempMovement.x = Input.GetAxis("Horizontal");
                tempMovement.y = Input.GetAxis("Vertical");
                currentState = PlayerState.run;
                UpdateAnimationAndMove();
            }

        }
        
    


    private void Shield()
    {
        if (currentStamina > 5)
        {
            currentStamina -= (5 * Time.deltaTime);
            currentState = PlayerState.shielding;
            staminaBar.value = currentStamina / maxStamina;
            animator.SetBool("Shielding", true);
          
        }
        else
        {
            animator.SetBool("Shielding", false);
            currentState = PlayerState.idle;
        }

    }
    public void FireRangedAttack()
    {
        Time.timeScale = 1f;

        currentStamina -= 30;
        if (currentStamina < 0)
            currentStamina = 0;
        staminaBar.value = currentStamina / maxStamina;
        //target = GameObject.FindWithTag("Boss").transform;
        if (canFire)
        {
            Debug.Log("firing arrow");
            Vector3 tempVector = target.transform.position - transform.position;
            GameObject current = Instantiate(arrow, transform.position, Quaternion.identity);
            current.GetComponent<Projectile>().Launch(tempVector);
            canFire = false;
        }
       
    }

    public IEnumerator ChangeCanFire()
    {
        canFire = true;
        yield return new WaitForSeconds(0.5f);
        canFire = false;

    }
  

    
    private void EquipDialogue(GameObject item)
    {
        //currentInterObj.SendMessage("DoInteraction");
        string itemName = item.name;
        currentState = PlayerState.interact;
        interacting = true;
        lastEquipped = item;
        if (itemName == "Sword")
        {
            currentInterObj.SetActive(false);
            sword.SetActive(true);
            shortcutSword.SetActive(true);
            pop.PopUp("Ah my trusty sword!", "Press SPACE to attack!");
            //currentlyEquipped = sword;
            lastEquippedInventory = sword;


        }
        else if (itemName == "Shield(Clone)")
        {
            currentInterObj.SetActive(false);
            shield.SetActive(true);
            shortcutShield.SetActive(true);
            lastEquippedShortcut = shortcutShield;
            lastEquippedInventory = shield;
            Debug.Log("picking up shield");

            pop.PopUp("I believe I can deflect some arrows with this shield", "Hold SPACE to block and E to time a counter attack!");
            boss = GameObject.FindGameObjectWithTag("BossSpawner").GetComponent<Enemy>();
            boss.boss = true;

        }

        else if (itemName == "Gloves")
        {
            currentInterObj.SetActive(false);
            lastEquippedInventory = gloves;

            gloves.SetActive(true);
            pop.PopUp("These gloves will help me grip my sword tighter", "Press F to charge a devastating Spin attack!");
        }
        else if (itemName == "Belt")
        {
            currentInterObj.SetActive(false);
            lastEquippedInventory = belt;

            belt.SetActive(true);
            pop.PopUp("I feel flexible! Watch me dodge those attacks now!", "Press SHIFT to dodge backwards! ");

        }
        else if (itemName == "Chest")
        {
            currentInterObj.SetActive(false);
            chest.SetActive(true);
            pop.PopUp("Now I'm feeling invincible", "You will now regenerate HP while standing still");
            lastEquippedInventory = chest;


        }
        else if (itemName == "Shoes")
        {
            currentInterObj.SetActive(false);
            lastEquippedInventory = shoes;

            shoes.SetActive(true);
            pop.PopUp("Good news! I can dash now!", "Stand still and press CTRL to charge up a powerful dash attack! ");

        }

    }

    private void UnEquip()
    {
        lastEquipped.SetActive(true);
        if(lastEquippedShortcut)
            lastEquippedShortcut.SetActive(false);
        pop.anim.SetTrigger("close");
        pop.Resume();
        //currentInterObj.SetActive(true);
        lastEquippedInventory.SetActive(false);

        currentState = PlayerState.idle;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("armor") || collision.CompareTag("weapons"))
        {
            currentInterObj = collision.gameObject;
            Debug.Log(currentInterObj);
            currentState = PlayerState.interact;
            //interacting = true;
            //EquipDialogue(currentInterObj);
         
            
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("armor") || collision.CompareTag("weapons"))
        {
            currentInterObj = null;
        }
    }
    private IEnumerator AttackCo()
    {

        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;
        yield return null;
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.33f);
        currentState = PlayerState.idle;
    }
    public void Move()
    {
        change.Normalize();
        rbody.MovePosition(
            transform.position + change * movementSpeed * Time.deltaTime
        );
    }
    void UpdateAnimationAndMove()
    {

        if (change != Vector3.zero)
        {
            Move();
            change.x = Mathf.Round(change.x);
            change.y = Mathf.Round(change.y);
            animator.SetFloat("moveX", (change.x));
            animator.SetFloat("moveY", (change.y));
            if (currentState != PlayerState.spinAttack)
            {
                animator.SetBool("moving", true);

            }

        }
        else
        {
            animator.SetBool("moving", false);
            currentState = PlayerState.idle;

        }



    }

    private IEnumerator Dodge()
    {

        Debug.Log("Dodging");
        if (this.currentStamina >= 5)
        {
            // Vector3 dashVector = rbody.transform.position = (Vector3) facingdir.normalized * 3
            float xPos = animator.GetFloat("moveX");
            float yPos = animator.GetFloat("moveY");
            Vector2 direction = new Vector2(-xPos, -yPos);
            Vector3 dodgeVector = rbody.transform.position + (Vector3)direction.normalized * 3f;
            rbody.DOMove(dodgeVector, .3f);
            //rbody.AddForce(direction, ForceMode2D.Impulse);
            currentState = PlayerState.dodge;

            yield return new WaitForSeconds(0.2f);

            //rbody.velocity = Vector2.zero;
            currentState = PlayerState.idle;

            currentStamina -= 5;
            staminaBar.value = currentStamina / maxStamina;

        }
    }


    private IEnumerator DashAttack()
    {
        Debug.Log("Dashing");
        if (this.currentStamina >= 20)
        {
            animator.SetBool("lanceAttack", true);
            float xPos = animator.GetFloat("moveX");
            float yPos = animator.GetFloat("moveY");
            Vector2 direction = new Vector2(xPos, yPos);
            Vector3 dodgeVector = rbody.transform.position + (Vector3)direction.normalized * 8f;
            rbody.DOMove(dodgeVector, .3f);
            currentState = PlayerState.dashAttack;

            yield return new WaitForSeconds(0.2f);

            rbody.velocity = Vector2.zero;
            currentState = PlayerState.idle;

            currentStamina -= 20;
            staminaBar.value = currentStamina / maxStamina;
            animator.SetBool("lanceAttack", false);

        }
    }

    private IEnumerator SpinAttack()
    {
        if (this.currentStamina >= 30)
        {
            currentState = PlayerState.spinAttack;
            animator.SetBool("spinAttack", true);
            yield return null;

            //yield return new WaitForSeconds(10f);
            animator.SetBool("spinAttack", false);
            currentStamina -= 30;
            staminaBar.value = currentStamina / maxStamina;
            currentState = PlayerState.idle;




        }
    }

    public void Knock(float knockTime, float damage)
    {

        currentHealth -= damage;
        healthBar.value = currentHealth / maxHealth;
        if (currentHealth < 1)
        {
            Time.timeScale = 0;
            currentState = PlayerState.dead;
            Debug.Log(currentState);
            animator.SetTrigger("death");            
        }
        else
        {
            currentState = PlayerState.stagger;

            StartCoroutine(KnockCo(knockTime));

        }
    }
    private IEnumerator KnockCo(float knockTime)
    {
        playerHit.Raise();
        if (rbody != null)
        {
            Debug.Log("Knocking player back");

            yield return new WaitForSeconds(knockTime);
            rbody.velocity = Vector2.zero;
            currentState = PlayerState.stagger;
            rbody.velocity = Vector2.zero;
        }
    }

}
