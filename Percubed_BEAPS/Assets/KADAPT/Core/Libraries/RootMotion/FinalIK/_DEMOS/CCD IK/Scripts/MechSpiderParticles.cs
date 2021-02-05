using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK.Demos {

	/// <summary>
	/// Emitting smoke for the mech spider
	/// </summary>
	public class MechSpiderParticles: MonoBehaviour {
		
		public MechSpiderController mechSpiderController;
		
		private ParticleSystem particles;
		
		void Start() {
			particles = (ParticleSystem)GetComponent(typeof(ParticleSystem));
		}
		
		void Update() {
			// Smoke
			float inputMag = mechSpiderController.inputVector.magnitude;
            var em = particles.emission;
			em.rateOverTime = Mathf.Clamp(inputMag * 50, 30, 50);
            var main = particles.main;
			main.startColor = new Color (main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, Mathf.Clamp(inputMag, 0.4f, 1f));
		}
	}
}
