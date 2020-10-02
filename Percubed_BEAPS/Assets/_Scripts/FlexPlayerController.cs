using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Percubed.Flex {

    public class FlexPlayerController : MonoBehaviour
    {
        public float speed;

        void Update()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            float moveUpDown = Input.GetAxis("UpandDown");
            Vector3 movement = new Vector3(moveHorizontal, moveUpDown, moveVertical);
            transform.Translate(movement);
        }
    }
}