using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerControllerBase : NetworkBehaviour
{
    public float moveSpeed;
    public CharacterController controller;
    public float gravityScale;

    Vector3 moveDirection;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isLocalPlayer)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, 0f, Input.GetAxis("Vertical") * moveSpeed);

            //moveDirection = (transform.forward * Input.GetAxis("Vertical") + transform.right*Input.GetAxis("Horizontal"));
            //moveSpeed = moveDirection.Normalize();


            transform.rotation = Quaternion.LookRotation(moveDirection);


            //gravity
            moveDirection.y = moveDirection.y + (Physics.gravity.y * Time.deltaTime * gravityScale);

            controller.Move(moveDirection * Time.deltaTime);
        }
    }
}
