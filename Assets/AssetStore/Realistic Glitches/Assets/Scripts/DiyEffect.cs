using System.Collections;
using UnityEngine;


[System.Serializable]
public struct ShaderParams {
	public bool enabled;
	public Shader shader;
	public float intensity;
	public float valueX;
	public float valueY;
	public float valueZ;
	public bool switchV;
	public Color color;
	public Texture texture;
}

[ExecuteInEditMode]
public class DiyEffect : MonoBehaviour {


	public ShaderParams[] shaders;
	private Material[] materials;

	// Creates a private material used to the effect
	void Awake ()
	{
		materials = new Material[shaders.Length];
		for (int i = 0; i < shaders.Length; i++) {
			materials[i] = new Material (shaders[i].shader);
		}
	}

	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		for (int i = 0; i < shaders.Length; i++) {
			if (shaders[i].enabled) {
				materials[i].SetFloat("_Intensity", shaders[i].intensity);
				materials[i].SetFloat("_ValueX", shaders[i].valueX);
				materials[i].SetFloat("_ValueY", shaders[i].valueY);
				materials[i].SetFloat("_ValueZ", shaders[i].valueZ);
				materials[i].SetFloat ("_Switch", shaders[i].switchV ? 1 : 0);
				materials[i].SetColor("_Color", shaders[i].color);
				materials[i].SetTexture("_Texture", shaders[i].texture);

				Graphics.Blit (source, source, materials[i]);
			}
		}
		Graphics.Blit (source, destination);
	}
}
