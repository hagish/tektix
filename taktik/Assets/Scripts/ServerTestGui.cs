using UnityEngine;
using System.Collections;

public class ServerTestGui : MonoBehaviour {
    public string Text;

	// Use this for initialization
	void OnEnable () {
	    gameObject.GetComponent<Server>().EvReceiveMessage += ServerTestGui_EvReceiveMessage;
	}

    void OnDisable()
    {
        gameObject.GetComponent<Server>().EvReceiveMessage -= ServerTestGui_EvReceiveMessage;
    }

    private void ServerTestGui_EvReceiveMessage(string message)
    {
        Text += message.Trim() + "\n";
    }	

    void OnGUI()
    {
        if (GUILayout.Button("CLEAR")) Text = "";
        GUILayout.Box(Text);
    }
}
