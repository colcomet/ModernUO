using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Accounting;
using Server.Mobiles;

namespace Server.Commands.Maint
{
    //Class for performing maintenance on Rucci's shard custom command-related Account Tags.
    public static class RuccisCommandMaint
    {
        private static bool maintHasRun = false;
        private static readonly string LogFilePath = Path.Combine("Logs", "RuccisCommandsMaintLog.txt");
        private static object maintLock = new object();
        public static void RunMaint()
        {
            lock (maintLock)
            {
                if (maintHasRun) return;
                maintHasRun = true;
            }
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                var allAccounts = Accounts.GetAccounts();
                foreach (var act in allAccounts)
                {
                    try
                    {
                        //Tag related properties and methods only exist on the Account class,
                        //not on the IAccount interface, so we need to cast to Account.
                        var account = act as Account;
                        var allTags = account.Tags.ToList();
                        foreach (var tag in allTags)
                        {
                            //only check tags that are EquipSet_ tags
                            if (tag.Name.StartsWith("EquipSet_"))
                            {
                                var parts = tag.Name.Split('_');
                                //if the tag doesn't have 3 parts, it's invalid
                                if (parts.Length != 3)
                                {
                                    DeleteInvalidEquipSetTag(tag, account, "Tag name appears invalid.");
                                    continue;
                                }
                                uint serial = 0;
                                //if the serial isn't a valid uint, it's invalid
                                if (!uint.TryParse(parts[1], out serial))
                                {
                                    DeleteInvalidEquipSetTag(tag, account, "character serial is not a valid uint");
                                    continue;
                                }
                                var pm = World.FindMobile((Serial)serial) as PlayerMobile;
                                //if the player isn't found or is deleted, it's invalid
                                if (pm == null || pm.Deleted)
                                {
                                    DeleteInvalidEquipSetTag(tag, account, "Character does not exist");
                                    continue;
                                }
                                int setId = 0;
                                //if the setId isn't a valid int between 0 and 9, it's invalid
                                if (!int.TryParse(parts[2], out setId) || setId < 0 || setId > 9)
                                {
                                    DeleteInvalidEquipSetTag(tag, account, "SetId out of 0-9 range");
                                    continue;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            });
        }
        private static void DeleteInvalidEquipSetTag(AccountTag tag, Account account, string reason)
        {
            account.RemoveTag(tag.Name);
            string line = $"Deleted invalid EquipSet tag: {tag.Name}:{tag.Value} from account {account.Username} for reason: {reason}";
            WriteLogEntry(line);
        }
        private static void WriteLogEntry(string line)
        {
            using (var sw = new StreamWriter(LogFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now.ToString("s")} :{line}");
            }
        }


    }
}
