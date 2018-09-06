using System.Collections;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

	[SerializeField]
	GameObject playerScoreboardItem;

	[SerializeField]
	Transform playerScoreboardList;

	void OnEnable() 
	{
		// Get an array of players
		Player[] players = GameManager.GetAllPlayers();

		// Loop through and set up a list item for each one
		foreach (Player player in players) {
			//Debug.Log (player.username + " | " + player.kills + " | " + player.deaths);
			GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
			PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem> ();
			if (item != null)
				item.Setup (player.username, player.kills, player.deaths);
		}
	}

	void OnDisable()
	{
		foreach (Transform child in playerScoreboardList) {
			Destroy (child.gameObject);
		}
	}

}
