//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------
//----------------------------------------------
//           MecanimNode
// 			by Alex Winx
//----------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BehaviourMachine;
using Motion=UnityEngine.Motion;
using ws.winx.unity;
using ws.winx.unity.attributes;
using UnityEditor.Animations;
using System.Runtime.Serialization;
using UnityEditor;

namespace ws.winx.bmachine.extensions
{

		/// <summary>
		/// !!! MecaniNode goes KABOOM on new compilation if added not saved
		/// </summary>
		[NodeInfo ( category = "Extensions/Mecanim/", icon = "Animator", description ="Use Mecanima inside BTree")]
		public class MecanimNode:CompositeNode
		{
				

				public new BlackboardCustom blackboard {
						get{ return (BlackboardCustom)base.blackboard; }
				}

				[HideInInspector]
				public AnimationCurve[]
						curves;
				[HideInInspector]
				public Color[]
						curvesColors;
				[HideInInspector]
				public UnityVariable[]
						variablesBindedToCurves;
				[AnimatorStateAttribute("animator","layer")]
				public AnimatorState
						animatorStateSelected;
				[HideInInspector]
				public int
						layer;
				[UnityVariablePropertyAttribute(typeof(AnimationClip),"Motion Override")]
				public UnityVariable
						motionOverride;
				public bool loop = false;
				[UnityVariableProperty(typeof(float))]
				public UnityVariable
						blendX;
				[UnityVariableProperty(typeof(float))]
				public UnityVariable
						blendY;
				[RangeAttribute(0f,1f)]
				public float
						transitionDuration = 0.1f;
				[RangeAttribute(0f,1f)]
				//[HideInInspector]
				public float
						timeNormalizedStart = 0f;
				[HideInInspector]
				[RangeAttribute(0f,1f)]
				public float
						timeNormalizedCurrent = 0f;
				[HideInInspector]
				public float
						animationRunTimeControl = 0.5f;



				//[HideInInspector]
				public bool
						animationRunTimeControlEnabled = false;
				public float speed = 1f;
				public float weight = 1f;
				float normalizedTimeLast = 0f;
				int numBlendParamters;
				AnimatorStateInfo animatorStateInfoCurrent;
				AnimatorStateInfo animatorStateInfoNext;
				bool isCurrentEqualToSelectedAnimaInfo;
				bool isSelectedAnimaInfoInTransition;
				List<int> _treeInx;
				int _LastTickedChildren = -1;
				[HideInInspector]
				public Animator
						animator;
				AnimatorOverrideController _animatorOverrideController;

				public AnimatorOverrideController animatorOverrideController {
						get {
								if (_animatorOverrideController == null) {
										if (animator.runtimeAnimatorController is AnimatorOverrideController) {
												//animator.runtimeAnimatorController is already overrided just take reference
												_animatorOverrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
										} else {
												_animatorOverrideController = new AnimatorOverrideController ();

												//bind all clips from animator.runtimeAnimatorController to overrider
												_animatorOverrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
										}												
				

								}

								return _animatorOverrideController;
						}
				}

				public override void OnEnable ()
				{
						//	Debug.Log ("OnEnable");
						base.OnEnable ();
						//	animator = self.GetComponent<Animator> ();
					
				}

				public override void Remove (ActionNode child)
				{
						base.Remove (child);
				}

				public override bool Add (ActionNode child)
				{
						bool result = false;

						if (!(child is SendEventNormalized)) {
								Debug.LogWarning ("You can add only SendEventNormailized type of ActionCode");
								return false;
						}

						result = base.Add (child);

						



						return result;
				}

//		public override void ResetStatus ()
//		{
//			Debug.Log (selectedAnimaStateInfo.label.text + ">ResetStatus");
//			base.ResetStatus ();
//		}
		
				public override void Reset ()
				{
				
						animator = self.GetComponent<Animator> ();

						
						transitionDuration = 0f;

						curvesColors = new Color[0];
						curves = new AnimationCurve[0];
						variablesBindedToCurves = new UnityVariable[0];


						
						blendX = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
						blendX.Value = 0f;//make it float type
						

						blendY = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
						blendY.Value = 0f;//make it float type
					
						motionOverride = UnityVariable.CreateInstanceOf (typeof(AnimationClip));
	
				
						Array.ForEach (this.children, (itm) => {
			
								//remove localy from node parent
								this.Remove (itm);
								//remove from internal behaviour tree
								this.tree.RemoveNode (itm, false);
						});


						//this.tree.SaveNodes ();
			
				}
		
				public override void Awake ()
				{
						//	Debug.Log ("Awake");
						//animaStateInfoSelected1 = ((AnimatorController)animator.runtimeAnimatorController).
						//layers [0].stateMachine.states [0].state;

						if (animatorStateSelected != null) {
						
						}
			
				}


				//
//								public override void OnEnable ()
//								{
//							
//									//	Debug.Log (animaStateInfoSelected.label.text + ">Enable");
//				
//								}
				//		
				//				public override void OnDisable ()
				//				{
				//			
				//						Debug.Log (selectedAnimaStateInfo.label.text + ">Disable");
				//			
				//			
				//			
				//				}
		
//				public override void Start ()
//				{
//
////						Debug.Log (selectedAnimaStateInfo.label.text + ">Start MecanimNode");
//
//						_LastTickedChildren = -1;
//
//						PlayAnimaState ();
//
//				}



				/// <summary>
				/// Plaies the state of the mecanima.
				/// </summary>
				void PlayAnimaState ()
				{
			
						//if we are already in that AnimaState
						//cos normalied time doesn't stop but continue to increase
						if (isCurrentEqualToSelectedAnimaInfo) {
								//if(isSelectedAnimaInfoInTransition)
								//normalizedTimeLast =animatorStateInfoNext.normalizedTime
								//else
								normalizedTimeLast = animatorStateInfoCurrent.normalizedTime;
				
				
						} else {
				
				
				
								normalizedTimeLast = 0f;
				
				
						}
			
			
						//						Debug.Log (animaStateInfoSelected.label.text + ">Crosfade() "); 
						//						Debug.Log (animaStateInfoSelected.label.text + ">current state: " + animatorStateInfoCurrent.nameHash + " requested state " + animaStateInfoSelected.hash);
						//						Debug.Log (animaStateInfoSelected.label.text + ">current state time= " + animatorStateInfoCurrent.normalizedTime + " normalizedTimeStart= " + normalizedTimeStart);
						//						Debug.Log (animaStateInfoSelected.label.text + "> state in transition" + animator.GetAnimatorTransitionInfo (animaStateInfoSelected.layer).nameHash); 
						//			
			
			
						//		animator.Play (selectedAnimaStateInfo.hash, selectedAnimaStateInfo.layer, normalizedTimeStart);
						animator.CrossFade (animatorStateSelected.nameHash, transitionDuration, layer, timeNormalizedStart);
			
				}

				public override Status Update ()
				{
					
						

						if (animatorStateSelected == null)
								return Status.Failure;


						
						animatorStateInfoCurrent = animator.GetCurrentAnimatorStateInfo (layer);
			
						animatorStateInfoNext = animator.GetNextAnimatorStateInfo (layer);
			
						isSelectedAnimaInfoInTransition = animator.IsInTransition (layer) && (animatorStateInfoNext.shortNameHash == animatorStateSelected.nameHash);
			
						isCurrentEqualToSelectedAnimaInfo = animatorStateInfoCurrent.shortNameHash == animatorStateSelected.nameHash;



						///////////////////////  START  //////////////////////////

						//The second check ins't nessery if I could reset Status when this node is switched
						if (this.status != Status.Running) {
				
								
								if (motionOverride.Value != null && animatorOverrideController [(AnimationClip)animatorStateSelected.motion] != (AnimationClip)motionOverride.Value) {
					
					
												//	Debug.Log (this.name + ">Selected state Motion " + animaStateInfoSelected.motion + "to be overrided with " + motionOverride);
					
												animatorOverrideController [(AnimationClip)animatorStateSelected.motion] = (AnimationClip)motionOverride.Value;
					
												//	Debug.Log (this.name + ">Override result:" + animatorOverrideController [(AnimationClip)animaStateInfoSelected.motion] );
					
					
												//to avoid nesting 
												if (animator.runtimeAnimatorController is AnimatorOverrideController) {
														animator.runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;
												}
					
												//rebind back												
												animator.runtimeAnimatorController = animatorOverrideController;
										
					
								}
				
				
				
				
								animator.speed = this.speed;
				
								animator.SetLayerWeight (layer, this.weight);
				
								//this.Start ();	

								_LastTickedChildren = -1;
								
								PlayAnimaState ();
								////////////////////
								
								return Status.Running;
						}

						//////////////////////////////////////////////////////////////////////
					
				

						///////////////////  UPDATE  /////////////////////

						if (isCurrentEqualToSelectedAnimaInfo) {	




								if (loop)
										timeNormalizedCurrent = animatorStateInfoCurrent.normalizedTime - (int)animatorStateInfoCurrent.normalizedTime;
								else
										timeNormalizedCurrent = animatorStateInfoCurrent.normalizedTime;




//						Debug.Log (animaStateInfoSelected.label.text + ">Update() "); 
//						Debug.Log (animaStateInfoSelected.label.text + ">current state: " + animatorStateInfoCurrent.nameHash + " requested state " + animaStateInfoSelected.hash);
//						Debug.Log (animaStateInfoSelected.label.text + ">current state time= " + animatorStateInfoCurrent.normalizedTime + " normalizedTimeStart= " + normalizedTimeStart);
//						Debug.Log (animaStateInfoSelected.label.text + "> state in transition" + animator.GetAnimatorTransitionInfo (animaStateInfoSelected.layer).nameHash); 
//		

						

		
					
								if (isCurrentEqualToSelectedAnimaInfo
										&& normalizedTimeLast != timeNormalizedCurrent) {

										//Debug.Log ("NormalizedTime: " + (animatorStateInfoCurrent.normalizedTime));


							
							
										if (timeNormalizedCurrent > 1f) {
												if (!loop) {
														

														return Status.Success;
												}

										} else {

												//send event if its between previous and current time
												//Test only, event sending is done try SentEventNormalized
												//if (normalizedTimeLast < 0.67f && normalizedTimeCurrent >= 0.67f)
												//Debug.Log ("Event sent designated at 0.67 sent at:" + normalizedTimeCurrent);




												if (this.speed < 0f && timeNormalizedCurrent < 0f) {
														if (!loop) {
																

																return Status.Success;
												
														} else {
																animator.Play (animatorStateSelected.nameHash, layer, timeNormalizedStart);
																return Status.Running;
														}

														
												}

												////////   TICK CHILDREN ///////
												int len = children.Length;

												if (len > 0) {
														_LastTickedChildren++;

														//reset
														if (_LastTickedChildren >= len)
																_LastTickedChildren = 0;

														for (int i=_LastTickedChildren; i<len; i++) {
																children [i].OnTick ();
														}
												}

										}

							
										

										if (animatorStateSelected != null && animatorStateSelected.motion != null && animatorStateSelected.motion is BlendTree) {

												BlendTree blendTree = animatorStateSelected.motion as BlendTree;


												//TODO opmitize this to us Animator.StringToHash
												if (!String.IsNullOrEmpty (blendTree.blendParameter)) {
														animator.SetFloat (blendTree.blendParameter, (float)blendX.Value);

														if (!String.IsNullOrEmpty (blendTree.blendParameterY)) {
																animator.SetFloat (blendTree.blendParameterY, (float)blendY.Value);

														}
							
												} 

							
						
						
										}
					
					
										//Debug.Log (animaStateInfoSelected.label.text + ">Update at: " + timeNormalizedCurrent);	



										if (animationRunTimeControlEnabled) {
												animator.Update (animationRunTimeControl - animatorStateInfoCurrent.normalizedTime);

										}

										//Calculate curves values
										int numCurves = curves.Length;
										for (int j=0; j<numCurves; j++) 
												variablesBindedToCurves [j].Value = curves [j].Evaluate (timeNormalizedCurrent);

										normalizedTimeLast = timeNormalizedCurrent;



			
								} else {

//										Debug.LogWarning ("MecanimNode Update on wrong  AnimaState");
										//this.status = Status.Error;
										//return;
								
								}


						
						
								

						}


						return Status.Running;
						

				}

				public override void End ()
				{
						if (animatorStateSelected != null)
			   			//restore layer weight
								animator.SetLayerWeight (layer, 0);
				}





		}
}