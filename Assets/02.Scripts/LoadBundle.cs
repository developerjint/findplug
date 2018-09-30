using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LoadBundle : MonoBehaviour {
	public Image Test;
	public string BundleURL = "D:\\Unity\\FireTest\\Assets\\AssetBundles\\Android\\test";
    public string AssetName = "test";
    public int version = 1;

	AssetBundle testbundle;

	// Use this for initialization		
	IEnumerator LoadAsset () {
		// change url for you asset bundle path
		// string url = "C:\\Users\\jint\\AppData\\LocalLow\\DefaultCompany\\FireTest\\test";
		string url = "D:\\Unity\\FireTest\\Assets\\StreamingAssets\\AssetBundles\\test";
		// load asset bundle using WWW class
		WWW www = new WWW ("file:///" + url);
		while (!www.isDone)
			yield return null;
		testbundle= www.assetBundle;

		
		if (testbundle != null) {
			GameObject cube = testbundle.LoadAsset ("Cube") as GameObject;
			Instantiate (cube);
		} else {
			Debug.LogError ("Asset Bundle is NULL");
		}

		Test.sprite =  testbundle.LoadAsset<Sprite>("rivers");

		www.Dispose();
	}


	public void OnClickUnload(){
		testbundle.Unload(true);
	}

	public void OnClickLoad(){
		StartCoroutine(LoadAsset());
	}

	public void OnClickDownLoadCache(){
		StartCoroutine(DownloadAndCache());
	}
	IEnumerator DownloadAndCache (){ 
		// cache 폴더에 AssetBundle을 담을 것이므로 캐싱시스템이 준비 될 때까지 기다림
		string mBundleURL = "https://firebasestorage.googleapis.com/v0/b/unityfirebase-a597b.appspot.com/o/assetBundles%2Ftest.2?alt=media&token=42de35ea-66ad-403d-ae13-8479292610be";

		while (!Caching.ready) 
			yield return null; 
			// 에셋번들을 캐시에 있으면 로드하고, 없으면 다운로드하여 캐시폴더에 저장합니다. 
			using(WWW www = WWW.LoadFromCacheOrDownload (mBundleURL, 2)){ 
				yield return www; 
					if (www.error != null) 
						throw new Exception("WWW 다운로드에 에러가 생겼습니다.:" + www.error); 
			} // using문은 File 및 Font 처럼 컴퓨터 에서 관리되는 리소스들을 쓰고 나서 쉽게 자원을 되돌려줄수 있도록 기능을 제공 
	}

	IEnumerator LoadAssetBundle (){ 
			while (!Caching.ready) yield return null; 
			string mBundleURL = "https://firebasestorage.googleapis.com/v0/b/unityfirebase-a597b.appspot.com/o/assetBundles%2Ftest.2?alt=media&token=42de35ea-66ad-403d-ae13-8479292610be";
			using(WWW www = WWW.LoadFromCacheOrDownload (mBundleURL, 2)){ 
				yield return www; if (www.error != null) 
				throw new Exception("WWW 다운로드에 에러가 생겼습니다.:" + www.error); 


				AssetBundle bundle = www.assetBundle; 

				if (bundle != null) {
					GameObject cube = bundle.LoadAsset ("Cube") as GameObject;
					Instantiate (cube);
				} else {
					Debug.LogError ("Asset Bundle is NULL");
				}

				// for (int i = 0; i < 3; i++) {
				// 	 AssetBundleRequest request = bundle.LoadAssetAsync ("Cube " + (i + 1), typeof(GameObject)); 
				// 	 yield return request; GameObject obj = Instantiate (request.asset) as GameObject; 
				// 	 obj.transform.position = new Vector3 (-10.0f + (i * 10), 0.0f, 0.0f); 
				// }

				Test.sprite =  bundle.LoadAsset<Sprite>("temp_file");
				bundle.Unload(false); 
				www.Dispose (); 
			} // using문은 File 및 Font 처럼 컴퓨터 에서 관리되는 리소스들을 쓰고 나서 쉽게 자원을 되돌려줄수 있도록 기능을 제공 }
	}

	public void OnClickLoadAssetBundleCache(){
		StartCoroutine(LoadAssetBundle());
	}

}
