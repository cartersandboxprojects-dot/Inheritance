using UnityEngine;
using UnityEngine.InputSystem;

namespace Inheritance.Prototype
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class KeyboardCharacterMotor : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float speed = 4f;
        private CharacterController controller;
        private float verticalSpeed;

        private void Awake() => controller = GetComponent<CharacterController>();

        private void Update()
        {
            var keyboard = Keyboard.current;
            var input = Vector2.zero;
            if (keyboard != null && Application.isFocused)
            {
                input.x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1 : 0)
                    - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1 : 0);
                input.y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1 : 0)
                    - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1 : 0);
            }

            // Clamp diagonal input; movement follows the fixed camera's world X/Z axes.
            input = Vector2.ClampMagnitude(input, 1f);
            if (controller.isGrounded && verticalSpeed < 0f) verticalSpeed = -2f;
            verticalSpeed += Physics.gravity.y * Time.deltaTime;
            controller.Move(new Vector3(input.x * speed, verticalSpeed, input.y * speed) * Time.deltaTime);
        }
    }
}
