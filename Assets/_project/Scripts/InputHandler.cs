using UnityEngine;
using System.Collections;

namespace _project.Scripts
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private float airSpeed = 1000f;
        
        private Vector3 groundMovement;
        private Vector3 airMovement;
        private Vector3 playerRotation;
        
        private Vector3 inputs;
        
        private bool isJumping;
        
        public float iAcceleration = 1f;
        public float iDeceleration = 2f;
        // MovementSmoothingVars
        public float xSmoothed;
        public float ySmoothed;
        
        /// <summary>
        /// Function that handles all of the player's inputs
        /// </summary>
        public void PlayerInput()
        {
            // Player movement axis
            inputs = Vector3.zero;

            inputs.x = InputSmoothing("Horizontal", ref xSmoothed);
            inputs.z = InputSmoothing("Vertical", ref ySmoothed);
        
            inputs = Vector3.ClampMagnitude(inputs, 1f);

            var forward = playerTransform.forward;
            var right = playerTransform.right;
            airMovement = (forward * (airSpeed * inputs.x * 100)) + (right * (airSpeed * -inputs.z * 100));
        
            var eulerAngles = playerTransform.eulerAngles;
            playerRotation = new Vector3(eulerAngles.x, playerCamera.transform.eulerAngles.y, eulerAngles.z);

            // Jumping
            if (Input.GetButtonDown("Jump"))
            {
                isJumping = true;
                StartCoroutine(JumpBuffer());
            }
        }
        // Makes the specified input move smoothly from 0 to 1 and vice-versa. A bit buggy still, esp. controllers...
        public float InputSmoothing(string axis, ref float smoothed)
        {
            var accelerating = Input.GetAxisRaw(axis);

            if (accelerating > 0)
                smoothed = Mathf.Clamp(smoothed + iAcceleration * Time.deltaTime, -1f, accelerating);
            else if (accelerating < 0)
                smoothed = Mathf.Clamp(smoothed - iAcceleration * Time.deltaTime, accelerating, 1f);
            else
                smoothed = Mathf.Clamp01(Mathf.Abs(smoothed) - iDeceleration * Time.deltaTime) * Mathf.Sign(smoothed);
            return smoothed;
        }
        private IEnumerator JumpBuffer()
        {
            yield return new WaitForSeconds(.1f);
            isJumping = false;
        }
    }
}