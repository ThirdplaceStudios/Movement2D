using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Controller2D : MonoBehaviour
{

  private GameObject o_Player;
  private Rigidbody2D r_Player;

  [Header("Checks")]
  [SerializeField][Tooltip("The Empty Game Object, At The Feet Of The Player")]
  private Transform c_Ground; // Ground Check
  [SerializeField][Tooltip("The Empty Game Object, At The Head Of The Player")]
  private Transform c_Roof; // Roof Check

  [Header("Movement Settings")]
  [SerializeField][Range(0,1)][Tooltip("The Crouch Multiplier Of The Player")]
  private float f_CrouchSpeed = 0.36f; // Crouch Speed Multiplier
  [SerializeField][Range(0,0.3f)][Tooltip("The Amount The Change In Speed Is Smoothed")]
  private float f_MovementSmoothing = 0.3f;
  [SerializeField][Tooltip("The Jump Force Of The Player")]
  private float f_JumpForce = 400f; // Jump Force Multiplier
  [SerializeField][Tooltip("What Is The Ground? (Layer)")]
  private LayerMask l_Ground; // What Is The Ground
  [SerializeField][Tooltip("Should The Player Move In The Air")]
  private bool b_AirControl = true; // Toggles Player Air Control
  [SerializeField][Tooltip("The Gravity Multiplier While Falling")]
  private float f_GravityMultiplierFalling = 10f; // Multiplier Of Force Added While Falling
  [SerializeField][Tooltip("The Max Speed The Player Can Reach While Falling (Terminal Velocity)")]
  private float f_MaxGravity = 100f; // Terminal Velocity Of Player

  [Header("Optional")]
  [SerializeField][Tooltip("The Collider To Disable When Crouching")]
  private Collider2D c_CrouchDisable;

  [Header("Events")][Space]
  public UnityEvent OnLandEvent;
  [System.Serializable] public class BoolEvent : UnityEvent<bool> {}
  public BoolEvent OnCrouchEvent;

  // Private Variables
  private float f_GravityScale;
  private bool b_WasCrouching = false; // Was The Player Crouching In The Previous Frame
  private bool b_WasGrounded = true; // Was The Player Grounded In The Previous Frame
  [HideInInspector]
  public bool b_isGrounded = true; // Is The Player Crouching
  [HideInInspector]
  public float f_CheckRadius = 0.2f;
  private Vector2 v_MinVelocity = Vector2.zero;
  private bool b_FacingRight = true;

  private void OnValidate() { // forces values to be positive
    if(f_JumpForce < 0f) f_JumpForce = 0f;
    if(f_GravityMultiplierFalling < 1f) f_GravityMultiplierFalling = 1f;
  }

  private void Awake() {
    o_Player = this.gameObject;
    r_Player = o_Player.GetComponent<Rigidbody2D>();

    // creates events if not already created
    if(OnLandEvent == null) OnLandEvent = new UnityEvent();
    if(OnCrouchEvent == null) OnCrouchEvent = new BoolEvent();
  }

  private void FixedUpdate() {
    b_WasGrounded = b_isGrounded;
    b_isGrounded = false;

    // Checks If The Player Is Grounded
    // The Player Is Grounded If A Circle Around The Feet Of The Player
    // Hits Any Layer Marked As Ground
    Collider2D[] ac_Colliders = Physics2D.OverlapCircleAll(c_Ground.position, f_CheckRadius, l_Ground);
    for(int i = 0; i < ac_Colliders.Length; i++) { b_isGrounded = true;
      if(!b_WasGrounded) OnLandEvent.Invoke(); // If The Player Just Landed, Calls Event Announcing It
    }
    GravityScaler();
  }

  public void Move(float f_Move, bool b_Crouch, bool b_Jump) {
    /*
    Checks If The Player Is Not Crouching
    And If The Player Can Crouch
    Roof Is Checked Like Ground Is
    If A Circle Around The Head Of The Player
    Hits Any Layer Marked As Ground
    */
    if(!b_Crouch && Physics2D.OverlapCircle(c_Roof.position, f_CheckRadius, l_Ground)) b_Crouch = true;
    /*
    Proceeds With Movement
    If The Player Is On The Grounded
    Or Can Move In The Air
    */
    if(b_isGrounded || b_AirControl) {
      if(b_Crouch) { // If The Player Is Crouching, Continue With Crouching
        // If The Player Starts Crouching, Calls Event Announcing That The Player Is Crouching
        if(!b_WasCrouching) { b_WasCrouching = true; OnCrouchEvent.Invoke(true); }
        f_Move *= f_CrouchSpeed; // Changes To Crouch Speed
        if(c_CrouchDisable != null) c_CrouchDisable.enabled = false; // Disables Top Collider
      } else { // If The Player Is Not Crouching, Continue With Crouching
        if(c_CrouchDisable != null) c_CrouchDisable.enabled = true; // Enables Top Collider
        // If The Player Stops Crouching, Calls Event Announcing That The Player Is No Longer Crouching
        if(b_WasCrouching) { b_WasCrouching = false; OnCrouchEvent.Invoke(false); }
      }
      Vector2 v_targetVelocity = new Vector2(f_Move * 10f, r_Player.velocity.y); // Sets Player Speed
      // Smoothing Is Applied To Speed
      r_Player.velocity = Vector2.SmoothDamp(r_Player.velocity, v_targetVelocity, ref v_MinVelocity, f_MovementSmoothing);
      // Flips The Player Based On Movement And Direction
      if(f_Move > 0 && !b_FacingRight) { Flip(); } else if(f_Move < 0 && b_FacingRight) { Flip(); }
      // Makes The Player Jump
      if(b_isGrounded && b_Jump) { b_isGrounded = false; r_Player.AddForce(new Vector2(0f,f_JumpForce)); }
    }
  }

  private void Flip() { // Flips The Player
    b_FacingRight = !b_FacingRight;
    Vector3 v3_Scale = transform.localScale;
    v3_Scale.x *= -1;
    transform.localScale = v3_Scale;
  }

  private void GravityScaler() {
    // If The Player Is Falling, Add A Force Down To Push The Player Down Quickly
    if(b_isGrounded) return;

    if(r_Player.velocity.y < -f_MaxGravity) return; // If The Player Hasn't Reached Terminal Velocity
    // Essentially Adds More Gravity Lol
    if(r_Player.velocity.y < 0) r_Player.AddForce(new Vector2(0f,f_GravityMultiplierFalling * -r_Player.gravityScale));
  }

}
