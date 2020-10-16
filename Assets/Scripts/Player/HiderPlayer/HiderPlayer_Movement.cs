﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace IPS.Inputs
{
    public class HiderPlayer_Movement : NetworkBehaviour
    {
        public float moveSpeed;
        public CharacterController controller;

        [SerializeField] Transform cam;
        public float gravityScale;

        [SerializeField] Animator myAnim;
        Vector2 previousInput;


     


        void Awake()//  Changed this while drunk for testing change back-> public override void OnStartAuthority()
        {
            enabled = true;
            cam = GameObject.Find("MainCam").transform;
            Debug.Log( "GOT HERE PLAYER MOVEMENT:" +cam.name);
            InputManager.Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
            InputManager.Controls.Player.Move.canceled += ctx => ResetMovement();
        }


        [ClientCallback]
        private void OnEnable() => InputManager.Controls.Enable();

        [ClientCallback]
        private void OnDisable() => InputManager.Controls.Disable();

        [Client]
        private void SetMovement(Vector2 movement)
        {
            previousInput = movement;
           myAnim.SetBool("isWalking", true);
        }//=> previousInput = movement;

        [Client]
        private void ResetMovement()
        {
            previousInput = Vector2.zero;
            myAnim.SetBool("isWalking", false);


        }
        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<CharacterController>();
            //                  Debug.Log("isLocalPlayer? "+isLocalPlayer);
            //  Debug.Log("isServer? " + isServer);
            //  Debug.Log("isClient? " + isClient);
            //  Debug.Log("hasAuthority? " + hasAuthority);
        }

        [ClientCallback]
        void Update()
        {
            Move();

        }


        [Client]
        private void Move(){
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal,0f,vertical).normalized;

            if(direction.magnitude > 0.1f){
                float targetAngle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                transform.rotation = Quaternion.Euler(-90f,targetAngle,0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle,0f) * Vector3.forward;
                controller.Move(moveDir.normalized*moveSpeed*Time.deltaTime);
            }
        }

        //[Client]
        // private void Move2()
        // {


        //     Vector3 right = controller.transform.right;
        //     Vector3 forward = controller.transform.forward;

        //     right.y = 0f;
        //     forward.y = 0f;

        //     Vector3 movementDir = right.normalized * previousInput.x + forward.normalized * previousInput.y;

        //     controller.Move(movementDir * moveSpeed * Time.deltaTime);

        // }
    }

}
