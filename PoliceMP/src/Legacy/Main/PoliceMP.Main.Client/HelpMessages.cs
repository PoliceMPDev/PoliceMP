using CitizenFX.Core;
using PoliceMP.Main.Core.Client.Extensions;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Main.Client
{
    public class HelpMessages : BaseScript
    {
        private readonly string[] _messages =
        {
            "You can turn on ANPR by going into the vehicle menu (M) when in a police vehicle.",
            "You can put people into vehicles by grabbing them and walking next to one of the rear doors.",
            "Check out our Getting Started guide at policemp.com/start",
            "You can press F1 to access the toolbox or change your role.",
            "You can interact with vehicles and peds by holding right mouse button.",
            "Are you new to the server? Make sure to read our rules at policemp.com/rules",
            "This server is still work in progress. Please report any bugs or provide feedback on our Discord server discord.gg/policemp",
            "Press LCTRL to lock onto a vehicle in ANPR.",
            "You can open the Police Radio menu by pressing LSHIFT + F2",
            "This is a UK themed server. As a PCSO you will not have access to any weapons. Only Firearms Officers carry weapons.",
            "Got ideas on how to improve the server? Share them with us on our Discord at discord.gg/policemp",
            "Visit our Discord at discord.gg/policemp to see our planned features and to keep up to date with the latest news and updates.",
            "The server automatically restarts at: 00:00, 06:00, 12:00, and 18:00.",
            "You can limit the speed of your vehicle by either pressing Delete or using the /limitspeed command.",
            "You can unlock new uniforms and vehicles by applying for a Police Constable. Find more details at policemp.com/apply",
            "When communicating via radio, remember: A - Accuracy, B - Brevity and C - Clarity.",
            "You can request back-up and press your panic button via the F2 menu.",
            "You can find lots of guides and support on our website at policemp.com/guides",
            "You can call the recovery truck, coroners or the local ambulance using G menu.",
            "Having issues with your textures? Go to your Settings -> Graphics and increase the Extended texture budget.",
            "Need to report something or have an issue? Make sure you follow the Chain of Command.",
            "You can set a callsign using /callsign command in chat."
        };

        [Tick]
        private async Task OnTick()
        {
            var messageIndex = PoliceMpRandom.Next(_messages.Length - 1);
            var message = _messages[messageIndex];

            this.SendChatMessage(message, "PoliceMP.com");

            // Every 10 minutes
            await Delay(600000);
        }
    }
}