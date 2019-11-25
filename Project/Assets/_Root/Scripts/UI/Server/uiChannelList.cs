using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class uiChannelList : Singleton<uiChannelList>
{
    readonly Dictionary<string, uiChannel> _channels = new Dictionary<string, uiChannel>();

    [SerializeField]
    uiChannel _itemPrefab;

    [SerializeField]
    ScrollRect _scroll;

    [SerializeField]
    InputField _text;

    void Awake() {
        ChannelManager.Instance.OnChanellCreated += OnChannelCreated;
    }

    void OnChannelCreated(object sender, ChannelFeed channel) {
        var item = Instantiate(_itemPrefab, _scroll.content);
        item.Init(channel);

        _channels.Add(channel.Name, item);
    }

    public void OnChannelRemoved(string name) {
        if (_channels.TryGetValue(name, out var channel)) {
            Destroy(channel.gameObject);
        }
        _channels.Remove(name);
    }

    public void UI_Create() {
        ChannelManager.Instance.CreateChannel(_text.text);
    }

    public void UI_Remove() {
        ChannelManager.Instance.RemoveChannel(_text.text);
    }
}

