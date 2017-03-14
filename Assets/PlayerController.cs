using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public static PlayerController instance;
    //Movement
    public float speed = 10, jumpVelocity = 10;
    public LayerMask playerMask;
    public bool canMoveInAir = true;

    //Combat
    public int health = 3;
    public float invincibleTimeAfterHurt = 2;

    [HideInInspector]
    public Collider2D[] myColls;

    Transform myTrans, tagGround;
    Rigidbody2D myBody;
    bool isGrounded = false;
    float hInput = 0;
    AnimatorController myAnim;

	void Start ()
    {
        instance = this;
        myColls = this.GetComponents<Collider2D>();
        tagGround = GameObject.Find(this.name + "/tag_ground").transform;
        myBody = GetComponent<Rigidbody2D>();
        myTrans = this.transform;
        myAnim = AnimatorController.instance;
	}
	
	void FixedUpdate ()
    {
        isGrounded = Physics2D.Linecast(myTrans.position, tagGround.position, playerMask);
        myAnim.UpdateIsGrounded(isGrounded);

        #if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_BLACKBERRY && !UNITY_WINRT || UNITY_EDITOR
        hInput = Input.GetAxisRaw("Horizontal");
        myAnim.UpdateSpeed(hInput);
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        
        #endif
        Move(hInput);
    }
    void Move(float horizontalInput)
    {
        if (!canMoveInAir && !isGrounded)
        {
            return;
        }

        Vector2 moveVel = myBody.velocity;
        moveVel.x = horizontalInput * speed;
        myBody.velocity = moveVel;
    }
    public void Jump()
    {
        if (isGrounded)
        {
            myBody.velocity += jumpVelocity * Vector2.up;
        }
    }
    public void StartMoving(float horizonalInput)
    {
        hInput = horizonalInput;
        myAnim.UpdateSpeed(horizonalInput);
    }
    void Hurt()
    {
        health--;
        if(health <= 0)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        else
        {
            myAnim.TriggerHurt(invincibleTimeAfterHurt);
        }
    }

    void OnGUI()
    {
        GUIStyle myGUI = new GUIStyle(GUI.skin.label);
        myGUI.fontSize = 25;
        myGUI.normal.textColor = Color.red;
        GUI.Box(new Rect(51, 12, 80, 30), "x" + health, myGUI);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if(enemy != null)
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                Debug.DrawLine(point.point, point.point + point.normal, Color.red, 10);
                if (point.normal.y >= 0.9f)
                {
                    Vector2 velocity = myBody.velocity;
                    velocity.y = jumpVelocity;
                    myBody.velocity = velocity;
                    enemy.Hurt();
                }
                else
                {
                    Hurt();
                }
            }
        }
    }
}
