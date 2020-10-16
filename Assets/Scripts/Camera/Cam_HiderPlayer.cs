using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Cinemachine;

namespace IPS.Inputs
{
    public class Cam_HiderPlayer : NetworkBehaviour
    {
        public Transform camTarget;
        public Vector3 offset;

        [SerializeField] private Vector2 camVelocity = new Vector2(4f, .25f);
        public GameObject camPrefab;
        //[SerializeField] private CinemachineVirtualCamera virtualCamera = null;

        private Controls controls;
         private Controls Controls
        {
            //needed to make sure controls(lowercase) is not = null when OnEnabled() is called below
            get
            {
                if (controls != null) { return controls; }
                return controls = new Controls();
            }
        }
        private CinemachineTransposer transposer;

        public override void OnStartAuthority()
        {

           //transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            Debug.Log("Got here OnStartAuthority()!-x-x-x--x-x--x-x-x--x" + gameObject.name);
            //make sure cameras/this behavior are not enabled unless it's "ours"(this local player's)
            GameObject myCam = Instantiate(camPrefab);
            myCam.GetComponent<CinemachineFreeLook>().Follow = this.transform;
            myCam.GetComponent<CinemachineFreeLook>().LookAt = this.transform;

           // Debug.Log("Is the camera active? " + virtualCamera.gameObject.active + gameObject.name + SceneManager.GetActiveScene().name);

            this.enabled = true;

            //Controls.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
        }

        //ClientCallBack= this only gets called on the client, not the server
        [ClientCallback]
        private void OnEnable() => Controls.Enable();

        [ClientCallback]
        private void OnDisable() => Controls.Disable();

       private void Start() {
           // DontDestroyOnLoad(gameObject);

        }
        private void Look(Vector2 lookAxis)
        {
    
            // transposer.m_FollowOffset.y = Mathf.Clamp(
            //     transposer.m_FollowOffset.y - (lookAxis.y * camVelocity.y * Time.deltaTime),
            //     offset.x,
            //     offset.y);

            //camTarget.Rotate(0f, lookAxis.x * camVelocity.x * Time.deltaTime, 0f);
        }

   
    }
}