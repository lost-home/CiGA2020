using System;
using UnityEngine;


public static class GameSetting {

	#region Application Setting

	public static bool RunInBackground { get; set; }

	#endregion

	#region Language Setting

	public static SystemLanguage Language { get; set; } = Application.systemLanguage;

	#endregion

	#region Graphics Setting

	public static bool           VSync            { get; set; }
	public static int            FrameRate        { get; set; }
	public static Vector2        Resolution       { get; set; }
	public static string         ScreenResolution { get; set; }
	public static bool           FullScreen       { get; set; }
	public static bool           HasBorder        { get; set; }
	public static FullScreenMode FullScreenMode   { get; set; }
	public static string         QualityLevel     { get; set; }
	public static int            PixelLightCount  { get; set; }

	#endregion

	#region Audio Setting

	public static float MasterVolume { get; set; }
	public static float MusicVolume  { get; set; }
	public static float SFXVolume    { get; set; }

	#endregion

	#region Debug Setting

	public static bool FastBoot                { get; set; }
	public static bool DeveloperConsole        { get; set; }

	#endregion

	public static void Load () {

		RunInBackground = Get ( "Application", "RunInBackground", false );
		
		Language = Get ( "Application", "Language", Application.systemLanguage );

		VSync            = Get ( "Graphics", "V-Sync", true );
		FrameRate        = Get ( "Graphics", "Fps", 60 );
		Resolution       = Get ( "Graphics", "Resolution", new Vector2 ( 1920, 1080 ) );
		FullScreen       = Get ( "Graphics", "FullScreen", true );
		HasBorder        = Get ( "Graphics", "HasBorder", true );
		FullScreenMode   = Get< FullScreenMode > ( "Graphics", "FullScreenMode", FullScreenMode.ExclusiveFullScreen );
		ScreenResolution = Get ( "Graphics", "ScreenResolution", "1920x1080" );
		QualityLevel     = Get ( "Graphics", "QualityLevel", "Good" );
		PixelLightCount  = Get ( "Graphics", "PixelLightCount", 30 );

		MasterVolume = Get ( "Audio", "MasterVolume", 1f );
		MusicVolume  = Get ( "Audio", "MusicVolume", 1f );
		SFXVolume    = Get ( "Audio", "SFXVolume", 1f );

		FastBoot                = Get ( "Debug", "FastBoot", false );
		DeveloperConsole        = Get ( "Debug", "Console", false );

		// 由于不支持的语言问题启动游戏黑屏
		switch ( Language ) {
			case SystemLanguage.Chinese:
			case SystemLanguage.ChineseSimplified:
			case SystemLanguage.ChineseTraditional:
			case SystemLanguage.English:
			case SystemLanguage.Japanese:
				break;
			default:
				Language = SystemLanguage.English;
				break;
		}
	}

	public static void Save () {
		Set ( "Application", "RunInBackground", RunInBackground );
		Set ( "Application", "Language",        Language );
		
		Set ( "Graphics", "Fps",              FrameRate );
		Set ( "Graphics", "V-Sync",           VSync );
		Set ( "Graphics", "Resolution",       $"{Resolution.x},{Resolution.y}" );
		Set ( "Graphics", "ScreenResolution", ScreenResolution );
		Set ( "Graphics", "FullScreen",       FullScreen );
		Set ( "Graphics", "HasBorder",        HasBorder );
		Set ( "Graphics", "FullScreenMode",   FullScreenMode );
		Set ( "Graphics", "QualityLevel",     QualityLevel );
		Set ( "Graphics", "PixelLightCount",  PixelLightCount );

		Set ( "Audio", "MasterVolume", MasterVolume );
		Set ( "Audio", "MusicVolume",  MusicVolume );
		Set ( "Audio", "SFXVolume",    SFXVolume );
		
#if UNITY_EDITOR
		Set ( "Debug", "FastBoot",                FastBoot );
		Set ( "Debug", "Console",                 DeveloperConsole );
#endif
	}

	private static string Get ( string key, string defaultValue ) {
		string outValue = defaultValue;

		if ( PlayerPrefs.HasKey ( key ) ) {
			return PlayerPrefs.GetString ( key );
		}
		
		return outValue;
	}

	private static bool Get ( string key, bool defaultValue = false ) {
		string outValue = string.Empty;
		
		if ( PlayerPrefs.HasKey ( key ) ) {
			outValue = PlayerPrefs.GetString ( key );
			if ( outValue.ToLower () == "true" || outValue.ToLower () == "false" ) {
				return bool.Parse ( outValue );
			}
		}

		return defaultValue;
	}

	private static bool Get ( string sectionName = "", string key = "", bool defaultValue = false ) {
		string outValue = PlayerPrefs.GetString ( $"{sectionName}.{key}" );

		if ( !string.IsNullOrEmpty ( outValue ) &&
		     ( outValue.ToLower () == "true" || outValue.ToLower () == "false" ) ) {
			return bool.Parse ( outValue );
		}
		else {
			return defaultValue;
		}
	}

	private static int Get ( string sectionName = "", string key = "", int defaultValue = default ( int ) ) {
		int outValue = PlayerPrefs.GetInt ( $"{sectionName}.{key}" );
		if ( PlayerPrefs.HasKey ( key ) ) {
			return outValue;
		}
		return defaultValue;
	}

	private static float Get ( string sectionName = "", string key = "", float defaultValue = default ( float ) ) {
		float outValue = PlayerPrefs.GetFloat ( $"{sectionName}.{key}" );
		if ( PlayerPrefs.HasKey ( key ) ) {
			return outValue;
		}
		return defaultValue;
	}
	
	private static T Get< T > ( string sectionName = "", string key = "", T defaultValue = default ( T ) ) {
		string outValue = PlayerPrefs.GetString ( $"{sectionName}.{key}" );

		try {
			return ( T )Enum.Parse ( typeof ( T ), outValue );
		}
		catch {
			return defaultValue;
		}
	}

	private static Vector2 Get ( string  sectionName  = "", string key = "",
	                             Vector2 defaultValue = default ( Vector2 ) ) {
		string outValue = PlayerPrefs.GetString ( $"{sectionName}.{key}" );

		try {
			return ParseUtil.ParseVector2 ( outValue );
		}
		catch {
			return defaultValue;
		}
	}
	
	private static void Set ( string sectionName, string key, bool val ) {
		PlayerPrefs.SetString ( $"{sectionName}.{key}", val.ToString () );
	}
	
	private static void Set ( string sectionName, string key, int val ) {
		PlayerPrefs.SetInt ( $"{sectionName}.{key}", val );
	}
	
	private static void Set ( string sectionName, string key, float val ) {
		PlayerPrefs.SetFloat ( $"{sectionName}.{key}", val );
	}
	
	private static void Set ( string sectionName, string key, string val ) {
		PlayerPrefs.SetString ( $"{sectionName}.{key}", val );
	}
	
	private static void Set ( string sectionName, string key, Enum val ) {
		PlayerPrefs.SetString ( $"{sectionName}.{key}", val.ToString () );
	}
	
	private static void Set ( string sectionName, string key, Vector2 val ) {
		PlayerPrefs.SetString ( $"{sectionName}.{key}", ParseUtil.WriteVector2 ( val ) );
	}
}