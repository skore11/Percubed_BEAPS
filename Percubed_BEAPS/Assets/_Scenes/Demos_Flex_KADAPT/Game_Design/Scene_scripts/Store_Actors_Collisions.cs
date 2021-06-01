using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

namespace Percubed.Flex
{
    public class Store_Actors_Collisions : MonoBehaviour
    {

  

        private FlexSoftActor[] soft_actor_array;

        private FlexSolidActor[] solid_actor_array;

        public bool contact;

        private void Awake()
        {       
            soft_actor_array = FindObjectsOfType<FlexSoftActor>();
            solid_actor_array = FindObjectsOfType<FlexSolidActor>();
        }

        private void Update()
        {
            //check if any two (or more) flex actors are colliding?
            //If they are find the closest particles interacting in the associated flex actors.
            //Return the indices of said particles and bool contact true.
        }
    }
}

