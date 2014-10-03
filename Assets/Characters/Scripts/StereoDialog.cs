/************************************************************************************

Filename    :   StereoDialog.cs
Content     :   3D dialog supporting sterescopic views
Created     :   25 August 2014
Authors     :   Chris Julian Zaharia

Place on a game object facing the camera.
************************************************************************************/

using UnityEngine;
using System.Collections;

public class StereoDialog : MonoBehaviour {
    public Font font;
    public Material fontMaterial;
    public Vector3 localScale = new Vector3 (10f, 10f, 10f);    
    public TextAlignment defaultAlignment = TextAlignment.Left;
    public int defaultFontSize = 0;
    public FontStyle defaultFontStyle = FontStyle.Normal;

    public void Create (float x, float y, string text, Color color, string name = "StereoDialog", float destroyTimer = 0) {
        Create (x, y, text, color, defaultAlignment, defaultFontSize, defaultFontStyle, name, destroyTimer);
    }

    public void Create (float x, float y, string text, Color color, TextAlignment alignment,
                        int fontSize, FontStyle fontStyle, string name = "StereoDialog", float destroyTimer = 0) {
        GameObject dialog = new GameObject ();
        dialog.name = name;

        TextMesh textMesh = (TextMesh)dialog.AddComponent (typeof(TextMesh));

        textMesh.text = text;
        textMesh.color = color;
        textMesh.font = font;
        
        textMesh.alignment = alignment;
        textMesh.fontSize = fontSize;
        textMesh.fontStyle = fontStyle;

        dialog.renderer.material = fontMaterial;

        dialog.transform.parent = transform;
        dialog.transform.localPosition = new Vector3 (x, y, 0);
        dialog.transform.localRotation = new Quaternion (0, 0, 0, 0);
        dialog.transform.localScale = localScale;

        if (destroyTimer > 0)
            Destroy (dialog, destroyTimer);
    }
}
