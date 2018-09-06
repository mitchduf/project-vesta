using UnityEngine;
using UnityEngine.UI;

public class UserAccount_Lobby : MonoBehaviour {

	public Text usernameText;

	// Use this for initialization
	void Start () {
		if (UserAccountManager.isLoggedIn)
			usernameText.text = UserAccountManager.loggedInUsername;
	}

	public void LogOut() 
	{
		if (UserAccountManager.isLoggedIn)
			UserAccountManager.instance.LogOut();		
	}
}
