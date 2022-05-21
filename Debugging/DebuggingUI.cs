using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebuggingUI : MonoBehaviour
{

  // the gameobject this script is attatched too, set in Start() function
  GameObject o_This;

  [Header("Debugging Objects")]
  [Tooltip("The Player")]
  // the playable character, set by developer via inspector
  public GameObject o_Player;
  Rigidbody2D r_Player;

  [Header("Frame Debugging")]
  [Tooltip("FPS Text Object")]
  // fps text to be  updated, set by developer via inspector
  public Text t_FPS;

  [Tooltip("Frametime Text Object")]
  // frametime text to be updated, set by the developer via inspector
  public Text t_FrameTime;

  [Header("Player Debugging")]
  [Tooltip("Player Text Object")]
  // player text to be updated, set by the developer via inspector
  public Text t_Player;

  [Header("Options")]
  [Tooltip("Is FPS Enabled?")]
  public bool b_FPS = true;
  [Tooltip("Is Other Debugging Enabled?")]
  public bool b_Debugging = false;
  [Tooltip("'True' Color")]
  public Color c_True = Color.green;
  [Tooltip("'False' Color")]
  public Color c_False = Color.red;
  [Tooltip("'Other' Color")]
  public Color c_Other = new Color(255,127,0);

  void Start() {
    o_This = this.gameObject;
    r_Player = o_Player.GetComponent<Rigidbody2D>();
  }

  void Update() {
    DebugFPS();
    DebugFrametime();
    DebugPlayer();
  }

  void DebugFPS() {
    if(t_FPS != null && b_FPS) { t_FPS.text = "FPS: " + Mathf.Round(1 / Time.deltaTime); } else { t_FPS.text = ""; }
  }

  void DebugFrametime() {
    if(!b_Debugging) {t_FrameTime.text = ""; return; }
    if(t_FrameTime != null) { t_FrameTime.text = "Frametime: " + (Time.deltaTime * 1000) + "ms"; }
  }

  void DebugPlayer() {
    if(t_Player == null) return;
    if(!b_Debugging) {t_Player.text = ""; return; }
    if(o_Player == null) return;

    t_Player.text = "POSITION " + o_Player.transform.position + "\nSPEED: " + r_Player.velocity;

    Debug.DrawRay(o_Player.transform.position, Vector2.up, c_Other, 3f);

    // Debug Raycast For Ground Check
    if(o_Player.GetComponent<Controller2D>().b_isGrounded) { Debug.DrawRay(new Vector2(o_Player.transform.position.x, o_Player.transform.position.y - o_Player.transform.localScale.y), Vector2.down * o_Player.GetComponent<Controller2D>().f_CheckRadius, c_True); } else {
      Debug.DrawRay(new Vector2(o_Player.transform.position.x, o_Player.transform.position.y - (o_Player.transform.localScale.y)), Vector2.down * o_Player.GetComponent<Controller2D>().f_CheckRadius, c_False);
    }

    // Debug Raycast For Roof Check
    if(o_Player.GetComponent<PlayerMovement>().b_Crouch) { Debug.DrawRay(new Vector2(o_Player.transform.position.x, o_Player.transform.position.y + o_Player.transform.localScale.y), Vector2.up * o_Player.GetComponent<Controller2D>().f_CheckRadius, c_True); } else {
      Debug.DrawRay(new Vector2(o_Player.transform.position.x, o_Player.transform.position.y + o_Player.transform.localScale.y), Vector2.up * o_Player.GetComponent<Controller2D>().f_CheckRadius, c_False);
    }

  }

}
