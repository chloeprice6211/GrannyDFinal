using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect : MonoBehaviour {

	public float intensity;
	public Color color;
	public int linesSize = 1;
	public Shader shader;
	public float value;
	public Texture displacementTex;
	private Material material;

	// Creates a private material used to the effect
	void Awake ()
	{
		material = new Material( shader );
	}

	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (intensity == 0)
		{
			Graphics.Blit (source, destination);
			return;
		}

		material.SetFloat("_Blend", intensity);
		material.SetColor("_Color", color);
		material.SetInt("_LinesSize", linesSize);
		material.SetFloat("_Value", value);
		material.SetTexture("_DisplacementTex", displacementTex);

		Graphics.Blit (source, destination, material);
	}
}
