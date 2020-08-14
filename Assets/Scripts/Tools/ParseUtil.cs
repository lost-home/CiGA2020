using System;
using System.Globalization;
using UnityEngine;


/// <summary>
/// 转换工具类
/// </summary>
static public class ParseUtil {
	
	static readonly CultureInfo StaticCultureInfo = CultureInfo.CreateSpecificCulture ( "zh-CN" );
	// static CultureInfo StaticCultureInfo = CultureInfo.CreateSpecificCulture ( "en-US" );

	static ParseUtil () {
		
	}
	
	/// <summary>
	/// 尝试将Object转换为Int值
	/// </summary>
	/// <param name="intValue"></param>
	static public int ParseInt ( string intValue ) {
		try {
			if ( string.IsNullOrEmpty ( intValue ) ) {
				return 0;
			}
			if ( intValue.Contains ( "." ) ) {
				return ( int )ParseFloat ( intValue );
			}

			// return Convert.ToInt32 ( intValue, NumberFormatInfo.InvariantInfo );
			return Convert.ToInt32 ( intValue, StaticCultureInfo );
		}
		catch ( Exception EX_NAME ) {
#if UNITY_EDITOR
			//if (!intValue.ToString().Equals("——"))
			Debug.LogWarning ( "Invaild Int:" + intValue + " " + EX_NAME.Message );
#endif
			return 0;
		}
	}

	/// <summary>
	/// 尝试将Object转换为Int值
	/// </summary>
	/// <param name="intValue"></param>
	static public int ParseInt ( System.Object intValue ) {
		try {
			// return Convert.ToInt32 ( intValue, NumberFormatInfo.InvariantInfo );
			return Convert.ToInt32 ( intValue, StaticCultureInfo );
		}
		catch ( Exception EX_NAME ) {
#if UNITY_EDITOR
			if ( !intValue.ToString ().Equals ( "——" ) )
				Debug.LogWarning ( "Invaild Int:" + intValue + " " + EX_NAME.Message );
#endif
			return 0;
		}
	}

	/// <summary>
	/// 尝试将Object转换为Float值
	/// </summary>
	/// <param name="floatValue"></param>
	static public float ParseFloat ( string floatValue ) {
		try {
			if ( string.IsNullOrEmpty ( floatValue ) )
				return 0f;

			// return Convert.ToSingle ( floatValue, NumberFormatInfo.InvariantInfo );
			return Convert.ToSingle ( floatValue, StaticCultureInfo );
		}
		catch ( Exception EX_NAME ) {
#if UNITY_EDITOR
			//if (!floatValue.ToString().Equals("——"))
			Debug.LogWarning ( "Invaild Float:" + floatValue + " " + EX_NAME.Message );
#endif
			return 0;
		}
	}

	/// <summary>
	/// 尝试将Object转换为Float值
	/// </summary>
	/// <param name="floatValue"></param>
	static public float ParseFloat ( System.Object floatValue ) {
		try {
			// return Convert.ToSingle ( floatValue, NumberFormatInfo.InvariantInfo );
			return Convert.ToSingle ( floatValue, StaticCultureInfo );
		}
		catch ( Exception EX_NAME ) {
#if UNITY_EDITOR
			if ( !floatValue.ToString ().Equals ( "——" ) )
				Debug.LogWarning ( "Invaild Float:" + floatValue + " " + EX_NAME.Message );
#endif
			return 0;
		}
	}

	/// <summary>
	/// 转换float区间字符串(0.5,0.5)
	/// </summary>
	/// <param name="floatValue"></param>
	/// <returns>指定区间大小的数组</returns>
	static public float[] ParseFloatInterval ( System.Object floatValue, int intervalSize = 2, char Separator = ',' ) {
		try {
			string[] intervalStr = floatValue.ToString ().Split ( Separator );
			var      firstValue  = Convert.ToSingle ( intervalStr [ 0 ], NumberFormatInfo.InvariantInfo );

			float[] returnInterval = new float[ intervalSize ];

			for ( var i = 0; i < returnInterval.Length; i++ )
				returnInterval [ i ] = firstValue;

			for ( var i = 0; i < intervalStr.Length; i++ ) {
				if ( i >= returnInterval.Length )
					break;
				returnInterval [ i ] = Convert.ToSingle ( intervalStr [ i ], NumberFormatInfo.InvariantInfo );
			}

			return returnInterval;
		}
		catch ( Exception EX_NAME ) {
#if UNITY_EDITOR
			if ( !floatValue.ToString ().Equals ( "——" ) )
				Debug.LogWarning ( "Invaild Float:" + floatValue + " " + EX_NAME.Message );
#endif
			return null;
		}
	}

	/// <summary>
	/// 解析字符串坐标
	/// </summary>
	/// <param name="vec2">eg: 20,20</param>
	static public Vector2 ParseVector2 ( string vec2, char Separator = ',' ) {
		
		var array = vec2.Split ( Separator );

#if UNITY_EDITOR
		if ( array.Length != 2 ) {
			Debug.LogError ( $"ParseUtil.ParseVector2( {vec2}, '{Separator}' ) Error!" );
			return default ( Vector2 );
		}
#endif

		// var x = float.Parse ( array [ 0 ], NumberFormatInfo.InvariantInfo );
		// var y = float.Parse ( array [ 1 ], NumberFormatInfo.InvariantInfo );
		var x = ParseFloat ( array [ 0 ] );
		var y = ParseFloat ( array [ 1 ] );
		return new Vector2 ( x, y );
	}

	/// <summary>
	/// 解析字符串坐标
	/// </summary>
	/// <param name="vec3">eg: 20,20,20</param>
	static public Vector3 ParseVector3 ( string vec3, char Separator = ',' ) {
		var array = vec3.Split ( Separator );
		var x     = ParseFloat ( array [ 0 ] );
		var y     = ParseFloat ( array [ 1 ] );
		var z     = ParseFloat ( array [ 2 ] );
		return new Vector3 ( x, y, z );
	}

	/// <summary>
	/// 解析字符串颜色
	/// </summary>
	/// <param name="rgbStr">eg: 255,255,255 or 255,255,255,1</param>
	/// <returns>Color</returns>
	static public Color ParseColor32 ( string rgbStr, char Separator = ',' ) {
		try {
			var array = rgbStr.Split ( Separator );
			var r     = ( byte ) int.Parse ( array [ 0 ], NumberFormatInfo.InvariantInfo );
			var g     = ( byte ) int.Parse ( array [ 1 ], NumberFormatInfo.InvariantInfo );
			var b     = ( byte ) int.Parse ( array [ 2 ], NumberFormatInfo.InvariantInfo );
			var a = array.Length > 3
				? ( byte ) int.Parse ( array [ 3 ], NumberFormatInfo.InvariantInfo )
				: ( byte ) 1;
			return new Color32 ( r, g, b, a );
		}
		catch ( Exception e ) {
			Debug.LogError ( $"解析Color32出现问题:{rgbStr} {e.Message}" );
			return new Color32 ();
		}
	}
	
	static public void ParseBackgroundFadeEvent ( string    evt, out float fadeInTime,
	                                              out float afterInDelayTime,
	                                              out float fadeOutTime, out float afterOutDelayTime ) {
		// 参数使用空白分割
		var args = evt.Split ( ' ' );

		try {
			// 参数0是指令参数，通常为bgfade
			fadeInTime        = ParseUtil.ParseFloat ( args [ 1 ] );
			afterInDelayTime  = ParseUtil.ParseFloat ( args [ 2 ] );
			fadeOutTime       = ParseUtil.ParseFloat ( args [ 3 ] );
			afterOutDelayTime = ParseUtil.ParseFloat ( args [ 4 ] );
		}
		catch ( Exception e ) {
			fadeInTime        = 0.5f;
			afterInDelayTime  = 0.2f;
			fadeOutTime       = 0.3f;
			afterOutDelayTime = 0f;
			Debug.LogError ( $"解析BackgroundFade事件字符串失败:{ evt } {e.Message}" );
		}
	}


	static public string PrintFloat ( float value, int dp ) {
		return value.ToString ( $"n{dp}" );
	}

	static public string PrintVector2 ( Vector2 value, int dp ) {
		return $"( {PrintFloat ( value.x, dp )}, {PrintFloat ( value.y, dp )} )";
	}
	
	static public string PrintXNAVector2 ( Vector2 value, int dp ) {
		return $"( X:{PrintFloat ( value.x, dp )}, Y:{PrintFloat ( value.y, dp )} )";
	}
	
	static public string WriteVector2 ( Vector2 value ) {
		return $"{value.x.ToString ()},{value.y.ToString ()}";
	}
}
