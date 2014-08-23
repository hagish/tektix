using UnityEngine;
using System.Collections;

public class UKColorPicker : MonoBehaviour {

	public Transform planeForPicker;
	public Camera cameraForRayCast;
	public int textureSize = 128;
	public int resolution = 21;

	public Color lastPickedColor { get; private set; }

	public delegate void EventColorPicker( Color c );
	public delegate void EventColorPickerRelease();
	public event EventColorPicker onPress;
	public event EventColorPicker onMove;
	public event EventColorPickerRelease onRelease;
	
	private Texture2D mTexture;
	private bool mHasUsableTransform = true;
	
	void Start () 
	{
		if( planeForPicker == null ) {
			Debug.LogWarning("No transform applied!");
			mHasUsableTransform = false;
		}

		if( planeForPicker.GetComponent<Collider>() == null ) {
			Debug.LogWarning("No collider found!");
			mHasUsableTransform = false;
		}

		if( planeForPicker.renderer == null ) {
			Debug.LogWarning("No renderer found!");
			mHasUsableTransform = false;
		}

		if( mHasUsableTransform ) {
			CreateTexture();
			planeForPicker.renderer.material.mainTexture = mTexture;
		}

		planeForPicker.renderer.material.mainTexture = mTexture;
	}

	void Update() 
	{

		if( mHasUsableTransform ) {
			if( Input.GetMouseButton( 0 ) ) {
				if( onMove != null ) {
					lastPickedColor = GetColor();
					if( lastPickedColor != Color.clear ) onMove( lastPickedColor );
				}
			}
			if( Input.GetMouseButtonDown( 0 ) ) {
				lastPickedColor = GetColor();
				if( onPress != null ) {
					if( lastPickedColor != Color.clear ) onPress( lastPickedColor );
				}
			}
			if( Input.GetMouseButtonUp( 0 ) ) {
				if( onRelease != null ) {
					onRelease();
				}
			}
		}

	}

	Color GetColor() 
	{
		Color color = Color.clear;
		RaycastHit hit;
		if( Physics.Raycast( cameraForRayCast.ScreenPointToRay( Input.mousePosition ), out hit ) ) {
			if( hit.collider == planeForPicker.collider ) {
				Vector2 pixelUV = hit.textureCoord;
				pixelUV.x *= textureSize;
				pixelUV.y *= textureSize;
				color = mTexture.GetPixel( (int)pixelUV.x, (int)pixelUV.y );
			}
		}

		return color;
	}

	void CreateTexture() 
	{
		mTexture = new Texture2D(textureSize,textureSize);
		mTexture.wrapMode = TextureWrapMode.Clamp;
		drawRect(mTexture,0,0,textureSize,textureSize,Color.clear);
		
		int blockAmount = resolution * 6;
		int blockSize = (int)((float)textureSize / (float)blockAmount);
		Color color = Color.clear;
		for( int y=0; y<blockAmount; y++ ) {
			for( int xSegment=0; xSegment<resolution; xSegment++ ) {
				for( int xBlock=0; xBlock<6; xBlock++ ) {
					float a = 1 * ((float)(xSegment)/(float)resolution);
					
					switch( xBlock ) {
						
					case 0: color = new Color( 1, a, 0 ); break;
					case 1: color = new Color( 1-a, 1, 0 ); break;
						
					case 2: color = new Color( 0, 1, a ); break;
						
					case 3: color = new Color( 0, 1-a, 1 ); break;
						
					case 4: color = new Color( a, 0, 1 ); break;
					case 5: color = new Color( 1, 0, 1-a ); break;
					}
					
					float t = (float)y/(float)blockAmount;
					
					if( y > blockAmount * 0.5f ) {
						color = Color.Lerp( color, Color.black, UKMathHelper.MapIntoRange( t, 0.5f, 1, 0, 1 ) );
					}
					
					if( y < blockAmount * 0.5f  || true ) {
						color = Color.Lerp( Color.white, color, UKMathHelper.MapIntoRange( t, 0, 0.5f, 0, 1 ) );
					}
					
					drawRect(mTexture, (xBlock*blockSize*resolution) + (xSegment*blockSize), y*blockSize-1, blockSize, blockSize, color);
					
				}
			}
		}
		mTexture.Apply();
	}
	
	void drawRect( Texture2D tex, int xpos, int ypos, int width, int height, Color color , bool apply=false ) 
	{
		for( int x=0; x<width; x++ ) {
			for( int y=0; y<height; y++ ) {
				tex.SetPixel(xpos+x, ypos+y, color);
			}
		}
	}
}
