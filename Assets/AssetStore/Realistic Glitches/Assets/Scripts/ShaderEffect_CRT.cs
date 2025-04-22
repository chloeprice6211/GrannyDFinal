using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_CRT : MonoBehaviour {

	public float scanlineIntensity = 100;
	public int scanlineWidth = 1;
	public Color scanlineColor = Color.black;
	public bool bulge = true;
	private Material material_Displacement;
	private Material material_Scanlines;

	void Awake ()
	{
		material_Displacement = new Material( Shader.Find("Hidden/CRTBulge") );
		material_Scanlines = new Material( Shader.Find("Hidden/Scanlines") );
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		material_Scanlines.SetFloat("_Intensity", scanlineIntensity * 0.01f);
		material_Scanlines.SetFloat("_ValueX", scanlineWidth);
		material_Scanlines.SetColor("_Color", scanlineColor);

		Graphics.Blit (source, source, material_Scanlines);
		if (bulge)
			Graphics.Blit (source, destination, material_Displacement);
		else
			Graphics.Blit (source, destination);

	}
}
