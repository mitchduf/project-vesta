using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreboardItem : MonoBehaviour {

	[SerializeField]
	Text usernameText;

	[SerializeField]
	Text killsText;

	[SerializeField]
	Text deathsText;

	public void Setup (string _username, int _kills, int _deaths)
	{
		usernameText.text = _username;
		killsText.text = "Kills: " + _kills;
		deathsText.text = "Deaths: " + _deaths;
	}
}
