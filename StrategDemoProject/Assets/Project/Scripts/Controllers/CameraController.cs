using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float lerpDuration = 0.003f;
        [SerializeField] private float maxY = 350f;
        [SerializeField] private float minY = -30f;
        [SerializeField] private float maxX = 350f;
        [SerializeField] private float minX = -30f;

        private Camera mainCamera;
        private Vector3 lerpToPosition;
        private Vector3 lerpFromPosition;
        private float lerpTimer;
        private bool lerpPosition;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleCameraMovement();
        }

        private void HandleCameraMovement()
        {
            GetInput();

            if (!lerpPosition)
                return;

            // Interpolate the camera position based on the lerp timer progress.
            mainCamera.transform.position = Vector3.Lerp(lerpFromPosition, lerpToPosition, lerpTimer / lerpDuration);

            lerpTimer += Time.deltaTime;

            //If we have reached the end of the lerp timer, explicitly force the camera to the last known correct position.
            if (lerpTimer >= lerpDuration)
            {
                transform.position = lerpToPosition;
                lerpPosition = false;
            }
        }

        private void GetInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 inputPosition = Vector3.up * vertical + Vector3.right * horizontal;

            if (inputPosition != Vector3.zero)
            {
                lerpFromPosition = mainCamera.transform.position;
                lerpToPosition = mainCamera.transform.position + inputPosition;
                lerpToPosition.Set(Mathf.Clamp(lerpToPosition.x, minX, maxX), Mathf.Clamp(lerpToPosition.y, minY, maxY), lerpToPosition.z);
                lerpTimer = 0;
                lerpPosition = true;
            }
        }
    }
}
