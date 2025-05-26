using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Commands;

public static class Pets
{
    public static void Configure()
    {
        CommandSystem.Register("Pets", AccessLevel.Player, Pets_OnCommand);
    }
    [Usage("Pets"), Description("Displays your currently-active pets.")]
    public static void Pets_OnCommand(CommandEventArgs e)
    {
        var pm = e.Mobile as PlayerMobile;
        if (pm == null) return;
        var pmAllFollowers = pm.AllFollowers;
        if (pmAllFollowers is not null && pmAllFollowers.Count > 0)
        {
            e.Mobile.SendGump(new PetListGump(e.Mobile, pmAllFollowers));
        }
        else
        {
            e.Mobile.PrivateOverheadMessage(MessageType.Regular, MessageHues.BlueNoticeHue, false, "You have no followers.", e.Mobile.NetState);
        }
    }

    public static string GetDistanceAndCompassDirectionToPet(PlayerMobile pm, BaseCreature pet)
    {
        if (pet == null) return string.Empty;
        if (pet == pm.Mount)
        {
            return "(mounted)";
        }
        else if (pm.Map == pet.Map)
        {
            var distance = (int)(pm.GetDistanceToSqrt(pet));
            var direction = pm.GetDirectionTo(pet);
            var dirString = direction.ToString();
            if (direction == Direction.Up)
            {
                dirString = "Northwest";
            }
            else if (direction == Direction.Right)
            {
                dirString = "Northeast";
            }
            else if (direction == Direction.Left)
            {
                dirString = "Southwest";
            }
            else if (direction == Direction.Down)
            {
                dirString = "Southeast";
            }
            return $"{distance} tiles, {dirString} ({pet.Region.Name})";
        }
        else
        {
            return $"in {pet.Map.Name}";
        }
    }
}

public class PetListGump : Gump
{
    private readonly Mobile m_From;
    private readonly List<Mobile> m_List;

    public static string GetPetEntryText(Mobile pm, BaseCreature pet)
    {
        string baseString = $"{pet.Name} (s: {pet.ControlSlots}) ";
        if (pet == null) return string.Empty;
        if (pet == pm.Mount)
        {
            return baseString + "(mounted)";
        }
        else if (pm.Map == pet.Map)
        {
            var distance = (int)(pm.GetDistanceToSqrt(pet));
            var direction = pm.GetDirectionTo(pet);
            var dirString = direction.ToString();
            if (distance == 0)
            {
                return baseString + "(standing on your feet)";
            }
            else if (direction == Direction.Up)
            {
                dirString = "Northwest";
            }
            else if (direction == Direction.Right)
            {
                dirString = "Northeast";
            }
            else if (direction == Direction.Left)
            {
                dirString = "Southwest";
            }
            else if (direction == Direction.Down)
            {
                dirString = "Southeast";
            }
            return baseString + $"({pet.X}, {pet.Y}, {pet.Z}), {distance} tiles, {dirString} ({pet.Region.Name})";
        }
        else
        {
            return baseString + $"({pet.X}, {pet.Y}, {pet.Z}) in {pet.Map.Name}";
        }
    }
    public override bool Singleton => true;

    public PetListGump(Mobile from, HashSet<Mobile> mlist) : base(50, 50)
    {
        m_From = from;
        m_List = mlist.Where(c => c is BaseCreature).ToList();

        AddPage(0);

        AddBackground(0, 0, 425, 50 + m_List.Count * 20, 9250);
        AddAlphaRegion(5, 5, 415, 40 + m_List.Count * 20);

        AddHtml(15, 15, 275, 20, "<BASEFONT COLOR=#FFFFFF>Your current pets:</BASEFONT>");

        for (var i = 0; i < m_List.Count; ++i)
        {
            var pet = m_List[i];

            if (pet?.Deleted != false)
            {
                continue;
            }

            AddButton(15, 39 + i * 20, 10006, 10006, i + 1);

            AddHtml(32, 35 + i * 20, 275, 18, GetPetEntryText(from, (BaseCreature)pet).Color(0xC0C0EE));
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var index = info.ButtonID - 1;
        var pm = sender.Mobile as PlayerMobile;
        if (pm == null || index < 0 || index >= m_List.Count)
        {
            return;
        }
        if(pm.Mount == m_List[index])
        {
            pm.PrivateOverheadMessage(MessageType.Regular, MessageHues.RedErrorHue, false, $"You can't release {m_List[index].Name} while riding them!", pm.NetState);
            return;
        }
        pm.PrivateOverheadMessage(MessageType.Regular, MessageHues.BlueNoticeHue, false, $"{m_List[index].Name} is now free!", pm.NetState);
        pm.SendGump(new ConfirmReleaseGump(pm, (BaseCreature)m_List[index]));
        
    }
}
