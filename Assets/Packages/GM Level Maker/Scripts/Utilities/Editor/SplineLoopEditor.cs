using UnityEngine;
using UnityEditor;
namespace GM.LevelMaker
{
	[CustomEditor(typeof(SplineLoop))]
	public class SplineLoopEditor : Editor
	{

		private SplineLoop spline;
		private Quaternion rot;
		private Transform tr;

		private void OnSceneGUI()
		{
			spline = target as SplineLoop;
			tr = spline.transform;
			rot = Tools.pivotRotation == PivotRotation.Local ? tr.rotation : Quaternion.identity;

			Handles.color = Color.white;

			for (int x = 0; x < spline.GetControlPointCount(); x++)
			{
				if (x == spline.GetControlPointCount() - 1)
					continue;

				Handles.color = Color.yellow;
				Vector3 pos = spline.GetControlPoint(x);
				bool pressed = false;

				Vector3 hn = spline.GetHandle(x, true);
				Handles.DrawLine(pos, hn);

				if (x == spline.selectedPointID)
				{
					Handles.SphereHandleCap(0, hn, rot, HandleUtility.GetHandleSize(hn) * 0.1f, EventType.Repaint);
				}
				else
				{
					Handles.color = Color.gray;
					pressed |= Handles.Button(hn, rot, HandleUtility.GetHandleSize(hn) * 0.1f, HandleUtility.GetHandleSize(hn) * 0.06f, Handles.SphereHandleCap);
					Handles.color = Color.yellow;
				}

				if (x < spline.GetCurveCount())
				{
					hn = spline.GetHandle(x, false);
					Handles.DrawLine(pos, hn);
					Handles.SphereHandleCap(0, hn, rot, HandleUtility.GetHandleSize(hn) * 0.1f, EventType.Repaint);
					if (x != spline.selectedPointID)
					{
						Handles.color = Color.gray;
						pressed |= Handles.Button(hn, rot, HandleUtility.GetHandleSize(hn) * 0.1f, HandleUtility.GetHandleSize(hn) * 0.06f, Handles.SphereHandleCap);
						Handles.color = Color.yellow;
					}
				}

				if (x == spline.selectedPointID)
				{
					Handles.color = Color.green;
					Handles.SphereHandleCap(0, pos, rot, HandleUtility.GetHandleSize(pos) * 0.1f, EventType.Repaint);
					continue;
				}

				Handles.color = Color.white;
				pressed |= Handles.Button(pos, rot, HandleUtility.GetHandleSize(pos) * 0.15f, HandleUtility.GetHandleSize(pos) * 0.06f, Handles.SphereHandleCap);
				if (pressed)
				{
					EditorUtility.SetDirty(target);
					spline.selectedPointID = x;
				}
			}

			if (spline.selectedPointID == -1)
				return;

			Vector3 pt = spline.GetControlPoint(spline.selectedPointID);
			EditorGUI.BeginChangeCheck();
			pt = Handles.PositionHandle(pt, rot);
			if (EditorGUI.EndChangeCheck())
			{
				pt.x = spline.lockX ? tr.position.x : pt.x;
				pt.y = spline.lockY ? tr.position.y : pt.y;
				pt.z = spline.lockZ ? tr.position.z : pt.z;
				Undo.RecordObject(spline, "Move Point");
				spline.SetControlPoint(spline.selectedPointID, pt);
				EditorUtility.SetDirty(spline);
			}

			pt = spline.GetHandle(spline.selectedPointID, true);
			EditorGUI.BeginChangeCheck();
			pt = Handles.PositionHandle(pt, rot);
			if (EditorGUI.EndChangeCheck())
			{
				pt.x = spline.lockX ? tr.position.x : pt.x;
				pt.y = spline.lockY ? tr.position.y : pt.y;
				pt.z = spline.lockZ ? tr.position.z : pt.z;
				Undo.RecordObject(spline, "Move Left Handle");
				spline.SetHandle(spline.selectedPointID, true, pt);
				EditorUtility.SetDirty(spline);
			}

			if (spline.selectedPointID < spline.GetCurveCount())
			{
				pt = spline.GetHandle(spline.selectedPointID, false);
				EditorGUI.BeginChangeCheck();
				pt = Handles.PositionHandle(pt, rot);
				if (EditorGUI.EndChangeCheck())
				{
					pt.x = spline.lockX ? tr.position.x : pt.x;
					pt.y = spline.lockY ? tr.position.y : pt.y;
					pt.z = spline.lockZ ? tr.position.z : pt.z;
					Undo.RecordObject(spline, "Move Right Handle");
					spline.SetHandle(spline.selectedPointID, false, pt);
					EditorUtility.SetDirty(spline);
				}
			}
		}

		public override void OnInspectorGUI()
		{

			spline = target as SplineLoop;

			if (spline == null)
			{
				return;
			}

			EditorGUI.BeginChangeCheck();
			spline.DrawGizmo = EditorGUILayout.Toggle("Draw Gizmo", spline.DrawGizmo);

			if (spline.selectedPointID != -1 && spline.GetControlPointCount() > spline.selectedPointID)
			{
				EditorGUILayout.BeginHorizontal();
				float originalValue = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 50;
				GUILayout.Space(200);
				GUILayout.FlexibleSpace();
				spline.lockX = EditorGUILayout.Toggle("Lock X", spline.lockX);
				GUILayout.FlexibleSpace();
				spline.lockY = EditorGUILayout.Toggle("Lock Y", spline.lockY);
				GUILayout.FlexibleSpace();
				spline.lockZ = EditorGUILayout.Toggle("Lock Z", spline.lockZ);
				EditorGUIUtility.labelWidth = originalValue;
				EditorGUILayout.EndHorizontal();


				EditorGUI.BeginChangeCheck();
				Vector3 point = EditorGUILayout.Vector3Field("Point:", spline.GetControlPoint(spline.selectedPointID));
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(spline, "Move Point");
					spline.SetControlPoint(spline.selectedPointID, point);
					EditorUtility.SetDirty(spline);
				}

				EditorGUI.BeginChangeCheck();
				point = EditorGUILayout.Vector3Field("Left Handle:", spline.GetHandle(spline.selectedPointID, true));
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(spline, "Move Left Handle");
					spline.SetHandle(spline.selectedPointID, true, point);
					EditorUtility.SetDirty(spline);
				}

				if (spline.selectedPointID < spline.GetCurveCount())
				{
					EditorGUI.BeginChangeCheck();
					point = EditorGUILayout.Vector3Field("Right Handle:", spline.GetHandle(spline.selectedPointID, false));
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(spline, "Move Left Handle");
						spline.SetHandle(spline.selectedPointID, false, point);
						EditorUtility.SetDirty(spline);
					}
				}

				GUILayout.Space(10);

				EditorGUI.BeginChangeCheck();
				SplineLoop.HandleConstraint nw = (SplineLoop.HandleConstraint)EditorGUILayout.EnumPopup("Handle Constraint: ", spline.GetConstraint(spline.selectedPointID));
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(spline, "Set Constraint");
					spline.SetConstraint(spline.selectedPointID, nw);
					if (spline.selectedPointID == spline.GetControlPointCount() - 1)
						spline.selectedPointID = 0;
					EditorUtility.SetDirty(spline);
				}

				GUILayout.Space(10);

				GUILayout.Label("Subdivide Curve:");
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Left"))
				{
					Undo.RecordObject(spline, "Subdivide Curve Left");
					spline.SubdivideCurve(spline.selectedPointID);
					EditorUtility.SetDirty(spline);
				}
				if (GUILayout.Button("Right"))
				{
					Undo.RecordObject(spline, "Subdivide Curve Right");
					spline.SubdivideCurve(spline.selectedPointID + 1);
					EditorUtility.SetDirty(spline);
				}
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(10);

				if (GUILayout.Button("Remove Curve"))
				{
					Undo.RecordObject(spline, "Remove Curve");
					spline.RemoveCurve(spline.selectedPointID);
					EditorUtility.SetDirty(spline);
				}
			}
		}

		private void OnDisable()
		{
			spline = target as SplineLoop;
			spline.selectedPointID = -1;
		}
	}
}