using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace CustomItems {
    [ApiVersion (2, 1)]
    public class CustomItems : TerrariaPlugin {
        public override string Name => "CustomItems";
        public override string Author => "Johuan";
        public override string Description => "Allows you to spawn custom items";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public CustomItems(Main game) : base(game) {
        }

        public override void Initialize() {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.NetSendData.Register(this, OnSendData);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NetSendData.Deregister(this, OnSendData);
            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args) {
            Commands.ChatCommands.Add(new Command("customitem", CustomItem, "customitem", "citem")
            {
                HelpText = "/customitem <id/itemname> <parameters> <#> ... \nParameters: hexcolor (hc), prefix (p), stack (st), damage (d), knockback (kb), useanimation (ua), " +
                "usetime (ut), shoot (s), shootspeed (ss), width (w), height (h), scale (sc), ammo (a), useammo (uam), notammo (na)."
            });

            Commands.ChatCommands.Add(new Command("customitem.give", GiveCustomItem, "givecustomitem", "gcitem")
            {
                HelpText = "/givecustomitem <name> <id/itemname> <parameters> <#> ... \nParameters: hexcolor (hc), prefix (p), stack (st), damage (d), knockback (kb), useanimation (ua), " +
                "usetime (ut), shoot (s), shootspeed (ss), width (w), height (h), scale (sc), ammo (a), useammo (uam), notammo (na)."
            });
        
        }

        private void OnSendData(SendDataEventArgs e)
        {
            if ((int)e.MsgId == 88)
            {
                TSPlayer.All.SendInfoMessage($"number: {e.number}");
                TSPlayer.All.SendInfoMessage($"number2: {e.number2}");
                TSPlayer.All.SendInfoMessage($"number3: {e.number3}");
                TSPlayer.All.SendInfoMessage($"number4: {e.number4}");
                TSPlayer.All.SendInfoMessage($"number5: {e.number5}");
            }
        }


        private void CustomItem(CommandArgs args) {
            List<string> parameters = args.Parameters;
            int num = parameters.Count();

            if (num == 0) {
                args.Player.SendErrorMessage("Invalid Syntax. /customitem <id/itemname> <parameters> <#> ... \nParameters: hexcolor (hc), prefix (p), stack (st), damage (d), knockback (kb), useanimation (ua), " +
                "usetime (ut), shoot (s), shootspeed (ss), width (w), height (h), scale (sc), ammo (a), useammo (uam), notammo (na).");
                return;
            }

            List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[0]);
            Item item = items[0];
            
            TSPlayer player = new TSPlayer(args.Player.Index);
            int itemIndex = Item.NewItem((int)player.X, (int)player.Y, item.width, item.height, item.type, item.maxStack);
            
            Item targetItem = Main.item[itemIndex];
            targetItem.playerIndexTheItemIsReservedFor = args.Player.Index;
            BitsByte bb1 = new BitsByte();
            BitsByte bb2 = new BitsByte();

            for (int index = 1; index < num; ++index) {
                string lower = parameters[index].ToLower();
                if ((lower.Equals("hexcolor") || lower.Equals("hc")) && index + 1 < num) {
                    targetItem.color = new Color(int.Parse(args.Parameters[index + 1].Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                        int.Parse(args.Parameters[index + 1].Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                        int.Parse(args.Parameters[index + 1].Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                    bb1[0] = true;
                } else if ((lower.Equals("damage") || lower.Equals("d")) && index + 1 < num) {
                    targetItem.damage = int.Parse(args.Parameters[index + 1]);
                    bb1[1] = true;
                } else if ((lower.Equals("knockback") || lower.Equals("kb")) && index + 1 < num) {
                    targetItem.knockBack = int.Parse(args.Parameters[index + 1]);
                    bb1[2] = true;
                } else if ((lower.Equals("useanimation") || lower.Equals("ua")) && index + 1 < num) {
                    targetItem.useAnimation = int.Parse(args.Parameters[index + 1]);
                    bb1[3] = true;
                } else if ((lower.Equals("usetime") || lower.Equals("ut")) && index + 1 < num) {
                    targetItem.useTime = int.Parse(args.Parameters[index + 1]);
                    bb1[4] = true;
                } else if ((lower.Equals("shoot") || lower.Equals("s")) && index + 1 < num) {
                    targetItem.shoot = int.Parse(args.Parameters[index + 1]);
                    bb1[5] = true;
                } else if ((lower.Equals("shootspeed") || lower.Equals("ss")) && index + 1 < num) {
                    targetItem.shootSpeed = int.Parse(args.Parameters[index + 1]);
                    bb1[6] = true;
                } else if ((lower.Equals("width") || lower.Equals("w")) && index + 1 < num) {
                    targetItem.width = int.Parse(args.Parameters[index + 1]);
                    bb2[0] = true;
                } else if ((lower.Equals("height") || lower.Equals("h")) && index + 1 < num) {
                    targetItem.height = int.Parse(args.Parameters[index + 1]);
                    bb2[1] = true;
                } else if ((lower.Equals("scale") || lower.Equals("sc")) && index + 1 < num) {
                    targetItem.scale = int.Parse(args.Parameters[index + 1]);
                    bb2[2] = true;
                } else if ((lower.Equals("ammo") || lower.Equals("a")) && index + 1 < num) {
                    targetItem.ammo = int.Parse(args.Parameters[index + 1]);
                    bb2[3] = true;
                } else if ((lower.Equals("useammo") || lower.Equals("uam")) && index + 1 < num) {
                    targetItem.useAmmo = int.Parse(args.Parameters[index + 1]);
                    bb2[4] = true;
                } else if ((lower.Equals("notammo") || lower.Equals("na")) && index + 1 < num) {
                    targetItem.notAmmo = Boolean.Parse(args.Parameters[index + 1]);
                    bb2[5] = true;
                } else if ((lower.Equals("prefix") || lower.Equals("p") && index  + 1 < num))
                {
                    targetItem.prefix = (byte)TShock.Utils.GetPrefixByIdOrName(args.Parameters[index + 1])[0];
                } else if ((lower.Equals("stack") || lower.Equals("st") && index + 1 < num))
                {
                    targetItem.stack = int.Parse(args.Parameters[index + 1]);
                }
                ++index;
            }
            if (bb2 > 0)
            {
                bb1[7] = true;
            }
            TSPlayer.All.SendData(PacketTypes.UpdateItemDrop, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.ItemOwner, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.TweakItem, null, itemIndex, bb1, bb2);

        }

        private void GiveCustomItem(CommandArgs args) {
            List<string> parameters = args.Parameters;
            int num = parameters.Count();

            if (num == 0) {
                args.Player.SendErrorMessage("Invalid Syntax. /givecustomitem <name> <id/itemname> <parameters> <#> ... \nParameters: hexcolor (hc), prefix (p), stack (st), damage (d), knockback (kb), useanimation (ua), " +
                "usetime (ut), shoot (s), shootspeed (ss), width (w), height (h), scale (sc), ammo (a), useammo (uam), notammo (na).");
                return;
            }

            List<TSPlayer> players = TSPlayer.FindByNameOrID(args.Parameters[0]);
            if (players.Count != 1) {
                args.Player.SendErrorMessage("Failed to find player of: " + args.Parameters[0]);
                return;
            }

            if (num == 1) {
                args.Player.SendErrorMessage("Failed to provide arguments to item.");
                return;
            }

            List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
            Item item = items[0];

            TSPlayer player = new TSPlayer(players[0].Index);
            int itemIndex = Item.NewItem((int)player.X, (int)player.Y, item.width, item.height, item.type, item.maxStack);

            Item targetItem = Main.item[itemIndex];
            targetItem.playerIndexTheItemIsReservedFor = player.Index;

            for (int index = 2; index < num; ++index) {
                string lower = parameters[index].ToLower();
                if ((lower.Equals("hexcolor") || lower.Equals("hc")) && index + 1 < num) {
                    targetItem.color = new Color(int.Parse(args.Parameters[index + 1].Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                        int.Parse(args.Parameters[index + 1].Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                        int.Parse(args.Parameters[index + 1].Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                } else if ((lower.Equals("damage") || lower.Equals("d")) && index + 1 < num) {
                    targetItem.damage = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("knockback") || lower.Equals("kb")) && index + 1 < num) {
                    targetItem.knockBack = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("useanimation") || lower.Equals("ua")) && index + 1 < num) {
                    targetItem.useAnimation = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("usetime") || lower.Equals("ut")) && index + 1 < num) {
                    targetItem.useTime = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("shoot") || lower.Equals("s")) && index + 1 < num) {
                    targetItem.shoot = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("shootspeed") || lower.Equals("ss")) && index + 1 < num) {
                    targetItem.shootSpeed = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("width") || lower.Equals("w")) && index + 1 < num) {
                    targetItem.width = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("height") || lower.Equals("h")) && index + 1 < num) {
                    targetItem.height = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("scale") || lower.Equals("sc")) && index + 1 < num) {
                    targetItem.scale = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("ammo") || lower.Equals("a")) && index + 1 < num) {
                    targetItem.ammo = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("useammo") || lower.Equals("uam")) && index + 1 < num) {
                    targetItem.useAmmo = int.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("notammo") || lower.Equals("na")) && index + 1 < num) {
                    targetItem.notAmmo = Boolean.Parse(args.Parameters[index + 1]);
                } else if ((lower.Equals("prefix") || lower.Equals("p") && index + 1 < num)) {
                    targetItem.prefix = (byte)TShock.Utils.GetPrefixByIdOrName(args.Parameters[index + 1])[0];
                } else if ((lower.Equals("stack") || lower.Equals("st") && index + 1 < num)) {
                    targetItem.stack = int.Parse(args.Parameters[index + 1]);
                }
                ++index;
            }
            TSPlayer.All.SendData(PacketTypes.UpdateItemDrop, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.ItemOwner, null, itemIndex);
            TSPlayer.All.SendData(PacketTypes.TweakItem, null, itemIndex, 255, 63);
        }
    }
}
