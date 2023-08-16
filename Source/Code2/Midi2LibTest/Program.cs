using Microsoft.Devices.Midi2;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi2;

namespace Midi2LibTest;

internal static class Program
{
    static Task Main(string[] args)
    {
        return Midi2Sample();
    }

    static async Task Midi2Sample()
    {
        // for comments on what each step does, please see the C++/WinRT sample
        // the code is almost identical

        Console.WriteLine($"MIDI Services Sdk Version: {MidiServices.SdkVersion}");
        Console.WriteLine("Checking for MIDI Services");

        var checkResult = MidiServices.CheckForWindowsMidiServices();

        // proceed only if MIDI services is present and compatible. Your own application may decide
        // to fall back to WinRT/WinMM MIDI 1.0 APIs, or to prompt the user to install the latest 
        // version of Windows MIDI Services

        if (checkResult == WindowsMidiServicesCheckResult.PresentAndUsable)
        {
            Console.WriteLine("MIDI Services Present and usable");
            Console.WriteLine("Creating session settings");

            // create the MIDI session, giving us access to Windows MIDI Services. An app may open 
            // more than one session. If so, the session name should be meaningful to the user, like
            // the name of a browser tab, or a project.

            var sessionSettings = MidiSessionSettings.Default;

            Console.WriteLine("Creating session");

            // > AccessViolation when MIDI2 not installed. Needs perhaps an extra check and a message.
            using var session = MidiSession.CreateSession("Sample Session", sessionSettings);

            // you can ask for MIDI 1.0 Byte Stream devices, MIDI 2.0 UMP devices, or both. Note that Some MIDI 2.0
            // endpoints may have MIDI 1.0 function blocks in them, so this is endpoint/device-level only.
            // Note that every device uses UMP through this API, but it can be helpful to know when a device is
            // a MIDI 1.0 device at the main interface level.

            Console.WriteLine("Creating Device Selector.");

            var deviceSelector = MidiEndpointConnection.GetDeviceSelector();

            // Enumerate UMP endpoints. Note that per C++, main cannot be a co-routine,
            // so we can't just co_await this async call, but instead use the C++/WinRT Extension "get()". 
            // We may end up wrapping this enumeration code into another class to instantly transform to 
            // MidiDeviceInformation instances, and to simplify calling code (reducing need for apps to
            // incorporate async handling).

            Console.WriteLine("Enumerating through Windows.Devices.Enumeration.");

            // > The endpointDevices include audio devices (!?) What does the deviceSelector do then?
            var endpointDevices = await DeviceInformation.FindAllAsync(deviceSelector);

            // this currently requires you have a USB MIDI 1.0 device. If you have nothing connected, just remove this check for now
            // That will change once MIDI 2.0 device selectors have been created
            if (endpointDevices.Count > 0)
            {
                Console.WriteLine("MIDI Endpoints were found (not really, but pretending they are for now).");

                // We're going to just pick the first one we find. Normally, you'd have the user pick from a list
                // or you'd otherwise have an Id at hand.

                // > Find your midi device in the list.
                DeviceInformation selectedEndpointInformation = endpointDevices[0];
                // > report selected endpoint name
                Console.WriteLine($"Selected Endpoint: {selectedEndpointInformation.Name}");

                // then you connect to the UMP endpoint
                Console.WriteLine("Connecting to UMP Endpoint.");
                Console.WriteLine("Note: For this example to fully work, you need to the special Loopback MidiSrv installed.");
                Console.WriteLine("Otherwise, creating an endpoint will fail, and no messages will be sent or received.");

                var endpoint = session.ConnectBidirectionalEndpoint(selectedEndpointInformation.Id, null);

                // after connecting, you can send and receive messages to/from the endpoint. Sending and receiving is
                // performed one complete UMP at a time. 
                // -----------------------------
                // Wire up an event handler to receive the message. This event handler type receives an IMidiUmp type
                // but you can also wire up one which receives a uint32_t array and a uint64_t timestamp instead. The
                // performance is almost identical (within a couple ms total over 1000 send/receives despite the 
                // additional type activations and casting) but one may be more convenient than the other to you, so
                // both are provided. 
                
                if (endpoint is null)
                {
                    // > If ConnectBidirectionalEndpoint fails (null)
                    // - was the C:\Program Files\Microsoft\Midi 2.0\Midi2.SampleAbstraction.dll registered?
                    // Run (as administrator) regsvr32.exe Midi2.SampleAbstraction.dll

                    Console.WriteLine($"Failed to connect to bidirectional endpoint: {selectedEndpointInformation.Name} - Aborting.");
                    return;
                }

                endpoint.MessageReceived += (_, args) =>
                {
                    var ump = args.GetUmp();

                    Console.WriteLine();
                    Console.WriteLine("Received UMP");
                    Console.WriteLine("- Current Timestamp: " + MidiClock.GetMidiTimestamp());
                    Console.WriteLine("- UMP Timestamp: " + ump.Timestamp);
                    Console.WriteLine("- UMP Type: " + ump.MessageType);

                    // if you wish to cast the IMidiUmp to a specific Ump Type, you can do so using .as<T>.
                    if (ump is MidiUmp32 ump32)
                    {
                        Console.WriteLine("Word 0: {0:X}", ump32.Word0);
                    }
                };

                var ump32 = new MidiUmp32
                {
                    MessageType = MidiUmpMessageType.Midi1ChannelVoice32
                };
                var midiUmp = (IMidiUmp)ump32;
                endpoint.SendUmp(midiUmp);

                Console.WriteLine("Wait for the message to arrive, and then press enter to cleanup.");
                Console.ReadLine();

                // not strictly necessary as the session.Close() call will do it, but it's here in case you need it
                session.DisconnectEndpointConnection(endpoint.Id);
            }
            else
            {
                Console.WriteLine("No MIDI Endpoints were found.");
            }
        }
        else
        {
            if (checkResult == WindowsMidiServicesCheckResult.NotPresent)
            {
                Console.WriteLine("MIDI Services Not Present");
            }
            else if (checkResult == WindowsMidiServicesCheckResult.IncompatibleVersion)
            {
                Console.WriteLine("MIDI Present, but is not a compatible version.");
                Console.WriteLine("Here you would prompt the user to install the latest version from " + MidiServices.LatestMidiServicesInstallUri.ToString());
            }
        }

        // close the session, detaching all Windows MIDI Services resources and closing all connections
        // You can also disconnect individual Endpoint Connections when you are done with them

        // using calls => session.Dispose();
    }
}