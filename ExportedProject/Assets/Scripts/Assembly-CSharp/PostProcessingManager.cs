using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
	private static float postProcessInterpolate_kP = 0.3f;

	[SerializeField]
	private Volume normal;

	[SerializeField]
	private Volume hit;

	[SerializeField]
	private Volume heal;

	[SerializeField]
	private Volume dead;

	[SerializeField]
	private Volume insane;

	[SerializeField]
	private Volume suppressed;

	[SerializeField]
	private Volume menu;

	[SerializeField]
	private Volume focused;

	[SerializeField]
	private bool isDead;

	[SerializeField]
	private bool inMenu;

	private float hitRecoveryRate = 0.12f;

	private float suppressedRecoveryRate = 0.04f;

	private float healRecoveryRate = 0.09f;

	private float focusedStrength_kP = 0.1f;

	private float focusedStrengthExit_kP = 0.35f;

	private float minValueToRound = 0.01f;

	private static float hitEffectMultiplier = 0.35f;

	private static float healEffectMultiplier = 0.35f;

	private static float suppressedEffectMultiplier = 0.3f;

	private static float postProcessMinValue = 0.02f;

	private float normalStrength;

	[SerializeField]
	private float hitStrength;

	[SerializeField]
	private float deadStrength;

	[SerializeField]
	private float focusStrength;

	private float suppressedStrength;

	private float healStrength;

	private float menuStrength;

	private MotionBlur insaneMotionBlur;

	[Header("Insanity Effect Filter")]
	[SerializeField]
	private float insaneMotionBlurAmount;

	[SerializeField]
	private float insanityLevel;

	private float insaneWeight;

	private float insaneWeightTarget;

	[SerializeField]
	private float insaneWeight_kP;

	[SerializeField]
	private float insaneWeightPulseSpeed;

	[SerializeField]
	private float insaneWeightPulseAmount;

	public float InsanityLevel
	{
		get
		{
			return insanityLevel;
		}
		set
		{
			insanityLevel = value;
		}
	}

	public Volume Normal
	{
		get
		{
			return normal;
		}
		set
		{
			normal = value;
		}
	}

	public Volume Dead
	{
		get
		{
			return dead;
		}
		set
		{
			dead = value;
		}
	}

	public Volume Insane
	{
		get
		{
			return insane;
		}
		set
		{
			insane = value;
		}
	}

	public bool IsDead
	{
		get
		{
			return IsDead1;
		}
		set
		{
			IsDead1 = value;
		}
	}

	public float RecoveryRate
	{
		get
		{
			return hitRecoveryRate;
		}
		set
		{
			hitRecoveryRate = value;
		}
	}

	public float NormalStrength
	{
		get
		{
			return normalStrength;
		}
		set
		{
			normalStrength = value;
		}
	}

	public float HitStrength
	{
		get
		{
			return hitStrength;
		}
		set
		{
			hitStrength = value;
		}
	}

	public float DeadStrength
	{
		get
		{
			return deadStrength;
		}
		set
		{
			deadStrength = value;
		}
	}

	public MotionBlur InsaneMotionBlur
	{
		get
		{
			return insaneMotionBlur;
		}
		set
		{
			insaneMotionBlur = value;
		}
	}

	public float SuppressedStrength
	{
		get
		{
			return suppressedStrength;
		}
		set
		{
			suppressedStrength = value;
		}
	}

	public Volume Suppressed
	{
		get
		{
			return suppressed;
		}
		set
		{
			suppressed = value;
		}
	}

	public bool IsDead1
	{
		get
		{
			return isDead;
		}
		set
		{
			isDead = value;
		}
	}

	public bool InMenu
	{
		get
		{
			return inMenu;
		}
		set
		{
			inMenu = value;
		}
	}

	public float FocusStrength
	{
		get
		{
			return focusStrength;
		}
		set
		{
			focusStrength = value;
		}
	}

	private void FixedUpdate()
	{
		Normal.weight += (NormalStrength - Normal.weight) * postProcessInterpolate_kP;
		hit.weight += (HitStrength - hit.weight) * postProcessInterpolate_kP;
		Dead.weight += (DeadStrength - Dead.weight) * postProcessInterpolate_kP;
		Suppressed.weight += (SuppressedStrength - Suppressed.weight) * postProcessInterpolate_kP;
		heal.weight += (healStrength - heal.weight) * postProcessInterpolate_kP;
		menu.weight += (menuStrength - menu.weight) * postProcessInterpolate_kP;
		if (focused.weight < focusStrength)
		{
			focused.weight += (FocusStrength - focused.weight) * focusedStrength_kP;
		}
		else
		{
			focused.weight += (FocusStrength - focused.weight) * focusedStrengthExit_kP;
		}
		if (focused.weight <= postProcessMinValue)
		{
			focused.weight = 0f;
		}
		if (menu.weight <= postProcessMinValue)
		{
			menu.weight = 0f;
		}
		if (hit.weight < minValueToRound)
		{
			hit.weight = 0f;
		}
		if (dead.weight < minValueToRound)
		{
			dead.weight = 0f;
		}
		if (Suppressed.weight < minValueToRound)
		{
			Suppressed.weight = 0f;
		}
		if (heal.weight < minValueToRound)
		{
			heal.weight = 0f;
		}
		if (Normal.weight < minValueToRound)
		{
			Normal.weight = 0f;
		}
		if (!InMenu)
		{
			menuStrength = 0f;
			if (IsDead)
			{
				DeadStrength = 1f;
				HitStrength = 0f;
				NormalStrength = 0f;
				SuppressedStrength = 0f;
			}
			else
			{
				DeadStrength = 0f;
				NormalStrength = 1f - HitStrength - SuppressedStrength;
				HitStrength += (0f - HitStrength) * hitRecoveryRate;
				SuppressedStrength += (0f - SuppressedStrength) * suppressedRecoveryRate;
				healStrength += (0f - healStrength) * healRecoveryRate;
			}
		}
		else
		{
			DeadStrength = 0f;
			NormalStrength = 0f;
			HitStrength = 0f;
			suppressedStrength = 0f;
			healStrength = 0f;
			menuStrength = 1f;
		}
	}

	public void Hit(float strength)
	{
		HitStrength += strength * hitEffectMultiplier;
		HitStrength = Mathf.Clamp(HitStrength, 0f, 1.05f);
	}

	public void Suppress(float strength)
	{
		SuppressedStrength += strength * suppressedEffectMultiplier;
		SuppressedStrength = Mathf.Clamp(SuppressedStrength, 0f, 1.05f);
	}

	public void Heal(float strength)
	{
		healStrength += strength * 0.35f;
		healStrength = Mathf.Clamp(healStrength, 0f, 1.05f);
	}

	public void UpdateInsanityEffect()
	{
		Insane.profile.TryGet<MotionBlur>(out insaneMotionBlur);
		InsaneMotionBlur.intensity.value = insaneMotionBlurAmount;
		Insane.weight = insaneWeight;
		if (insanityLevel == 1f)
		{
			insaneWeightTarget = 0.2f;
		}
		if (insanityLevel == 2f)
		{
			insaneWeightTarget = 0.6f;
		}
		if (insanityLevel == 3f)
		{
			insaneWeightTarget = 1f;
		}
		if (insanityLevel > 0f)
		{
			insaneWeight += (insaneWeightTarget - insaneWeight) * insaneWeight_kP + Mathf.Sin(Time.time * insaneWeightPulseSpeed) * insaneWeightPulseAmount;
			insaneWeight = Mathf.Clamp(insaneWeight, 0f, 1f);
		}
		else
		{
			insaneWeight = 0f;
		}
	}
}
