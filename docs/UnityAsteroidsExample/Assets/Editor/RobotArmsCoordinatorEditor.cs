using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace RobotArms {
	[CustomEditor(typeof(RobotArmsCoordinator))]
	public class RobotArmsCoordinatorEditor : Editor {

		HashSet<string> tags;
		Type[] processorTypes;

		public void OnEnable() {
			processorTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
				assembly => assembly.GetTypes().Where(
					type => type.IsSubclassOf(typeof(RobotArmsProcessor)))).ToArray();

			foreach (var type in processorTypes) {
				if (type.GetCustomAttributes(typeof(ProcessorOptionsAttribute), true).Length == 0) {
					Debug.LogError(string.Format("RobotArmsProcessor {0} requires a ProcessorOptions attribute", type));
				}
			}
			tags = new HashSet<string>(
				processorTypes.Select(processorType => (processorType.GetCustomAttributes(typeof(ProcessorOptionsAttribute), true)[0] as ProcessorOptionsAttribute).Tag).OrderBy(t => t)
			);
		}

		public override void OnInspectorGUI() {
			var coordinator = target as RobotArmsCoordinator;
			DrawDefaultInspector();
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal(); {
				EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false)); {
					foreach(var t in tags) {
						if (EditorGUILayout.ToggleLeft(t, coordinator.EnabledProcessorTags.Contains(t))) {
							if (!coordinator.EnabledProcessorTags.Contains(t)) {
								coordinator.EnabledProcessorTags.Add(t);
							}
						}
						else {
							if (coordinator.EnabledProcessorTags.Contains(t)) {
								coordinator.EnabledProcessorTags.Remove(t);
							}
						}
					}
				} EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(); {
					GUILayout.TextArea(
						string.Join(", ", 
							processorTypes
							.Select(type => new KeyValuePair<Type, ProcessorOptionsAttribute>(type, type.GetCustomAttributes(typeof(ProcessorOptionsAttribute), true)[0] as ProcessorOptionsAttribute))
								.Where(kvp => coordinator.EnabledProcessorTags.Contains(kvp.Value.Tag))
								.Select(kvp => kvp.Key.Name)
								.ToArray()
					));
				} EditorGUILayout.EndVertical();
			} EditorGUILayout.EndHorizontal();
		}
	}
}