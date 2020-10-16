using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IPS.Inputs
{
    public class InputManager : MonoBehaviour
    {
        private static readonly IDictionary<string, int> playerStates = new Dictionary<string, int>();

        private static Controls controls;

        public static Controls Controls{
            get{
                if(controls != null){return controls;}
                return controls =new Controls();
            }
        }

        void Awake(){
                if(controls != null){return;}
                 controls =new Controls();

                 DontDestroyOnLoad(this);
        }

        private void OnEnable() => Controls.Enable();
        private void OnDisable() => Controls.Disable();
        private void OnDestroy() => controls = null;

        public static void Add(string stateName){ //Add= this state is now being blocked
            
            playerStates.TryGetValue(stateName, out int value);
            playerStates[stateName] = value+1;

            UpdatePlayerState(stateName);
        }

        public static void Remove(string stateName){ //this state is no longer being blocked
                  playerStates.TryGetValue(stateName, out int value);
            playerStates[stateName] = Mathf.Max(value-1,0);

            UpdatePlayerState(stateName);
        }

        public static void UpdatePlayerState(string actionName){
            int value = playerStates[actionName];

            if(value>0){
                Controls.asset.FindActionMap(actionName).Disable();
                return;
            }

            Controls.asset.FindActionMap(actionName).Enable();
        }
    }
}