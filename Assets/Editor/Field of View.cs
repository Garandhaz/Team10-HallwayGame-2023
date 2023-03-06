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
        fieldOfViewPosition = new Vector3(fow.transform.position.x, (fow.transform.position.y + fow.guardEyeLevel), fow.transform.position.z);		
		Handles.color = Color.white;
		Handles.DrawWireArc (fieldOfViewPosition, Vector3.up, Vector3.forward, 360, fow.viewRadius); //Draws a circle around FoV radius
		Vector3 viewAngleA = fow.DirFromAngle (-fow.viewAngle / 2, false);
		Vector3 viewAngleB = fow.DirFromAngle (fow.viewAngle / 2, false);

		Handles.DrawLine (fieldOfViewPosition, fieldOfViewPosition + viewAngleA * fow.viewRadius); //Draws lines to indicate angle of FoV
		Handles.DrawLine (fieldOfViewPosition, fieldOfViewPosition + viewAngleB * fow.viewRadius);

		Handles.color = Color.blue;
		Handles.DrawWireArc (fieldOfViewPosition, Vector3.up, Vector3.forward, 360, fow.attackRadius); //Draw attack radius

		Handles.color = Color.red;
		foreach (Transform visibleTarget in fow.visibleTargets) //draws a line to all targets in FoV not blocked by obstacles
        {
			Handles.DrawLine (fieldOfViewPosition, visibleTarget.position); 
		}
	}
}
