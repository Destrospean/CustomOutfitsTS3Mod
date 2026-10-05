using System;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;

namespace Destrospean.CustomOutfits
{
    public class Common
    {
        internal const string kLocalizationPath = "Destrospean/";

        public static void CopyTuning(Type baseType, Type oldType, Type newType)
        {
            if (AutonomyTuning.GetTuning(newType.FullName, baseType.FullName) == null)
            {
                InteractionTuning tuning = AutonomyTuning.GetTuning(oldType, oldType.FullName, baseType);
                if (tuning != null)
                {
                    AutonomyTuning.AddTuning(newType.FullName, baseType.FullName, tuning);
                }
            }
            InteractionObjectPair.sTuningCache.Remove(new Pair<Type, Type>(newType, baseType));
        }

        public static ResourceKey CreateAndAddSpecialOutfit(Sim actor, string specialOutfitKey, ResourceKey uniformKey)
        {
            if (uniformKey == ResourceKey.kInvalidResourceKey)
            {
                return uniformKey;
            }
            SimDescription simDescription = actor.SimDescription;
            if (simDescription.HasSpecialOutfit(specialOutfitKey))
            {
                return simDescription.GetSpecialOutfit(specialOutfitKey).Key;
            }
            ResourceKey key = OutfitUtils.ApplyUniformToOutfit(simDescription.GetOutfit(OutfitCategories.Everyday, 0), new SimOutfit(uniformKey), simDescription, "CreateAndAddSpecialOutfit");
            simDescription.AddSpecialOutfit(new SimOutfit(key), specialOutfitKey);
            return key;
        }

        public static bool EditSpecialOutfit(Sim actor, string localizationKey, string specialOutfitKey)
        {
            SimDescription simDescription = actor.SimDescription;
            if (!simDescription.HasSpecialOutfit(specialOutfitKey))
            {
                simDescription.AddSpecialOutfit(simDescription.GetOutfit(OutfitCategories.Everyday, 0), specialOutfitKey);
            }
            OutfitCategories previousOutfitCategory = actor.CurrentOutfitCategory;
            int previousOutfitIndex = actor.CurrentOutfitIndex;
            simDescription.AddOutfit(simDescription.GetSpecialOutfit(specialOutfitKey), OutfitCategories.Everyday, 0);
            simDescription.RemoveSpecialOutfit(specialOutfitKey);
            actor.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday, 0);
            CASLogic casLogic = CASLogic.GetSingleton();
            casLogic.ShowUI += OnShowUI;
            casLogic.UseTempSimDesc = true;
            casLogic.LoadSim(simDescription, actor.CurrentOutfitCategory, actor.CurrentOutfitIndex);
            CASChangeReporter.Instance.ClearChanges();
            GameStates.TransitionToCASStylistMode();
            //Notify(Localize(actor.IsFemale, localizationKey + "Warning", actor.Name), simDescription, StyledNotification.NotificationStyle.kSystemMessage);
            while (GameStates.NextInWorldStateId != 0)
            {
                Simulator.Sleep(0);
            }
            CASChangeReporter.Instance.SendChangedEvents(actor);
            casLogic.ShowUI -= OnShowUI;
            simDescription.AddSpecialOutfit(simDescription.GetOutfit(OutfitCategories.Everyday, 0), specialOutfitKey);
            simDescription.RemoveOutfit(OutfitCategories.Everyday, 0, true);
            actor.SwitchToOutfitWithoutSpin(previousOutfitCategory, previousOutfitIndex);
            if (!CASChangeReporter.Instance.CasCancelled)
            {
                Notify(Localize(actor.IsFemale, localizationKey + "Feedback", actor.Name), simDescription, StyledNotification.NotificationStyle.kSystemMessage);
            }
            return true;
        }

        public static bool EditSpecialOutfit(Sim actor, string localizationKey, string specialOutfitKey, string outfitName, uint group)
        {
            return EditSpecialOutfit(actor, localizationKey, specialOutfitKey, outfitName, group, actor.SimDescription.GetOutfit(OutfitCategories.Everyday, 0));
        }

        public static bool EditSpecialOutfit(Sim actor, string localizationKey, string specialOutfitKey, string outfitName, uint group, SimOutfit outfitToApplyTo)
        {
            SimDescription simDescription = actor.SimDescription;
            if (!simDescription.HasSpecialOutfit(specialOutfitKey))
            {
                SimOutfit resultOutfit;
                if (OutfitUtils.TryApplyUniformToOutfit(outfitToApplyTo, new SimOutfit(ResourceKey.CreateOutfitKey(outfitName, group)), simDescription, "EditSpecialOutfit", out resultOutfit))
                {
                    simDescription.AddSpecialOutfit(resultOutfit, specialOutfitKey);
                }
            }
            return EditSpecialOutfit(actor, localizationKey, specialOutfitKey);
        }

        public static bool EditSpecialOutfit(Sim actor, string localizationKey, string specialOutfitKey, string outfitName, ProductVersion productVersion)
        {
            return EditSpecialOutfit(actor, localizationKey, specialOutfitKey, outfitName, ResourceUtils.ProductVersionToGroupId(productVersion));
        }

        public static bool EditSpecialOutfit(Sim actor, string localizationKey, string specialOutfitKey, string outfitName, ProductVersion productVersion, SimOutfit outfitToApplyTo)
        {
            return EditSpecialOutfit(actor, localizationKey, specialOutfitKey, outfitName, ResourceUtils.ProductVersionToGroupId(productVersion), outfitToApplyTo);
        }

        public static string Localize(string entryKey)
        {
            return Localization.LocalizeString(kLocalizationPath + entryKey);
        }

        public static string Localize(string entryKey, params object[] parameters)
        {
            return Localization.LocalizeString(kLocalizationPath + entryKey, parameters);
        }

        public static string Localize(bool isFemale, string entryKey, params object[] parameters)
        {
            return Localization.LocalizeString(isFemale, kLocalizationPath + entryKey, parameters);
        }

        public static void Notify(string message, SimDescription simDescription, StyledNotification.NotificationStyle style)
        {
            Notify(message, simDescription, style, true);
        }

        public static void Notify(string message, SimDescription fakeSimDescription, StyledNotification.NotificationStyle style, bool checkForFake)
        {
            SimDescription simDescription = fakeSimDescription;
            if (simDescription == null)
            {
                StyledNotification.Show(new StyledNotification.Format(message, style));
                return;
            }
            if (checkForFake)
            {
                simDescription = SimDescription.Find(fakeSimDescription.SimDescriptionId);
                if (simDescription == null)
                {
                    StyledNotification.Show(new StyledNotification.Format(message, style));
                    return;
                }
            }
            StyledNotification.Show(simDescription.CreatedSim == null ? new StyledNotification.Format(message, style) : new StyledNotification.Format(message, ObjectGuid.InvalidObjectGuid, simDescription.CreatedSim.ObjectId, style));
        }

        public static void OnShowUI(bool toShow)
        {
            if (!toShow)
            {
                return;
            }
            CASDresserSheet casDresserSheet = CASDresserSheet.gSingleton;
            if (casDresserSheet == null || casDresserSheet.mButtons == null)
            {
                return;
            }
            for (int i = 1; i < casDresserSheet.mButtons.Length; i++)
            {
                if (casDresserSheet.mButtons[i] != null)
                {
                    casDresserSheet.mButtons[i].Visible = false;
                }
                if (casDresserSheet.mButtonText[i] != null)
                {
                    casDresserSheet.mButtonText[i].Visible = false;
                }
            }
            CASDresserClothing casDresserClothing = CASDresserClothing.gSingleton;
            if (casDresserClothing == null || casDresserClothing.mOutfitButtons == null || casDresserClothing.mDeleteOutfitButtons == null)
            {
                return;
            }
            for (int i = 1; i < casDresserClothing.mOutfitButtons.Length; i++)
            {
                casDresserClothing.mOutfitButtons[i].Visible = false;
                casDresserClothing.mDeleteOutfitButtons[i].Visible = false;
            }
            casDresserClothing.mAddOutfitButton.Visible = false;
        }

        /// <summary>This method was borrowed from Lazy Duchess' Mono Patcher</summary>
        public static void ReplaceMethod(System.Reflection.MethodInfo oldMethod, System.Reflection.MethodInfo newMethod)
        {
            byte[] replacementByteArray = new byte[40];
            System.Runtime.InteropServices.Marshal.Copy(newMethod.MethodHandle.Value, replacementByteArray, 0, 40);
            System.Runtime.InteropServices.Marshal.Copy(replacementByteArray, 0, oldMethod.MethodHandle.Value, 24);
            System.Runtime.InteropServices.Marshal.Copy(replacementByteArray, 28, new IntPtr(oldMethod.MethodHandle.Value.ToInt32() + 28), 12);
        }
    }
}
