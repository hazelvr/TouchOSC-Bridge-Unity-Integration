//Unity Integration with TouchOSC Bridge
//TouchOSC Bridge can be found at https://hexler.net/software/touchosc
//Author: Hazel Mora, http://hazelthings.com/osckit
//Sources: https://www.nyu.edu/classes/bello/FMT_files/9_MIDI_code.pdf, http://opensoundcontrol.org/spec-1_0

using UnityEngine;
using System.Net.Sockets;

public class TouchOSCBridge : MonoBehaviour {

    public string host = "127.0.0.1";
    public int port = 12101;    //Default for TouchOSC Bridge. Wouldn't recommend changing this.
    public enum MidiMessageType { NoteOff = 0x80, NoteOn = 0x90, PolyphonicAftertouch = 0xA0, ControlChange = 0xB0, ProgramChange = 0xC0, ChannelAftertouch = 0xD0, PitchWheel = 0xE0 };

    private UdpClient udpClient;

	void Start () {
        udpClient = new UdpClient(0);
        try {
            host = PlayerPrefs.GetString("Host Address");
            udpClient.Connect(host, port);
            Debug.Log("MIDI Host Connected:" + host);
        }
        catch (System.Exception e) {
            Debug.Log(e.ToString());
        }
    }

    public void midiSend(MidiMessageType type, int channel, int data1, int data2) {
        //OSC midi message basic format, with 0xff bytes as placeholders for the actual midi message bytes.
        byte[] message = new byte[] { 0x2f, 0x6d, 0x69, 0x64, 0x69, 0x00, 0x00, 0x00, 0x2c, 0x6d, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff };

        //Sanity check on input
        if (channel < 1 || channel > 15 || data1 < 0 || data1 > 127 || data2 < 0 || data2 > 127) {
            Debug.LogError("midiSend: Bad input format.");
            return;
        }

        message[15] = (byte)type;
        message[15] += (byte)(channel - 1);
        message[14] = (byte)data1;
        message[13] = (byte)data2;

        udpClient.Send(message, 16);
        Debug.Log("Midi Message Sent!");
    }

    void OnApplicationQuit() {
        udpClient.Close();
    }
}
