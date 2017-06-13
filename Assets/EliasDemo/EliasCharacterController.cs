using System.Collections;
using UnityEngine;

public class EliasCharacterController : MonoBehaviour
{
	public Transform myCamera;
	public float movementSpeed;
	public float rotationSpeed;

	private void Update()
	{
		UpdateRotation();
		UpdateMovement();
	}

	private void UpdateMovement()
	{
		float deltaSpeed = movementSpeed * Time.deltaTime;
		transform.position += transform.forward * Input.GetAxis("Vertical") * deltaSpeed;
		transform.position += transform.right * Input.GetAxis("Horizontal") * deltaSpeed;
	}

	private void UpdateRotation()
	{
		float deltaRotation = rotationSpeed * Time.deltaTime;
		transform.forward = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * deltaRotation, transform.up) * transform.forward;
		myCamera.forward = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * deltaRotation, transform.right) * myCamera.forward;
		Vector3 angle = myCamera.transform.eulerAngles;
		if (angle.x > 50 && angle.x < 180)
		{
			angle.x = 50;
		}
		else if (angle.x < 300 && angle.x > 180)
		{
			angle.x = 300;	
		}
		myCamera.transform.eulerAngles = angle;
	}
}