using GameSystems.BuildingSystem;
using UnityEngine;

public class PlayerHeldItemVisuals : MonoBehaviour
{
    public static PlayerHeldItemVisuals Instance;

    //private Vector3 offset;

    private PlayerController controller;
    //[SerializeField] private BuildingPlacer BuildingPlacer;

    public Vector3 SpriteOffset {  get; private set; }

    [SerializeField]private SpriteRenderer heldItemVisuals;
    private BuildableTiles heldItem;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this);
            return;
        }
        //heldItemVisuals = GetComponent<SpriteRenderer>();
        controller = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        heldItem = controller.BuildableForVisuals;
        if (heldItem != null && heldItem.DisplaySprite != null)
        {
            if(controller.LookDirection.y > 0)
            {
                heldItemVisuals.sortingOrder = 4;
            }
            else
            {
                heldItemVisuals.sortingOrder = 6;
            }
           
            heldItemVisuals.sprite = heldItem.DisplaySprite;
            heldItemVisuals.color = Color.white;
        }
        else
        {
            heldItemVisuals.sprite = null;
            heldItemVisuals.color = Color.clear;
        }
      
    }

    public void SetHeldItem(BuildableTiles tile)
    {
        heldItem = tile;
    }

}
