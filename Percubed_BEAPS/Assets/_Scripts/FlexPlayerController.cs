using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Percubed.Flex {

    public class FlexPlayerController : MonoBehaviour
    {
        public float speed = 0.001f;
        private Vector3 movement = Vector3.zero;

        void Update()
        {
            // Replace with corresponding setup for new Input package:
            // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/QuickStartGuide.html
            //            float moveHorizontal = Input.GetAxis("Horizontal");
            //            float moveVertical = Input.GetAxis("Vertical");
            //            float moveUpDown = Input.GetAxis("UpandDown");
            //            Vector3 movement = new Vector3(moveHorizontal, moveUpDown, moveVertical);
            if (movement != Vector3.zero)
            {
                this.transform.Translate(movement);
            }
        }

        /// <summary>
        /// Callback for default defined move inputaction, i.e. a 2D move
        /// If we want a 3D move again, we need to add input device triggers
        /// to that move definition for up and down
        /// and turn it from Vector2 to a Vector3 action
        /// 
        /// The input system sends one started, one performed and one cancelled
        /// version of this event. On cancel the returned vector is zero.
        /// So we can just directly save the vector and transform in update.
        /// </summary>
        public void MoveCB(InputAction.CallbackContext context)
        {
            //Debug.Log("MoveCB context: " + context);
            Vector2 input = context.ReadValue<Vector2>();
            movement.x = speed * input.x;
            movement.z = speed * input.y;
        }
    }
}