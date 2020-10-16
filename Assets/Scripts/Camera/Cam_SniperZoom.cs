using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


namespace IPS.Inputs
{
    public class Cam_SniperZoom : NetworkBehaviour
    {
        // Handles the camera movement when the sniper is zoomed in

        public float sensitivity = 100f;
        public Transform sniperBody;
        public GameObject zoomOverlay;
        public Camera myCam;


        //xclampmin/max = how far the player can move the camera when zoomed in
        [SerializeField] float xClampMin = 20f;
        [SerializeField] float xClampMax = 50f;

        float xRotation = 0f;

        bool isZoomed;
        Vector3 camZoomedLastRotation;


        SniperController myController;
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            myController = GetComponent<SniperController>();
        }

        // Update is called once per frame
        [Client]
        void Update()
        {

            if (isZoomed)
            {
                float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

                
                xRotation -= mouseY;


//                Debug.Log(xRotation + "|" + sniperBody.transform.rotation.y + "|" + sniperBody.transform.rotation.z);
                myCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                sniperBody.Rotate(Vector3.up * mouseX);
            }
            else
            {
                myCam.transform.LookAt(MatchManagerIPS.Instance.sniperRotationPoint);
            }

        }

        [Client]
        public void ToggleZoom(bool zoomStatus)
        {

            // //Get spot that the sniper was looking at
            // if (isZoomed)
            // {
            //     RaycastHit hit;
            //     if (Physics.Raycast(myCam.transform.position, myCam.transform.forward, out hit))
            //     {

            //         camZoomedLastRotation = hit.transform.position;
            //     }
            // }
            // else
            // {
            //     myCam.transform.LookAt(camZoomedLastRotation);
            // }
          
            isZoomed = zoomStatus;


            if (isZoomed)
            {
                //Zoom in
                zoomOverlay.SetActive(true);
                xRotation = 31;
                sniperBody.LookAt(MatchManagerIPS.Instance.sniperRotationPoint.transform.position);
                Input.ResetInputAxes();
                myCam.fieldOfView = 10f;
                myCam.transform.LookAt(MatchManagerIPS.Instance.sniperRotationPoint.transform.position);
            }
            else
            {
                //Zoom out
                myCam.fieldOfView = 60f;
                zoomOverlay.SetActive(false);
            }
        }

    }
}