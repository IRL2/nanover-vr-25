using UnityEngine;

public class ReferenceLineAnimator : MonoBehaviour
{
    [SerializeField]
    Material material;
    
    string textureName = "_MainTex";

    float offset = 0.0f;

    Vector2 horizontalVector = new Vector2 (1, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textureName = material.GetTexturePropertyNames()[0];
    }

    // Update is called once per frame
    void Update()
    {
        material.SetTextureOffset(textureName, horizontalVector * offset);
        offset -= Time.deltaTime * 0.1f; // Adjust speed as needed
        offset %= 1.0f; // Keep offset within [0, 1] range to avoid overflow
    }
}
