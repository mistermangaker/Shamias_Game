using System.Collections.Generic;
using UnityEngine;

public interface IDamagable 
{
   public void Damage(int damage, DamageType damageType);

   
}

public enum DamageType
{
    Blunt,
    Peircing,
    Slash,
    Harvest_Stone,
    Harvest_Wood,
    Harvest_Plant
}