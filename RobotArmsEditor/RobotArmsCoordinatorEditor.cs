using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace RobotArms {
	[CustomEditor(typeof(RobotArmsCoordinator))]
	public class RobotArmsCoordinatorEditor : Editor {

		Dictionary<Type, string> processorTags;

		public void OnEnable() {
			processorTags = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(
					assembly => assembly.GetTypes().Where(
						type => type.IsSubclassOf(typeof(RobotArmsProcessor))))
				.Where(t => !t.IsAbstract)
				.Aggregate(new Dictionary<Type, string>(), (dict, type) => {
					var attributes = type.GetCustomAttributes(typeof(ProcessorOptionsAttribute), true);

					dict[type] = attributes.Length > 0 
						? ((ProcessorOptionsAttribute)attributes[0]).Tag
						: RobotArmsCoordinator.DEFAULT_TAG;
					return dict;
				});
		}

		public override void OnInspectorGUI() {
			var coordinator = target as RobotArmsCoordinator;
			DrawDefaultInspector();
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal(); {
				EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false)); {
					EditorGUILayout.LabelField("Enabled processor tags");
					foreach(var tag in processorTags.Values.Distinct()) {
						if (EditorGUILayout.ToggleLeft(tag, coordinator.EnabledProcessorTags.Contains(tag))) {
							if (!coordinator.EnabledProcessorTags.Contains(tag)) {
								coordinator.EnabledProcessorTags.Add(tag);
							}
						}
						else {
							if (coordinator.EnabledProcessorTags.Contains(tag)) {
								coordinator.EnabledProcessorTags.Remove(tag);
							}
						}
					}
				} EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(); {
					EditorGUILayout.LabelField("Enabled processors");
					GUILayout.TextArea(
						string.Join("\n", 
							processorTags
							.Where(kvp => coordinator.EnabledProcessorTags.Contains(kvp.Value))
							.OrderBy(kvp => kvp.Key.Name)
							.Select(kvp => kvp.Key.Name)
							.ToArray()));
				} EditorGUILayout.EndVertical();
			} EditorGUILayout.EndHorizontal();
		}
	}
}