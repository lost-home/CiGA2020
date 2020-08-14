using UnityEngine;
using UnityEngine.UI;

static public class AlphaExtensions {
	public static void SetAlpha (this Color color, float alpha) {
		//Color next = color;
		//next.a = Mathf.Clamp01 ( alpha );
		//color = next;
		color.a = alpha;
	}

	public static void SetAlpha (this SpriteRenderer self, float alpha) {
		Color next = self.color;
		next.a = Mathf.Clamp01 ( alpha );
		self.color = next;
	}

	public static void SetAlpha (this Image self, float alpha) {
		Color next = self.color;
		next.a = Mathf.Clamp01 ( alpha );
		self.color = next;
	}
	
	public static void SetAlpha (this RawImage self, float alpha) {
		Color next = self.color;
		next.a     = Mathf.Clamp01 ( alpha );
		self.color = next;
	}

	public static void SetAlpha (this Text self, float alpha) {
		Color next = self.color;
		next.a = Mathf.Clamp01 ( alpha );
		self.color = next;
	}

	public static void SetAlpha (this tk2dTextMesh self, float alpha) {
		Color next = self.color;
		next.a = Mathf.Clamp01 ( alpha );
		self.color = next;
	}

	public static Color ColorTranslate ( Color a, Color b, EaseUtils.EaseType easeType, float t ) {
		Color ret = a;
		ret.r = EaseUtils.Ease ( easeType, a.r, b.r, t );
		ret.g = EaseUtils.Ease ( easeType, a.g, b.g, t );
		ret.b = EaseUtils.Ease ( easeType, a.b, b.b, t );
		ret.a = EaseUtils.Ease ( easeType, a.a, b.a, t );
		return ret;
	}
}
