using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed = 5f;
	[SerializeField]
	private float lookSensitivity = 3;

	[SerializeField]
	private float thrusterForce = 1000f;

	[SerializeField]
	private float thrusterFuelBurnSpeed = 1f;
	[SerializeField]
	private float thrusterFuelRegenSpeed = 0.3f;
	private float thrusterFuelAmount = 1f;

	public float GetThrusterFuelAmount()
	{
		return thrusterFuelAmount;
	}

	[SerializeField]
	private LayerMask environmentMask;

	[Header("Spring Settings:")]
//	[SerializeField]
//	private JointDriveMode jointMode = JointDriveMode.Position;
	[SerializeField]
	private float jointSpring = 20f;
	[SerializeField]
	private float jointMaxForce = 40f;

	// Component caching
	private PlayerMotor motor;
	private ConfigurableJoint joint;
	private Animator animator;

	void Start () 
	{
		motor = GetComponent<PlayerMotor>();
		joint = GetComponent<ConfigurableJoint> ();
		animator = GetComponent<Animator> ();

		SetJointSettings (jointSpring);
	}

	void Update () 
	{
		if (PauseMenu.isOn) {
			if (Cursor.lockState != CursorLockMode.None)
				Cursor.lockState = CursorLockMode.None;

			motor.Move (Vector3.zero);
			motor.Rotate (Vector3.zero);
			motor.RotateCamera (0f);

			return;
		}

		if (Cursor.lockState != CursorLockMode.Locked)
			Cursor.lockState = CursorLockMode.Locked;

		// Setting target position for spring
		// This makes the physics act right when it comes to applying gravity when flying over objects
		RaycastHit _hit;
		if (Physics.Raycast (transform.position, Vector3.down, out _hit, 100f, environmentMask)) {
			joint.targetPosition = new Vector3 (0f, -(_hit.point.y + 1), 0f);
		} else {
			joint.targetPosition = new Vector3 (0f, 0f, 0f);
		}

		// Calculate movement velocity as a 3D vector
		float _xMov = Input.GetAxis("Horizontal");
		float _zMov = Input.GetAxis("Vertical");

		Vector3 movHorizontal = transform.right * _xMov;
		Vector3 movVertical = transform.forward * _zMov;

		Vector3 _velocity = (movHorizontal + movVertical) * speed;

		// Animate movement
		animator.SetFloat("ForwardVelocity", _zMov);

		motor.Move (_velocity);


		// Calculate rotation
		float _yRot = Input.GetAxisRaw ("Mouse X");

		Vector3 _rotation = new Vector3 (0f, _yRot, 0f) * lookSensitivity;

		motor.Rotate (_rotation);


		// Calculate camera rotation
		float _xRot = Input.GetAxisRaw ("Mouse Y");

		float _cameraRotationX = _xRot * lookSensitivity;

		motor.RotateCamera (_cameraRotationX);

		// Calculate thruster force
		Vector3 _thrusterForce = Vector3.zero;
		if (Input.GetButton ("Jump") && thrusterFuelAmount > 0f) {
			thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

			if (thrusterFuelAmount >= 0.01f) {
				_thrusterForce = Vector3.up * thrusterForce;
				SetJointSettings (0f);
			}
		} else {
			thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;

			SetJointSettings (jointSpring);
		}

		thrusterFuelAmount = Mathf.Clamp (thrusterFuelAmount, 0f, 1f);

		motor.ApplyThruster (_thrusterForce);
	}

	private void SetJointSettings(float _jointSpring) 
	{
		joint.yDrive = new JointDrive { 
		//	mode = jointMode, 
			positionSpring = _jointSpring, 
			maximumForce = jointMaxForce 
		};
	}

}
