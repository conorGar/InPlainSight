using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


namespace IPS.Inputs
{
    public class SniperController : NetworkBehaviour
    {
        public Vector2 Velocity = new Vector2(0, 0);
        //public Transform rotationPoint;

        public Camera sniperCamera;

        [Range(0, 5)]
        public float moveSpeed = 1f;
        [Range(5, 55)]
        public float Radius = 10f;
        private Vector3 _centre;
        private float _angle;
        private float moveDirection;
        private bool isScoped;

        private CharacterController controller;

        Cam_SniperZoom sniperZoom;
        Vector2 previousInput;





        public override void OnStartAuthority()
        {
            enabled = true;

            sniperCamera.gameObject.SetActive(true);


            InputManager.Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
            InputManager.Controls.Player.Move.canceled += ctx => ResetMovement();
        }




        [ClientCallback]
        private void OnEnable()
        {
            //InputManager.Controls.Enable();
            controller = GetComponent<CharacterController>();
        }

       // [ClientCallback]
       // private void OnDisable() => Controls.Disable();

        [Client]
        private void SetMovement(Vector2 movement)
        {
            previousInput = movement;
            Debug.Log("Set Movement activated");
        }//=> previousInput = movement;

        [Client]
        private void ResetMovement() => previousInput = Vector2.zero;

        private void Start()
        {
            _centre = MatchManagerIPS.Instance.sniperRotationPoint.position;
            sniperZoom = GetComponent<Cam_SniperZoom>();
                                    Debug.Log("isLocalPlayer? "+isLocalPlayer);
             Debug.Log("isServer? " + isServer);
             Debug.Log("isClient? " + isClient);
             Debug.Log("hasAuthority? " + hasAuthority);
        }

        [ClientCallback]
        private void Update()
        {

            if (Input.GetMouseButtonDown(1) && hasAuthority)
            {

               
                isScoped = !isScoped;
                sniperZoom.ToggleZoom(isScoped);
            }
            else if (Input.GetMouseButtonDown(0) && isScoped)
            {
                Shoot();
            }
            else
            {
                Move();
            }


        }


        [Client]
        private void Move()
        {




            Vector3 right = controller.transform.right;

            right.y = 0f;

            float movementDir = right.normalized.x * previousInput.x; //+ forward.normalized * previousInput.y;



            _angle += movementDir * Time.deltaTime;

            var offset = new Vector3(Mathf.Sin(_angle), .6f, Mathf.Cos(_angle)) * Radius;

            transform.position = _centre + offset;
        }


        [Client]
        void Shoot()
        {
            Debug.Log("Shoot() activated");
            RaycastHit hit;

                                GetComponent<Player_ScoreKeeper>().ShootPlayer();

            if (Physics.Raycast(sniperZoom.myCam.transform.position, sniperZoom.myCam.transform.forward, out hit))
            {
                if (hit.transform.GetComponent<NPC>())
                {
                    hit.transform.GetComponent<NPC>().Shot();
                }
                else if (hit.transform.GetComponent<HiderPlayer_Movement>())
                {

                    Debug.Log("SUCESSFULLY HIT A HIDER PLAYER YAAAAAAAYYYYYYYYYYYYYYYY");

                    GetComponent<Player_ScoreKeeper>().ShootPlayer();

                }
            }
        }

    }

}
