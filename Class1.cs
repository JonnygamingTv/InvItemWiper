using Rocket.Core.Plugins;
using SDG.Unturned;
using Rocket.API;
using System.IO;
using System.Collections.Generic;
using Rocket.Core.Logging;

namespace InvItemWiper
{
    public class Class1 : RocketPlugin<Config>
    {
        protected override void Load()
        {
            // Trigger the item removal process after the level is loaded
            if(Configuration.Instance.enabled) RemoveItemFromAllPlayers();
        }
        private void RemoveItemFromAllPlayers()
        {
            string playersDirectory = Path.Combine(ReadWrite.PATH, "Players");
            Logger.Log("Checking " + playersDirectory);
            if (System.IO.Directory.Exists(playersDirectory))
            {
                foreach (string playerFolder in System.IO.Directory.GetDirectories(playersDirectory))
                {
                    string inventoryFile = Path.Combine(playerFolder, "Retrovia", "Inventory.dat");
                    if (File.Exists(inventoryFile))
                    {
                        // Read and process the inventory file
                        RemoveSpecificItemFromInventory(inventoryFile);
                    }else Logger.Log(inventoryFile + " did not exist");
                }
            }
        }

        private void RemoveSpecificItemFromInventory(string inventoryFile)
        {
            Logger.Log("Removing items for " + inventoryFile);
            // Read the inventory file into a byte array
            byte[] data = File.ReadAllBytes(inventoryFile);

            using (MemoryStream memoryStream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(memoryStream))
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
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
                    ushort itemId = reader.ReadUInt16();
                    byte itemAmount = reader.ReadByte();

                    if (itemId != Configuration.Instance.ItemID)
                    {
                        inventory.Add(new InventoryItem(itemId, itemAmount));
                        Logger.Log("Keeping item " + itemId);
                    }
                    else
                    {
                        Logger.Log("Skipping item "+itemId);
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
                byte[] modifiedData = ((MemoryStream)writer.BaseStream).ToArray();

                // Save the modified data back to the file
                File.WriteAllBytes(inventoryFile, modifiedData);
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
