using System;
using UnityEngine;
using System.Collections.Generic;

namespace UI
{
    public class uiCtrl : StaticMonoBehaviour<uiCtrl>
    {
        [SerializeField]
        Panels _panels;

        protected override void Awake()
        {
            base.Awake();

            GotoLogin();
        }

        public static void GotoLogin()
        {
            SetActive(Instance._panels.login);
        }

        public static void GotoRegister()
        {
            SetActive(Instance._panels.register);
        }

        static void SetActive(Component panel)
        {
            Instance._panels.SetActive(panel);
        }

        [Serializable]
        class Panels
        {
            public uiLogin login;
            public uiRegister register;
            public uiChat chat;

            public IEnumerable<Component> All
            {
                get
                {
                    yield return login;
                    yield return register;
                    yield return chat;
                }
            }

            public void SetActive(Component panel)
            {
                foreach (var p in All) {
                    p.gameObject.SetActive(ReferenceEquals(p, panel));
                }
            }
        }
    }
}