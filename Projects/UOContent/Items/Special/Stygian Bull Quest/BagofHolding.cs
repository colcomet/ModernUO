/****************************************
 * Original Author Unknown              *
 * Updated for MUO by Delphi            *
 * Updated again by AKAWilbur           *
 * Date: March 26, 2025                 *
 ****************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class BagOfHolding : Bag
    {
        private readonly Dictionary<Item, double> originalWeights = new Dictionary<Item, double>();

        [Constructible]
        public BagOfHolding() : this(10) // create a 10 item bag by default
        {
        }

        [Constructible]
        public BagOfHolding(int maxitems)
        {
            Weight = 0.0;
            MaxItems = maxitems;
            Name = "Bag of Holding";
            Hue = 1929;
        }

        public BagOfHolding(Serial serial) : base(serial)
        {
        }

        public override void OnItemAdded(Item item)
        {
            base.OnItemAdded(item);
            if (!originalWeights.ContainsKey(item))
            {
                originalWeights[item] = item.Weight;
                item.Weight /= 10.0;
                if (RootParent is Mobile mobile)
                {
                    mobile.SendMessage(68, "Your item feels much lighter!");
                }
            }
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);
            if (originalWeights.ContainsKey(item))
            {
                item.Weight = originalWeights[item];
                originalWeights.Remove(item);
                if (RootParent is Mobile mobile)
                {
                    mobile.SendMessage(68, "This item seems to be its normal weight now.");
                }
            }
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            // Save the original weights
            writer.Write(originalWeights.Count);
            foreach (var kvp in originalWeights)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            // Load the original weights
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Item item = reader.ReadEntity<Item>();
                double weight = reader.ReadDouble();
                if (item != null)
                {
                    originalWeights[item] = weight;
                    // Restore the correct weight in case the item is currently in the bag
                    if (Items.Contains(item))
                    {
                        item.Weight /= 10.0;
                    }
                }
            }
        }

        public void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, $"Item capacity\t{MaxItems}"); // ~1_val~: ~2_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            from.SendMessage(68, $"Item capacity: {MaxItems}");
        }
    }
}
