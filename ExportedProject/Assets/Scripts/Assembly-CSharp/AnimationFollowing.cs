using System;
using UnityEngine;

public class AnimationFollowing : MonoBehaviour
{
	private Transform slave;

	private Transform master;

	private Transform[] slaveTransforms;

	private Transform[] masterTransforms;

	private Transform[] slaveRigidTransforms;

	private Transform[] masterRigidTransforms;

	private Vector3[] rigidbodiesPosToCOM;

	private ConfigurableJoint[] slaveConfigurableJoints;

	private Quaternion[] startLocalRotation;

	private Quaternion[] localToJointSpace;

	private JointDrive jointDrive;

	private Vector3[] forceLastError;

	private int numOfRigids;

	[NonSerialized]
	public bool isAlive = true;

	[NonSerialized]
	public float forceCoefficient = 1f;

	[NonSerialized]
	public float torqueCoefficient = 1f;

	[Range(0f, 340f)]
	private float angularDrag;

	[Range(0f, 2f)]
	private float drag = 0.1f;

	[Range(0f, 1000f)]
	private float maxAngularVelocity = 1000f;

	[Range(0f, 10f)]
	private float jointDamping = 0.6f;

	[Tooltip("Proportional force of PID controller.")]
	[Range(0f, 160f)]
	public float PForce = 8f;

	[Tooltip("Derivative force of PID controller.")]
	[Range(0f, 0.064f)]
	public float DForce = 0.01f;

	[Range(0f, 100f)]
	public float maxForce = 10f;

	[Range(0f, 10000f)]
	public float maxJointTorque = 2000f;

	public bool useGravity = true;

	private float[] maxForceProfile = new float[22]
	{
		1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
		1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
		1f, 1f
	};

	private float[] maxJointTorqueProfile = new float[22]
	{
		1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
		1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
		1f, 1f
	};

	private float[] jointDampingProfile = new float[22]
	{
		1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
		1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
		1f, 1f
	};

	private bool[] limbProfile = new bool[22]
	{
		true, true, true, true, true, true, true, true, true, true,
		true, true, true, true, true, true, true, true, true, true,
		true, true
	};

	private void Start()
	{
		HumanoidSetUp componentInParent = GetComponentInParent<HumanoidSetUp>();
		master = componentInParent.masterRoot;
		slave = componentInParent.slaveRoot;
		slaveTransforms = slave.GetComponentsInChildren<Transform>();
		masterTransforms = master.GetComponentsInChildren<Transform>();
		if (masterTransforms.Length != slaveTransforms.Length)
		{
			Debug.LogWarning("Master transform count does not equal slave transform count.\n");
			return;
		}
		numOfRigids = slave.GetComponentsInChildren<Rigidbody>().Length;
		slaveRigidTransforms = new Transform[numOfRigids];
		masterRigidTransforms = new Transform[numOfRigids];
		rigidbodiesPosToCOM = new Vector3[numOfRigids];
		slaveConfigurableJoints = new ConfigurableJoint[numOfRigids];
		startLocalRotation = new Quaternion[numOfRigids];
		localToJointSpace = new Quaternion[numOfRigids];
		forceLastError = new Vector3[numOfRigids];
		int num = 0;
		int num2 = 0;
		Transform[] array = slaveTransforms;
		foreach (Transform transform in array)
		{
			if (transform.GetComponent<Rigidbody>() != null)
			{
				slaveRigidTransforms[num] = transform;
				masterRigidTransforms[num] = masterTransforms[num2];
				rigidbodiesPosToCOM[num] = Quaternion.Inverse(transform.rotation) * (transform.GetComponent<Rigidbody>().worldCenterOfMass - transform.position);
				ConfigurableJoint component = transform.GetComponent<ConfigurableJoint>();
				if (component != null)
				{
					slaveConfigurableJoints[num] = component;
					Vector3 forward = Vector3.Cross(component.axis, component.secondaryAxis);
					Vector3 secondaryAxis = component.secondaryAxis;
					localToJointSpace[num] = Quaternion.LookRotation(forward, secondaryAxis);
					startLocalRotation[num] = transform.localRotation * localToJointSpace[num];
					jointDrive = component.slerpDrive;
					component.slerpDrive = jointDrive;
				}
				else if (num != 0)
				{
					Debug.LogWarning("Rigidbody " + transform?.ToString() + " doesn't have configurable joint\n");
					return;
				}
				num++;
				transform.gameObject.AddComponent<CollisionDetector>();
			}
			num2++;
		}
		array = slaveRigidTransforms;
		foreach (Transform obj in array)
		{
			obj.GetComponent<Rigidbody>().useGravity = useGravity;
			obj.GetComponent<Rigidbody>().angularDrag = angularDrag;
			obj.GetComponent<Rigidbody>().drag = drag;
			obj.GetComponent<Rigidbody>().maxAngularVelocity = maxAngularVelocity;
		}
		EnableJointLimits(jointLimits: false);
	}

	public void FollowAnimation()
	{
		if (!isAlive)
		{
			SetJointTorque(0f, 0f);
			return;
		}
		SetJointTorque(maxJointTorque, jointDamping);
		for (int i = 0; i < slaveRigidTransforms.Length; i++)
		{
			if (limbProfile[i])
			{
				Rigidbody component = slaveRigidTransforms[i].GetComponent<Rigidbody>();
				component.angularDrag = angularDrag;
				component.drag = drag;
				component.maxAngularVelocity = maxAngularVelocity;
				component.useGravity = useGravity;
				Vector3 error = masterRigidTransforms[i].position + masterRigidTransforms[i].rotation * rigidbodiesPosToCOM[i] - component.worldCenterOfMass;
				Vector3 vector = PDControl(PForce, DForce, error, ref forceLastError[i]);
				vector = Vector3.ClampMagnitude(vector, maxForce * maxForceProfile[i] * forceCoefficient);
				component.AddForce(vector, ForceMode.VelocityChange);
				if (i != 0)
				{
					slaveConfigurableJoints[i].targetRotation = Quaternion.Inverse(localToJointSpace[i]) * Quaternion.Inverse(masterRigidTransforms[i].localRotation) * startLocalRotation[i];
				}
			}
		}
	}

	private Vector3 PDControl(float P, float D, Vector3 error, ref Vector3 lastError)
	{
		Vector3 result = P * (error + D * (error - lastError) / Time.fixedDeltaTime);
		lastError = error;
		return result;
	}

	private void SetJointTorque(float positionSpring, float positionDamper)
	{
		for (int i = 1; i < slaveConfigurableJoints.Length; i++)
		{
			if (limbProfile[i])
			{
				jointDrive.positionSpring = positionSpring * maxJointTorqueProfile[i] * torqueCoefficient;
				jointDrive.positionDamper = positionDamper * jointDampingProfile[i];
				slaveConfigurableJoints[i].slerpDrive = jointDrive;
			}
		}
	}

	private void EnableJointLimits(bool jointLimits)
	{
		for (int i = 1; i < slaveConfigurableJoints.Length; i++)
		{
			if (jointLimits)
			{
				slaveConfigurableJoints[i].angularXMotion = ConfigurableJointMotion.Limited;
				slaveConfigurableJoints[i].angularYMotion = ConfigurableJointMotion.Limited;
				slaveConfigurableJoints[i].angularZMotion = ConfigurableJointMotion.Limited;
			}
			else
			{
				slaveConfigurableJoints[i].angularXMotion = ConfigurableJointMotion.Free;
				slaveConfigurableJoints[i].angularYMotion = ConfigurableJointMotion.Free;
				slaveConfigurableJoints[i].angularZMotion = ConfigurableJointMotion.Free;
			}
		}
	}
}
