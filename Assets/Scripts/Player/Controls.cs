using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class Controls : MonoBehaviour
{
    bool facingRight = true;
    bool gotKey = false;

    private Timer timer;
    bool invulnerable = false;


    Animator bossDoorAnimator, mainDoorAnimator;
    Rigidbody rigidBody;
    CameraControls cameraControls;


    public AudioClip[] Clips;

    public bool alive = false;
    //Player control members
    const float animationSmoothTime = .1f;
    public float moveSpeed = 1f;
    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private Animator animator;

    private Camera mainCamera;


    private void Awake()
    {
        gameObject.transform.parent = null;
        animator = GetComponentInChildren<Animator>();
        timer = Camera.main.GetComponent<Timer>();
        rigidBody = GetComponent<Rigidbody>();
        mainCamera = FindObjectOfType<Camera>();
        mainCamera.GetComponent<CameraControls>().SetCamera();
        cameraControls = (CameraControls)Camera.main.GetComponent(typeof(CameraControls));  
    }

    public void Ready()
    {
        alive = true;
        StartCoroutine(StartGame());
    }

    //Tie this to start game
    private IEnumerator StartGame()
    {
        mainDoorAnimator = GameObject.FindGameObjectWithTag("MainDoor").GetComponentInChildren<Animator>();
        yield return new WaitForSeconds(1.0f); //Remove me
        mainDoorAnimator.SetBool("isOpen", true);
        yield return new WaitForSeconds(1.5f);
        timer.begin();
    }


    void FixedUpdate()
    {
        if (alive)
        {
             rigidBody.velocity = moveVelocity;


            //animator.SetBool("Ground", grounded);
            //TODO if (alive)
            //   animator.SetFloat("Speed", Mathf.Abs(rigidBody.velocity.x));


            /* if (moveHorizontal > 0 && !facingRight)
                 Flip();
             else if (moveHorizontal < 0 && facingRight)
                 Flip();*/
        }
    }

    void Update()
    {
        if (alive)
        {
            Vector3 forward = mainCamera.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            moveInput = (horizontalInput * right + verticalInput * forward);

            if (moveInput.magnitude > 1.0f ) 
            {
                moveInput = moveInput.normalized;
            }

            moveVelocity = moveInput * moveSpeed;

            float speedPercent = rigidBody.velocity.magnitude / moveSpeed;
            animator.SetFloat("SpeedPercent", speedPercent, animationSmoothTime, Time.deltaTime);


            Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            float rayLength;

            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                Vector3 pointToLook = cameraRay.GetPoint(rayLength);
                Debug.DrawLine(cameraRay.origin, pointToLook, Color.red);
                transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        //speechBubble.transform.localScale = theScale;
        transform.localScale = theScale;
    }

    public void Enabler()
    {
        alive = !alive;
       //TODO? rigidBody.simulated = !rigidBody.simulated;
    }


    public IEnumerator HitByEnemy()
    {
        Color normalColor;
        invulnerable = true;

        timer.RemoveTime(3.0f, gameObject);
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        //Play hit sound
        gameObject.GetComponent<AudioSource>().clip = Clips[1];
        gameObject.GetComponent<AudioSource>().Play();
        
        foreach (Renderer renderer in renderers) {
            if (renderer.gameObject.name == "Knight")
            {
                normalColor = renderer.material.color;
                for (int i = 0; i < 4; i++)
                {
                    renderer.material.color = Color.red;
                    yield return new WaitForSeconds(.15f);
                    renderer.material.color = normalColor;
                    yield return new WaitForSeconds(.15f);
                }
                renderer.material.color = normalColor;
            }
        }

        yield return new WaitForSeconds(.8f);
        invulnerable = false;
    }

    public Timer getTimer()
    {
        return timer;
    }

    public void EndGame(bool winner)
    {
        if (winner)
            timer.EndGame(true);
        else
            timer.EndGame(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Room"))
        {
            foreach (Transform child in collider.transform)
            {
                if (child.gameObject.tag.Equals("Enemy"))
                {
                    child.GetComponent<IEnemy>().Awaken();
                }
                else if (child.gameObject.tag.Equals("Fog"))
                {
                    Component[] renderers = child.GetComponentsInChildren(typeof(Renderer));
                    foreach (Renderer curRenderer in renderers)
                    {
                        Color color;
                        foreach (Material material in curRenderer.materials)
                        {
                            color = material.color;
                            color.a = 0;
                            material.color = color;
                        }
                    }
                }
            }
        }
        else if (collider.gameObject.CompareTag("BossRoom"))
        {

            GameObject[] bossDoors = GameObject.FindGameObjectsWithTag("BossDoor");
            foreach (GameObject door in bossDoors)
            {
                door.GetComponentInChildren<Animator>().SetBool("isOpen", false);
                gotKey = false;
            }
           
            foreach (Transform child in collider.transform)
            {
                if (child.gameObject.CompareTag("Enemy"))
                {
                    child.gameObject.SetActive(true);
                    child.GetComponent<IEnemy>().Awaken();
                }
            }
        }
        else if (collider.gameObject.CompareTag("WeaponContainer"))
        {
            if (!collider.GetComponentInChildren<IWeapon>().isEquiped)
            {
                //Play hit sound
                gameObject.GetComponent<AudioSource>().clip = Clips[0];
                gameObject.GetComponent<AudioSource>().Play();
                collider.GetComponentInChildren<IWeapon>().EquipWeapon(gameObject.GetComponent<PlayerWeaponController>());
                Destroy(collider.gameObject);
            }
        }
        else if (collider.gameObject.CompareTag("LootBox"))
        {
            collider.GetComponentInChildren<LootBox>().Open();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Room"))
        {
            foreach (Transform child in collider.transform)
            {
                if (child.gameObject.tag.Equals("Enemy"))
                {
                    //child.GetComponent<IEnemy>().Sleepen();
                }
                else if (child.gameObject.tag.Equals("Fog"))
                {
                    Component[] renderers = child.GetComponentsInChildren(typeof(Renderer));
                    foreach (Renderer curRenderer in renderers)
                    {
                        Color color;
                        foreach (Material material in curRenderer.materials)
                        {
                            color = material.color;
                            color.a = 255;
                            material.color = color;
                        }
                    }
                }
            }
        }
    }

    //Handle death, keys and such here
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("BossDoor") && gotKey)
        {
            bossDoorAnimator = other.gameObject.GetComponent<Animator>();
            bossDoorAnimator.SetBool("isOpen", true);
        } 
        else if (other.gameObject.CompareTag("Key"))
        {
            //Play hit sound
            gameObject.GetComponent<AudioSource>().clip = Clips[0];
            gameObject.GetComponent<AudioSource>().Play();
            Destroy(other.gameObject);
            gotKey = true;
        } else if( other.gameObject.CompareTag("Enemy"))
        {
            if (!invulnerable)
                StartCoroutine(HitByEnemy());
            if (other.gameObject.name == "arrow(Clone)")
                Destroy(other.gameObject);
        }   
    }
}
