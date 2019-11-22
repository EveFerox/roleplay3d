using System;
using UnityEngine;
using System.Collections.Generic;

namespace UI
{
    public class uiCtrl : Singleton<uiCtrl>
    {
        [SerializeField]
        Panels _panels;

        protected void Awake()
        {
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

            public IEnumerable<Component> All
            {
                get
                {
                    yield return login;
                    yield return register;
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