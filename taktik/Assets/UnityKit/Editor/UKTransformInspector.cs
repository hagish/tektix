// Alternative version, with redundant code removed
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Transform))]
public class UKTransformInspector : Editor
{
	private static Vector3 storedPosition;
	private static Vector3 storedRotation;
	private static Vector3 storedScale;

	public override void OnInspectorGUI()
	{
		bool transformIsGlobal = false;

		Transform t = (Transform)target;

		// Replicate the standard transform inspector gui
		EditorGUIUtility.LookLikeControls();
		EditorGUI.indentLevel = 0;
		Vector3 position = EditorGUILayout.Vector3Field("Position", t.localPosition);
		Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
		Vector3 scale = EditorGUILayout.Vector3Field("Scale", t.localScale);

		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button (new GUIContent("id", "Sets transform to 0 values"))) {
			position = Vector3.zero;
			eulerAngles = Vector3.zero;
			scale = Vector3.one;
		}
		if (GUILayout.Button (new GUIContent("0P", "Sets position to 0/0/0"))) position = Vector3.zero;
		if (GUILayout.Button (new GUIContent("0R", "Sets rotation to 0/0/0"))) eulerAngles = Vector3.zero;
		if (GUILayout.Button (new GUIContent("1S", "Sets scale to 1/1/1"))) scale = Vector3.one;

		if (GUILayout.Button (new GUIContent("Cl", "Copies complete transform infos (local)"))) {
			storedPosition = position;
			storedRotation = eulerAngles;
			storedScale = scale;
		}
		if (GUILayout.Button (new GUIContent("Pl", "Pastes last copied transform into into this transform (local)"))) {
			position = storedPosition;
			eulerAngles = storedRotation;
			scale = storedScale;
		}

		if (GUILayout.Button (new GUIContent("Cg", "Copies complete transform infos (global)"))) {
			storedPosition = t.position;
			storedRotation = t.rotation.eulerAngles;
			storedScale = t.localScale;
		}
		if (GUILayout.Button (new GUIContent("Pg", "Pastes last copied transform into into this transform (global)"))) {
			position = storedPosition;
			eulerAngles = storedRotation;
			scale = storedScale;
			transformIsGlobal = true;
		}

		if (GUILayout.Button (new GUIContent("C", "Creates a new GameObject as a child of this GameObject"))) {
			GameObject child = new GameObject ("_child");
			child.transform.parent = t.transform;
			child.transform.localPosition = Vector3.zero;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
			Selection.objects = new Object[]{child};
		}
		if (GUILayout.Button (new GUIContent("N", "Creates a new GameObject as a neighbour of this GameObject"))) {
			GameObject neighbour = new GameObject ("_neighbour");
			neighbour.transform.parent = t.transform.parent;
			neighbour.transform.localPosition = Vector3.zero;
			neighbour.transform.localRotation = Quaternion.identity;
			neighbour.transform.localScale = Vector3.one;
			Selection.objects = new Object[]{neighbour};
		}

		EditorGUILayout.EndHorizontal ();

		// TODO: refactor this, LookLikeInspector is deprecated
		#pragma warning disable 0618
		EditorGUIUtility.LookLikeInspector();

		if (GUI.changed)
		{
			Undo.RegisterUndo(t, "Transform Change");

			if (transformIsGlobal) {
				t.position = FixIfNaN(position);
				t.rotation = Quaternion.Euler(FixIfNaN(eulerAngles));
				t.localScale = FixIfNaN(scale);
			} else {
				t.localPosition = FixIfNaN(position);
				t.localEulerAngles = FixIfNaN(eulerAngles);
				t.localScale = FixIfNaN(scale);
			}
		}
	}

	private Vector3 FixIfNaN(Vector3 v)
	{
		if (float.IsNaN(v.x))
		{
			v.x = 0;
		}
		if (float.IsNaN(v.y))
		{
			v.y = 0;
		}
		if (float.IsNaN(v.z))
		{
			v.z = 0;
		}
		return v;
	}

}