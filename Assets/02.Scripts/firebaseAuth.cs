using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;


public class firebaseAuth : MonoBehaviour {

	public static firebaseAuth _instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

	public static firebaseAuth instance
	{
		get{
			if(_instance == null){
				_instance = FindObjectOfType(typeof(firebaseAuth)) as firebaseAuth;
				if(_instance == null){
					Debug.Log("There's no active object");
				}
			}
			return _instance;
		}
	}

	public Image ImageBackgorund;
	public InputField input_email;
	public InputField input_pass;
	public Text result_text;

	public Firebase.Auth.FirebaseAuth auth;
	public Firebase.Auth.FirebaseUser firebase_user;
	// FirebaseStorage storage = FirebaseStorage.DefaultInstance;

	void Start(){	
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		firebaseDB.instance.InitialDB();
	}

	public void EmailSignUp(){
		auth.CreateUserWithEmailAndPasswordAsync(input_email.text, input_pass.text).ContinueWith(task =>{
			if(!task.IsCanceled && !task.IsFaulted){
				firebase_user = task.Result;
				result_text.text = "Success Sign Up !!" +  firebase_user.UserId + "," + firebase_user.Email;
				// firebaseDB.instance.InsertUserDB(firebase_user.UserId, firebase_user.Email);
				firebaseDB.instance.CheckUserDB(firebase_user.UserId, firebase_user.Email);
			}else{
				result_text.text = "Fail Sign Up !!";
			}
		});
	}

	public void EmailSignIn(){
		auth.SignInWithEmailAndPasswordAsync(input_email.text ,input_pass.text).ContinueWith( task =>{
			if(task.IsCompleted && !task.IsCanceled && !task.IsFaulted){
				firebase_user = task.Result;
				result_text.text = "Success Sign In !!" +  firebase_user.UserId + "," + firebase_user.Email;
				// firebaseDB.instance.InitialDBSignIn(firebase_user.UserId, firebase_user.Email);
				// firebaseDB.instance.InitialDBSignIn(firebase_user.UserId, firebase_user.Email);
				
			}else{
				result_text.text = "Fail Sign In !!";
			}
		});
	}


	public void OnClickSignOut(){
		result_text.text = "";
		auth.SignOut();
		PlayGamesPlatform.Instance.SignOut();
	}


	//===============구글 로그인 관련 =====================

	public void OnClickGoogleLogin(){
		InitGooglePlayService();

		Social.localUser.Authenticate(success =>
		{
			result_text.text = string.Format("Google Login Result = {0}:{1}" , success, Social.localUser.userName);

			if(success == false){
				return;
			}
			StartCoroutine(coLogin());

		});
	}

	void InitGooglePlayService(){
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
		.RequestIdToken()
		.Build();

		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = false;
		PlayGamesPlatform.Activate();
	}



	IEnumerator coLogin(){
		result_text.text = string.Format("\n Try to get Token...");
		while(System.String.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
			yield return null;

		string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();

		result_text.text = string.Format("\n Token : {0}", idToken);
		

		Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(idToken, null);
		
		auth.SignInWithCredentialAsync(credential).ContinueWith(
			task =>
			{
				if(task.IsCompleted && !task.IsCanceled && !task.IsFaulted){
					firebase_user = task.Result;
					result_text.text = string.Format("FirebaseUser : {0}\nEmail:{1}", firebase_user.UserId, firebase_user.Email);
					firebaseDB.instance.CheckUserDB(firebase_user.UserId, firebase_user.Email);




					// firebase_user.SendEmailVerificationAsync().ContinueWith(t => {
					// 	Debug.Log("Verification Email Sent");
					// });
					// System.Uri photo_url = firebase_user.PhotoUrl;
				}
			});
	}

	// public void OnClickDownload(){
	// 	if(firebase_user == null){
	// 		result_text.text = "firebase_user is null";
	// 		return;
	// 	}

	// 	Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
	// 	Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://unityfirebase-a597b.appspot.com");
	// 	Firebase.Storage.StorageReference assetBundles_ref = storage_ref.Child("assetBundles/AssetBundles");
	// 	Firebase.Storage.StorageReference assetBundlesM_ref = storage_ref.Child("assetBundles/AssetBundles.manifest");
	// 	Firebase.Storage.StorageReference assetTest_ref = storage_ref.Child("assetBundles/test");
	// 	Firebase.Storage.StorageReference assetTestM_ref = storage_ref.Child("assetBundles/test.manifest");

	// 	string local_url1 = Application.persistentDataPath +"/AssetBundles";
	// 	string local_url2 = Application.persistentDataPath +"/AssetBundles.manifest";
	// 	string local_url3 = Application.persistentDataPath +"/test";
	// 	string local_url4 = Application.persistentDataPath +"/test.manifest";

	// 	Debug.Log(local_url1);
	// 	Debug.Log(string.Format("Target Path : {0}", local_url1));

	// 	assetBundles_ref.GetFileAsync(local_url1).ContinueWith(file_task =>{
	// 		if(!file_task.IsFaulted && !file_task.IsCanceled){
	// 			result_text.text = "File downloaded.";
	// 		}else{
	// 			result_text.text = "File download fail , result : " + file_task.Exception.ToString();
	// 		}
	// 	});

	// 	assetBundles_ref.GetFileAsync(local_url2).ContinueWith(file_task =>{
	// 		if(!file_task.IsFaulted && !file_task.IsCanceled){
	// 			result_text.text = "File downloaded.";
	// 		}else{
	// 			result_text.text = "File download fail , result : " + file_task.Exception.ToString();
	// 		}
	// 	});
	// 	assetBundles_ref.GetFileAsync(local_url3).ContinueWith(file_task =>{
	// 		if(!file_task.IsFaulted && !file_task.IsCanceled){
	// 			result_text.text = "File downloaded.";
	// 		}else{
	// 			result_text.text = "File download fail , result : " + file_task.Exception.ToString();
	// 		}
	// 	});
	// 	assetBundles_ref.GetFileAsync(local_url4).ContinueWith(file_task =>{
	// 		if(!file_task.IsFaulted && !file_task.IsCanceled){
	// 			result_text.text = "File downloaded.";
	// 		}else{
	// 			result_text.text = "File download fail , result : " + file_task.Exception.ToString();
	// 		}
	// 	});
		
	// }

	public void OnClickUserPhotoLoad(){
		string photoUrl = "https://firebasestorage.googleapis.com/v0/b/unityfirebase-a597b.appspot.com/o/images%2Frivers.png?alt=media&token=322fe510-1aa4-4ed4-beba-40a0bb324ab9";
		StartCoroutine(PhotoLoad(photoUrl));
	}

	IEnumerator PhotoLoad(string photoUrl)
	{
		// Start a download of the given URL
		using (WWW www = new WWW(photoUrl))
		{
			// Wait for download to complete
			yield return www;
			//결과페이지 이미지 로드
			ImageBackgorund.sprite =  Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0), 1);
			//Email, QR 페이지 이미지 로드
			// TextureUser = www.texture;
		}
	}

	// public void OnClickPhotoupload(){
		
	// 	Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
	// 	Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://unityfirebase-a597b.appspot.com");
	// 	// Create a reference to the file you want to upload
	// 	// Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("images/rivers.png");
	// 	Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("images/mountains.png");	


	// 	// rivers_ref.PutFileAsync(local_file, null, new Firebase.Storage.StorageProgress<UploadState>(state => {
	// 	// 	// called periodically during the upload
	// 	// 		Debug.Log(string.Format("Progress: {0} of {1} bytes transferred.", state.BytesTransferred, state.TotalByteCount));
	// 	// }), System.Threading.CancellationToken, null);

	// 	// Upload the file to the path "images/rivers.jpg"
	// 	rivers_ref.PutFileAsync(local_file).ContinueWith (task => {
	// 			Debug.Log(" uploading...");
	// 			if (task.IsFaulted && !task.IsCanceled) {
	// 				Debug.Log("Finished uploading.............................");
	// 				//  Firebase.Storage.StorageMetadata metadata = task.Result;
	// 				//  Debug.Log(rivers_ref.GetDownloadUrlAsync());

					 
    //    				//  string download_url = rivers_ref.GetDownloadUrlAsync().ToString();
						
	// 				// Metadata contains file metadata such as size, content-type, and download URL.
					
	// 				// string download_url = .Result.ToString();
    //   				// Debug.Log(download_url);
					
	// 				//   Debug.Log("download url = " + download_url);
					
	// 			// Uh-oh, an error occurred!
	// 			} else {
	// 				Debug.Log(task.Exception.ToString());
	// 			}
	// 		});
	// 	}
		

	// 	Firebase.Storage.StorageReference mountains_ref = storage_ref.Child("images/mountains.jpg")
	// 	.PutFileAsync(local_file, null, new Firebase.Storage.StorageProgress<UploadState>(state => {
	// 		// called periodically during the upload
	// 		Debug.Log(String.Format("Progress: {0} of {1} bytes transferred.",
	// 							state.BytesTransferred, state.TotalByteCount));
	// 	}), CancellationToken.None, null);

	// task.ContinueWith(resultTask => {
	// 	if (!resultTask.IsFaulted && !resultTask.IsCancelled) {
	// 		Debug.Log("Upload finished.");
	// 	}
	// });

	// public void TakeSnapshot(){
	// 	StartCoroutine(TakeSnapshotCoroutine());
	// }
	// public IEnumerator TakeSnapshotCoroutine()
	// {
	// 	yield return new WaitForEndOfFrame();

	// 	RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
	// 	camera.targetTexture = rt;
	// 	camera.Render();
	// 	RenderTexture.active = rt;

	// 	tex = new Texture2D(Screen.width /5, Screen.height /5, TextureFormat.RGB24, false);
	// 	tex.ReadPixels(new Rect(0, 0, Screen.width/5, Screen.height/5), 0, 0);
	// 	tex.Apply(); 

	// 	camera.targetTexture = null;


		
	// 	StartCoroutine(TakeSnapshotCoroutine2());
	// }

	// public IEnumerator TakeSnapshotCoroutine2()
	// {
	// 	yield return new WaitForEndOfFrame();

	// 	var data = tex.GetRawTextureData();
	// 	Debug.Log(data);
	// 	Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
	// 	Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://unityfirebase-a597b.appspot.com");
	// 	Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("images/rivers7.png");

	// 	// Upload the file to the path "images/rivers.jpg"
	// 	// rivers_ref.PutBytesAsync(data)
	// 	// .ContinueWith (task => {
	// 	// 	if (task.IsFaulted && !task.IsCanceled) {
	// 	// 		// Metadata contains file metadata such as size, content-type, and download URL.
	// 	// 		Firebase.Storage.StorageMetadata metadata = task.Result;
	// 	// 		// string download_url = metadata.DownloadUrl.ToString();
	// 	// 		Debug.Log("Finished uploading...");
	// 	// 		// Debug.Log("download url = " + download_url);
			
	// 	// 	} else {
	// 	// 			Debug.Log(task.Exception.ToString());
	// 	// 		// Uh-oh, an error occurred!
	// 	// 	}
	// 	// });


	// 	rivers_ref.PutBytesAsync(data).ContinueWith (task => {
	// 			Debug.Log(" uploading...");
	// 			if (task.IsFaulted && !task.IsCanceled) {
	// 				Debug.Log("Finished uploading.............................");
	// 				//  Firebase.Storage.StorageMetadata metadata = task.Result;
	// 				//  Debug.Log(rivers_ref.GetDownloadUrlAsync());

					 
    //    				//  string download_url = rivers_ref.GetDownloadUrlAsync().ToString();
						
	// 				// Metadata contains file metadata such as size, content-type, and download URL.
					
	// 				// string download_url = .Result.ToString();
    //   				// Debug.Log(download_url);
					
	// 				//   Debug.Log("download url = " + download_url);
					
	// 			// Uh-oh, an error occurred!
	// 			} else {
	// 				Debug.Log(task.Exception.ToString());
	// 			}
	// 		});
	// }
}
