using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

public static class PreDefinedAssemblyUtil
{
    enum AssemblyType
    {
        AssemblyCSharp,
        AssemblyCSharpEditor,
        AssemblyCSharpEditorFirstPass,
        AssemblyCSharpFirstPass
    }
    static AssemblyType? GetAssemblyType(string name)
    {
        return name switch
        {
            "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
            "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
            "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
            "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
            _ => null
        };
    }

    static void AddTypesFromAssembly(Type[] assembly, ICollection<Type> types, Type interfaceType)
    {
        if(assembly == null) return;
        for(int i = 0; i < assembly.Length; i++)
        {
            Type type = assembly[i];
            if (type != interfaceType && interfaceType.IsAssignableFrom(type))
            {
                types.Add(type);
            }
        }
    }

    public static List<Type> GetType(Type interfaceType)
    {
        Assembly[] assembies = AppDomain.CurrentDomain.GetAssemblies();
        Dictionary<AssemblyType, Type[]> assemblyType = new Dictionary<AssemblyType, Type[]>();
        List<Type> types = new List<Type>();
        for(int i = 0; i < assembies.Length; i++)
        {
            AssemblyType? assemblytype = GetAssemblyType(assembies[i].GetName().Name);
            if(assemblytype != null)
            {
                assemblyType.Add((AssemblyType)assemblytype, assembies[i].GetTypes());
            }
        }
        assemblyType.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCsharpTypes);
        AddTypesFromAssembly(assemblyCsharpTypes, types, interfaceType);
        assemblyType.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCsharpFirstPassTypes);
        AddTypesFromAssembly(assemblyCsharpFirstPassTypes, types, interfaceType);

        return types;
    }
}