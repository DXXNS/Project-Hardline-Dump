using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace TrickShotAssets.CustomInputManager
{
	public class InputManagerStorage : MonoBehaviour
	{
		private XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<KeyMapping>), new XmlRootAttribute("KeyMappings"));

		public static InputManagerStorage Instance;

		[SerializeField]
		[XmlElement(ElementName = "KeyMappings")]
		public List<KeyMapping> keyMappings = new List<KeyMapping>();

		public string path = "config";

		public string configFileName = "inputs.cfg";

		public bool allowDuplicateKeys;

		private void Awake()
		{
			if (Instance == null)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				Instance = this;
			}
			else if (Instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void Start()
		{
			Directory.CreateDirectory(path);
			try
			{
				LoadFromXML();
			}
			catch (Exception)
			{
				SaveToXML();
			}
		}

		public int GetIndexByName(string inputName)
		{
			for (int i = 0; i < keyMappings.Count; i++)
			{
				if (keyMappings[i].inputName == inputName)
				{
					return i;
				}
			}
			return -1;
		}

		public KeyCode GetKeycodeByName(string inputName)
		{
			return keyMappings[GetIndexByName(inputName)].key;
		}

		public bool isKeyAlreadyMapped(KeyCode key)
		{
			for (int i = 0; i < keyMappings.Count; i++)
			{
				if (keyMappings[i].key == key)
				{
					return true;
				}
			}
			return false;
		}

		public void ResetAllKeyMappings()
		{
			for (int i = 0; i < keyMappings.Count; i++)
			{
				keyMappings[i].key = keyMappings[i].defaultKey;
			}
			UpdateAllKeyDisplays();
			SaveToXML();
		}

		public void SaveToXML()
		{
			Directory.CreateDirectory(path);
			FileStream fileStream = File.Create(path + "/" + configFileName);
			xmlSerializer.Serialize(fileStream, keyMappings);
			fileStream.Close();
		}

		public void LoadFromXML()
		{
			keyMappings = (List<KeyMapping>)xmlSerializer.Deserialize(new StreamReader(path + "/" + configFileName));
			UpdateAllKeyDisplays();
		}

		public void UpdateAllKeyDisplays()
		{
			KeyDetector[] array = UnityEngine.Object.FindObjectsOfType<KeyDetector>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateKeyDisplay();
			}
		}
	}
}
