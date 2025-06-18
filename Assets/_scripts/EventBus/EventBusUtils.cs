using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EventBusUtils
{
    public static IReadOnlyList<Type> EventTypes { get; set; }
    public static IReadOnlyList<Type> EventBusTypes { get; set; }
#if UNITY_EDITOR
    public static PlayModeStateChange PlayModeStateChange { get; set; }
    [InitializeOnLoadMethod]
    public static void InitializeEditor()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        PlayModeStateChange = state;
        if (PlayModeStateChange == PlayModeStateChange.ExitingEditMode)
        {
            ClearallBuses();
        }
    }
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitialIze()
    {
        EventTypes = PreDefinedAssemblyUtil.GetType(typeof(IEvent));
        EventBusTypes = InitializeAllBusses();
    }

    static List<Type> InitializeAllBusses()
    {
        List<Type> list = new List<Type>();
        var typeOfType = typeof(EventBus<>);
        foreach(var eventType in EventTypes)
        {
            var bustype = typeOfType.MakeGenericType(eventType);
            list.Add(bustype);
            //Debug.Log($"Initialized EventBus<{eventType.Name}>");
        }
        return list;
    }

    public static void ClearallBuses()
    {
        Debug.Log("clearing all busses");
        for(int i = 0; i < EventBusTypes.Count; i++)
        {
            var bustypes = EventBusTypes[i];
            var clearMethod = bustypes.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            clearMethod.Invoke(null, null);
        }
    }

}