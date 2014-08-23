using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UKStoreScreenshots : EditorWindow {

	int WINDOW_BORDER = 17;


	bool useLandscape = true;
	bool usePortrait = false;

	string savePath = Application.dataPath;

	int captureCounter = 0;
	bool recording = false;
	float savedTimeScale = 0.0f;
	int capturedFrames = 0;
	int waitCounter = 0;

	static System.Type gameViewType;

	private static Vector2[] mSizesLandscape = new Vector2[]{
		new Vector2(960,640),
		new Vector2(1024,768),
		new Vector2(1136,640)
	};

	private static Vector2[] mSizesPortrait = new Vector2[]{
		new Vector2(640,960),
		new Vector2(768,1024),
		new Vector2(640,1136)
	};

	private List<Vector2> mSizes = new List<Vector2>();
	
	[MenuItem ("UnityKit/StoreScreenshots %^s")]
	static void Init () { 
		EditorWindow.GetWindow(typeof(UKStoreScreenshots));
	}
	
	void OnGUI () {
		if( mSizes.Count == 0 ) {
			useLandscape = true;
			usePortrait = false;
			mSizes.AddRange(mSizesLandscape);
		}

		if( (IsWindowInit() || recording) && IsInPlayMode() ) {

			string btnText = recording? "PLEASE WAIT": "CAPTURE: "+getScreenShotName()+"_WxH.png";

			if( GUILayout.Button(btnText, GUILayout.Height(100)) && !recording ) {
				savedTimeScale = Time.timeScale;
				Time.timeScale = 0;
				recording = true;	
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			bool useLandscapeOld = useLandscape;
			useLandscape = EditorGUILayout.Toggle("Landscape", useLandscape);
			usePortrait = EditorGUILayout.Toggle("Portrait", usePortrait);

			EditorGUILayout.Space();
			if( GUILayout.Button("SAVE: "+savePath) ) {
				string oldPath = savePath;
				savePath = EditorUtility.SaveFolderPanel("Select Folder", savePath, "");
				if( savePath.Length == 0 ) savePath = oldPath;
			}

			if( GUI.changed ) {
				mSizes.Clear();
				if( !useLandscape && !usePortrait ) {
					if( useLandscapeOld ) {
						usePortrait = true;
					} else {
						useLandscape = true;
					}
				}
				if( useLandscape || !usePortrait ) mSizes.AddRange(mSizesLandscape);
				if( usePortrait  || !useLandscape ) mSizes.AddRange(mSizesPortrait);
				SetWindowSize( mSizes[0] );
				GetMainGameView().Focus();
			}

			if( capturedFrames == mSizes.Count ) {
				// hack to skip frame
				if( waitCounter == 10 ) {
					SetWindowSize( mSizes[0] );
					captureCounter++;
					capturedFrames = 0;
					recording = false;
					Time.timeScale = savedTimeScale;
					waitCounter = 0;
				} else {
					waitCounter++;
				}
			} 

		} else {
			if( GUILayout.Button("INIT", GUILayout.Height(100)) ) {
				// hack to reset aspect ratio
				if( GetMainGameView() != null ) {
					GetMainGameView().Close();
				}
				EditorWindow.GetWindow(gameViewType);
				SetWindowSize( mSizes[0] );
				EditorApplication.isPlaying = true;
			}
		}
	}
	
	void Update () {
		if(recording) {
			if( IsInPlayMode() && capturedFrames < mSizes.Count ){
				SetWindowSize( mSizes[capturedFrames] );
				CaptureImages();
				Repaint();
			}
		}
	}

	void OnInspectorUpdate() {
		Repaint();
	}

	bool IsInPlayMode() {
		return EditorApplication.isPlaying && !EditorApplication.isPaused;
	}
	
	void CaptureImages() {
		string filename = System.IO.Path.Combine(savePath, getScreenShotName()+"_"+mSizes[capturedFrames].x+"x"+mSizes[capturedFrames].y + ".png");
		Application.CaptureScreenshot(filename);
		capturedFrames++;
	}

	string getScreenShotName() {
		return "Screenshot"+captureCounter;
	}

	bool IsWindowInit() 
	{
		EditorWindow gameView = GetMainGameView();
		if( gameView == null ) return false;
		Rect pos = gameView.position;
		Vector2 v2 = new Vector2(pos.width,pos.height-WINDOW_BORDER);
		return mSizes.Contains(v2);
	}

	void SetWindowSize( Vector2 size ) 
	{
		EditorWindow gameView = GetMainGameView();
		Rect pos = gameView.position;
		pos.x = 0;
		pos.y = 100;
		pos.width = size.x;
		pos.height = size.y + WINDOW_BORDER;
		gameView.position = pos;
	}

	EditorWindow GetMainGameView() 
	{
		if( gameViewType == null ) {
			gameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		}
		System.Reflection.MethodInfo GetMainGameView = gameViewType.GetMethod("GetMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetMainGameView.Invoke(null,null);
		return (EditorWindow)Res;
	}

}
