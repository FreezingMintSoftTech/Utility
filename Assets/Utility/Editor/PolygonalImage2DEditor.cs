using UnityEditor;
using UnityEngine;

namespace Utility
{
    [CustomEditor(typeof(PolygonalImage2D)), CanEditMultipleObjects]
    public class PolygonalImage2DEditor : ImageEditor
    {
        private const float ButtonMiniWidth = 20.0f;
        private static GUIContent ButtonDuplicateContent = new GUIContent("+", "duplicate");
        private static GUIContent ButtonDeleteContent = new GUIContent("-", "delete");
        private static GUIContent ButtonAddContent = new GUIContent("+", "add");

        public static void ShowList(SerializedProperty list)
        {
            if(!list.isArray) {
                EditorGUILayout.HelpBox(list.name + " is neither an array nor a list.", MessageType.Error);
                return;
            }

            EditorGUILayout.PropertyField(list);
            EditorGUI.indentLevel += 1;
            if(list.isExpanded) {
                SerializedProperty propSize = list.FindPropertyRelative("Array.size");
                EditorGUILayout.PropertyField(propSize);
                if(propSize.hasMultipleDifferentValues) {
                    EditorGUILayout.HelpBox("Now showing arrays or lists with different sizes.", MessageType.Info);
                } else {

                    GUILayoutOption buttonMiniWidth = GUILayout.Width(ButtonMiniWidth);
                    for(int i = 0; i < list.arraySize; ++i) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));

                        if(GUILayout.Button(ButtonDuplicateContent, EditorStyles.miniButtonLeft, buttonMiniWidth)) {
                            list.InsertArrayElementAtIndex(i);

                        }

                        if(GUILayout.Button(ButtonDeleteContent, EditorStyles.miniButtonRight, buttonMiniWidth)) {
                            int size = list.arraySize;
                            list.DeleteArrayElementAtIndex(i);
                            if(size == list.arraySize) {
                                list.DeleteArrayElementAtIndex(i);
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                    if(list.arraySize <= 0 && GUILayout.Button(ButtonAddContent, EditorStyles.miniButton)) {
                        list.arraySize += 1;
                    }
                } //if(propSize.hasMultipleDifferentValues) {
            } //if(!showLabel || list.isExpanded) {

            EditorGUI.indentLevel -= 1;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            SerializedProperty propPoints = serializedObject.FindProperty("points_");
            //EditorGUILayout.PropertyField(propPoints, true);
            ShowList(propPoints);

            if(!serializedObject.isEditingMultipleObjects) {
                int numPoints = propPoints.arraySize;
                if(numPoints < 3) {
                    EditorGUILayout.HelpBox("At least three points are required.", MessageType.Warning);
                } else {
                    EditorGUILayout.HelpBox(numPoints + " points in total.", MessageType.Info);
                }
            }

            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Reset", EditorStyles.miniButtonLeft)){
                for(int i = 0; i < targets.Length; ++i) {
                    if(PrefabUtility.GetPrefabType(targets[i]) != PrefabType.Prefab) {
                        PolygonalImage2D polygonalImage2D = targets[i] as PolygonalImage2D;
                        polygonalImage2D.resetPoints();
                        polygonalImage2D.SetVerticesDirty();
                        EditorUtility.SetDirty(targets[i]);
                    }
                }
            }

            if(GUILayout.Button("Reset Smaller", EditorStyles.miniButtonRight)){
                for(int i = 0; i < targets.Length; ++i) {
                    if(PrefabUtility.GetPrefabType(targets[i]) != PrefabType.Prefab) {
                        PolygonalImage2D polygonalImage2D = targets[i] as PolygonalImage2D;
                        polygonalImage2D.resetPointsSmaller();
                        polygonalImage2D.SetVerticesDirty();
                        EditorUtility.SetDirty(targets[i]);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            if(serializedObject.ApplyModifiedProperties()
                || (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")) {
                for(int i = 0; i < targets.Length; ++i) {
                    if(PrefabUtility.GetPrefabType(targets[i]) != PrefabType.Prefab) {
                        EditorUtility.SetDirty(targets[i]);
                    }
                }
            }
        }

        public static readonly Vector3 SnapeSize = new Vector3(0.5f, 0.5f, 0.5f);

        void OnSceneGUI()
        {
            PolygonalImage2D polygon2D = target as PolygonalImage2D;
            if(null == polygon2D.points_) {
                return;
            }
            for(int i = 0; i < polygon2D.points_.Length; ++i) {
                Vector3 pos = polygon2D.points_[i];
                pos = polygon2D.transform.TransformPoint(pos);
                Vector3 nextPos = Handles.FreeMoveHandle(pos, Quaternion.identity, 1.5f, SnapeSize, Handles.DotCap);
                if(pos != nextPos) {
                    polygon2D.points_[i] = polygon2D.transform.InverseTransformPoint(nextPos);
                    polygon2D.SetVerticesDirty();
                    EditorUtility.SetDirty(target);
                }
            }
        }
    }
}
