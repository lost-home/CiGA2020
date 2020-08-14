// simple fading script
// A texture is stretched over the entire screen. The color of the pixel is set each frame until it reaches its target color.

using UnityEngine;
using System.Collections;


/// <summary>
/// 淡入淡出效果
/// </summary>
public class CameraFade : SingletonBehaviour< CameraFade > {
	private       GUIStyle  m_BackgroundStyle = new GUIStyle ();        // Style for background tiling
	private       Texture2D m_FadeTexture;                // 1x1 pixel texture used for fading
	public static Color     FullyTransparrentColor = new Color ( 0, 0, 0, 0 );  //全透明颜色

	[SerializeField]
	private Color
		m_CurrentScreenOverlayColor = new Color ( 0, 0, 0, 0 ); // default starting color: black and fully transparrent

	private Color
		m_TargetScreenOverlayColor = new Color ( 0, 0, 0, 0 ); // default target color: black and fully transparrent

	private Color
		m_DeltaColor =
			new Color ( 0, 0, 0,
				0 ); // the delta-color is basically the "speed / second" at which the current color should change

	private int m_FadeGUIDepth = -1000; // make sure this texture is drawn on top of everything

	public static float alpha => Instance.m_CurrentScreenOverlayColor.a;


	// initialize the texture, background-style and initial color:
	protected override void Awake () {
		Persistent = true;

		base.Awake ();

		m_FadeTexture                       = new Texture2D ( 1, 1 );
		m_BackgroundStyle.normal.background = m_FadeTexture;
		SetScreenOverlayColor ( m_CurrentScreenOverlayColor );

		// TEMP:
		// usage: use "SetScreenOverlayColor" to set the initial color, then use "StartFade" to set the desired color & fade duration and start the fade
		//SetScreenOverlayColor(new Color(0, 0, 0, 1));
		//StartFade(new Color(0, 0, 0, 1), 2.5f);
	}


	// draw the texture and perform the fade:
	private void OnGUI () {
		// if the current color of the screen is not equal to the desired color: keep fading!
		if ( m_CurrentScreenOverlayColor != m_TargetScreenOverlayColor ) {
			// if the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
			if ( Mathf.Abs ( m_CurrentScreenOverlayColor.a - m_TargetScreenOverlayColor.a ) <
			     Mathf.Abs ( m_DeltaColor.a ) * Time.deltaTime ) {
				m_CurrentScreenOverlayColor = m_TargetScreenOverlayColor;
				SetScreenOverlayColor ( m_CurrentScreenOverlayColor );
				m_DeltaColor = new Color ( 0, 0, 0, 0 );
			}
			else {
				// fade!
				SetScreenOverlayColor ( m_CurrentScreenOverlayColor + m_DeltaColor * Time.deltaTime );
			}
		}

		// only draw the texture when the alpha value is greater than 0:
		if ( m_CurrentScreenOverlayColor.a > 0 ) {
			GUI.depth = m_FadeGUIDepth;
			GUI.Label ( new Rect ( -10, -10, Screen.width + 10, Screen.height + 10 ), m_FadeTexture,
				m_BackgroundStyle );
		}
	}


	// instantly set the current color of the screen-texture to "newScreenOverlayColor"
	// can be usefull if you want to start a scene fully black and then fade to opague
	public static void SetScreenOverlayColor ( Color newScreenOverlayColor ) {
		Instance.m_CurrentScreenOverlayColor = newScreenOverlayColor;
		Instance.m_FadeTexture.SetPixel ( 0, 0, Instance.m_CurrentScreenOverlayColor );
		Instance.m_FadeTexture.Apply ();
	}


	// initiate a fade from the current screen color (set using "SetScreenOverlayColor") towards "newScreenOverlayColor" taking "fadeDuration" seconds
	public static void StartFade ( Color newScreenOverlayColor, float fadeDuration ) {
		if ( fadeDuration <= 0.0f ) // can't have a fade last -2455.05 seconds!
		{
			SetScreenOverlayColor ( newScreenOverlayColor );
		}
		else // initiate the fade: set the target-color and the delta-color
		{
			Instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
			Instance.m_DeltaColor = ( Instance.m_TargetScreenOverlayColor - Instance.m_CurrentScreenOverlayColor ) /
			                        fadeDuration;
		}
	}

	// ********************************************************************
	// 淡出(黑->全透明)
	// ********************************************************************
	public static void FadeOut ( float fadeDuration = 0.5f ) {
		SetScreenOverlayColor ( Color.black );
		StartFade ( FullyTransparrentColor, fadeDuration );
	}

	/// <summary>
	/// 淡出(黑->全透明)
	/// </summary>
	/// <param name="fadeDuration"></param>
	/// <returns></returns>
	public static IEnumerator CFadeOut ( float fadeDuration = 0.5f ) {
		FadeOut ( fadeDuration );
		yield return new WaitForSeconds ( fadeDuration );
	}

	// ********************************************************************
	// 淡入(全透明->黑)
	// ********************************************************************
	public static void FadeIn ( float fadeDuration = 0.5f ) {
		SetScreenOverlayColor ( FullyTransparrentColor );
		StartFade ( Color.black, fadeDuration );
	}

	/// <summary>
	/// 淡入(全透明->黑)
	/// </summary>
	/// <param name="fadeDuration"></param>
	/// <returns></returns>
	public static IEnumerator CFadeIn ( float fadeDuration = 0.5f ) {
		FadeIn ( fadeDuration );
		yield return new WaitForSeconds ( fadeDuration );
	}
	
	public static IEnumerator CFade ( Color startColor, Color endColor, float fadeDuration = 0.5f ) {
		SetScreenOverlayColor ( startColor );
		StartFade ( endColor, fadeDuration );
		yield return new WaitForSeconds ( fadeDuration );
	}

	// ********************************************************************
	// 淡入淡出(全透明->黑->全透明)
	// ********************************************************************
	public static IEnumerator CFadeInToOut ( float         fadeInTime, float afterInDelayTime, float fadeOutTime,
	                                         System.Action completedCallback ) {
		yield return Instance.StartCoroutine ( CFadeIn ( fadeInTime ) );
		yield return new WaitForSeconds ( fadeInTime );
		yield return new WaitForSeconds ( afterInDelayTime );
		yield return Instance.StartCoroutine ( CFadeOut ( fadeOutTime ) );
		completedCallback?.Invoke ();
	}
}
