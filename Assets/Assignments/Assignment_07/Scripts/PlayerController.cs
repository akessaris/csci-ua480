using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

namespace A07_ank352
{
    public class PlayerController : NetworkBehaviour
    {
        public GameObject bulletPrefab;
        public Transform bulletSpawn;

        private GameObject cam_Holder; //holds camera since GVR overrides camera's position
        private Camera[] cams;
        public Camera cam;
        private int cam_counter = 0;

        public static A07_ank352.PlayerController Instance;

        private void Awake()
        {
            //Singleton
            if (isLocalPlayer)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            cams = Camera.allCameras;
            cam_Holder = GameObject.Find("Cam_Holder"); //get parent object of camera
        }

        void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            //Switch cameras
            foreach (Camera i in cams) {
                i.enabled = false;
            }
            cam.enabled = true;

            //Track rotation
            transform.rotation = cam.transform.rotation;

            //If trigger, fire and move
            if (Input.GetMouseButton(0))
            {
                CmdFire(); //fire projectiles

                //Calculate where to move
                Vector3 forward = transform.forward;
                forward.y = 0;
                Vector3 newPosition = forward * Time.deltaTime * 5.0f + transform.position;

                //Constrain movement
                newPosition.x = Mathf.Clamp(newPosition.x, -10, 10);
                newPosition.z = Mathf.Clamp(newPosition.z, -10, 10);

                //Update position
                transform.position = newPosition; //move player
            }
            //Update camera (parent) position
            cam_Holder.transform.position = transform.position;
        }


        // This [Command] code is called on the Client …
        // … but it is run on the Server!
        [Command]
        void CmdFire()
        {
            // Create the Bullet from the Bullet Prefab
            var bullet = (GameObject)Instantiate(
                bulletPrefab,
                bulletSpawn.position,
                bulletSpawn.rotation);

            // Add velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

            // Spawn the bullet on the Clients
            NetworkServer.Spawn(bullet);

            // Destroy the bullet after 2 seconds
            Destroy(bullet, 2.0f);
        }

        public override void OnStartLocalPlayer()
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.blue); //set color of local player to blue
            cam = cams[cam_counter++]; //set new camera
            cam_Holder.transform.position = transform.position; //set position of camera's parent object to player
        }
    }
}
