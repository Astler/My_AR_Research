// Copyright 2022 Niantic, Inc. All Rights Reserved.
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/ARDK/Utilities/Input/InputSystem/InputInteractions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#if ENABLE_INPUT_SYSTEM

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Object = UnityEngine.Object;

namespace Niantic.ARDK.Utilities.Input.InputSystem
{
    public class InputInteractions : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public InputInteractions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputInteractions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""d231b2ca-f5c1-4a92-8cc6-173aef1526e9"",
            ""actions"": [
                {
                    ""name"": ""Point"",
                    ""type"": ""Value"",
                    ""id"": ""a1fa0d4f-297a-46aa-9a33-cfe41443f9e8"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9b7465e1-cad9-4078-a8b0-688ff9a2da35"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""dcc106e1-eb80-4bb2-91a4-ecf60546c4ab"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TouchPosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e0441f73-83f6-451b-ba30-1d91c0246619"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""aa86c666-6a12-4426-8a14-94ace7013c6e"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0dffa5e1-8411-487d-baba-c1169779f7f6"",
                    ""path"": ""<Touchscreen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1290515d-73b0-4d57-ac3d-5dd8dbdb659a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b62a7e47-06c6-404d-89e0-794908d46761"",
                    ""path"": ""<Touchscreen>/Press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""132a1b09-41e3-4845-b6a5-f0cbbc9cf634"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1178a903-8e80-440d-8c73-8da1812feb40"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dc59e80f-7900-4f79-9e71-91a14ea1c863"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Point = m_Player.FindAction("Point", throwIfNotFound: true);
            m_Player_LeftClick = m_Player.FindAction("LeftClick", throwIfNotFound: true);
            m_Player_RightClick = m_Player.FindAction("RightClick", throwIfNotFound: true);
            m_Player_TouchPosition = m_Player.FindAction("TouchPosition", throwIfNotFound: true);
        }

        public void Dispose()
        {
            Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }
        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }
        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_Point;
        private readonly InputAction m_Player_LeftClick;
        private readonly InputAction m_Player_RightClick;
        private readonly InputAction m_Player_TouchPosition;
        public struct PlayerActions
        {
            private InputInteractions m_Wrapper;
            public PlayerActions(InputInteractions wrapper) { m_Wrapper = wrapper; }
            public InputAction Point => m_Wrapper.m_Player_Point;
            public InputAction LeftClick => m_Wrapper.m_Player_LeftClick;
            public InputAction RightClick => m_Wrapper.m_Player_RightClick;
            public InputAction TouchPosition => m_Wrapper.m_Player_TouchPosition;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    Point.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPoint;
                    Point.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPoint;
                    Point.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPoint;
                    LeftClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftClick;
                    LeftClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftClick;
                    LeftClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftClick;
                    RightClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightClick;
                    RightClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightClick;
                    RightClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightClick;
                    TouchPosition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouchPosition;
                    TouchPosition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouchPosition;
                    TouchPosition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTouchPosition;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Point.started += instance.OnPoint;
                    Point.performed += instance.OnPoint;
                    Point.canceled += instance.OnPoint;
                    LeftClick.started += instance.OnLeftClick;
                    LeftClick.performed += instance.OnLeftClick;
                    LeftClick.canceled += instance.OnLeftClick;
                    RightClick.started += instance.OnRightClick;
                    RightClick.performed += instance.OnRightClick;
                    RightClick.canceled += instance.OnRightClick;
                    TouchPosition.started += instance.OnTouchPosition;
                    TouchPosition.performed += instance.OnTouchPosition;
                    TouchPosition.canceled += instance.OnTouchPosition;
                }
            }
        }
        public PlayerActions Player => new PlayerActions(this);
        public interface IPlayerActions
        {
            void OnPoint(InputAction.CallbackContext context);
            void OnLeftClick(InputAction.CallbackContext context);
            void OnRightClick(InputAction.CallbackContext context);
            void OnTouchPosition(InputAction.CallbackContext context);
        }
    }
}

#endif
