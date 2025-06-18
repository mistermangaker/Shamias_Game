using System.Collections.Generic;
using UnityEngine;

public struct OnSoulEffectersActivate : IEvent
{

}
public struct OnSoulHoldersActivate : IEvent
{

}

public struct OnRegisterSoulData : IEvent
{
    public SerializableGuid buildingId;
    public SoulData SoulData;
}

public struct OnDeregisterSoulData : IEvent
{
    public SerializableGuid buildingId;
}


public class SoulManager : MonoBehaviour
{
    [SerializeField] private List<BaseSoulBaseStoryPreset> baseSoulBaseStoryPresets;
    private SerializableDictionary<SerializableGuid, SoulData> buildingSouls = new SerializableDictionary<SerializableGuid, SoulData>();

    public SoulData TryGetSoulFromID(SerializableGuid id)
    {
        buildingSouls.TryGetValue(id, out var soulData);
        return soulData;
    }

    private EventBinding<OnRegisterSoulData> onRegisterSoulData;
    private EventBinding<OnDeregisterSoulData> onDeRegisterSoulData;

    private void Awake()
    {
        onRegisterSoulData = new EventBinding<OnRegisterSoulData>(RegisterSoulDataWithManager);
        EventBus<OnRegisterSoulData>.Register(onRegisterSoulData);
        onDeRegisterSoulData = new EventBinding<OnDeregisterSoulData>(DeRegisterSoulDataWithManager);
        EventBus<OnDeregisterSoulData>.Register(onDeRegisterSoulData);
        
    }

    private void OnDestroy()
    {
        EventBus<OnRegisterSoulData>.Deregister(onRegisterSoulData);
        EventBus<OnDeregisterSoulData>.Deregister(onDeRegisterSoulData);
    }

    private void RegisterSoulDataWithManager(OnRegisterSoulData data)
    {
        RegisterSoulDataWithManager(data.buildingId, data.SoulData);
    }
    private void DeRegisterSoulDataWithManager(OnDeregisterSoulData data)
    {
        DeRegisterSoulDataWithManager(data.buildingId);
    }

    public void RegisterSoulDataWithManager(SerializableGuid id, SoulData soulData)
    {
        if(buildingSouls.ContainsKey(id)) buildingSouls[id] = soulData;
        else  buildingSouls.Add(id, soulData);
       
    }

    public void DeRegisterSoulDataWithManager(SerializableGuid id)
    {
        buildingSouls.Remove(id);
    }


    [ContextMenu("do the thing")]
    public void DoSoulInteractions()
    {
        EventBus<OnSoulEffectersActivate>.Raise(new OnSoulEffectersActivate());
        EventBus<OnSoulHoldersActivate>.Raise(new OnSoulHoldersActivate());
    }

    [ContextMenu("test backStory")]
    public void Test()
    {
        SoulData data = SoulBuilder.GenerateRandomSoulFromPreset(baseSoulBaseStoryPresets[0],3);
        Debug.Log(data.ToString());
        //Debug.Log(SoulBuilder.Default.ToString());
        //SoulBackStory backstory = baseSoulBaseStoryPresets[Random.Range(0, baseSoulBaseStoryPresets.Count)].GenerateBackStoryFromPreset();
        //Debug.Log(backstory.GetFullBackStory());
    }

}
