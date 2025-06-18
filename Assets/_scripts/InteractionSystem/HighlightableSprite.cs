using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[RequireComponent(typeof(SpriteRenderer))]
public class HighlightableSprite : MonoBehaviour
{
    private Material startMaterial;
    public UnityAction OnHoverOver;
    public UnityAction OnHoverOut;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private BoxCollider _collider;
   
   
    private void Awake()
    {
        
        SpriteRenderer = GetComponent<SpriteRenderer>();
        startMaterial = SpriteRenderer.material;
        _collider = GetComponent<BoxCollider>();
        UpdateCollider();
    }
    
    public void UpdateCollider()
    {
        if (_collider != null)
        {
            Bounds bounds = SpriteRenderer.localBounds;
            Vector3 newSize = bounds.size;
            newSize.z = 0.1f;
            _collider.size = newSize;
            _collider.center = bounds.center;
        }
    }

    public void Hover()
    {
        if(SpriteRenderer == null) return; 
        SpriteRenderer.material = PlayerActionHandler.Instance.highlightMaterial;
        //OnHoverOver?.Invoke();
    }
    public void UnHover()
    {
        if (SpriteRenderer == null) return;
        SpriteRenderer.material = startMaterial;
       // OnHoverOut?.Invoke();
    }

}
