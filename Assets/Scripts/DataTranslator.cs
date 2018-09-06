using UnityEngine;
using System;

public class DataTranslator : MonoBehaviour {

	private static string KILLS_TAG = "[KILLS]";
	private static string DEATHS_TAG = "[DEATHS]";

	public static int DataToKills (string _data)
	{
		return int.Parse (DataToValue (_data, KILLS_TAG));
	}

	public static int DataToDeaths (string _data)
	{
		return int.Parse (DataToValue (_data, DEATHS_TAG));
	}

	private static string DataToValue (string _data, string symbol)
	{
		string[] pieces = _data.Split ('/');
		foreach (string piece in pieces) {
			if (piece.StartsWith (symbol))
				return piece.Substring (symbol.Length);
		}

		Debug.LogError (symbol + " not found in " + _data);
		return "";
	}

	public static string ValuesToData (int _kills, int _deaths) 
	{
		return KILLS_TAG + _kills + "/" + DEATHS_TAG + _deaths;
	}
}
