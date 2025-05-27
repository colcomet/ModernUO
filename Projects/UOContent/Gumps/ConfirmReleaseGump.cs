using System;
using MimeKit.Text;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps;

public class ConfirmReleaseGump : StaticGump<ConfirmReleaseGump>
{
    private readonly Mobile _from;
    private readonly BaseCreature _pet;
    private readonly bool _checkRange = true;

    public override bool Singleton => true;

    public ConfirmReleaseGump(Mobile from, BaseCreature pet, bool checkRange = true) : base(50, 50)
    {
        _from = from;
        _pet = pet;
        _checkRange = checkRange;

    }

    protected override void BuildLayout(ref StaticGumpBuilder builder)
    {
        builder.AddPage();

        builder.AddBackground(0, 0, 270, 170, 5054);
        builder.AddBackground(10, 10, 250, 150, 3000);

        // Are you sure you want to release your pet?
        //builder.AddHtmlLocalized(20, 15, 230, 110, 1046257, true, true);
        ReadOnlySpan<char> htmlText = $"Are you sure you wish to release {_pet.Name}? If they are summoned, they will immediately disappear.";
        builder.AddHtml(20, 15, 230, 110, htmlText, true, true);

        builder.AddButton(20, 130, 4005, 4007, 2);
        builder.AddHtmlLocalized(55, 130, 75, 20, 1011011); // CONTINUE

        builder.AddButton(135, 130, 4005, 4007, 1);
        builder.AddHtmlLocalized(170, 130, 75, 20, 1011012); // CANCEL
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (
                info.ButtonID != 2 || _pet.Deleted ||
                !(
                    _pet.Controlled && _from == _pet.ControlMaster &&
                    _from.CheckAlive() &&
                    (
                        !_checkRange ||
                        (_pet.Map == _from.Map && _pet.InRange(_from, 14))
                    )
                 )
           )
        {
            _from.SendMessage("You decide not to release your pet.");
            return;
        }
        _from.PrivateOverheadMessage(MessageType.Regular, MessageHues.BlueNoticeHue, false, $"You release {_pet.Name}!", _from.NetState);
        _pet.ControlTarget = null;
        _pet.ControlOrder = OrderType.Release;
    }
}
