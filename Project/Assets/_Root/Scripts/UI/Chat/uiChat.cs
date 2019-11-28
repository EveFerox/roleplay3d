using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class uiChat : MonoBehaviour
    {
        [SerializeField]
        uiChatItem _itemPrefab;

        [SerializeField]
        InputField _inputField;

        [SerializeField]
        ScrollRect _scroll;
        
        [SerializeField]
        string _currentChannel = "Global";

        ChatManager _manager => ChatManager.Instance;

        readonly List<uiChatItem> _chatItems = new List<uiChatItem>();

        void Awake()
        {
            _manager.CanSendChange += Manager_CanSendChange;
            _manager.MessageReceived += Manager_MessageReceived;

            Manager_CanSendChange(_manager, _manager.CanSend);
        }

        void Manager_CanSendChange(object sender, bool canSend)
        {
            if (canSend == false) {
                ClearMessages();
            }

            gameObject.SetActive(canSend);
        }

        void Manager_MessageReceived(object sender, ChatMessage e)
        {
            var item = Instantiate(_itemPrefab, _scroll.content);
            item.Init(e);

            _chatItems.Add(item);
        }

        void ClearMessages()
        {
            foreach (var i in _chatItems) {
                Destroy(i.gameObject);
            }

            _chatItems.Clear();
        }

        public void UI_Send()
        {
            if (_inputField.text.IndexOf("/set ") == 0) {
                _currentChannel = _inputField.text.Substring(5);
            } else {
                _manager.Send(_inputField.text, _currentChannel);
            }
            _inputField.text = "";
        }
    }
}
