using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;


namespace Percubed.Flex
{
    public class Flex_Collision : MonoBehaviour
    {
        private string this_object_name;

        private ShapeData collided_shapeData;

        public bool hitFlex;

        public GameObject other_object;

        //bool coroutineStarted = false;

        private void Awake()
        {
            this_object_name = this.name;
            
        }


        private void Update()
        {
            int layerMask = 1 << 7;
            layerMask = ~layerMask;

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                //Debug.Log("Did Hit layer: " + layerMask + " name: " + hit.collider.name + " " + " number: " + hit.collider.gameObject.layer);
                //If layermask is flexinteract which is interactable flex objects
                //if (hit.collider.gameObject.layer == 6)
                //{
                //    Debug.Log("hit a flex object");
                //    hitFlex = true;
                //}
                //if (!coroutineStarted)
                //{
                StartCoroutine(RegisterCollision(hit.collider));
                //hitFlex = false;
                //}
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                //Debug.Log("Did Hit layer: " + layerMask + " name: " + hit.collider.name + " " + " number: " + hit.collider.gameObject.layer);
                //if (hit.collider.gameObject.layer == 6)
                //{
                //    Debug.Log("hit a flex object");
                //    hitFlex = true;
                //}
                //if (!coroutineStarted)
                //{
                StartCoroutine(RegisterCollision(hit.collider));
                //hitFlex = false;
                //}
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity, layerMask))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.yellow);
                //Debug.Log("Did Hit layer: " + layerMask + " name: " + hit.collider.name + " " + " number: " + hit.collider.gameObject.layer); if (layerMask == 6)
                //if (hit.collider.gameObject.layer == 6)
                //{
                //    Debug.Log("hit a flex object");
                //    hitFlex = true;
                //}
                //if (!coroutineStarted)
                //{
                StartCoroutine(RegisterCollision(hit.collider));
                //hitFlex = false;
                //}
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, Mathf.Infinity, layerMask))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * hit.distance, Color.yellow);
                //Debug.Log("Did Hit layer: " + layerMask + " name: " + hit.collider.name + " " + " number: " + hit.collider.gameObject.layer); if (layerMask == 6)
                //if (hit.collider.gameObject.layer == 6)
                //{
                //    Debug.Log("hit a flex object");
                //    hitFlex = true;
                //}
                //if (!coroutineStarted)
                //{
                StartCoroutine(RegisterCollision(hit.collider));
                //hitFlex = false;
                //}
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }

        IEnumerator RegisterCollision(Collider collider)
        {
            other_object = collider.gameObject;
            if (collider.gameObject.layer == 6 && 
                Vector3.Distance(collider.gameObject.transform.position, this.transform.position) < 1.0f) 
            {
                Debug.Log("hit a flex object");
                Debug.Log(collider.name);
                hitFlex = true; 
            }
            yield return new WaitForSeconds(2);
            //coroutineStarted = true;
        }
    }
}

