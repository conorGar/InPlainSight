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

        [Range(15, 30)]
        public float moveSpeed = 30f;
      
        public float Radius = 10f;
        private Vector3 _centre;
        private float _angle;
        private float moveDirection;
        private bool isScoped;

        [SerializeField] LineRenderer aimBeam;

        private CharacterController controller;

        Cam_SniperZoom sniperZoom;
        Vector2 previousInput;





        public override void OnStartAuthority()
        {
            enabled = true;
            Radius = MatchManagerIPS.Instance.sniperViewDistance;

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
             var offset = new Vector3(Mathf.Sin(_angle), .6f, Mathf.Cos(_angle)) * Radius;
             transform.position = _centre + offset;
        }

        [ClientCallback]
        private void Update()
        {

                        Debug.DrawRay(sniperZoom.myCam.transform.position, sniperZoom.myCam.transform.forward*50, Color.green, 0, false);    


            if (Input.GetMouseButtonDown(1) && hasAuthority)
            {

               Debug.Log("GOT HERE- AIM DOWN");
                isScoped = !isScoped;
                sniperZoom.ToggleZoom(isScoped);
            }
            else if (Input.GetMouseButtonDown(0) && isScoped)
            {
                Shoot();
            }
            else if(!isScoped)
            {
                Move();
            }

            if(isScoped){
                //draw red aim line when scoped
            
                aimBeam.enabled = true;
                aimBeam.SetVertexCount (2);
                //aimBeam.SetPosition(0, sniperZoom.myCam.transform.position); 
                //  if (Physics.Raycast(sniperZoom.myCam.transform.position, sniperZoom.myCam.transform.forward, out hit))
                // {
                    
                // }
            
            }else{
                aimBeam.enabled = false;
            }


        }


        [Client]
        private void Move()
        {




            // Vector3 right = controller.transform.right;

            // right.y = 0f;

            //  float movementDir = right.normalized.x * previousInput.x; //+ forward.normalized * previousInput.y;

            float horizontal = Input.GetAxisRaw("Horizontal");      

            // _angle += movementDir * Time.deltaTime;

            // var offset = new Vector3(Mathf.Sin(_angle), .6f, Mathf.Cos(_angle)) * Radius;

            // transform.position = _centre + offset;

            transform.RotateAround(MatchManagerIPS.Instance.sniperRotationPoint.transform.position,new Vector3(0f,horizontal,0f),moveSpeed*Time.deltaTime);
        }


        [Client]
        void Shoot()
        {
            Debug.Log("Shoot() activated");
            RaycastHit hit;

                               // GetComponent<Player_ScoreKeeper>().ShootPlayer();
            if (Physics.Raycast(sniperZoom.myCam.transform.position, sniperZoom.myCam.transform.forward, out hit))
            {
                Debug.Log("Got here raycast" + hit.transform.position + hit.transform.gameObject.name);
                if (hit.transform.gameObject.GetComponent<NPC>())
                {
                    Debug.Log("hit and NPC- got here");
                    hit.transform.GetComponent<NPC>().Shot(this.gameObject.transform);
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
