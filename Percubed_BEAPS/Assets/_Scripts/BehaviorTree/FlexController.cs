using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using NVIDIA.Flex;

namespace Percubed.Flex
{
    public class FlexController : MonoBehaviour
    {
        public MeltEffectFlex melt = null;

        public ShockEffectFlex shock = null;

        public SkinnedMorphTargets morphComponent = null;
        //Insert the merging component here for the skinned mesh target

        //public int flexParams;

        private bool iterBool;

        public FlexContainer flex_cont;

        //public float gravityY;

        // Start is called before the first frame update
        void Awake() { this.Initialize(); }

        protected void Initialize()
        {
            this.melt = this.GetComponent<MeltEffectFlex>();
            //this.shock = this.GetComponent<ShockEffectFlex>();
            //this.flexParams = this.GetComponent<FlexParameters>();
        }

        public Node Node_blend(float value)
        {
            return new LeafInvoke(
                () => this.morphComponent.blendWeights[0] = value
                );
        }

        public Node Node_Melt(Val<bool> trigger)
        {
            return new LeafInvoke(
                () => this.melt.melt = trigger.Value
                );
        }

        public Node Node_Shock(Val<bool> trigger)
        {
            return new LeafInvoke(
                () => this.shock.shock = trigger.Value
                );
        }

    }
}
