using UnityEngine;
using System.Collections;

public class UKClickable : MonoBehaviour {
	
	public bool requireReceiver = true;
	public bool includeSendTarget = false;

	public Camera targetCamera;
	public GameObject receiver;

    public string ClickMessage;
	
	public string methodeNameWithoutTarget = "OnClick";
	public string methodeNameWithTarget = "OnClickWithTarget";

	void Update () {
		if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 ) // check for left-mouse
		{
			
			Vector3 pos = Input.mousePosition;
			if( Input.touchCount > 0 ) {
				Touch t = Input.GetTouch(0);
				pos = new Vector3( t.position.x, t.position.y, 0 );
				if( t.phase != TouchPhase.Began ) return;
			}

		    var ray = targetCamera.ScreenPointToRay( pos );
		    RaycastHit hit;
		    if (collider && collider.Raycast (ray, out hit, float.MaxValue))
		    {
                if (!string.IsNullOrEmpty(ClickMessage)) UKMessenger.Broadcast(ClickMessage);

				SendMessageOptions smo = requireReceiver? SendMessageOptions.RequireReceiver: SendMessageOptions.DontRequireReceiver;
				if( receiver == null ) {
					if( includeSendTarget ) {
			        	SendMessageUpwards( methodeNameWithTarget, gameObject, smo );
					} else {
						SendMessageUpwards( methodeNameWithoutTarget, smo );
					}
				} else {
					if( includeSendTarget ) {
			        	receiver.SendMessage( methodeNameWithTarget, gameObject, smo );
					} else {
						receiver.SendMessage( methodeNameWithoutTarget, smo );
					}
				}
		    }
		}
	}
}
