using UnityEngine;

[System.Serializable]
public class Tag
{
    public string Text;
    public Network net;
    public IP ip;
    public Mask mask;

    public Tag(string text, Network network, IP ip, Mask mask)
    {
        this.Text = text;
        this.net = network;
        this.ip = ip;
        this.mask = mask;
    }
}
