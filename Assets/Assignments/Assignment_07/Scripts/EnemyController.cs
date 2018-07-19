using UnityEngine;
using UnityEngine.Networking;

namespace A07_ank352
{
    public class EnemyController : NetworkBehaviour
    {
        public GameObject bulletPrefab;
        public Transform bulletSpawn;
        public float fireRate = 50.0f;
        public float temp = 0.0f;

        public float bounds = 10.0f;
        public float rotation;
        public float z;
        public float x;

		private void Start()
		{
            if (!isServer)
            {
                return;
            }
            //Set random rotation within 1 and 10
            rotation = Random.Range(1, 10);

            //Set initial random movement
            x = Random.Range(-5.0f, 5.0f) * Time.deltaTime;
            z = Random.Range(-5.0f, 5.0f) * Time.deltaTime;
		}

		void Update()
        {
            //Constrain movement
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x + x, -10, 10);
            pos.z = Mathf.Clamp(pos.z + z, -10, 10);

            //Update position
            transform.position = pos;

            //Rotate random amount
            transform.Rotate(0, rotation, 0);

            //Fire if temp equals fire rate and change movement
            if (temp >= fireRate) {
                CmdFire();
                temp = 0;
                x = Random.Range(-5.0f, 5.0f) * Time.deltaTime;
                z = Random.Range(-5.0f, 5.0f) * Time.deltaTime;
            }
            else {
                temp++;
            }
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
        }
    }
}
