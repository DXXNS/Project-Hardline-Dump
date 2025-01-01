using System;
using UnityEngine;
using UnityEngine.UI;

namespace TrickShotAssets.CustomInputManager
{
	public class KeyDetector : MonoBehaviour
	{
		private InputManagerStorage inputManagerStorage;

		private KeyCode currentKey;

		public string inputName;

		private bool detectKey;

		[Header("UI Elements")]
		public Text inputNameText;

		public Text keyCodeText;

		private void Start()
		{
			inputManagerStorage = InputManagerStorage.Instance;
			inputNameText.text = inputName;
			AssignKeyToInput(inputName, inputManagerStorage.GetKeycodeByName(inputName));
		}

		private void Update()
		{
			if (!detectKey)
			{
				return;
			}
			foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
			{
				if (Input.GetKey(value))
				{
					if (!inputManagerStorage.allowDuplicateKeys && inputManagerStorage.isKeyAlreadyMapped(value))
					{
						Debug.LogError("Key " + value.ToString() + " already in use!");
					}
					else if ((!inputManagerStorage.allowDuplicateKeys && !inputManagerStorage.isKeyAlreadyMapped(value)) || inputManagerStorage.allowDuplicateKeys)
					{
						currentKey = value;
						AssignKeyToInput(inputName, value);
						inputManagerStorage.SaveToXML();
					}
					detectKey = false;
				}
			}
		}

		public void UpdateKeyDisplay()
		{
			currentKey = inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName(inputName)].key;
			keyCodeText.text = currentKey.ToString();
		}

		public void AssignKeyToInput(string inputName, KeyCode newKey)
		{
			inputManagerStorage.keyMappings[inputManagerStorage.GetIndexByName(inputName)].key = newKey;
			keyCodeText.text = newKey.ToString();
		}

		public void ListenForButton()
		{
			detectKey = true;
		}
	}
}
