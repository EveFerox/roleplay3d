using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class uiChannel : MonoBehaviour
{
    ChannelFeed _channel;

    [SerializeField]
    protected Text _message;

    public virtual void Init(ChannelFeed channel) {
        _channel = channel;
        _channel.Removed += (s, e) => uiChannelList.Instance.OnChannelRemoved(_channel.Name);
        _channel.UserChanged += UserChanged;
    }

    private void UserChanged(object sender, ChannelFeed.UserChangeInfo e) {
        _message.text = $"[{_channel.Name}] : {string.Join(", ", _channel.Users.Select(u => u.Username))}";
    }
}
