﻿using UnityEngine;
using UnityEditor;
using System.Collections;

namespace RootMotion.FinalIK.Demos {

	/// <summary>
	/// Custom inspector and scene view helpers for the InteractionTarget.
	/// </summary>
	[CustomEditor(typeof(InteractionTarget))]
	public class InteractionTargetInspector : Editor {

		private InteractionTarget script { get { return target as InteractionTarget; }}

		private const string twistAxisLabel = " Twist Axis";
		private const float size = 0.005f;
		private static Color targetColor = new Color(0.2f, 1f, 0.5f);
		private static Color pivotColor = new Color(0.2f, 0.5f, 1f);

		void OnSceneGUI() {
			Handles.color = targetColor;

			Handles.SphereHandleCap(0, script.transform.position, Quaternion.identity, size, Event.current.type);

			DrawChildrenRecursive(script.transform);

			if (script.pivot != null) {
				Handles.color = pivotColor;
				GUI.color = pivotColor;

				Handles.SphereHandleCap(0, script.pivot.position, Quaternion.identity, size, Event.current.type);

				Vector3 twistAxisWorld = script.pivot.rotation * script.twistAxis.normalized * size * 40;
				Handles.DrawLine(script.pivot.position, script.pivot.position + twistAxisWorld);
				Handles.SphereHandleCap(0, script.pivot.position + twistAxisWorld, Quaternion.identity, size, Event.current.type);

				Handles.CircleHandleCap(0, script.pivot.position, Quaternion.LookRotation(twistAxisWorld), size * 20, Event.current.type);
				Handles.Label(script.pivot.position + twistAxisWorld, twistAxisLabel);
			}

			Handles.color = Color.white;
			GUI.color = Color.white;
		}

		private void DrawChildrenRecursive(Transform t) {
			for (int i = 0; i < t.childCount; i++) {

				Handles.DrawLine(t.position, t.GetChild(i).position);
				Handles.SphereHandleCap(0, t.GetChild(i).position, Quaternion.identity, size, Event.current.type);

				DrawChildrenRecursive(t.GetChild(i));
			}
		}
	}
}
