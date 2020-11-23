using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Demo Highlights: 
There will be a few interactions between agents
Each interaction will have a description
Each description will translate to Flex effect/s and KADAPT nodes
Flex acts as a filter on top of KADAPT and the database of motion clips
The feature: with physically based effects as an animation layer, there is room for modular, adaptive behaviors 
 */

/*Hero is searching for gold and being followed by Enemy*/
    //Hero will repeat all events given here until gold is found
    //For Demo purposes the main camera will be on the Hero
    //**Change this to player controlled version in Demo2**.
    //A TRANSITION means the underlying IK root motion anims are overlaid with Flex particles

    // Hero starts at starting point:
        //Hero begins journey here
        //Possibly a Sequence node for all sequences of actions
        //Enemy also starts at their own starting point
        //Enemy will follow Hero at a distance till the moment is right
    
    // Hero walks across a friend:
        //They chat : Selector node perhaps for chatting animation
        //Enemy crouches behind an obstacle out of view.
        //The Hero recieves directions to gold: Make friend point in direction of gold
        //ADAPTABLE : transition friend to physics object and make them point with low or high points
        //Hero says thanks and leaves

    // Hero steps on some sharp object:
        //Hero seems hurt
        //TRANSITION to Flex
        //Effect: Lock particles
        //Enforce a slight limp on Hero : Initialize box collider to lock particles at the limping leg
        
    // Hero comes across his lover:
        //Pretend to not limp much
        //ADAPTABLE: the Hero may pretend to not limp or may limp furhter based on internal state
        //Effect: Lock Particles
        //While approaching lover lock particles along base and middle of spine to keep posture straight
        //When reached lover: Transition back to KADAPT anims
        //Play Hi, Hello anims for both

    //Hero leaves lover and heads to a crowd:
        //ADAPTABLE: Limp more or less based on internal state
        //The crowd pushes him around
        //TRANSITION to Flex
        //Based on Flex parameters the Hero may get pushed around a lot or none at all
        //Effect: Iteration count of Solver, rigid or soft
        //The enemy waits for crowd to disperse then follows the Hero again more quickly

    //Hero notices the Enemy:
        //ADAPTABLE: Limp more or less based on internal state
        //The enemy enters the range of the Hero's detection
        //Once detected they two characters battle
        //TRANSITION to Flex, show that both characters can actually hit each other
        //The Hero places hex on Enemy, the Enemy is electrocuted
        //Effects: Jiggle/Shock on Enemy
        //The enemy is defeated and falls back as rigi object
        //Effects: Turn off underlying animations and unlock particles

    //Hero is visibly tired:
        //ADAPTABLE: Hero has got a more noticeable Limp (or not based on internal state)
        //A gust of wind blows our Hero in the direction of the gold
        //ADAPTABLE: Hero is pushed by wind, might affect him a little or a lot
        //Effects: Wind/Gravity in a certan axis
        //Effects: Iteration count to simulate being affected more or less by wind
        //While wind is blowing play anims that resemble covering the body

    //Hero finds gold:
        //Hero has found his gold
        //TRANSITION to Flex, show a limp then TRANSITION back to normal
        //He is elated play celebration anim



