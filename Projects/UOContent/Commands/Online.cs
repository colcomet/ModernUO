using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Commands;

public static class Online
{
    public static void Configure()
    {
        CommandSystem.Register("Online", AccessLevel.Player, Online_OnCommand);
        CommandSystem.Register("Who", AccessLevel.Player, Online_OnCommand); // Alias for compatibility
    }

    [Usage("Online"), Description("Displays currently connected players.")]
    private static void Online_OnCommand(CommandEventArgs e)
    {
        var pm = e.Mobile as PlayerMobile;
        var omobs = NetState.Instances.Select(pmns => pmns.Mobile).Where(m => m != pm).ToList();
        if (omobs.Count == 0)
        {
            pm.PrivateOverheadMessage(MessageType.Regular, MessageHues.BlueNoticeHue, false, "No other players are currently online.", pm.NetState);
            return;
        }
        pm.SendGump(new OnlineListGump(pm, omobs));
    }

}

public class OnlineListGump : Gump
{
    private readonly Mobile m_From;
    private readonly List<Mobile> m_List;

    public override bool Singleton => true;

    public OnlineListGump(Mobile from, List<Mobile> onlineMobiles) : base(50, 50)
    {
        m_From = from;
        m_List = onlineMobiles;

        AddPage(0);

        AddBackground(0, 0, 455, 50 + m_List.Count * 20, 9250);
        AddAlphaRegion(5, 5, 445, 40 + m_List.Count * 20);

        AddHtml(15, 15, 405, 20, "<BASEFONT COLOR=#FFFFFF>Current Online Players</BASEFONT>");

        for (var i = 0; i < m_List.Count; ++i)
        {
            var pm = m_List[i];

            if (pm is not null && pm != from)
            {
                AddHtml(32, 35 + i * 20, 415, 18, GetOnlineEntryText(from, pm).Color(0xC0C0EE));
            }
        }
    }

    public string GetOnlineEntryText(Mobile pm, Mobile opm)
    {
        var coordsString = $"{opm.X}, {opm.Y}, {opm.Z}";
        string isGmString = opm.AccessLevel >= AccessLevel.Counselor ? " (GM)" : string.Empty;
        string baseString = $"{opm.Name}{isGmString} ";
        if (pm.Map == opm.Map)
        {
            var distance = (int)(pm.GetDistanceToSqrt(opm));
            var direction = pm.GetDirectionTo(opm.X, opm.Y, false);
            var dirString = PetListGump.GetDirectionString(direction);
            var distanceString = PetListGump.GetDistanceString(distance);
            if (distance == 0)
            {
                return baseString + "(standing on your feet)";
            }
            return baseString + $"({coordsString}) {distanceString}, {dirString}{PetListGump.GetRegionString(opm)}";
        }
        else
        {
            return baseString + $"({coordsString}) in {opm.Map.Name}";
        }
    }
}
