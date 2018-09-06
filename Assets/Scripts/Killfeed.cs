using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killfeed : MonoBehaviour {

	[SerializeField]
	GameObject killfeedItemPrefab;

	// Use this for initialization
	void Start () {
		GameManager.instance.onPlayerKilledCallback += OnKill;
	}

	public void OnKill (string _player, string _source)
	{
		// Debug.Log (_source + " killed " + _player);
		GameObject go = (GameObject)Instantiate(killfeedItemPrefab, this.transform);
		go.GetComponent<KillfeedItem> ().Setup (_player, _source);

		Destroy (go, 4f);
	}

}
