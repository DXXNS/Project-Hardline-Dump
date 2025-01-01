using UnityEngine;

public class PIDController
{
	private float kP;

	private float kI;

	private float kD;

	private float errorAccumulation;

	private float errorDerivative;

	private float lastError;

	private float lastTime;

	public void Init()
	{
		lastTime = Time.time;
	}

	public PIDController(float kP, float kI, float kD)
	{
		this.kP = kP;
		this.kI = kI;
		this.kD = kD;
	}

	public float GetOutput(float error)
	{
		float num = Time.time - lastTime;
		lastTime = Time.time;
		if (num > 0f)
		{
			errorAccumulation += error * num;
			errorDerivative = (error - lastError) * num;
		}
		else
		{
			errorDerivative = 0f;
		}
		return error * kP + errorAccumulation * kI + errorDerivative * kD;
	}

	public float GetkP()
	{
		return kP;
	}

	public float GetkI()
	{
		return kI;
	}

	public float GetkD()
	{
		return kD;
	}
}
