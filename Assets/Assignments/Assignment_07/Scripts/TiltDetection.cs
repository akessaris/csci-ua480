using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A07_ank352 {
    [RequireComponent(typeof(Camera))]
    public class TiltDetection : MonoBehaviour
    {
        public float TiltingSpeed = 0.05f;
        public float speed = 10;
        public bool easingMovement = true;
        private bool isMoving = false;
        private float currentSpeed = 0.0f;
        private float bounds = 60f;

        public float ThresholdAngle
        {
            get
            {
                return _thresholdAngle;
            }
            set
            {
                _thresholdAngle = value;
                _thresholdMagnitude = Mathf.Sin(ThresholdAngle * Mathf.Deg2Rad);
            }
        }

        private void Start()
        {
            _thresholdMagnitude = Mathf.Sin(ThresholdAngle * Mathf.Deg2Rad);
        }

        [Tooltip("If the tilting angle is below the threshold, player will not move")]
        [SerializeField]
        private float _thresholdAngle = 25f;

        private float _thresholdMagnitude;


        // It will be called after the camera is updated
        void LateUpdate()
        {
            Vector3 translation = transform.up;
            translation.y = 0;

            if (translation.magnitude > _thresholdMagnitude)
            {

                // accelerate to speed
                currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime);   //currentSpeed + (.01f * speed);
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0;

                //Debug.Log("translation = " + translation);
                //Debug.Log("Tilting speed = " + TiltingSpeed);
                //Debug.Log("currentSpeed = " + currentSpeed);
                //Debug.Log("forward = " + forward);
                A07_ank352.PlayerController.Instance.Translate(translation * TiltingSpeed);
            }
         }
    }
}
