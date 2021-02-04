using UnityEngine;
using System.Collections;
using UnityEditor;

namespace RootMotion.FinalIK.Demos {

	/// <summary>
	/// Scene view helper for the LimbIK BendGoal
	/// </summary>
	[CustomEditor(typeof(BendGoal))]
	public class BendGoalInspector : Editor {
		
		private BendGoal script { get { return target as BendGoal; }}
		
		public override void OnInspectorGUI() {
			DrawDefaultInspector();
		}
		
		void OnSceneGUI() {
			if (script.limbIK == null) return;
			if (script.limbIK.solver.bone2 == null) return;
			if (script.limbIK.solver.bone2.transform == null) return;
			
			Handles.color = Color.cyan;
			
			Vector3 bonePosition = script.limbIK.solver.bone2.transform.position;
			Handles.DrawLine(script.transform.position, bonePosition);
			Handles.SphereHandleCap(0, script.transform.position, Quaternion.identity, 0.05f, Event.current.type);
			Handles.SphereHandleCap(0, bonePosition, Quaternion.identity, 0.025f, Event.current.type);
			
			Handles.color = Color.white;
		}
	}
}
