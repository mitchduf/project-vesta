using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof (WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

	private const string PLAYER_TAG = "Player";

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	private PlayerWeapon currentWeapon;
	private WeaponManager weaponManager;

	// Use this for initialization
	void Start () 
	{
		if (cam == null) {
			Debug.LogError ("PlayerShoot: No camera referenced!");
			this.enabled = false;
		}

		weaponManager = GetComponent<WeaponManager> ();

	}

	// Update is called once per frame
	void Update () 
	{
		currentWeapon = weaponManager.GetCurrentWeapon ();

		if (PauseMenu.isOn)
			return;

		if (currentWeapon.bullets < currentWeapon.maxBullets) {
			if (Input.GetButtonDown("Reload")) {
				weaponManager.Reload ();
				return;
			}
		}

		if (currentWeapon == null)
			Debug.Log ("Weapon is null");

		if (currentWeapon.fireRate <= 0f) {
			if (Input.GetButtonDown ("Fire1")) {
				Shoot ();
			}
		} else {
			if (Input.GetButtonDown ("Fire1")) {
				InvokeRepeating ("Shoot", 0f, 1f / currentWeapon.fireRate);
			} else if (Input.GetButtonUp ("Fire1")) {
				CancelInvoke ("Shoot");
			}
		}
	}

	// Is called on the server when a player shoots
	[Command]
	void CmdOnShoot()
	{
		RpcDoShootEffect ();
	}

	// Is called on all clients when we need to do a shoot effect
	[ClientRpc]
	void RpcDoShootEffect() 
	{
		weaponManager.GetCurrentGraphics ().muzzleFlash.Play ();	
	}

	// Is called on the server when we hit something
	// Takes in the hit point and the normal of the surface
	[Command]
	void CmdOnHit (Vector3 _pos, Vector3 _normal)
	{
		RpcDoHitEffect (_pos, _normal);
	}

	// Is called on all clients 
	// Here we can spawn in cool effects
	[ClientRpc]
	void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
	{
		// Spawn it with normal such that it comes out of hit surface instead of through, no matter the direction
		GameObject _hitEffect = Instantiate (weaponManager.GetCurrentGraphics ().hitEffectPrefab, _pos, Quaternion.LookRotation (_normal));
		Destroy (_hitEffect, 2f);
		// If resources are becoming a problem spawning all these objects, look into 'Object Pooling'
		// Also, consider less sparks and/or making them less bouncy as in they don't collide with the environment (sparks involve a lot of physics and are taxing)
	}

	[Client]
	void Shoot() 
	{
		if (!isLocalPlayer || weaponManager.isReloading)
			return;

		if (currentWeapon.bullets <= 0) {
			//Debug.Log ("Out of bullets.");
			weaponManager.Reload();
			return;
		}

		currentWeapon.bullets--;

		Debug.Log ("Remaining bullets: " + currentWeapon.bullets);

		// We are shooting, call the OnShoot method on the server
		CmdOnShoot ();

		RaycastHit _hit;
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask) ) {
			// We hit something
			//Debug.Log("We hit " + _hit.collider.name);
			if (_hit.collider.tag == PLAYER_TAG) {
				CmdPlayerShot (_hit.collider.name, currentWeapon.damage, transform.name);
			}

			// We hit something, call the OnHit method on the server
			CmdOnHit (_hit.point, _hit.normal);
		}

		if (currentWeapon.bullets <= 0)
			weaponManager.Reload ();
	}

	// Called on the server
	[Command]
	void CmdPlayerShot(string _playerID, int _damage, string _sourceID)
	{
		Debug.Log (_playerID + " has been shot.");

		Player _player = GameManager.GetPlayer (_playerID);
		_player.RpcTakeDamage (_damage, _sourceID);
	}
}
