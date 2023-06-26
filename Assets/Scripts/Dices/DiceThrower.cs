using Project.Input;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

namespace Project.Dice
{
	public class DiceThrower : MonoBehaviour, IInputReceiver
	{
		public event Action<int, int> OnDiceLanded;

		[SerializeField] private Camera _camera;

		[SerializeField] private Dice _firstDice;
		[SerializeField] private Dice _secondDice;

		[SerializeField] private float _followSpeed = 100f;
		[SerializeField] private float _throwForceMultiplier = 100f;
		[SerializeField] private float _maxRandomRotation = 60f;

		[SerializeField] private AudioClip throwSound;
		private AudioSource _audioSource;

		private PlayerInput _input;

		private Rigidbody _firstDiceRigidbody;
		private Rigidbody _secondDiceRigidbody;

		private Transform _firstDiceTransform;
		private Transform _secondDiceTransform;

		private Vector3 _mousePosition;
		private Vector3 _previousMousePosition;


		public void Initialize(PlayerInput input)
		{
			_input = input;

			SetDicesComponents();

			GetAudioSource();
		}

		public void StartDragging(Vector3 mousePosition, Camera camera)
		{
			_camera = camera;

			_firstDiceRigidbody.isKinematic = true;
			_secondDiceRigidbody.isKinematic = true;

			_input.LockControl();
		}

		public void StopDragging(Vector3 mousePosition, Camera camera)
		{
			_camera = camera;

			_firstDiceRigidbody.isKinematic = false;
			_secondDiceRigidbody.isKinematic = false;

			ApplyThrowForce();
		}

		public void OnDrag(Vector3 mousePosition, Camera camera)
		{
			_camera = camera;

			_previousMousePosition = _mousePosition;
			_mousePosition = mousePosition;

			Vector3 desiredPosition1 = GetDesiredPosition(_mousePosition.x - 100f);
			Vector3 desiredPosition2 = GetDesiredPosition(_mousePosition.x + 100f);
			_firstDiceTransform.position = LerpPosition(_firstDiceTransform.position, desiredPosition1);
			_secondDiceTransform.position = LerpPosition(_secondDiceTransform.position, desiredPosition2);
		}
		private IEnumerator WaitForDiceNumber()
		{
			yield return new WaitForSeconds(1.2f);

			OnDiceLanded?.Invoke(_firstDice.GetDiceNumber(), _secondDice.GetDiceNumber());
		}
		private void SetDicesComponents()
		{
			_firstDiceRigidbody = _firstDice.GetComponent<Rigidbody>();
			_secondDiceRigidbody = _secondDice.GetComponent<Rigidbody>();

			_firstDiceTransform = _firstDice.transform;
			_secondDiceTransform = _secondDice.transform;
		}
		private void GetAudioSource()
		{
			_audioSource = GetComponent<AudioSource>();
			if (_audioSource == null)
			{
				_audioSource = gameObject.AddComponent<AudioSource>();
			}
		}

		private Vector3 GetDesiredPosition(float xPosition)
		{
			return _camera.ScreenToWorldPoint(new Vector3(xPosition, _mousePosition.y, _camera.nearClipPlane + 0.1f));
		}

		private Vector3 LerpPosition(Vector3 current, Vector3 desired)
		{
			return Vector3.Lerp(current, desired, _followSpeed * Time.deltaTime);
		}

		private void ApplyThrowForce()
		{
			Vector3 throwDirection = CalculateThrowDirection();
			Vector3 forwardAxis = GetForwardAxis();
			Vector3 finalThrowDirection = throwDirection + forwardAxis;
			float throwForce = CalculateThrowForce(finalThrowDirection);

			ApplyForceToDice(finalThrowDirection, throwForce);

			ApplyRandomRotationToDice();

			PlayThrowSound();

			StartCoroutine(WaitForDiceNumber());
		}

		private Vector3 CalculateThrowDirection()
		{
			return (_mousePosition - _previousMousePosition).normalized;
		}

		private Vector3 GetForwardAxis()
		{
			Vector3 forwardAxis = _camera.transform.forward;
			forwardAxis.y = 0f;
			return forwardAxis.normalized;
		}

		private float CalculateThrowForce(Vector3 throwDirection)
		{
			return Mathf.Clamp((_mousePosition - _previousMousePosition).magnitude * _followSpeed * _throwForceMultiplier, 0f, 1.5f);
		}

		private void ApplyForceToDice(Vector3 direction, float force)
		{
			_firstDiceRigidbody.AddForce(direction * force, ForceMode.Impulse);
			_secondDiceRigidbody.AddForce(direction * force, ForceMode.Impulse);
		}

		private void ApplyRandomRotationToDice()
		{
			Vector3 randomRotation1 = GenerateRandomRotation() * 2f;
			Vector3 randomRotation2 = GenerateRandomRotation() * 2f;

			_firstDiceRigidbody.angularVelocity = randomRotation1;
			_secondDiceRigidbody.angularVelocity = randomRotation2;
		}

		private Vector3 GenerateRandomRotation()
		{
			return new Vector3(
				UnityEngine.Random.Range(-_maxRandomRotation, _maxRandomRotation),
				UnityEngine.Random.Range(-_maxRandomRotation, _maxRandomRotation),
				UnityEngine.Random.Range(-_maxRandomRotation, _maxRandomRotation)
			);
		}

		private void PlayThrowSound()
		{
			_audioSource.PlayOneShot(throwSound);
		}
	}

}
