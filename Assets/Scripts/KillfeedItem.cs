using UnityEngine;
using UnityEngine.UI;

public class KillfeedItem : MonoBehaviour {

	[SerializeField]
	Text text;

	public void Setup (string _player, string _source)
	{
		text.text = "<b>" + _source + "</b>" + " killed " + "<i>" + _player + "</i>";
	}

}
