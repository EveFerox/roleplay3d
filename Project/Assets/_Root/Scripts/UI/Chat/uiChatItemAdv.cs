using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI
{
    public class uiChatItemAdv : uiChatItem
    {
        [SerializeField]
        Text _time;

        [SerializeField]
        Text _sender;

        [SerializeField]
        Image _icon;

        public override void Init(ChatMessage msg)
        {
            if (_time != null) _time.text = $"[{_msg.Time.ToShortTimeString()}] ";
            if (_sender != null) _sender.text = $"{_msg.Sender}: ";
            _message.text = _msg.Message;

            if (_icon != null) _icon.sprite = _icon.sprite; //TODO
        }
    }
}
