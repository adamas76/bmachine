// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using BehaviourMachine;
using BehaviourMachineEditor;
using UnityEditor;
using UnityEngine;
using ws.winx.bmachine.extensions;
using ws.winx.editor.extensions;

namespace ws.winx.editor.bmachine.extensions
{

		
		/// <summary>
		/// Custom editor for the RandomChild node.
		/// <seealso cref="BehaviourMachine.RandomChild" />
		/// </summary>
		[CustomNodeEditor(typeof(GetInputNode), true)]
		public class GetInputNodeEditor : NodeEditor
		{
				/// <summary>
				/// The custom inspector.
				/// </summary>
				public override void OnInspectorGUI ()
				{
						

						

						//why is this line for???
//						if (Event.current.type == EventType.Layout) {
//								this.serializedNode.Update ();
//				
//						}

						bool isAnalogInput = false;
						bool isFullAxis = false;


						NodePropertyIterator iterator = this.serializedNode.GetIterator ();

						if (iterator.Find ("inputType")) {
							

								isAnalogInput = (GetInputNode.InputType)iterator.current.value == GetInputNode.InputType.GetInput;
						}

						if (isAnalogInput && iterator.Find ("fullAxis")) {

								isFullAxis = (bool)iterator.current.value;
						}


						iterator = this.serializedNode.GetIterator ();

						int indentLevel = EditorGUI.indentLevel;
						while (iterator.Next (iterator.current == null || (iterator.current.propertyType != NodePropertyType.Variable && !iterator.current.hideInInspector))) {
								SerializedNodeProperty current = iterator.current;
								
			
								if (!current.hideInInspector) {
										if (!isAnalogInput) {
												if (
								current.path == "sensitivity" || current.path == "dreadzone" || 
														current.path == "gravity" || 
														current.path == "multiplier" ||
														current.path == "variable" || 
														current.path == "fullAxis" ||
														current.path == "inputStateNeg"
								) {
														continue;
												}
										} else 
												if (!isFullAxis && current.path == "inputStateNeg")
												continue;
				  

										EditorGUI.indentLevel = indentLevel + iterator.depth;
										GUILayoutHelper.DrawNodeProperty (new GUIContent (current.label, current.tooltip), current, this.target, null, true);
								}
						}
						EditorGUI.indentLevel = indentLevel;


			this.serializedNode.ApplyModifiedProperties ();


					
				}


		}
}

