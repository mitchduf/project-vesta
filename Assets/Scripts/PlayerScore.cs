using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

	int lastKills = 0;
	int lastDeaths = 0;

	Player player;

	void Start () 
	{
		player = GetComponent<Player> ();
		StartCoroutine (SyncScoreLoop ());
	}

	void OnDestroy ()
	{
		if (player != null)
			SyncNow();
	}

	IEnumerator SyncScoreLoop ()
	{
		while (true) {
			yield return new WaitForSeconds (5f);

			SyncNow ();
		}	
	}

	void SyncNow ()
	{
		if (UserAccountManager.isLoggedIn) {
			UserAccountManager.instance.GetData (OnDataReceived);
		}
	}

	void OnDataReceived (string _data)
	{
		if (player.kills <= lastKills && player.deaths <= lastDeaths)
			return;

		int killsSinceLast = player.kills - lastKills;
		int deathsSinceLast = player.deaths - lastDeaths;

		if (killsSinceLast == 0 && deathsSinceLast == 0)
			return;

		int _kills = DataTranslator.DataToKills (_data);
		int _deaths = DataTranslator.DataToDeaths (_data);

		int _newKills = killsSinceLast + _kills;
		int _newDeaths = deathsSinceLast + _deaths;

		string _newData = DataTranslator.ValuesToData (_newKills, _newDeaths);

		Debug.Log ("Syncing: " + _newData);

		lastKills = player.kills;
		lastDeaths = player.deaths;

		UserAccountManager.instance.SendData (_newData);
	}

}
