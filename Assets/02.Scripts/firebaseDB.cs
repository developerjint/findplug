using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;
using System;

public class UserData {
	public string seq;
	public string name;
	public string fileName;
	public string userId;
	public int score;
	public string email;
	public string regTime;
	public UserData() {
	}

	public UserData(string seq, string name, string fileName, string userId, int score, string email, string regTime) {
		this.seq = seq;
		this.name = name;
		this.fileName = fileName;
		this.userId = userId;
		this.score = score;
		this.email = email;
		this.regTime = regTime;
	}
}

public class AssetBundlesData {
	public string bundleUrl;
	public string bundleVersion;
	
	public AssetBundlesData() {
	}

	public AssetBundlesData(string bundleUrl, string bundleVersion) {
		this.bundleUrl = bundleUrl;
		this.bundleVersion = bundleVersion;
	}
}



public class firebaseDB : MonoBehaviour {

	public static firebaseDB _instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

	public static firebaseDB instance
	{
		get{
			if(_instance == null){
				_instance = FindObjectOfType(typeof(firebaseDB)) as firebaseDB;
				if(_instance == null){
					Debug.Log("There's no active object");
				}
			}
			return _instance;
		}
	}

	void OnDestroy(){
		_instance = null; 
	}
	public Text TextLog;
	public string AssetBundleUrl;
	public int AssetBundleVersion;
	public string TotalUserCount;


	// Use this for initialization
	private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
	private DatabaseReference mDatabaseRef;
	// Use this for initialization
	void Start () {
		// InitialDB();		
	}


	public void InitialDB(){
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			if(task.IsCompleted && !task.IsCanceled && !task.IsFaulted){
				dependencyStatus = task.Result;
				if (dependencyStatus == DependencyStatus.Available) {
					InitializeFirebase();
				} else {
					Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
				}
			}else{
				Debug.Log("Task");
			}
		});
	}

	// Initialize the Firebase database:
	protected virtual void InitializeFirebase() {
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unityfirebase-a597b.firebaseio.com/");
	}

	public void OnClickSetDatabaseRef(){
		mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
		Debug.Log("InitializeFirebase3");
		// InsertUserDB();
		LoadBundleInfo();

		Debug.Log("InitializeFirebase4");
	}

	public void AutoGoogleLogin(){
		mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
		Debug.Log("InitializeFirebase3");
		// InsertUserDB();
		LoadBundleInfo();

		Debug.Log("InitializeFirebase4");

		firebaseAuth.instance.OnClickGoogleLogin();
	}

	// protected virtual void InitializeFirebaseGoogle(string userId, string email) {
	// 	Debug.Log("InitializeFirebaseGoogle ================");
	// 	FirebaseApp app = FirebaseApp.DefaultInstance;
	// 	// NOTE: You'll need to replace this url with your Firebase App's database
	// 	// path in order for the database connection to work correctly in editor.
	// 	app.SetEditorDatabaseUrl("https://unityfirebase-a597b.firebaseio.com/");
	// 	if (app.Options.DatabaseUrl != null) {
	// 		app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
	// 		Debug.Log("Aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
	// 	}
			

	// 	mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
	// 	Debug.Log("InitializeFirebase");
	// 	// CheckUserDB(userId, email);
	// 	// LoadBundleInfo();
		
	// 	// GetUserCount();
	// }




	//사용자 DB 등록
	public void writeNewUser(string primaryKey, string seq, string name, string fileName, string userId, int score, string email, string regTime) {
		UserData user = new UserData(seq, name, fileName, userId, score, email, regTime);
		string json = JsonUtility.ToJson(user);
		mDatabaseRef.Child("users").Child(primaryKey).SetRawJsonValueAsync(json);
		TextLog.text = "Insert DB!";
		Debug.Log("Write!");
	}


	private void updateScore(int userSeq, int score) {
		string seq;
		seq = userSeq.ToString();
		mDatabaseRef.Child("users").Child(seq).Child("score").SetValueAsync(score);
	}


	private void updateUsername(int userSeq, string name) {
		string seq;
		seq = userSeq.ToString();
		mDatabaseRef.Child("users").Child(seq).Child("name").SetValueAsync(name);
		
	}
	

	// void Update () {
	// 	if (Input.GetKeyDown(KeyCode.Q)) {
	// 		writeNewUser ("2","name", "asdmalskd.jpg", "id", 110,"email");
	// 	}
	// 	else if(Input.GetKeyDown(KeyCode.W))
	// 	{
	// 		FirebaseDatabase.DefaultInstance.GetReference("assetbundles").GetValueAsync().ContinueWith(task => {
	// 			if (task.IsFaulted) {
	// 				Debug.Log ("failed");
	// 			}
	// 			else if (task.IsCompleted) {
	// 				Firebase.Database.DataSnapshot snapshot = task.Result;
	// 				Debug.Log (snapshot.Value.ToString());
	// 				foreach (var childSnapshot in snapshot.Children) {
	// 					Debug.Log("users name : " +
	// 						childSnapshot.Child("bundleUrl").Value.ToString() + "," + childSnapshot.Child("bundleVersion").Value.ToString());
	// 				}
	// 			}
	// 		});
	// 	}
	// 	else if(Input.GetKeyDown(KeyCode.E)){
	// 		for(int i = 1 ; i <3; i ++){
	// 			updateScore(i , 0);
	// 			updateUsername(i , "qqq");
	// 		}
	// 	}
	// }

	void LoadBundleInfo(){
		mDatabaseRef.Child("assetbundles").GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				Debug.Log ("failed");
			}
			else if (task.IsCompleted) {
				Firebase.Database.DataSnapshot snapshot = task.Result;
				// Debug.Log (snapshot.Value.ToString());
				foreach (var childSnapshot in snapshot.Children) {
					AssetBundleUrl = childSnapshot.Child("bundleUrl").Value.ToString();
					// Debug.Log((int)childSnapshot.Child("bundleVersion").Value);
					AssetBundleVersion = int.Parse(childSnapshot.Child("bundleVersion").Value.ToString());
					Debug.Log(AssetBundleUrl +", " + AssetBundleVersion);
					OnClickShowBundleInfo();
				}	
			}
		});
	}
	public void OnClickShowBundleInfo(){
		TextLog.text = "AssetBundleUrl  : "  + AssetBundleUrl + "\nver : " + AssetBundleVersion;
	}
	
	public void InsertUserDB(string userId, string email){
		mDatabaseRef.Child("users").GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				Debug.Log ("failed");
			}else if (task.IsCompleted) {
				Firebase.Database.DataSnapshot snapshot = task.Result;
				TotalUserCount = snapshot.ChildrenCount.ToString();
				TextLog.text = "InsertUserDB";
				string[] strArr = email.Split('@');

				string curretTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
				writeNewUser(TotalUserCount, TotalUserCount, strArr[0], "0", userId , 0 , email, curretTime);
				// Debug.Log(snapshot.ChildrenCount);
			}
		});
	}


	public void CheckUserDB(string userId, string email){
		mDatabaseRef.Child("users").GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				Debug.Log(task.Exception);
				Debug.Log ("failed");
			}else if (task.IsCompleted) {
				Firebase.Database.DataSnapshot snapshot = task.Result;
				bool idExist = false;
				Debug.Log ("CheckUserDB");
				TextLog.text = "CheckUserDB";
				foreach (var childSnapshot in snapshot.Children) {
					string uid = childSnapshot.Child("userId").Value.ToString();
					Debug.Log(uid);
					if(uid == userId){
						TextLog.text = "ID 존재함";
						idExist = true;
					}else{
						// InsertUserDB(userId, email);
					}					
				}
				if(idExist == false){
					InsertUserDB(userId, email);
				}

				// TextLog.text = "snapshot.ChildrenCount";
				// Firebase.Database.DataSnapshot snapshot = task.Result;
	
				// TextLog.text = snapshot.ToString();
				// // TextLog.text = "snapshot.ChildrenCount2";
				// int ct = 0;
				
				// if(snapshot.HasChildren){
				// 	TextLog.text = "ID 존재함";
				// }else{
				// 	TextLog.text = "No Has Child";
				// 	InsertUserDB(userId, email);
				// }
				// TextLog.text = ct.ToString();
				// if(ct == 0){
				// 	// TextLog.text = "snapshot.ChildrenCount  " + snapshot.ChildrenCount;
					
				// }else{
					
				// }
			}
		});
	}

	public void SelectUserDB(){
		
		// FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("userId").EqualTo(firebaseAuth.instance.firebase_user.UserId).GetValueAsync().ContinueWith(task => {
		// 	if (task.IsFaulted) {
		// 		Debug.Log ("failed");
		// 	}else if (task.IsCompleted) {
		// 		Firebase.Database.DataSnapshot snapshot = task.Result;
		// 		foreach (var childSnapshot in snapshot.Children) {
		// 				Debug.Log("email : " +
		// 					childSnapshot.Child("email").Value.ToString() + "," + childSnapshot.Child("score").Value.ToString());
		// 		}
		// 	}
		// });

		
		// string mUserID = .Child("users").OrderByChild("userId").EqualTo(firebaseAuth.instance.firebase_user.UserId).ToString();
		// Debug.Log(mUserID.);
	}


	
	
}
