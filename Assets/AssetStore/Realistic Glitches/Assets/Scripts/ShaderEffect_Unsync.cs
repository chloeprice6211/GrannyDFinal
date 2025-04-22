using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_Unsync : MonoBehaviour {

	public enum Movement {JUMPING, SCROLLING, STATIC};
	public Movement movement = Movement.JUMPING;
	public float speed = 1;
	private float position = 0;
	private Material material;

	void Awake ()
	{
		material = new Material( Shader.Find("Hidden/VUnsync") );
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		switch (movement) {
			case Movement.JUMPING:
				position = Random.Range(-10,11) * 0.1f;
				break;
			case Movement.SCROLLING:
				position = (position + speed * Time.deltaTime * 0.5f) % 1;
				break;
			case Movement.STATIC:
				position = speed * 0.1f;
				break;
		}

		material.SetFloat("_ValueX", position);
		Graphics.Blit (source, destination, material);
	}
}
