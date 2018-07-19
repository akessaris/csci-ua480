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

        public float smooth = 2.0F;
        public float tiltAngle = 30.0F;

        public Vector3 initialPosition;
        public Vector3 newPosition;

        public static A07_ank352.PlayerController Instance;

        private void Awake()
        {
            //Singleton
            if (Instance == null)
            {
                Instance = this;
                initialPosition = transform.position;
                newPosition = initialPosition;
                transform.position = newPosition;
            }
            else if (Instance != this)
            {
                Destroy(this);
            }
        }

        public void Translate(Vector3 translation)
        {
            transform.Translate(translation, Space.World);
        }

        void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                CmdFire();
                Vector3 forward = transform.forward;
                forward.y = 0;
                transform.position += newPosition + transform.position * Time.deltaTime * 10.0f;

                //Constrain movement
                newPosition = transform.position;
                newPosition.x = Mathf.Clamp(newPosition.x, -10, 10);
                newPosition.z = Mathf.Clamp(newPosition.z, -10, 10);
                newPosition.y = 0;
            }
            transform.position = newPosition;
        }


        // This [Command] code is called on the Client …
        // … but it is run on the Server!
        [Command]
        void CmdFire()
        {
            Vector3 playerPosition = newPosition;
            playerPosition += transform.forward;

            // Create the Bullet from the Bullet Prefab
            var bullet = (GameObject)Instantiate(
                bulletPrefab,
                playerPosition,
                transform.rotation);

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
        }
    }
}
