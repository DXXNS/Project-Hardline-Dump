using System.Collections.Generic;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatHandler : NetworkBehaviour
{
	private static string[] BANNED_WORDS;

	private static string REPLACE_BANNED_MESSAGE;

	[SerializeField]
	private GameObject channelBox;

	[SerializeField]
	private Text channelText;

	[SerializeField]
	private bool useChannelBox;

	public static bool chatEnabled;

	private static float messageHideDelay;

	private float timeToHide;

	private float currentTime;

	private static float messageSpacing;

	private int maxMessages = 15;

	[SerializeField]
	private string playerName;

	[SerializeField]
	private Player player;

	private int team;

	private bool sendAll = true;

	[SerializeField]
	private MessageBox messageTemplate;

	[SerializeField]
	private InputField inputField;

	[SerializeField]
	private GameObject content;

	[SerializeField]
	private GameObject scrollView;

	[SerializeField]
	private bool hideAfterTime;

	private List<GameObject> messageBoxes = new List<GameObject>();

	public InputField InputField
	{
		get
		{
			return inputField;
		}
		set
		{
			inputField = value;
		}
	}

	private void Start()
	{
		if (PlayerPrefs.GetInt("Gameplay_ToggleChat", 1) == 1)
		{
			chatEnabled = true;
		}
		else
		{
			chatEnabled = false;
		}
		GetPlayerInfo();
	}

	public void GetPlayerInfo()
	{
		NetworkRoomPlayer[] array = Object.FindObjectsOfType<NetworkRoomPlayer>();
		foreach (NetworkRoomPlayer networkRoomPlayer in array)
		{
			if (networkRoomPlayer.hasAuthority)
			{
				playerName = "[" + networkRoomPlayer.PlayerName + "]";
			}
		}
		Player[] array2 = Object.FindObjectsOfType<Player>();
		foreach (Player player in array2)
		{
			if (player.hasAuthority)
			{
				playerName = "[" + player.HumanName + "]";
				this.player = player;
			}
		}
	}

	private void Update()
	{
		if (chatEnabled)
		{
			inputField.gameObject.SetActive(value: true);
			RunChatUpdate();
		}
		else
		{
			inputField.gameObject.SetActive(value: false);
			channelBox.SetActive(value: false);
			scrollView.SetActive(value: false);
		}
	}

	private void RunChatUpdate()
	{
		if (hideAfterTime)
		{
			UpdateChatScrollVisibility();
		}
		if (InputField.isFocused)
		{
			timeToHide = Time.time + messageHideDelay;
			if (useChannelBox)
			{
				channelBox.SetActive(value: true);
				if (sendAll)
				{
					channelText.text = "[ALL]";
				}
				else
				{
					channelText.text = "[TEAM]";
				}
			}
			else
			{
				channelBox.SetActive(value: false);
			}
		}
		else
		{
			channelBox.SetActive(value: false);
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			if (!InputField.isFocused)
			{
				sendAll = true;
				InputField.Select();
			}
		}
		else if (Input.GetKeyDown(KeyCode.Y) && !InputField.isFocused)
		{
			sendAll = false;
			InputField.Select();
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			EventSystem.current.SetSelectedGameObject(null);
			if (InputField.text != "")
			{
				MonoBehaviour.print("SEND");
				EventSystem.current.SetSelectedGameObject(null);
				if ((bool)player)
				{
					int channel = 0;
					if (!sendAll)
					{
						channel = 1;
					}
					ServerSendTextMessage(playerName, player.Team, channel, InputField.text);
				}
				else
				{
					ServerSendTextMessage(playerName, 0, 0, InputField.text);
				}
				InputField.text = "";
			}
		}
		if (Input.GetKey(KeyCode.Escape))
		{
			InputField.text = "";
		}
	}

	private void UpdateChatScrollVisibility()
	{
		currentTime = Time.time;
		if (currentTime > timeToHide)
		{
			scrollView.SetActive(value: false);
		}
		else
		{
			scrollView.SetActive(value: true);
		}
	}

	private void ServerSendTextMessage(string player, int team, int channel, string message)
	{
		if (base.isServer)
		{
			SendTextMessage(player, team, channel, message);
			RpcSendTextMessage(player, team, channel, message);
		}
		else
		{
			CmdSendTextMessage(player, team, channel, message);
		}
	}

	private void SendTextMessage(string player, int team, int channel, string message)
	{
		bool flag = false;
		if (channel == 0)
		{
			flag = true;
		}
		else if (this.player != null && this.player.Team == team)
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		timeToHide = Time.time + messageHideDelay;
		GameObject gameObject = Object.Instantiate(messageTemplate.gameObject, messageTemplate.transform.position, base.transform.rotation);
		gameObject.transform.parent = content.transform;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		messageBoxes.Insert(0, gameObject);
		gameObject.gameObject.SetActive(value: true);
		bool flag2 = true;
		for (int i = 0; i < BANNED_WORDS.Length; i++)
		{
			string[] array = message.Split(' ');
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].ToLower() == BANNED_WORDS[i].ToLower())
				{
					flag2 = false;
				}
			}
		}
		if (flag2)
		{
			gameObject.GetComponent<MessageBox>().SetMessage(player, team, channel, message);
		}
		else
		{
			gameObject.GetComponent<MessageBox>().SetMessage(player, team, channel, REPLACE_BANNED_MESSAGE);
		}
		ResetMessagePositions();
	}

	private void ResetMessagePositions()
	{
		for (int i = 0; i < messageBoxes.Count; i++)
		{
			messageBoxes[i].transform.localPosition = messageTemplate.transform.localPosition + new Vector3(0f, messageSpacing * (float)i, 0f);
			if (i > maxMessages)
			{
				GameObject obj = messageBoxes[i];
				messageBoxes.RemoveAt(i);
				Object.Destroy(obj);
			}
		}
	}

	[Command(requiresAuthority = false)]
	public void CmdSendTextMessage(string player, int team, int channel, string message)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(player);
		writer.WriteInt(team);
		writer.WriteInt(channel);
		writer.WriteString(message);
		SendCommandInternal(typeof(ChatHandler), "CmdSendTextMessage", writer, 0, requiresAuthority: false);
		NetworkWriterPool.Recycle(writer);
	}

	[ClientRpc]
	public void RpcSendTextMessage(string player, int team, int channel, string message)
	{
		PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
		writer.WriteString(player);
		writer.WriteInt(team);
		writer.WriteInt(channel);
		writer.WriteString(message);
		SendRPCInternal(typeof(ChatHandler), "RpcSendTextMessage", writer, 0, includeOwner: true);
		NetworkWriterPool.Recycle(writer);
	}

	static ChatHandler()
	{
		BANNED_WORDS = new string[611]
		{
			"Homosexual", "Homophobic", "Racist", "Gay", "Lgbt", "Jew", "Jewish", "Anti-semitic", "Chink", "Muslims",
			"Muslim", "Isis", "Islamophobe", "homophobe ", "Bombing", "Sexyhot", "Bastard", "Bitch", "Fucker", "Cunt",
			"Damn", "Fuck", "Shit", "Motherfucker", "Nigga", "Nigger", "Prick", "Shit", "shit ass", "Shitass",
			"son of a bitch", "Whore", "Thot", "Slut", "Faggot", "Dick", "Pussy", "Penis", "Vagina", "Negro",
			"Coon", "Bitched", "Sexist", "Freaking", "Cock", "Sucker", "Lick", "Licker", "Rape", "Molest",
			"Anal", "Buttrape", "Sex", "Retard", "Fuckface", "Dumbass", "5h1t", "5hit", "A_s_s", "a2m",
			"a55", "adult", "amateur", "anal", "anilingus", "anus", "ar5e", "arrse", "arse", "arsehole",
			"ass", "ass fuck†††", "asses", "assfucker", "ass-fucker", "assfukka", "asshole", "asshole", "assholes", "assmucus",
			"assmunch", "asswhole", "autoerotic", "b!tch", "b00bs", "b17ch", "b1tch", "ballbag", "ballsack", "bang (one's) box†††",
			"bangbros", "bareback", "bastard", "beastial", "beastiality", "beef curtain†††", "bellend", "bestial", "bestiality", "biatch",
			"bimbos", "birdlock", "bitch", "bitch tit†††", "bitcher", "bitchers", "bitches", "bitchin", "bitching", "bloody",
			"blow job", "blow me†††", "blow mud", "blowjob", "blowjobs", "blue waffle", "blumpkin", "boiolas", "boner", "boob",
			"boobs", "breasts", "buceta", "bum", "bunny fucker", "bust a load†††", "busty", "butt", "butthole", "buttmuch",
			"buttplug", "c0ck", "c0cksucker", "carpet muncher", "carpetmuncher", "cawk", "chink", "choade†††", "chota bags†††", "cipa",
			"cl1t", "clit", "clit licker†††", "clitoris", "clits", "clitty litter†††", "clusterfuck", "cnut", "cock", "cock pocket†††",
			"cock snot†††", "cockface", "cockhead", "cockmunch", "cockmuncher", "cocks", "cocksuck ", "cocksucked ", "cocksucker", "cock-sucker",
			"cocksucking", "cocksucks ", "cocksuka", "cocksukka", "cok", "cokmuncher", "coksucka", "coon", "cop some wood†††", "cornhole†††",
			"corp whore†††", "cox", "cum", "cumdump†††", "cummer", "cumming", "cums", "cumshot", "cunilingus", "cunillingus",
			"cunnilingus", "cunt", "cunt hair†††", "cuntbag†††", "cuntlick ", "cuntlicker ", "cuntlicking ", "cunts", "cuntsicle†††", "cunt-struck†††",
			"cut rope†††", "cyalis", "cyberfuc", "cyberfuck ", "cyberfucked ", "cyberfucker", "cyberfuckers", "cyberfucking ", "d1ck", "damn",
			"dick", "dick hole†††", "dick shy†††", "dickhead", "dildo", "dildos", "dink", "dinks", "dirsa", "dirty Sanchez†††",
			"dlck", "dog-fucker", "doggie style", "doggiestyle", "doggin", "dogging", "donkeyribber", "doosh", "duche", "dyke",
			"eat a dick†††", "eat hair pie†††", "ejaculate", "ejaculated", "ejaculates ", "ejaculating ", "ejaculatings", "ejaculation", "ejakulate", "erotic",
			"f4nny", "facial", "fag", "fagging", "faggitt", "faggot", "faggs", "fagot", "fagots", "fags",
			"fanny", "fannyflaps", "fannyfucker", "fanyy", "fatass", "fcuk", "fcuker", "fcuking", "feck", "fecker",
			"felching", "fellate", "fellatio", "fingerfuck ", "fingerfucked ", "fingerfucker ", "fingerfuckers", "fingerfucking ", "fingerfucks ", "fist fuck†††",
			"fistfuck", "fistfucked ", "fistfucker ", "fistfuckers ", "fistfucking ", "fistfuckings ", "fistfucks ", "flange", "flog the log†††", "fook",
			"fooker", "fuck hole†††", "fuck puppet†††", "fuck trophy†††", "fuck yo mama†††", "fuck†††", "fucka", "fuck-ass†††", "fuck-bitch†††", "fucked",
			"fucker", "fuckers", "fuckhead", "fuckheads", "fuckin", "fucking", "fuckings", "fuckingshitmotherfucker", "fuckme ", "fuckmeat†††",
			"fucks", "fucktoy†††", "fuckwhit", "fuckwit", "fudge packer", "fudgepacker", "fuk", "fuker", "fukker", "fukkin",
			"fuks", "fukwhit", "fukwit", "fux", "fux0r", "gangbang", "gangbang†††", "gang-bang†††", "gangbanged ", "gangbangs ",
			"gassy ass†††", "gaylord", "gaysex", "goatse", "god", "god damn", "god-dam", "goddamn", "goddamned", "god-damned",
			"ham flap†††", "hardcoresex ", "heshe", "hoar", "hoare", "hoer", "homo", "homoerotic", "hore", "horniest",
			"horny", "jackoff", "jack-off ", "jerk", "jerk-off", "jism", "jiz", "jizm", "jizz", "kawk",
			"kinky Jesus†††", "knob", "knob end", "knobead", "knobed", "knobend", "knobend", "knobhead", "knobjocky", "knobjokey",
			"kock", "kondum", "kondums", "kum", "kummer", "kumming", "kums", "kunilingus", "kwif†††", "l3i+ch",
			"l3itch", "labia", "LEN", "lmao", "lmfao", "lmfao", "lust", "lusting", "m0f0", "m0fo",
			"m45terbate", "ma5terb8", "ma5terbate", "mafugly†††", "masochist", "masterb8", "masterbat*", "masterbat3", "masterbate", "master-bate",
			"masterbation", "masterbations", "masturbate", "mof0", "mofo", "mo-fo", "mothafuck", "mothafucka", "mothafuckas", "mothafuckaz",
			"mothafucked ", "mothafucker", "mothafuckers", "mothafuckin", "mothafucking ", "mothafuckings", "mothafucks", "mother fucker", "mother fucker†††", "motherfuck",
			"motherfucked", "motherfucker", "motherfuckers", "motherfuckin", "motherfucking", "motherfuckings", "motherfuckka", "motherfucks", "muff", "muff puff†††",
			"mutha", "muthafecker", "muthafuckker", "muther", "mutherfucker", "n1gga", "n1gger", "nazi", "need the dick†††", "nigg3r",
			"nigg4h", "nigga", "niggah", "niggas", "niggaz", "nigger", "niggers ", "nob", "nob jokey", "nobhead",
			"nobjocky", "nobjokey", "numbnuts", "nut butter†††", "nutsack", "omg", "orgasim ", "orgasims ", "orgasm", "orgasms ",
			"p0rn", "pawn", "pecker", "penis", "penisfucker", "phonesex", "phuck", "phuk", "phuked", "phuking",
			"phukked", "phukking", "phuks", "phuq", "pigfucker", "pimpis", "piss", "pissed", "pisser", "pissers",
			"pisses ", "pissflaps", "porn", "porno", "pornography", "pornos", "prick", "pricks ", "pron", "pube",
			"pusse", "pussi", "pussies", "pussy", "pussy fart†††", "pussy palace†††", "pussys ", "queaf†††", "queer", "rectum",
			"retard", "rimjaw", "rimming", "s hit", "s.o.b.", "sadism", "sadist", "sandbar†††", "sausage queen†††", "schlong",
			"screwing", "scroat", "scrote", "scrotum", "semen", "sex", "sh!+", "sh!t", "sh1t", "shag",
			"shagger", "shaggin", "shagging", "shemale", "shi+", "shit", "shit fucker†††", "shitdick", "shite", "shited",
			"shitey", "shitfuck", "shitfull", "shithead", "shiting", "shitings", "shits", "shitted", "shitter", "shitters ",
			"shitting", "shittings", "shitty ", "skank", "slope†††", "slut", "slut bucket†††", "sluts", "smegma", "smut",
			"snatch", "son-of-a-bitch", "spac", "spunk", "t1tt1e5", "t1tties", "teets", "teez", "testical", "testicle",
			"tit", "tit wank†††", "titfuck", "tits", "titt", "tittie5", "tittiefucker", "titties", "tittyfuck", "tittywank",
			"titwank", "tosser", "turd", "tw4t", "twat", "twathead", "twatty", "twunt", "twunter", "v14gra",
			"v1gra", "vagina", "viagra", "vulva", "w00se", "wang", "wank", "wanker", "wanky", "whoar",
			"whore", "willies", "willy", "wtf", "xrated", "xxx", "sucker", "dumbass", "Kys", "Kill",
			"Die", "Cliff", "Bridge", "Shooting", "Shoot", "Bomb", "Terrorist", "Terrorism", "Bombed", "Trump",
			"Maga", "Conservative", "Make america great again", "Far right", "Necrophilia", "Mongoloid", "Furfag", "Cp", "Pedo", "Pedophile",
			"Pedophilia", "Child predator", "Predatory", "Depression", "Cut myself", "I want to die", "Fuck life", "Redtube", "Loli", "Lolicon",
			"Cub"
		};
		REPLACE_BANNED_MESSAGE = "[This message has been hidden due to profanity]";
		messageHideDelay = 5f;
		messageSpacing = 25f;
		RemoteCallHelper.RegisterCommandDelegate(typeof(ChatHandler), "CmdSendTextMessage", InvokeUserCode_CmdSendTextMessage, requiresAuthority: false);
		RemoteCallHelper.RegisterRpcDelegate(typeof(ChatHandler), "RpcSendTextMessage", InvokeUserCode_RpcSendTextMessage);
	}

	private void MirrorProcessed()
	{
	}

	protected void UserCode_CmdSendTextMessage(string player, int team, int channel, string message)
	{
		if (base.isServer)
		{
			MonoBehaviour.print("cmd send");
			SendTextMessage(player, team, channel, message);
			RpcSendTextMessage(player, team, channel, message);
		}
	}

	protected static void InvokeUserCode_CmdSendTextMessage(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendTextMessage called on client.");
		}
		else
		{
			((ChatHandler)obj).UserCode_CmdSendTextMessage(reader.ReadString(), reader.ReadInt(), reader.ReadInt(), reader.ReadString());
		}
	}

	protected void UserCode_RpcSendTextMessage(string player, int team, int channel, string message)
	{
		if (!base.isServer)
		{
			MonoBehaviour.print("rpc send");
			SendTextMessage(player, team, channel, message);
		}
	}

	protected static void InvokeUserCode_RpcSendTextMessage(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSendTextMessage called on server.");
		}
		else
		{
			((ChatHandler)obj).UserCode_RpcSendTextMessage(reader.ReadString(), reader.ReadInt(), reader.ReadInt(), reader.ReadString());
		}
	}
}
