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

namespace ws.winx.editor.bmachine.extensions
{
		
		public class SendEventNormalizedEditor:EditorWindow
		{
				private static SendEventNormalizedEditor window;
				private static SendEventNormalized node;
				NodeEditor editor;

				

				public static void Show(SendEventNormalized node, Rect position)
				{
								SendEventNormalizedEditor.node = node;
//
			if (SendEventNormalizedEditor.window != null)//restore last 
							position = window.position;

		
					   SendEventNormalizedEditor.window =(SendEventNormalizedEditor) EditorWindow.CreateInstance (typeof (SendEventNormalizedEditor));
					//(SendEventNormalizedEditor) EditorWindow.GetWindow(typeof(SendEventNormalizedEditor));
						SendEventNormalizedEditor.window.position = position;
						SendEventNormalizedEditor.window.ShowPopup ();
				}

				public static void Hide ()
				{
						if (window != null)
								window.Close ();
				}

				

				void OnGUI ()
				{
						// The actual window code goes here
						if (SendEventNormalizedEditor.node != null) {
								if (editor == null)
									editor = NodeEditor.CreateEditor(SendEventNormalizedEditor.node.GetType());
								if (editor.target != SendEventNormalizedEditor.node)//How to add target to editor??? or subclass the Node Editor
										editor.DrawNode (SendEventNormalizedEditor.node);
								else
										editor.OnInspectorGUI ();
						}
				
				}

		}
}

