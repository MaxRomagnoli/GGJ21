using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TextureSwiper : MonoBehaviour
{
    [SerializeField] private bool startAsActive = true;
    private GameManager gameManager;
    private Vector2 currentTexturePosition = new Vector2();
    private MeshRenderer meshRenderer;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        Activate(startAsActive);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!active) { return; }
        currentTexturePosition += gameManager.TextureSpeed() * Time.deltaTime;
        meshRenderer.material.SetTextureOffset("_MainTex", currentTexturePosition);
    }

    public void Activate(bool _active)
    {
        active = _active;
        this.gameObject.tag = _active ? "belt" : "Untagged";
    }
}
