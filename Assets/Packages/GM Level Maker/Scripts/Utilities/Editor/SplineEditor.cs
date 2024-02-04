using UnityEngine;
using UnityEditor;

namespace GM.LevelMaker
{
	[CustomEditor(typeof(Spline))]
	public class SplineEditor : Editor
	{

		private const int curve_draw_iter = 20;

		private Spline spline;
		private Quaternion rot;
		private Transform tr;

		private void OnSceneGUI()
		{
			spline = target as Spline;
			tr = spline.transform;
			rot = Tools.pivotRotation == PivotRotation.Local ? tr.rotation : Quaternion.identity;

			Handles.color = Color.white;

			int num_curves = spline.GetCurveCount();
			float t = 0;
			Vector3 prev = spline.GetControlPoint(0);
			for (int i = 0; i < num_curves; i++)
			{
				for (int x = 1; x <= curve_draw_iter; x++)
				{
					t = i + (float)x / (float)curve_draw_iter;

					Vector3 next = spline.GetPoint(t);
					Handles.DrawLine(prev, next);
					prev = next;
				}
			}


			for (int x = 0; x < spline.GetControlPointCount(); x++)
			{
				Handles.color = Color.yellow;
				Vector3 pos = spline.GetControlPoint(x);
				bool pressed = false;
				if (x != 0)
				{
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
				}
				if (x < spline.GetCurveCount())
				{
					Vector3 hn = spline.GetHandle(x, false);
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
				pressed |= Handles.Button(pos, rot, HandleUtility.GetHandleSize(pos) * 0.1f, HandleUtility.GetHandleSize(pos) * 0.06f, Handles.SphereHandleCap);
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
				Undo.RecordObject(spline, "Move Point");
				spline.SetControlPoint(spline.selectedPointID, pt);
				EditorUtility.SetDirty(spline);
			}

			if (spline.selectedPointID != 0)
			{
				pt = spline.GetHandle(spline.selectedPointID, true);
				EditorGUI.BeginChangeCheck();
				pt = Handles.PositionHandle(pt, rot);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(spline, "Move Left Handle");
					spline.SetHandle(spline.selectedPointID, true, pt);
					EditorUtility.SetDirty(spline);
				}
			}

			if (spline.selectedPointID < spline.GetCurveCount())
			{
				pt = spline.GetHandle(spline.selectedPointID, false);
				EditorGUI.BeginChangeCheck();
				pt = Handles.PositionHandle(pt, rot);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(spline, "Move Right Handle");
					spline.SetHandle(spline.selectedPointID, false, pt);
					EditorUtility.SetDirty(spline);
				}
			}
		}

		public override void OnInspectorGUI()
		{
			spline = target as Spline;

			if (spline == null)
			{
				return;
			}

			if (spline.selectedPointID != -1)
			{
				EditorGUI.BeginChangeCheck();
				Vector3 point = EditorGUILayout.Vector3Field("Point:", spline.GetControlPoint(spline.selectedPointID));
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(spline, "Move Point");
					spline.SetControlPoint(spline.selectedPointID, point);
					EditorUtility.SetDirty(spline);
				}
				if (spline.selectedPointID != 0)
				{
					EditorGUI.BeginChangeCheck();
					point = EditorGUILayout.Vector3Field("Left Handle:", spline.GetHandle(spline.selectedPointID, true));
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(spline, "Move Left Handle");
						spline.SetHandle(spline.selectedPointID, true, point);
						EditorUtility.SetDirty(spline);
					}
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

				GUILayout.Label("Add Curve At:");
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Beginning"))
				{
					Undo.RecordObject(spline, "Add Curve at Beginning");
					spline.AddCurveBeginning();
					EditorUtility.SetDirty(spline);
				}
				if (GUILayout.Button("End"))
				{
					Undo.RecordObject(spline, "Add Curve at End");
					spline.AddCurve();
					EditorUtility.SetDirty(spline);
				}
				EditorGUILayout.EndHorizontal();

				GUILayout.Label("Subdivide Curve:");
				EditorGUILayout.BeginHorizontal();
				if (spline.selectedPointID != 0 && GUILayout.Button("Left"))
				{
					Undo.RecordObject(spline, "Subdivide Curve Left");
					spline.SubdivideCurve(spline.selectedPointID);
					EditorUtility.SetDirty(spline);
				}
				if (spline.selectedPointID < spline.GetCurveCount() && GUILayout.Button("Right"))
				{
					Undo.RecordObject(spline, "Subdivide Curve Right");
					spline.SubdivideCurve(spline.selectedPointID + 1);
					EditorUtility.SetDirty(spline);
				}
				EditorGUILayout.EndHorizontal();

				if (GUILayout.Button("Remove Curve"))
				{
					Undo.RecordObject(spline, "Remove Curve");
					spline.RemoveCurve(spline.selectedPointID);
					EditorUtility.SetDirty(spline);
				}

				if (spline.selectedPointID != 0 && spline.selectedPointID < spline.GetControlPointCount() - 1)
				{
					EditorGUI.BeginChangeCheck();
					Spline.HandleConstraint nw = (Spline.HandleConstraint)EditorGUILayout.EnumPopup("Handle Constraint: ", spline.GetConstraint(spline.selectedPointID));
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(spline, "Set Constraint");
						spline.SetConstraint(spline.selectedPointID, nw);
						if (spline.selectedPointID == spline.GetControlPointCount() - 1)
							spline.selectedPointID = 0;
						EditorUtility.SetDirty(spline);
					}
				}
			}

			GUILayout.Space(10);

			if (GUILayout.Button("Re-Align"))
			{
				Undo.RecordObject(spline, "Realign spline");
				spline.RealignSpline();
				EditorUtility.SetDirty(spline);
			}
		}

		private void OnDisable()
		{
			spline = target as Spline;
			spline.selectedPointID = -1;
		}
	}
}