/****************************************
 * Original Author Unknown              *
 * Updated for MUO by Delphi            *
 * Updated again by AKAWilbur           *
 * Date: April 1, 2025                  *
 ****************************************/

using Server.Network;

namespace Server.Gumps
{
    public class GeorgiaGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("GeorgiaGump", AccessLevel.GameMaster, GeorgiaGump_OnCommand);
        }

        private static void GeorgiaGump_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new GeorgiaGump() );
        }

        public GeorgiaGump() : base( 50,50 )
        {
            //----------------------------------------------------------------------------------------------------

            AddPage( 0 );
            AddImageTiled(  54, 33, 369, 400, 2624 );
            AddAlphaRegion( 54, 33, 369, 400 );

            AddImageTiled( 416, 39, 44, 389, 203 );
            //--------------------------------------Window size bar--------------------------------------------

            AddImage( 97, 49, 9005 );
            AddImageTiled( 58, 39, 29, 390, 10460 );
            AddImageTiled( 412, 37, 31, 389, 10460 );
            AddLabel( 140, 60, 0x34, "Message" );


            AddHtml( 107, 140, 300, 230, "<BODY>" +
                                         //----------------------/----------------------------------------------/
                                         "<BASEFONT COLOR=YELLOW>Hey there!!!<BR><BR>I guess my brother sent you for more hides...<BR><BR>" +
                                         "<BASEFONT COLOR=YELLOW>He's too much of a chicken to get them himself!  He tells everyone he 'lost' his bulls...<BR><BR>They were never his to begin with, they were MY project!  I brought them here to keep him from stealing them.  I can make you a special backpack if you don't tell him I'm here.<BR>" +
                                         "<BASEFONT COLOR=YELLOW><BR>Bring me 30 of their stygian hides and I'll make you one of my awesome knapsacks!  I can only make you one backpack, so don't ask for more!<BR>" +
                                         "<BASEFONT COLOR=YELLOW><BR>Be careful, I bred the bulls from Stygian stock, so they're tough as nails! <BR><BR>Return to me once you have acquired 30 pieces of the stygian bull hides!" +
                                         "</BODY>", false, true);

            //			<BASEFONT COLOR=#7B6D20>

            AddImage( 430, 9, 10441);
            AddImageTiled( 40, 38, 17, 391, 9263 );
            AddImage( 6, 25, 10421 );
            AddImage( 34, 12, 10420 );
            AddImageTiled( 94, 25, 342, 15, 10304 );
            AddImageTiled( 40, 427, 415, 16, 10304 );
            AddImage( -10, 314, 10402 );
            AddImage( 56, 150, 10411 );
            AddImage( 155, 120, 2103 );
            AddImage( 136, 84, 96 );

            AddButton( 225, 390, 0xF7, 0xF8, 0 );

            //--------------------------------------------------------------------------------------------------------------
        }

        public override void OnResponse( NetState state, in RelayInfo info ) //Function for GumpButtonType.Reply Buttons
        {
            Mobile from = state.Mobile;

            switch ( info.ButtonID )
            {
                case 0: //Case uses the ActionIDs defenied above. Case 0 defenies the actions for the button with the action id 0
                    {
                        //Cancel
                        from.SendMessage( "Bring me some of my magical leather!" );
                        break;
                    }

            }
        }
    }
}
