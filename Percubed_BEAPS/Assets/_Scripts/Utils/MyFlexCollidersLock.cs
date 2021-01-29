using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Percubed.Flex
{
    public class MyFlexCollidersLock : FlexCollidersLock
    {
        // Start is called before the first frame update
        void Start()
        {
            myCol = GetComponent<BoxCollider>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown("o"))
            {
                myCol.center += new Vector3(0.0f, 2.0f, 0.0f);

                moveCol = true;
            }
            if (Input.GetKeyDown("k"))
            {
                myCol.center += new Vector3(0.0f, -2.0f, 0.0f);

                moveCol = true;
            }
            if (Input.GetKeyDown("j"))
            {
                myCol.center += new Vector3(2.0f, 0.0f, 0.0f);
                moveCol = true;
            }
            if (Input.GetKeyDown("l"))
            {
                myCol.center += new Vector3(-2.0f, 0.0f, 0.0f);
                moveCol = true;
            }
            if (Input.GetKeyDown("i"))
            {
                myCol.center += new Vector3(0.0f, 0.0f, 2.0f);
                moveCol = true;
            }
            if (Input.GetKeyDown("p"))
            {
                myCol.center += new Vector3(0.0f, 0.0f, -2.0f);
                moveCol = true;
            }
            if (Input.GetKeyDown("b"))
            {
                myCol.size += new Vector3(1.0f, 1.0f, 1.0f);
                moveCol = true;
            }
            if (Input.GetKeyDown("v"))
            {
                myCol.size -= new Vector3(1.0f, 1.0f, 1.0f);
                moveCol = true;
            }
        }
    }
}
