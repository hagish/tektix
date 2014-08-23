using UnityEngine;
using System.Collections;

public class UKSocialWrapper {

	public static bool isAuthenticate = false;
	
	public delegate void EventSozial();
	public static event EventSozial onLogin;

	public static void Init () 
	{
		Social.localUser.Authenticate( success => {
			isAuthenticate = success && Application.platform == RuntimePlatform.IPhonePlayer;
			Debug.Log ( "SocialWrapper::processAuthentication: "+isAuthenticate+" "+success+" "+onLogin );
			if( onLogin != null ) onLogin();
		} );
    }
	
	public static void CommitLeaderboard( string board, long score ) 
	{
		if( Social.localUser.authenticated ) 
		{

		    Social.ReportScore( score, board, success => {
		        Debug.Log( "SocialWrapper::commitHighscore: "+(success ? "Reported score successfully!" : "Failed to report score!") );
		    });
		}
		else
		{
			Debug.Log( "SocialWrapper::commitHighscore: User not authenticated!" );
		}
	}
	
	
	public static void CommitAchievement( string id, float status ) 
	{
		if( Social.localUser.authenticated ) 
		{
			try {
			    Social.ReportProgress( id, status, success => {
			        Debug.Log( "SocialWrapper::commitAchievments: "+(success ? "Reported achievement successfully!" : "Failed to report achievement!") );
			    });
			} catch ( System.Exception e ) {
				Debug.Log( "SocialWrapper::commitAchievments: "+e );
			}
		}
		else
		{
			Debug.Log( "SocialWrapper::commitAchievments: User not authenticated!" );
		}
	}
	
	
	
	public static void ShowLeaderboardUI() 
	{
		if( Social.localUser.authenticated ) 
		{
			Social.ShowLeaderboardUI();
		}
		else
		{
			Debug.Log( "SocialWrapper::showLeaderboardUI: User not authenticated!" );
		}
	}
	
	public static void ShowAchievementsUI() 
	{
		if( Social.localUser.authenticated ) 
		{
			Social.ShowAchievementsUI();
		}
		else
		{
			Debug.Log( "SocialWrapper::showAchievementsUI: User not authenticated!" );
		}
	}

}
