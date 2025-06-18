using GameSystems.WaetherSystem;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnviromentVisualsManager : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private Volume volume;


    private ColorAdjustments adjustments;
   
 
    private void OnEnable()
    {
       volume.profile.TryGet<ColorAdjustments>(out adjustments);
        //if(adjustments != null)
        //{
        //    Debug.Log("found it");
        //}
       
    }
    private void OnDisable()
    {
     
    }


    private void Update()
    {
        float timeofDay = TimeManager.Instance.TimeOfDayNormalized();
        
        Color ambientLightColor = gradient.Evaluate(timeofDay);
       
        adjustments.colorFilter.Override(ambientLightColor);
        
    }


}
