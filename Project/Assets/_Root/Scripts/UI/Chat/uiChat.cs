using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class uiChat : MonoBehaviour
    {
        [SerializeField]
        private uiChatItem _itemPrefab;

        [SerializeField]
        private InputField _inputField;

        [SerializeField]
        private ScrollRect _scroll;

        private string mainChannel = "Global";
        private ChatManager _manager;
        private readonly List<uiChatItem> _chatItems = new List<uiChatItem>();

        private void Awake()
        {
            _manager = FindObjectOfType<ChatManager>();

            if (_manager == null)
            {
                Debug.LogError("Failed to find ChatManager", this);
                return;
            }

            _manager.CanSendChange += Manager_CanSendChange;
            _manager.MessageReceived += Manager_MessageReceived;

            Manager_CanSendChange(_manager, _manager.CanSend);
        }

        private void Manager_CanSendChange(object sender, bool canSend)
        {
            if (canSend == false)
            {
                ClearMessages();
            }

            gameObject.SetActive(canSend);
        }

        private void Manager_MessageReceived(object sender, ChatMessage e)
        {
            var item = Instantiate(_itemPrefab, _scroll.content);
            item.Init(e);

            _chatItems.Add(item);
        }

        private void ClearMessages()
        {
            foreach (var i in _chatItems)
            {
                Destroy(i.gameObject);
            }

            _chatItems.Clear();
        }

        public void UI_Send()
        {
            if (_inputField.text.IndexOf("/set ") == 0)
            {
                mainChannel = _inputField.text.Substring(5);
            }
            else
            {
                _manager.Send(_inputField.text, mainChannel);
            }
            _inputField.text = "";
        }
    }
}
