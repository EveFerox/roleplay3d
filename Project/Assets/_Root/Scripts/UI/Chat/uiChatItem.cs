using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class uiChatItem : MonoBehaviour
    {
        [SerializeField]
        protected Text _message;

        protected ChatMessage _msg;

        public virtual void Init(ChatMessage msg)
        {
            _msg = msg;

            _message.text = $"[{_msg.Time.ToShortTimeString()}] {_msg.Sender}: {_msg.Message}";
        }
    }
}
