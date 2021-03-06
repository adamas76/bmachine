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
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Animations;
using ws.winx.unity;
using AnimatorState=UnityEditor.Animations.AnimatorState;
using ws.winx.editor.extensions;

namespace ws.winx.editor.utilities
{
		public class MecanimUtility
		{

				static MecanimUtility ()
				{

						//add handler to modification of AnimatorController
						AssetPostProcessorEventDispatcher.Imported += MecanimUtility.onAssetsReImported;

				}
				
				private static Dictionary<AnimatorController,Dictionary<AnimatorState,int>> __animatorControllerStateLayerInx;
		
				public static Dictionary<AnimatorController,Dictionary<AnimatorState,int>>  animatorControllerStateLayerInx {
						get {
								if (__animatorControllerStateLayerInx == null)
										__animatorControllerStateLayerInx = new Dictionary<AnimatorController, Dictionary<AnimatorState, int>> ();
								return __animatorControllerStateLayerInx;
						}
				}

				private static Dictionary<AnimatorController,GUIContent[]> __animatorControllerDisplayOptions;

				public static Dictionary<AnimatorController, GUIContent[]> animatorControllerDisplayOptions {
						get {
								if (__animatorControllerDisplayOptions == null)
										__animatorControllerDisplayOptions = new Dictionary<AnimatorController, GUIContent[]> ();
								return __animatorControllerDisplayOptions;
						}
				}

				private static Dictionary<AnimatorController,AnimatorState[]> __animatorControllerAnimatorStates;

				public static Dictionary<AnimatorController, AnimatorState[]> animatorControllerAnimatorStates {
						get {
								if (__animatorControllerAnimatorStates == null)
										__animatorControllerAnimatorStates = new Dictionary<AnimatorController, AnimatorState[]> ();
								return __animatorControllerAnimatorStates;
						}
				}


				

				//private Dictionary<AnimatorController,List<AnimatorControllerState>> __controllerAnimationStates;
		
				//public int GetLayerInx(AnimatorState state){ return __controllerAnimationStates[0].Get
				//public AnimatorState

				/// <summary>
				/// Ons the assets re imported.
				/// </summary>
				/// <param name="importedAssetsPath">Imported assets path.</param>
				static void onAssetsReImported (string[] importedAssetsPath)
				{

						string path;
						AnimatorController aniController;
						int len = importedAssetsPath.Length;
			
						//check if the reimported asset is our aniController
						for (int i=0; i<len; i++) {
								path = importedAssetsPath [i];

								aniController = animatorControllerAnimatorStates.Keys.FirstOrDefault ((item) => System.IO.Path.GetFileNameWithoutExtension (path) == item.name);

								if (aniController != null) {
									
										processAnimatorController (aniController);
								}
						}	
						
					
			
				}
		
				/// <summary>
				/// Processes the state machine path inside hierarchy.
				/// </summary>
				/// <param name="stateMachine">State machine.</param>
				/// <param name="parentName">Parent name.</param>
				/// <param name="layer">Layer.</param>
				/// <param name="resultsAnimaInfoList">Results anima info list.</param>
				static void processStateMachinePath (AnimatorStateMachine stateMachine, string parentName, int layerInx, List<AnimatorState> animatorStateList, List<GUIContent> displayOptionsList,Dictionary<AnimatorState,int> animatorStateLayerIndex)
				{
						int numStates = 0;
						int numStateMachines = 0;
			
						int currentStateInx;
						int currentStateMachineInx;
						UnityEditor.Animations.AnimatorStateMachine currentStateMachine;
						string path;
			
						UnityEditor.Animations.AnimatorState state;
			
						numStates = stateMachine.states.Length;

		

			
						for (currentStateInx=0; currentStateInx<numStates; currentStateInx++) {


								state = stateMachine.states [currentStateInx].state;
						
								animatorStateLayerIndex.Add(state,layerInx);
								displayOptionsList.Add (new GUIContent (parentName + '/' + state.name));
								animatorStateList.Add (state);

				
						}
			
			
						numStateMachines = stateMachine.stateMachines.Length;
			
						if (numStateMachines > 0) {
								for (currentStateMachineInx=0; currentStateMachineInx<numStateMachines; currentStateMachineInx++) {
										currentStateMachine = stateMachine.stateMachines [currentStateMachineInx].stateMachine;
										path = parentName + "/" + currentStateMachine.name;
					
										processStateMachinePath (currentStateMachine, path, layerInx, animatorStateList, displayOptionsList,animatorStateLayerIndex);
					
								}
						} else if (numStates == 0) {
								//statesPathStringBuilder.Append (parentName).Append ("(Empty)|");
								//searchList.Add (new AnimaStateInfo(state.uniqueNameHash,new GUIContent (parentName+"(Empty)"),layer));
						}
			
				}

				public static AnimatorState[] GetAnimatorStates (AnimatorController aniController)
				{
						if (!animatorControllerAnimatorStates.ContainsKey (aniController)) {
								processAnimatorController (aniController);
							
							
							
						}

						return animatorControllerAnimatorStates [aniController];
				}

				public static GUIContent[] GetDisplayOptions (AnimatorController aniController)
				{
						if (!animatorControllerDisplayOptions.ContainsKey (aniController)) {
								processAnimatorController (aniController);
						
						
						}


						return animatorControllerDisplayOptions [aniController];
				}

				public static int GetLayerIndex (AnimatorController aniController, AnimatorState state)
				{

						if (!animatorControllerStateLayerInx.ContainsKey (aniController)) {
								processAnimatorController (aniController);
								
							
						}

							Dictionary<AnimatorState,int> dict;

								dict = animatorControllerStateLayerInx [aniController];

								if (dict != null && dict.ContainsKey (state)) {
										return dict [state];
								}

						return -1;
				}


				/// <summary>
				/// Processes the animator controller.
				/// </summary>
				/// <param name="aniController">Ani controller.</param>
				public static void processAnimatorController (AnimatorController aniController)
				{
						AnimatorControllerLayer layer;
			
			
						int numLayers = aniController.layers.Length;
			
			
						int currentLayerInx = 0;

						
			
			
						List<AnimatorState> animatorStateList = new List<AnimatorState> ();
						List<GUIContent> displayOptionsList = new List<GUIContent> ();
						Dictionary<AnimatorState,int> stateLayerInx = new Dictionary<AnimatorState, int> ();

						int numAnimatorStatesInLayer=0;

						//layersAnimatorStates = new AnimatorState[numLayers][];
			
			
						for (; currentLayerInx<numLayers; currentLayerInx++) {
								layer = aniController.layers [currentLayerInx];				                
								processStateMachinePath (layer.stateMachine, layer.name, currentLayerInx, animatorStateList, displayOptionsList,stateLayerInx);	
								numAnimatorStatesInLayer = animatorStateList.Count - numAnimatorStatesInLayer;
								

								


						}

						animatorControllerStateLayerInx [aniController] = stateLayerInx;
						animatorControllerAnimatorStates [aniController] = animatorStateList.ToArray ();
						animatorControllerDisplayOptions [aniController] = displayOptionsList.ToArray ();
			
						

			
				}
		}
}

