// Script By Ottamind team

using System;
using UnityEngine;

namespace Ottamind.CarTemplate
{
	[Serializable]
	public struct CarKeyBinds
	{
		[Tooltip("Key for moving Forward")]
		public KeyCode ForwardKey;
		[Tooltip("Key for moving Forward")]
		public KeyCode BackwardKey;
		[Tooltip("Key for left turn")]
		public KeyCode LeftKey;
		[Tooltip("Key for right turn")]
		public KeyCode RightKey;
		[Tooltip("Key for Drift")]
		public KeyCode DriftKey;
	}

	[RequireComponent(typeof(Rigidbody))]
	public class CarMovement : MonoBehaviour
	{
		[Header("Car Properties")]
		[Tooltip("Hhow fast the car moves")]
		public float m_Acceleration = 8f;
		[Tooltip("The Max speed the car can move")]
		public float m_MaxSpeed = 20f;
		[Tooltip("How fast the car stops")]
		public float m_BrakingForce = 10f;
		[Tooltip("How fast the car should turn")]
		public float m_TurnSpeed = 60f;

		[Header("Drift Properties")]
		[Tooltip("How much drift")]
		public float m_DriftFactor = 1.5f;
		[Tooltip("How much angle to drift")]
		public float m_DriftAngle = 1.5f;
		[Tooltip("How much time to shift Angle")]
		public float m_DriftTime = 1.5f;

		[Header("KeyBinds")]
		public CarKeyBinds m_KeyBinds = new() 
			{ 
				ForwardKey = KeyCode.W,
				BackwardKey = KeyCode.S,
				LeftKey = KeyCode.A,
				RightKey = KeyCode.D,
				DriftKey = KeyCode.Space,
			};

		[Header("Reference")]
		[Tooltip("Parent object of the visual")]
		public Transform m_Visual;
		[Tooltip("Particle System child object")]
		public ParticleSystem m_ParticleSystem;
		private Rigidbody m_Rigidbody;
		
		private float m_CurrentSpeed = 0f;
		private int m_TurnDirection;
		private bool m_IsDrifting;
		private float m_TargetBodyAngle;
		
		void Start() 
		{ 
			m_Rigidbody = GetComponent<Rigidbody>(); 
			EndDrift();
		}

		void FixedUpdate() 
		{
			GetInput();
			HandleMovement();
			HandleSteering();
		}

		private void SteeringInput()
		{
			m_TurnDirection = 0;
			if (Input.GetKey(m_KeyBinds.LeftKey))
			{
				m_TurnDirection = -1;
			}
			else if (Input.GetKey(m_KeyBinds.RightKey))
			{
				m_TurnDirection = 1;
			}

			
			// Initiate Drifting
			if (Input.GetKeyDown(m_KeyBinds.DriftKey) && m_TurnDirection != 0 && !m_IsDrifting)
			{		
				m_IsDrifting = true;
				m_ParticleSystem.Play();
			}
			//Cancel drifting
			if (m_IsDrifting && m_TurnDirection == 0)
			{		
				EndDrift();
			}

			StartDrift();

			m_Visual.localEulerAngles = Vector3.up * Mathf.LerpAngle(m_TargetBodyAngle, m_Visual.localEulerAngles.y, m_DriftTime * Time.fixedDeltaTime);
		}
		private void StartDrift()
		{

			if (!m_Visual || !m_IsDrifting) return;

			m_TargetBodyAngle = m_DriftAngle * m_TurnDirection;
		}

		private void EndDrift()
		{
			m_IsDrifting = false;

			if (!m_Visual) return;

			m_TargetBodyAngle = 0;
			m_ParticleSystem.Stop();
		}


		private void GetInput()
		{
			MovementInput();
			SteeringInput();
		}

		private void MovementInput()
		{
			if (Input.GetKey(m_KeyBinds.ForwardKey))
			{
				m_CurrentSpeed += m_Acceleration * Time.fixedDeltaTime;
			}
			else if (Input.GetKey(m_KeyBinds.BackwardKey))
			{
				m_CurrentSpeed -= m_BrakingForce * Time.fixedDeltaTime;
			}
			else
			{
				// Slowing down
				m_CurrentSpeed = Mathf.Lerp(m_CurrentSpeed, 0, Time.fixedDeltaTime * 2);
			}
		}

		private void HandleMovement() 
		{
			m_CurrentSpeed = Mathf.Clamp(m_CurrentSpeed, -m_MaxSpeed, m_MaxSpeed); 
			Vector3 movement = m_CurrentSpeed * Time.fixedDeltaTime * transform.forward;
			m_Rigidbody.MovePosition(m_Rigidbody.position + movement); 
		}

		private void HandleSteering()
		{	
			float turn = m_TurnDirection * m_CurrentSpeed * m_TurnSpeed * (m_IsDrifting?m_DriftFactor:1f) * Time.fixedDeltaTime;
			Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
			m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
		}
	}
}
