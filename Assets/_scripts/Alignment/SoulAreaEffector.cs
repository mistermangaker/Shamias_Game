using System.Collections.Generic;
using UnityEngine;

public class SoulAreaEffector : BuildingBase
{
    [SerializeField] private List<AlignmentChangeSuggestion> effector;
    [SerializeField] private float range = 5f;


    EventBinding<OnSoulEffectersActivate> onSoulEffectersActivate;
    protected override void Awake()
    {
        base.Awake();
        onSoulEffectersActivate = new EventBinding<OnSoulEffectersActivate>(EffectSuroundingSouls);
        EventBus<OnSoulEffectersActivate>.Register(onSoulEffectersActivate);
    }
    private void OnDestroy()
    {
        EventBus<OnSoulEffectersActivate>.Deregister(onSoulEffectersActivate);
    }

    [ContextMenu("change SoulAlignment")]
    public void EffectSuroundingSouls()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        foreach (Collider collider in colliders)
        {
            SoulHolder holder = collider.GetComponent<SoulHolder>();
            if (holder != null)
            {
                Debug.Log(holder.name);
                holder.AddAlignmentSuggestion(effector);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public override bool Interact(InteractionAttempt interactor)
    {
        if(interactor.Intent == InteractionIntent.Interact)
        {
            RemoveBuilding();
            return true;
        }
       return false;
    }

    public override void OnInteractingEnd()
    {
      
    }

    public override OnToolTipRequested GetToolTip()
    {
        return new OnToolTipRequested
        {
            toolTipHeader = building.BuildableData.DisplayName,
            intent = InteractionIntent.Interact,
        };
    }

    public override void Damage(int damage, DamageType damageType)
    {
        if (damageType == DamageType.Slash)
        {
            DamageBuilding(damage);
        }
        else if (damageType == DamageType.Blunt)
        {
            DamageBuilding(damage / 2);
        }
    }
}
