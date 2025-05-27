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
}

public class PetListGump : Gump
{
    private readonly Mobile m_From;
    private readonly List<Mobile> m_List;

    public override bool Singleton => true;

    public PetListGump(Mobile from, HashSet<Mobile> mlist) : base(50, 50)
    {
        m_From = from;
        m_List = mlist.Where(c => c is BaseCreature).ToList();

        AddPage(0);

        AddBackground(0, 0, 455, 50 + m_List.Count * 20, 9250);
        AddAlphaRegion(5, 5, 445, 40 + m_List.Count * 20);

        AddHtml(15, 15, 405, 20, "<BASEFONT COLOR=#FFFFFF>Your current pets: (click button to release)</BASEFONT>");

        for (var i = 0; i < m_List.Count; ++i)
        {
            var pet = m_List[i];

            if (pet?.Deleted != false)
            {
                continue;
            }

            if (from.Mount != pet)
            {
                AddButton(15, 39 + i * 20, 10006, 10006, i + 1);
            }
            AddHtml(32, 35 + i * 20, 415, 18, GetPetEntryText(from, (BaseCreature)pet).Color(0xC0C0EE));
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
        var pet = m_List[index] as BaseCreature;
        if (pm.Mount == pet)
        {
            pm.PrivateOverheadMessage(MessageType.Regular, MessageHues.RedErrorHue, false, $"You can't release {pet.Name} while riding them!", pm.NetState);
            return;
        }
        pm.SendGump(new ConfirmReleaseGump(pm, pet, checkRange: false));
    }
    public string GetPetEntryText(Mobile pm, BaseCreature pet)
    {
        if (pet == null) return string.Empty;
        var coordsString = $"{pet.X}, {pet.Y}, {pet.Z}";
        string baseString = $"{pet.Name} (s: {pet.ControlSlots}) ";
        if (pet == pm.Mount)
        {
            return baseString + "(mounted)";
        }
        else if (pm.Map == pet.Map)
        {
            var distance = (int)(pm.GetDistanceToSqrt(pet));
            var direction = pm.GetDirectionTo(pet.X, pet.Y, false);
            var dirString = GetDirectionString(direction);
            var distanceString = GetDistanceString(distance);
            if (distance == 0)
            {
                return baseString + "(standing on your feet)";
            }
            return baseString + $"({coordsString}) {distanceString}, {dirString}{GetRegionString(pet)}";
        }
        else
        {
            return baseString + $"({coordsString}) in {pet.Map.Name}";
        }
    }
    private string GetDirectionString(Direction direction)
    {
        try
        {
            if ((direction | Direction.North) == Direction.North)
            {
                return "North";
            }
            else if ((direction | Direction.South) == Direction.South)
            {
                return "South";
            }
            else if ((direction | Direction.East) == Direction.East)
            {
                return "East";
            }
            else if ((direction | Direction.West) == Direction.West)
            {
                return "West";
            }
            else if ((direction | Direction.Left) == Direction.Left)
            {
                return "Southwest";
            }
            else if ((direction | Direction.Right) == Direction.Right)
            {
                return "Northeast";
            }
            else if ((direction | Direction.Down) == Direction.Down)
            {
                return "Southeast";
            }
            else if ((direction | Direction.Up) == Direction.Up)
            {
                return "Northwest";
            }
        }
        catch (Exception ex)
        {
            return $"Error in GetDirectionString: {ex.Message}";
        }
        return "?" + (int)direction + "?";
    }
    private string GetRegionString(Mobile from)
    {
        try
        {
            if (!string.IsNullOrEmpty(from.Region.Name)) { return $" ({from.Region.Name})"; }
            var map = from.Map;
            if (map != null)
            {
                var reg = Region.Find(from.Location, from.Map);

                if (!reg.IsDefault)
                {
                    var builder = new StringBuilder();

                    builder.Append(reg);
                    reg = reg.Parent;

                    while (reg != null)
                    {
                        builder.Append(reg);
                        reg = reg.Parent;
                    }

                    return $" ({builder})";
                }
                else { return "??"; }
            }
            return "?";
        }
        catch (Exception ex)
        {
            return $"Error in GetRegionString: {ex.Message}";
        }
    }
    private string GetDistanceString(int distance)
    {
        if (distance < 1000)
        {
            return $"{distance} steps";
        }
        else
        {
            return "very far";
        }
    }
}
