using System.IO;
using UnityEngine;
using UnityEditor;


public static class EditorUtils {
	
	[MenuItem ( "Tools/保存prefab图片 %T", false, 0 )]
	public static void Show () {
		
		foreach ( var selectionObj in Selection.gameObjects ) {
			if ( selectionObj != null ) {
				var texture = AssetPreview.GetAssetPreview ( selectionObj );
				// AssetDatabase.CreateAsset ( texture, Path.Combine ( "Assets", $"{selectionObj.name}.png" ) );
				File.WriteAllBytes ( Path.Combine ( Application.dataPath, $"{selectionObj.name}.png" ), texture.EncodeToPNG () );
			}
		}
		
	}
}
