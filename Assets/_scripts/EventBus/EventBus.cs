using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EventBus<T> where T : IEvent 
{
    static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();


    public static void Register(EventBinding<T> binding) => bindings.Add(binding);
    public static void Deregister(EventBinding<T> binding) => bindings.Remove(binding);

    public static void Raise(T @event)
    {
        foreach(var binding in bindings)
        {
            binding.OnEvent.Invoke(@event);
            binding.OnEventNoArgs.Invoke();
        }
    }

    static void Clear()
    {
        Debug.Log($"clearing {typeof(T)} bindings");
        bindings.Clear();
    }
}
