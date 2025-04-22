using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_Tint : MonoBehaviour {

	public float y = 1;
	public float u = 1;
	public float v = 1;
	public bool swapUV = false;
	private Material material;
	public Shader tintShader;

	// Creates a private material used to the effect
	void Awake ()
	{
		material = new Material( tintShader );
	}

	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{

		material.SetFloat("_ValueX", y);
		material.SetFloat("_ValueY", u);
		material.SetFloat("_ValueZ", v);
		material.SetFloat ("_Switch", swapUV ? 1 : 0);

		Graphics.Blit (source, destination, material);
	}
}
