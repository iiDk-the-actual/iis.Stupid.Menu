/*
 * ii's Stupid Menu  Patches/Menu/PurchasePatch.cs
 * A mod menu for Gorilla Tag with over 1000+ mods
 *
 * Copyright (C) 2026  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using GorillaNetworking;
using HarmonyLib;

namespace iiMenu.Patches.Menu
{
    [HarmonyPatch(typeof(CosmeticsController), nameof(CosmeticsController.PurchaseItem))]
    public class PurchasePatch
    {
        public static bool enabled;

        private static bool Prefix()
        {
            if (enabled)
            {
                CosmeticsController.CosmeticItem itemFromDict = CosmeticsController.instance.GetItemFromDict(CosmeticsController.instance.itemToBuy.itemName);
                if (itemFromDict.itemCategory == CosmeticsController.CosmeticCategory.Set)
                {
                    CosmeticsController.instance.UnlockItem(CosmeticsController.instance.itemToBuy.itemName);
                    foreach (string item in itemFromDict.bundledItems)
                        CosmeticsController.instance.UnlockItem(item);
                }
                else
                    CosmeticsController.instance.UnlockItem(CosmeticsController.instance.itemToBuy.itemName);

                CosmeticsController.instance.UpdateMyCosmetics();
                CosmeticsController.instance.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;

                CosmeticsController.instance.UpdateShoppingCart();
                CosmeticsController.instance.ProcessPurchaseItemState(null, CosmeticsController.instance.isLastHandTouchedLeft);

                return false;
            }

            return true;
        }
    }
}
