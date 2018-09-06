using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DatabaseControl; // << Remember to add this reference to your scripts which use DatabaseControl
using UnityEngine.SceneManagement;

public class UserAccountManager : MonoBehaviour {

	public static UserAccountManager instance;

	//Called at the very start of the game
	void Awake()
	{
		if (instance != null) {
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(this);
	}

	public static string loggedInUsername { get; protected set; }
	private static string loggedInPassword = "";

	public static string loggedInData { get; protected set; }

	public static bool isLoggedIn { get; protected set; }

	public string loggedInSceneName = "Lobby";
	public string loggedOutSceneName = "LoginMenu";

	public delegate void OnDataReceivedCallback(string data);

	public void LogOut() 
	{
		loggedInUsername = "";
		loggedInPassword = "";

		isLoggedIn = false;

		Debug.Log ("User logged out.");

		SceneManager.LoadScene (loggedOutSceneName);
	}

	public void LogIn (string _username, string _password)
	{
		loggedInUsername = _username;
		loggedInPassword = _password;

		isLoggedIn = true;

		Debug.Log ("Logged in as " + _username);

		SceneManager.LoadScene (loggedInSceneName);
	}


	public void SendData (string _data)
	{
		//Called when the player hits 'Set Data' to change the data string on their account. Switches UI to 'Loading...' and starts coroutine to set the players data string on the server
		if (isLoggedIn) {
			StartCoroutine (sendSendDataRequest (loggedInUsername, loggedInPassword, _data));
		} 
	}

	IEnumerator sendSendDataRequest (string _username, string _password, string _data)
	{
		IEnumerator e = DCF.SetUserData(_username, _password, _data); // << Send request to set the player's data string. Provides the username, password and new data string
		while (e.MoveNext())
		{
			yield return e.Current;
		}
		string response = e.Current as string; // << The returned string from the request

		if (response == "ContainsUnsupportedSymbol")
		{
			//One of the parameters contained a - symbol
			Debug.Log("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
		}
		if (response == "Error")
		{
			//Error occurred. For more information of the error, DC.Login could
			//be used with the same username and password
			Debug.Log("Data Upload Error: Contains Unsupported Symbol '-'");
		}
	}

	public void GetData(OnDataReceivedCallback onDataReceived) 
	{
		if (isLoggedIn) {
			StartCoroutine (sendGetDataRequest (loggedInUsername, loggedInPassword, onDataReceived));
		}
	}

	IEnumerator sendGetDataRequest(string _username, string _password, OnDataReceivedCallback onDataReceived) 
	{
		string data = "ERROR";

		IEnumerator e = DCF.GetUserData(_username, _password);
		while (e.MoveNext())
		{
			yield return e.Current;
		}
		string response = e.Current as string;
		if (response == "Error")
		{
			//Error occurred. For more information of the error, DC.Login could
			//be used with the same username and password
			Debug.Log("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
		}
		else
		{
			if (response == "ContainsUnsupportedSymbol")
			{
				//One of the parameters contained a - symbol
				Debug.Log("Get Data Error: Contains Unsupported Symbol '-'");
			}
			else
			{
				//Data received in returned.text variable
				string DataRecieved = response;
				data = DataRecieved;
			}
		}

		if (onDataReceived != null)
			onDataReceived.Invoke(data);
	}

}
