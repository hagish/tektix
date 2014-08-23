using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Example:
 * UKUIHelper.Instance.Init( GameObject.Find("GUICamera").GetComponent<Camera>() );
 * UKUIHelper.Instance.SetPosition( trans, Vector2.zero, UKUIHelper.Align.LeftBottom );
 * UKUIHelper.Instance.UpdatePositions();
 */

public class UKUIHelper {

	public enum Align { LeftTop, LeftMiddle, LeftBottom, CenterTop, CenterMiddle, CenterBottom, RightTop, RightMiddle, RightBottom }

	private int screenWidthOld;
	private int screenHeightOld;

	private Camera cameraHud;
	private float units;
	private float pixelsPerUnit;
	private float unitsPerPixel;
	private float aspect;

	private List<UpdateInformation>autoUpdateList = new List<UpdateInformation>();
	
	struct UpdateInformation {
		public Transform transform;
		public Vector2 position;
		public Align align;
	}

	public UKUIHelper () {
	}

	private static UKUIHelper _instance;
	public static UKUIHelper Instance 
	{
		get {
			if (_instance == null) {
				_instance = new UKUIHelper();
			}
			return _instance;
		}
	}

	public void Init( Camera cam ) 
	{

		if( cam.transform.rotation != Quaternion.identity ) {
			Debug.LogError ( "UKUIHelper: Set your camera rotation to 0!" );
			return;
		}
		if( !cam.isOrthoGraphic ) {
			Debug.LogError ( "UKUIHelper: Set your camera to orthographic!" );
			return;
		}

		screenWidthOld = Screen.width; 
		screenHeightOld = Screen.height;

		cameraHud = cam;
		UpdateAspectValues();
	}

	public void SetPosition( Transform trans,  Vector2 pos, Align align=Align.LeftTop ) 
	{
		if( cameraHud == null ) {
			Debug.LogError( "Call UKUIHelper.Instance.Init( Camera cam ); first!" );
			return;
		}
		if( trans.root.rotation != Quaternion.identity ) {
			Debug.LogError ( "UKUIHelper: Set your HUD rotation to 0!" );
			return;
		}


		UpdateInformation ui = new UpdateInformation(){
			transform = trans,
			position = pos,
			align = align
		};


		autoUpdateList.Add( ui );
		positionTransform( ui );
	}

	public void UpdatePositions( bool justOnScreenSizeChange=true ) 
	{
		int screenWidth = Screen.width; 
		int screenHeight = Screen.height;

		if( screenWidthOld != screenWidth  || screenHeightOld != screenHeight || !justOnScreenSizeChange ){
			screenWidthOld = screenWidth; 
			screenHeightOld = screenHeight;
			UpdateAspectValues();

			foreach( UpdateInformation ui in autoUpdateList ) {
				positionTransform( ui );
			}
		}
	}

	private void UpdateAspectValues() 
	{
		units = cameraHud.orthographicSize;
		pixelsPerUnit = Screen.height * 0.5f / units; 
		unitsPerPixel = 1f / pixelsPerUnit;
		aspect = (float)Screen.width / (float)Screen.height;
	}

	private void positionTransform( UpdateInformation ui ) 
	{
		Vector2 offset = GetAlignOffset( ui.align );

        if (ui.transform != null)
        {
		    ui.transform.localPosition = new Vector3(ui.position.x * unitsPerPixel + offset.x, 
		                                             ui.position.y * unitsPerPixel + offset.y, 
		                                             ui.transform.localPosition.z );
        }
	}

	private Vector2 GetAlignOffset( Align align ) 
	{
		switch( align ) {
			case Align.LeftTop: return new Vector2( -units * aspect, units ); 
			case Align.LeftMiddle: return new Vector2( -units * aspect, 0 ); 
			case Align.LeftBottom: return new Vector2( -units * aspect, -units );
			case Align.CenterTop: return new Vector2( 0, units ); 
			case Align.CenterMiddle: return new Vector2( 0, 0 ); 
			case Align.CenterBottom: return new Vector2( 0, -units );
			case Align.RightTop: return new Vector2( units * aspect, units ); 
			case Align.RightMiddle: return new Vector2( units * aspect, 0 ); 
			case Align.RightBottom: return new Vector2( units * aspect, -units );
			default: return Vector2.zero;
		}
	}

}
