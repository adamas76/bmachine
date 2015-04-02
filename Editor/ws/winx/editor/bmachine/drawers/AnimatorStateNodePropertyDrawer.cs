using BehaviourMachine;
using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using ws.winx.bmachine.extensions;
using ws.winx.editor.extensions;
using System.Collections.Generic;
using BehaviourMachineEditor;
using System.Linq;
using ws.winx.unity;
using ws.winx.unity.attributes;

namespace ws.winx.editor.bmachine.drawers
{
		[CustomNodePropertyDrawer (typeof(AnimatorStateAttribute))]
		public class AnimatorStateNodePropertyDrawer : NodePropertyDrawer
		{

				GUIContent[] animatorStateDisplayOptions;
				AnimatorState[] animatorStateValues;
				AnimatorController aniController;
				AnimatorState animatorStateSelected;
				AnimatorState animatorStateSelectedPrev;
				UnityEngine.Motion motionSelected;
				SerializedNodeProperty animatorSerialized;



				//
				// Properties
				//
				public new AnimatorStateAttribute attribute {
						get {
						
								return  (AnimatorStateAttribute)base.attribute;
						}
				}


				//
				// Methods
				//

				
		

				/// <summary>
				/// Handles the onGUI event.
				/// </summary>
				/// <param name="property">Property.</param>
				/// <param name="node">Node.</param>
				/// <param name="guiContent">GUI content.</param>
				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{

						
					
						animatorStateSelected = property.value as AnimatorState;

						if (animatorSerialized == null) {
								NodePropertyIterator iter= property.serializedNode.GetIterator();
								iter.Find(attribute.animatorFieldName);
								animatorSerialized=iter.current;

								RuntimeAnimatorController runtimeContoller;
								
								runtimeContoller =( (Animator)animatorSerialized.value).runtimeAnimatorController;
								
								if (runtimeContoller is AnimatorOverrideController)
									aniController = ((AnimatorOverrideController)runtimeContoller).runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
								else
									aniController = runtimeContoller as UnityEditor.Animations.AnimatorController;
				


						}
					
						
				
						



						
								
								
						animatorStateDisplayOptions = MecanimUtility.GetDisplayOptions (aniController);
						animatorStateValues = MecanimUtility.GetAnimatorStates (aniController);

										
						

						animatorStateSelected = EditorGUILayoutEx.CustomObjectPopup (guiContent, animatorStateSelected, animatorStateDisplayOptions, animatorStateValues);//,compare);
						
						



						if (animatorStateSelectedPrev != animatorStateSelected) {
								property.value = animatorStateSelected;

								NodePropertyIterator iter= property.serializedNode.GetIterator();
								iter.Find(attribute.layerIndexFieldName);
								SerializedNodeProperty layerIndexSerialized=iter.current;
								
								layerIndexSerialized.value= MecanimUtility.GetLayerIndex (aniController, animatorStateSelected);
								layerIndexSerialized.ApplyModifiedValue();

								property.ApplyModifiedValue ();

								animatorStateSelectedPrev = animatorStateSelected;
						}



						if (animatorStateSelected.motion == null)
								Debug.LogError ("Selected state doesn't have Motion set");

				}



				
		

		}
}
