﻿using Rocket.Core.Plugins;
using SDG.Unturned;
using Rocket.API;
using System.IO;
using System.Collections.Generic;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Steamworks;
using System.Threading.Tasks;

namespace InvItemWiper
{
    public class Class1 : RocketPlugin<Config>
    {
        List<CSteamID> Modified = new List<CSteamID>();
        protected override void Load()
        {
            // Trigger the item removal process after the level is loaded
            if(Configuration.Instance.enabled) RemoveItemFromAllPlayers();
            if (Configuration.Instance.Events)
            {
                Rocket.Unturned.U.Events.OnPlayerConnected += PC;
                LoadModified();
            }
        }
        protected override void Unload()
        {
            if (Configuration.Instance.Events)
            {
                Rocket.Unturned.U.Events.OnPlayerConnected -= PC;
                SaveModified();
            }
        }
        void PC(UnturnedPlayer player)
        {
            if (!Modified.Contains(player.CSteamID))
            {
                Items[] pi = player.Inventory.items;
                for(int i = 0; i < pi.Length; i++)
                {
                    if (pi[i] == null || pi[i].items == null) continue;
                    List<ItemJar> items = pi[i].items;
                    for(int y = 0; y < items.Count; y++)
                    {
                        if (items[y] == null || items[y].item == null) continue;
                        if(Configuration.Instance.ItemID.Contains(items[y].item.id))
                        {
                            //ItemManager.dropItem(items[y].item, player.Position, false, true, false);
                            player.Inventory.removeItem(pi[i].page, pi[i].getIndex(items[y].x, items[y].y));
                        }
                    }
                }
                Modified.Add(player.CSteamID);
                Task.Run(() => SaveModified());
            }
        }
        public void LoadModified()
        {
            string filePath = Path.Combine(Directory, "Modified.log");
            if (!File.Exists(filePath))
            {
                return;
            }
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                Modified.Add(new CSteamID(ulong.Parse(line)));
            }
        }
        public void SaveModified()
        {
            List<string> lines = new List<string>();

            for(int i=0;i<Modified.Count;i++)
            {
                string line = Modified[i].m_SteamID.ToString();
                lines.Add(line);
            }

            File.WriteAllLines(Path.Combine(Directory, "Modified.log"), lines);
        }
        private void RemoveItemFromAllPlayers()
        {
            string playersDirectory = Path.Combine(ReadWrite.PATH, "Servers", Configuration.Instance.ServerName, "Players");
            Logger.Log("Checking " + playersDirectory);
            if (System.IO.Directory.Exists(playersDirectory))
            {
                Logger.Log("Existed, will now for loop");
                foreach (string playerFolder in System.IO.Directory.GetDirectories(playersDirectory))
                {
                    string inventoryFile = Path.Combine(playerFolder, Configuration.Instance.LevelName, "Player", "Inventory.dat");
                    if (File.Exists(inventoryFile))
                    {
                        // Read and process the inventory file
                        RemoveSpecificItemFromInventory(inventoryFile);
                    }else Logger.Log(inventoryFile + " did not exist");
                }
            }
            else
            {
                Logger.Log("Did not exist.");
            }
        }

        private void RemoveSpecificItemFromInventory(string inventoryFile)
        {
            Logger.Log("Removing items for " + inventoryFile);
            // Read the inventory file into a byte array
            byte[] data = File.ReadAllBytes(inventoryFile);
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(data))
                using (BinaryReader reader = new BinaryReader(memoryStream))
                using (MemoryStream outputStream = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(outputStream))
                {
                    // Deserialize the inventory data
                    // Example: Adjust the following pseudo-code based on the actual inventory structure
                    // var inventory = DeserializeInventory(reader);

                    // Deserialize the inventory here
                    var inventory = new List<InventoryItem>();

                    // Simplified example of reading items from the inventory
                    int itemCount = reader.ReadInt32();
                    for (int i = 0; i < itemCount; i++)
                    {
                        if (memoryStream.Position + 3 > memoryStream.Length)
                        {
                            Logger.LogError($"Unexpected end of file in {inventoryFile} after reading {i} items.");
                            break;
                        }

                        ushort itemId = reader.ReadUInt16();
                        byte itemAmount = reader.ReadByte();

                        if (!Configuration.Instance.ItemID.Contains(itemId))
                        {
                            inventory.Add(new InventoryItem(itemId, itemAmount));
                            Logger.Log("Keeping item " + itemId);
                        }
                        else
                        {
                            Logger.Log("Skipping item " + itemId);
                        }
                    }

                    // Write the modified inventory back to a byte array
                    writer.Write(inventory.Count);
                    foreach (var item in inventory)
                    {
                        writer.Write(item.ItemID);
                        writer.Write(item.Amount);
                    }

                    // Get the modified data
                    byte[] modifiedData = outputStream.ToArray();

                    // Save the modified data back to the file
                    File.WriteAllBytes(inventoryFile, modifiedData);
                }
            }
            catch (System.Exception e)
            {
                Logger.LogException(e);
            }
        }
        private class InventoryItem
        {
            public ushort ItemID { get; }
            public byte Amount { get; }

            public InventoryItem(ushort itemId, byte amount)
            {
                ItemID = itemId;
                Amount = amount;
            }
        }
    }
}
