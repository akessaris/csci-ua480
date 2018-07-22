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

        public static A07_ank352.PlayerController Instance;

        private void Awake()
        {
            //Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this);
            }
        }

        void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            transform.rotation = Camera.main.transform.rotation;

            if (Input.GetMouseButton(0))
            {
                CmdFire();
                Vector3 forward = transform.forward;
                forward.y = 0;

                Vector3 newPosition = forward * Time.deltaTime * 5.0f + transform.position;

                //Constrain movement
                newPosition.x = Mathf.Clamp(newPosition.x, -10, 10);
                newPosition.z = Mathf.Clamp(newPosition.z, -10, 10);

                transform.position = newPosition;
                Camera.main.transform.position = transform.position;
            }
            Camera.main.transform.position = transform.position;
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
            GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            //Camera.main.transform.position = transform.position;
            transform.position = Camera.main.transform.position;

            Debug.Log(transform + " posbhhblh = " + transform.position);
            Debug.Log(Camera.main.transform + " posljhgljg = " + Camera.main.transform.position);
        }
    }
}
