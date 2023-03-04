using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (guardController))]
public class FieldofView : Editor
{
	private Vector3 fieldOfViewPosition;

    void OnSceneGUI() 
    {
		guardController fow = (guardController)target;
        fieldOfViewPosition = new Vector3(fow.transform.position.x, fow.guardEyeLevel, fow.transform.position.z);		
		Handles.color = Color.white;
		Handles.DrawWireArc (fieldOfViewPosition, Vector3.up, Vector3.forward, 360, fow.viewRadius);
		Vector3 viewAngleA = fow.DirFromAngle (-fow.viewAngle / 2, false);
		Vector3 viewAngleB = fow.DirFromAngle (fow.viewAngle / 2, false);

		Handles.DrawLine (fieldOfViewPosition, fieldOfViewPosition + viewAngleA * fow.viewRadius);
		Handles.DrawLine (fieldOfViewPosition, fieldOfViewPosition + viewAngleB * fow.viewRadius);

		Handles.color = Color.red;
		foreach (Transform visibleTarget in fow.visibleTargets) 
        {
			Handles.DrawLine (fieldOfViewPosition, visibleTarget.position);
		}
	}
}
