using GameSystems.Inventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack Instance;
    public LayerMask interactableLayerMask;
    private void Awake()
    {
        Instance = this;
    }
    public void MeleeAttack(Vector3 direction, GameItem item)
    {
        if (item.GameItemData == null) return;
        int damageamount = item.GameItemData.ItemAttackDamage;
        DamageType type = item.GameItemData.DamageType;
        float distance = item.GameItemData.WeaponReach;
        MeleeAttack(direction, damageamount, type, distance);
    }
    public void MeleeAttack(Vector3 direction, ItemSlot slot)
    {
        if(slot.ItemData==null) return;
        int damageamount = slot.ItemData.ItemAttackDamage;
        
        DamageType type = slot.ItemData.DamageType;
        float distance = slot.ItemData.WeaponReach;
        MeleeAttack(direction, damageamount, type, distance);
    }
    public void MeleeAttack(Vector3 direction, int amount, DamageType type,float distance)
    {
        Debug.Log("attacking");
        Vector3 directionToAttack = transform.position + (direction.normalized * distance);
        Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);
        Collider[] colliders= Physics.OverlapBox(direction, halfExtents, Quaternion.identity, interactableLayerMask);
        if (colliders.Length == 0) return;

        foreach (Collider collider in colliders)
        {
            IDamagable damagable = collider.gameObject.GetComponent<IDamagable>();
            if(damagable != null)
            {
                Debug.Log(collider.gameObject.name);
                damagable.Damage(amount, type);
            }
        }
    }

}
