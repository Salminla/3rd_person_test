using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _project.Scripts.Player
{
    public class InputHandler : MonoBehaviour, MyControls.IPlayerActions
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private float airSpeed = 20f;
        [SerializeField] private float groundSpeed = 12f;

        private MyControls controls;

        public Vector3 groundMovement { get; private set; }
        public Vector3 airMovement { get; private set; }
        public Vector3 playerRotation { get; private set; }
        
        private Vector3 inputs;
        
        [HideInInspector]
        public bool isJumping;

        public float iAcceleration = 2f;
        public float iDeceleration = 4f;

        private float xSmoothed;
        private float ySmoothed;
        
        public void PlayerInput()
        {
            // Player movement axis
            //inputs = Vector3.zero;

            inputs.x = InputSmoothing("Horizontal", ref xSmoothed);
            inputs.z = InputSmoothing("Vertical", ref ySmoothed);
        
            inputs = Vector3.ClampMagnitude(inputs, 1f);
        
            var playerTransformForward = playerTransform.forward * (groundSpeed * inputs.x);
            var playerTransformRight = playerTransform.right * (groundSpeed * -inputs.z);
            groundMovement = playerTransformForward + playerTransformRight;
        
            var forward = playerTransform.forward * (airSpeed * inputs.x * 100);
            var right = playerTransform.right * (airSpeed * -inputs.z * 100);
            airMovement = forward + right;
        
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

        public void ZeroMovement()
        {
            airMovement = Vector3.zero;
            groundMovement = Vector3.zero;
        }
        
        private IEnumerator JumpBuffer()
        {
            yield return new WaitForSeconds(.1f);
            isJumping = false;
        }

        #region New Input System Stuff

        public void OnMove(InputAction.CallbackContext context)
        {
            
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        #endregion
        
    }
}