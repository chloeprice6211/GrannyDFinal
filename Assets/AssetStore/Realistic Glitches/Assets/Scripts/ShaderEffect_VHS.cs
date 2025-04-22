using System.Collections;
using UnityEngine;

public class ShaderEffect_VHS : MonoBehaviour {

	public float distortion = 1;
	public float height = 100;
	public float width = 10;
	public bool colorBleeding = true;
	private Material material_Bleeding;
	private Material material_Waves;

	void Awake ()
	{
		material_Bleeding = new Material(Shader.Find("Hidden/BleedingColors"));
		material_Waves = new Material(Shader.Find("Hidden/Waves"));
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		material_Waves.SetFloat("_Intensity", distortion * 0.1f);
		material_Waves.SetFloat("_ValueX", height * 0.01f);
		material_Waves.SetFloat("_ValueY", 1073741824 / (Mathf.Pow(width * 0.1f, 8)));
		material_Waves.SetFloat("_ValueZ", Time.unscaledDeltaTime);

		if (colorBleeding)
			Graphics.Blit (source, source, material_Bleeding);
		Graphics.Blit (source, destination, material_Waves);
	}
}
