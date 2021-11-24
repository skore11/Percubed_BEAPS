using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using RootMotion.FinalIK;
using UnityEngine.UI;
using NVIDIA.Flex;

namespace Percubed.Flex
{
    public class ShockBehavior : MonoBehaviour
    {
        public Transform wander1;
        public Transform wander2;
        //public Transform wander3;
        //public bool meltSelection;
        public GameObject participant;
        public GameObject participant2;
        public Transform target;
        public FullBodyBipedEffector hand;
        public InteractionObject button;
        public bool shock;
        //public int iteration1;
        //public int iteration2;

        //public float gravityVal;

        private BehaviorAgent behaviorAgent;
        //private BehaviorAgent behaviorAgent2;
        // Use this for initialization

        public Text debugText;

        //public UIController uIController;
        //public enum iter
        //{
        //    rigid = 10,
        //    loose = 15
        //};
        private BehaviorUpdater behaviorUpdater;

        void Start()
        {
            behaviorUpdater = FindObjectOfType<BehaviorUpdater>();

            behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
            BehaviorManager.Instance.Register(behaviorAgent);
            behaviorAgent.StartBehavior();

            //behaviorAgent2 = new BehaviorAgent(this.Trial());
            //BehaviorManager.Instance.Register(behaviorAgent2);
            //behaviorAgent2.StartBehavior();
        }

        //Update is called once per frame
        void Update()
        {
            String DebugOutput = "";
            DebugOutput += Node.PrintTree(behaviorAgent.treeRoot);
            foreach (Node n in behaviorAgent.treeRoot.Trace())
            {
                String hCode = "" + n.GetHashCode();
                DebugOutput = DebugOutput.Replace(hCode, "<b>" + hCode + "</b>");
            }
            if (debugText != null)
            {
                debugText.text = DebugOutput;
                Debug.Log(DebugOutput);
            }
            //try to turn of behavior updater here and restart when needed!
            //if (uIController.turnOffAnim)
            //{
            //    behaviorUpdater.enabled = false;
            //}
            
        }

        protected Node ST_ApproachAndWait(Transform target)
        {
            Val<Vector3> position = Val.V(() => target.position);
            return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_GoTo(position)/*, new LeafWait(1000)*/);
        }

        //protected Node ST_Melt(bool x)
        //{
        //    return new Selector(participant2.GetComponent<FlexController>().Node_Melt(x));
        //}

        protected Node ST_shocker(Transform target, FullBodyBipedEffector hand, InteractionObject button)
        {
            Val<Vector3> position = Val.V(() => target.position);
            print("hand" + hand);
            print("button" + button);
            return new Sequence(participant.GetComponent<BehaviorMecanim>().Node_OrientTowards(position), participant.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, button), participant.GetComponent<BehaviorMecanim>().Node_ResumeInteraction(hand));
        }



        //protected Node ST_blend()
        //{
        //    return new Parallel();
        //}

        //protected Node ST_Iter(int iter)
        //{
        //    FlexController flexController = participant2.GetComponent<FlexController>();
        //    return new Selector(new LeafInvoke(
        //        () => flexController.flexParams = iter
        //    ));
        //}

        protected Node ST_Shock(bool x)
        {
            return new Selector(participant2.GetComponent<FlexController>().Node_Shock(x));
        }

        protected Node stylize()
        {
            //Call create behavior script here; add modifications as necessary
            //Alternatively the user may select from a dropdown a list of available modifications from the past
            //Here we will pause the running node by disabling the behaviorUpdater
            //Trigger the UI drop down for available behaviors
            //The behaviors in the drop down should also host the list of vertices for blending them
            //Apply the blend with said vertices as necessary
            //Hit done! on UI box when all modifications are done, if there are new modifications store them as XML...
            //Resume the behavior updater
            behaviorUpdater.enabled = false;
            participant2.GetComponent<Animator>().enabled = false;
            StartCoroutine(wait());
            
            return new Selector();
            
        }

        public IEnumerator wait()
        {
            print("waiting");
            yield return new WaitForSeconds(10);
            participant2.GetComponent<Animator>().enabled = true;
            behaviorUpdater.enabled = true;
        }
        //protected Node ST_Gravity(float y_gravity)
        //{
        //    FlexController flexController = participant2.GetComponent<FlexController>();
        //    return new Selector(new LeafInvoke(
        //        () => flexController.gravityY = y_gravity
        //        ));
        //}

        //protected Node ST_pauseTree()
        //{



        //}

        //protected Node ST_deform()
        //{
        //    FlexController flexController = participant2.GetComponent<FlexController>();
        //    return new Selector(new LeafInvoke(
        //        () => flexController.getBehavior.assign 
        //        )
        //        )
        //}

        //Over here run a parallel node that allows the user to interact with the character by choosing which are of the body needs to be shocked
        //Add additions/modifications to list of allowable additions when the menu for user interaction is revealed when the parallel node is triggered
        //Apply modifications, the blending will take place as long as the parallel node runs
        //Once shocking is done, and parallel node has completed successfully, blend back to original animation, basically as dictated by the user and hence parallel node.
        protected Node BuildTreeRoot()
        {
            //iter value = new iter();
            Node shocked = new DecoratorLoop(
                            new Selector(
                            new SequenceParallel(this.ST_ApproachAndWait(this.wander1) /*this.ST_Shock(this.shock)*/),
                            new SelectorParallel(this.stylize(),this.ST_ApproachAndWait(this.wander2)/*, this.stylize()*/),
                            this.ST_shocker(target, hand, button)));
            return shocked;

        }

        //Write another behavior tree to pick up an object.

        //protected Node Trial()
        //{
        //    Node gravity = new DecoratorLoop(
        //                    new Sequence(
        //                        this.ST_Gravity(gravityVal),
        //                        this.ST_Gravity(-9.8f), new LeafWait(500)
        //                        )
        //                        );
        //    return gravity;

        //}
    }

}