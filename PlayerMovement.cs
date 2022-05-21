using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("The Character Controller Script")]
    public Controller2D c_Controller;
    [Tooltip("The Speed Of The Player")]
    public float f_Speed = 40f;

    float f_horizontalMove;
    bool b_Jump = false;
    [HideInInspector]
    public bool b_Crouch = false;

    void OnValidate() {
      if(f_horizontalMove < 0f) f_horizontalMove = 0f;
    }

    void Update() {
      f_horizontalMove = Input.GetAxisRaw("Horizontal") * f_Speed; // sets movement speed based on input
      if(Input.GetButtonDown("Jump")) b_Jump = true; // sets player to jumping
      if(Input.GetButtonDown("Crouch")) { b_Crouch = true; } else if(Input.GetButtonUp("Crouch")) { b_Crouch = false; } // sets player to crouching/uncrouching respectively
    }

    void FixedUpdate() {
      c_Controller.Move(f_horizontalMove * Time.fixedDeltaTime, b_Crouch, b_Jump); // sends input to character controller
      b_Jump = false; // sets player to not jumping
    }

}
