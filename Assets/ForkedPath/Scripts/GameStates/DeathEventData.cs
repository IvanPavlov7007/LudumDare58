using UnityEngine;
public class DeathEventData
{
    public DamageEventData damageEventData;
    public bool fallenToDeath;
    public DeathEventData(DamageEventData damageEventData = null, bool fallenToDeath = false)
    {
        this.damageEventData = damageEventData;
        this.fallenToDeath = fallenToDeath;
    }
}