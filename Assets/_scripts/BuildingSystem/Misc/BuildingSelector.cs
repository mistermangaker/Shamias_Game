using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameSystems.BuildingSystem
{
    [Obsolete]
    public class BuildingSelector : MonoBehaviour
    {
        [SerializeField] private List<BuildableTiles> buildables;

        [SerializeField] private BuildingPlacer buildingPlacer;

        private int _activeBuildableIndex;

        private PlayerInput _InputActions;

        private void OnEnable()
        {
            //
            //_InputActions = PlayerInput.Instance;
            //_InputActions.Player.Interact.performed += Interact_performed;
            //PlayerInput.Instance.Player.Interact.performed += Interact_performed;
        }

        private void OnDisable()
        {
            //PlayerInput.Instance.Player.Interact.performed -= Interact_performed;
        }

        private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            
            NextItem();
        }

        private void NextItem()
        {
            _activeBuildableIndex++;
            if (_activeBuildableIndex >= buildables.Count)
            {
                _activeBuildableIndex = 0;
            }
            buildingPlacer.SetActiveBuildable(buildables[_activeBuildableIndex]) ;
        }
    }
}

