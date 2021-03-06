using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;


namespace ws.winx.unity.sequence
{
	
	
	//TODO make SequenceNode implements INode and AnimationNode,AudioNode,VideoNode...
	[System.Serializable]
	public class SequenceNode:ScriptableObject
	{
		
		
		
		//[HideInInspector]
		public SequenceNodeEvent
			onStart = new SequenceNodeEvent ();
		[HideInInspector]
		public SequenceNodeEvent
			onStop = new SequenceNodeEvent ();
		[HideInInspector]
		public SequenceNodeEvent
			onPause = new SequenceNodeEvent ();
		[HideInInspector]
		public SequenceNodeEvent
			onUpdate = new SequenceNodeEvent ();
		public int index = -1;
		public float transition = 0f;
		public bool loop;
		public float volume = 1f;
		public int layerIndex;
		public float speed = 1f;
		
		
		/// <summary>
		/// The start time in Frames
		/// </summary>
		public float timeStart;
		float _timeLocal;
		float _timeNormalized;
		float[] _particleEmitTimeCurrent;
		float[] _particleEmitTime;
		ParticleSystem[] _particleSystems;
		IAnimatedValues _animatedValues;
		
		
		
		
		//TODO use them for resize
		public float startOffset;
		public float endOffset;
		public EditorClipBinding clipBinding;
		
		/// <summary>
		/// The duration in [s] 
		/// </summary>
		float _duration = float.NaN;
		
		public float duration {
			get {
				
				
				
				//return _duration;
				if (source is AnimationClip) {
					
					
					
					_duration = (source as AnimationClip).length;
					
				} else if (source is AudioClip) {
					
					_duration = (source as AudioClip).length;
				} 
				#if !UNITY_IOS && !UNITY_ANDROID
				else if (source is MovieTexture) {
					
					_duration = (source as MovieTexture).duration;
					
					
				} 
				#endif
				else if (source is ParticleSystem) {
					
					//enable update in editor or if not initialized
					if (!Application.isPlaying || float.IsNaN (_duration)) {
						ParticleSystem particleSystemCurrent = (source as ParticleSystem);
						
						Transform ts = particleSystemCurrent.transform;
						_particleSystems = ts.GetComponentsInChildren<ParticleSystem> ();
						
						int particleSystemsNum = _particleSystems.Length;
						_particleEmitTime = new float[particleSystemsNum];
						_particleEmitTimeCurrent = new float[particleSystemsNum];
						
						float maxDuration = 0f;
						float particleSystemCurrentDuration = 0f;
						for (int i=0; i<particleSystemsNum; i++) {
							
							//reuse particleSystemRoot variable
							particleSystemCurrent = _particleSystems [i];
							_particleEmitTime [i] = 1f / particleSystemCurrent.emissionRate;
							particleSystemCurrent.loop = false;
							//particleSystemRoot.enableEmission = false;
							particleSystemCurrent.playOnAwake = false;
							particleSystemCurrent.Stop ();
							particleSystemCurrentDuration = particleSystemCurrent.duration + (particleSystemCurrent.startDelay + particleSystemCurrent.startLifetime);
							if (particleSystemCurrentDuration > maxDuration)
								maxDuration = particleSystemCurrentDuration;
						}
						_duration = maxDuration;
					}
					
				}
				
				float frameRate = this.channel.sequence.frameRate;
				
				_duration = Mathf.Round (_duration * frameRate) / frameRate;
				
				return _duration;
			}
			
		}
		
		//public int channelOrd;
		public SequenceChannel channel;
		bool _isRunning;
		
		public bool isRunning {
			get {
				return _isRunning;
			}
		}
		
		public UnityEngine.Object source;
		public int stateNameHash;
		
		void Emit (float time)
		{
			int particelSystemsNum = _particleSystems.Length;
			ParticleSystem particleSystemCurrent = null;
			for (int i=0; i<particelSystemsNum; i++) {
				particleSystemCurrent = _particleSystems [i];
				
				if (time < particleSystemCurrent.duration + particleSystemCurrent.startDelay + particleSystemCurrent.startLifetime && _particleEmitTimeCurrent [i] < time) {				
					//Debug.Log("time:"+time+" emit>"+_particleEmitTimeCurrent[i]);		
					_particleSystems [i].Emit (1);
					_particleEmitTimeCurrent [i] += _particleEmitTime [i];//increase the emit time (ex. rate=1[sec]/10particles => every 0.1s emit
				}
			}
		}
		
		
		
		/// <summary>
		/// Starts the node.
		/// </summary>
		public void StartNode ()
		{
			
			//Debug.Log ("StartNode " + source.name);
			
			
			
			GameObject target = channel.target;
			
			_isRunning = true;
			
			if (target != null) {
				if (Application.isPlaying) {
					if (source is AudioClip) {
						
						
						AudioSource audioSource = target.GetComponent<AudioSource> ();
						
						if (audioSource == null)
							audioSource = (AudioSource)target.AddComponent (typeof(AudioSource));
						
						audioSource.playOnAwake = false;
						audioSource.clip = source as AudioClip;
						
						audioSource.time = _timeLocal;
						audioSource.volume = volume;
						audioSource.Play ();
						
						
						
					} 
					#if !UNITY_IOS && !UNITY_ANDROID
					
					else if (source is MovieTexture) {
						MovieTexture movieTexture = (source as MovieTexture);
						if (target.tag == "MainCamera") {//==Camera.main
							RenderSettings.skybox.mainTexture = movieTexture;
						} else {
							
							Renderer renderer = channel.target.GetComponent<Renderer> ();
							if (renderer != null) {
								
								
								movieTexture.Stop ();
								
								
								renderer.material.mainTexture = movieTexture;
								
							} else
								Debug.LogWarning ("SequenceNode>Missing Renderer to render MovieTexture on target " + target.name);
						}
						
						
						AudioSource audioSource = null;
						if (movieTexture.audioClip != null) {
							
							audioSource = target.GetComponent<AudioSource> ();
							
							if (audioSource == null)
								audioSource = (AudioSource)target.AddComponent (typeof(AudioSource));
							
							audioSource.clip = movieTexture.audioClip;
							audioSource.playOnAwake = false;
							//audioSource.PlayOneShot (audioSource.clip);
							audioSource.Play ();
							
							
						}
						
						
						
						movieTexture.Play ();
						
						
						
						
						
						
						
						
					} 
					#endif
					else if (source is  AnimationClip) {
						
						_animatedValues = target.GetComponent<IAnimatedValues> ();
						
						
						
						
						Animator animator = target.GetComponent<Animator> ();
						
						if (animator == null) {
							animator = target.AddComponent<Animator> ();
						}
						animator.runtimeAnimatorController = this.channel.runtimeAnimatorController;
						animator.enabled = true;
						
						animator.speed = speed;
						
						//Debug.Log("time normalized"+_timeNormalized);
						//Animator.speed can only be negative when Animator recorder is enabled. Animator.recorderMode != AnimatorRecorderMode.Offline
						
						//change direction (from _timeNormalized to 0f or from _timeNormalized to 1f)
						if (!channel.sequence.playForward) {
							animator.SetFloat ("speedMultiplier", -1);
							//animator.speed = -1f;//from U5.1 this not possible
						} else {
							
							//animator.speed = 1f;
							animator.SetFloat ("speedMultiplier", 1);
						}
						
						if (transition > 0) {
							//	Debug.Log ("Crossfade " + this.name);
							animator.CrossFade (stateNameHash, transition, layerIndex, _timeNormalized);
						} else {
							Debug.Log ("Play " + this.name);
							animator.Play (stateNameHash, layerIndex, _timeNormalized);
							
						}
						
						
						
						
						
					} else if (source is ParticleSystem) {
						
						
						ParticleSystem particalSystem = (source as ParticleSystem);
						
						if (!particalSystem.isPlaying) {
							
							//int d=this.duration;
							particalSystem.Stop ();
							
							//Emit (0f);
							//(source as ParticleSystem).Play();//doens't work
							
							
							Debug.Log ("try to run particles");
						}
					}
					
					
				}
				
				
				
				
				onStart.Invoke (this);
				
			} else {
				Debug.LogWarning ("No target set on channel " + this.channel.name);
			}
			
			
			
			
		}
		
		public void Stop ()
		{
			GameObject target = channel.target;
			
			_isRunning = false;
			
			
			//	Debug.Log ("StopNode " + source.name + " at time:" + this.channel.sequence.timeCurrent + " timeNormalized:" + (this.channel.target.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (layerIndex).normalizedTime));
			
			
			
			if (target != null) {
				if (Application.isPlaying) {
					if (source is AudioClip) {
						AudioSource audioSource = target.GetComponent<AudioSource> ();
						
						if (audioSource != null)
							audioSource.Stop (); 
						
					} 
					#if !UNITY_IOS && !UNITY_ANDROID
					else if (source is MovieTexture) {
						Renderer renderer = target.GetComponent<Renderer> ();
						if (renderer != null) {
							MovieTexture movieTexture = (source as MovieTexture);
							
							if (_timeLocal > 0) {
								movieTexture.Pause ();
								Debug.Log ("SequenceNode>Video paused");
							} else {
								movieTexture.Stop ();
								Debug.Log ("SequenceNode>Video stopped");
							}
							//
							AudioSource audioSource = null;
							if (movieTexture.audioClip != null && (audioSource = target.GetComponent<AudioSource> ()) != null)
								audioSource.Stop ();
							
						} else
							Debug.LogWarning ("SequenceNode>Missing Renderer to render MovieTexture on target " + target.name);
					} 
					#endif
					else if (source is AnimationClip) {
						
						
						
						
						//hard stop
						Animator animator = target.GetComponent<Animator> ();
						
						//!!!! Warnning that 2 or more Animation channels can't share same target 
						//if (animator != null && this.index + 1 == this.channel.nodes.Count)
						animator.enabled = false;
						
						
						
						
						
					} else if (source is ParticleSystem) {
						int particleSystemNum = _particleSystems.Length;
						for (int i=0; i<particleSystemNum; i++) {
							_particleSystems [i].Stop ();
							_particleEmitTimeCurrent [i] = _particleEmitTime [i];
						}
						//(source as ParticleSystem).Stop();
					}
					
					
				}
			}
			
			
			
			
			
			onStop.Invoke (this);
			
		}
		
		public virtual void DoUpdate ()
		{
			if (onUpdate != null)
				onUpdate.Invoke (this);
			
			
			
			//	Debug.Log ("DoUpdate " + source.name + " at time:" + this.channel.sequence.timeCurrent + " timeNormalized:" + (this.channel.target.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (layerIndex).normalizedTime));
			
			
			
			//			if (_animatedValues != null) {
			//
			//				_animatedValues.UpdateValues ();
			//
			//
			//			}
			
			
			if (source is ParticleSystem) {
				
				Emit (_timeLocal);
				
			}
			
			
		}
		
		public void LateUpdateNode (double timeCurrent)
		{
			
			if(channel.type==SequenceChannel.SequenceChannelType.Animation && channel.target!=null)
			if (_animatedValues!=null || (_animatedValues = channel.target.GetComponent<IAnimatedValues> ()) != null) {
				
				_animatedValues.UpdateValues ();
				
				
			}		
			
		}
		
		public void GoTO (double time)
		{
			if (channel.type == SequenceChannel.SequenceChannelType.Animation
			    && channel.target != null) {
				Animator ani = channel.target.GetComponent<Animator> ();
				ani.enabled = true;
				
				
				_timeLocal = ((float)time - timeStart);
				_timeNormalized = _timeLocal / duration;
				
				ani.Play (stateNameHash, layerIndex, _timeNormalized);
				ani.speed = 0f;
				
				
				
				
			}
			
			
			
		}
		
		public void UpdateNode (double time)
		{
			_timeLocal = ((float)time - timeStart);
			_timeNormalized = (_timeLocal / duration);
			
			
			
			
			
			// 
			if (_isRunning) {
				if (_timeNormalized < 0.0f || _timeNormalized > 1.0f) 
					Stop ();
				else
					DoUpdate ();
			} else {
				if (_timeNormalized >= 0.0f && _timeNormalized <= 1.0f) {
					StartNode ();
				}
				
			}
			
			
			
		}
		
		
		
		
	}
}