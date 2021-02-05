using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Percubed.Flex
{
    [RequireComponent(typeof(BoxCollider))]
    public class BoxColliderKeys : MonoBehaviour
    {
        private BoxCollider boxColl;

        private void Awake()
        {
            boxColl = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            if (Input.GetKeyDown("o"))
            {
                boxColl.center += new Vector3(0.0f, 2.0f, 0.0f);
            }
            if (Input.GetKeyDown("k"))
            {
                boxColl.center += new Vector3(0.0f, -2.0f, 0.0f);
            }
            if (Input.GetKeyDown("j"))
            {
                boxColl.center += new Vector3(2.0f, 0.0f, 0.0f);
            }
            if (Input.GetKeyDown("l"))
            {
                boxColl.center += new Vector3(-2.0f, 0.0f, 0.0f);
            }
            if (Input.GetKeyDown("i"))
            {
                boxColl.center += new Vector3(0.0f, 0.0f, 2.0f);
            }
            if (Input.GetKeyDown("p"))
            {
                boxColl.center += new Vector3(0.0f, 0.0f, -2.0f);
            }
            if (Input.GetKeyDown("b"))
            {
                boxColl.size += new Vector3(1.0f, 1.0f, 1.0f);
            }
            if (Input.GetKeyDown("v"))
            {
                boxColl.size -= new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }
}
