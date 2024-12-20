using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ottamind.CarTemplate
{

	public class CarControllerV2 : MonoBehaviour
	{
		[Header("Car Settings")]
		[Tooltip("Acceleration of the car")]
		public float acceleration = 15f;

		[Tooltip("Deceleration of the car")]
		public float deceleration = 10f;

		[Tooltip("Maximum speed of the car")]
		public float maxSpeed = 50f;

		[Tooltip("Turn speed of the car")]
		public float turnSpeed = 5f;

		[Tooltip("Sensitivity of the turning based on speed")]
		public float turnSensitivity = 0.1f;

		[Header("Drift Settings")]
		[Tooltip("Drift factor when the car is drifting")]
		public float driftFactor = 0.95f;

		private Rigidbody rb;
		private float currentSpeed = 0f;
		private float horizontalInput;
		private bool isDrifting = false;

		void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

		void Update()
		{
			horizontalInput = Input.GetAxis("Horizontal");
			HandleDrifting();
		}

		void FixedUpdate()
		{
			HandleMovement();
			HandleSteering();
		}

		private void HandleMovement()
		{
			if (Input.GetKey(KeyCode.W))
			{
				currentSpeed += acceleration * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.S))
			{
				currentSpeed -= deceleration * Time.deltaTime;
			}
			else
			{
				currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 2);
			}

			currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
			Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
			rb.MovePosition(rb.position + movement);
		}

		private void HandleSteering()
		{
			float turn = horizontalInput * turnSpeed * (1 - Mathf.Abs(currentSpeed) / maxSpeed) * Time.deltaTime;

			if (isDrifting)
			{
				turn *= driftFactor;
			}

			Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
			rb.MoveRotation(rb.rotation * turnRotation);
		}

		private void HandleDrifting()
		{
			if (Input.GetKey(KeyCode.Space))
			{
				isDrifting = true;
			}
			else
			{
				isDrifting = false;
			}
		}
	}
}