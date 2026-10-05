using System;
using System.Collections.Generic;
using Destrospean;
using Destrospean.CustomOutfits;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Pools;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;

namespace Sims3.Gameplay.Destrospean.Utils
{
    public class NRaasMasterControllerIntegration
    {
        [Tunable]
        static bool kIntegrateNRaasMasterController = true;

        public static void Init()
        {
            LoadSaveManager.ObjectGroupsPreLoad += OnPreLoad;
        }

        public class EditAfterschoolActivityOutfit : CustomAfterschoolActivityOutfit.EditAfterschoolActivityOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditAfterschoolActivityOutfit>
            {
                public AfterschoolActivityType mAfterschoolActivityType;

                public DefinitionModified()
                {
                }

                public DefinitionModified(AfterschoolActivityType afterschoolActivityType)
                {
                    mAfterschoolActivityType = afterschoolActivityType;
                }

                public override void AddInteractions(InteractionObjectPair interaction, Sim actor, GameObject target, List<InteractionObjectPair> results)
                {
                    int index = 0;
                    foreach (AfterschoolActivityType afterschoolActivityType in Enum.GetValues(typeof(AfterschoolActivityType)))
                    {
                        results.Add(new InteractionObjectPair(new DefinitionModified(afterschoolActivityType), target));
                        if (index == 1)
                        {
                            break;
                        }
                        index++;
                    }
                }

                public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
                {
                    EditAfterschoolActivityOutfit editAfterschoolActivityOutfit = new EditAfterschoolActivityOutfit();
                    editAfterschoolActivityOutfit.SetAfterschoolActivityType(mAfterschoolActivityType);
                    editAfterschoolActivityOutfit.Init(ref parameters);
                    return editAfterschoolActivityOutfit;
                }

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return Common.Localize(actor.IsFemale, GetLocalizationKey(mAfterschoolActivityType) + "InteractionName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Common.Localize(isFemale, GetLocalizationKey(mAfterschoolActivityType) + "Path")
                    };
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return (actor == target && CustomOutfits.kShowSimMenu || target as Sim == null && CustomOutfits.kShowObjectMenu) && actor.SimDescription.Child && actor.SimDescription.IsHuman && !actor.SimDescription.IsRobot && !isAutonomous && AfterschoolActivity.HasAfterschoolActivityOfType(actor, mAfterschoolActivityType);
                }
            }

            public override bool Run()
            {
                string outfitName = CustomAfterschoolActivityOutfit.GetAfterschoolActivityOutfitName(Actor, mAfterschoolActivityType);
                return EditSpecialOutfit(Actor, GetLocalizationKey(mAfterschoolActivityType), outfitName, outfitName, ProductVersion.EP4);
            }
        }

        public class EditBachelorPartyOutfit : CustomBachelorPartyOutfit.EditBachelorPartyOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditBachelorPartyOutfit>
            {
                public CustomBachelorPartyOutfit.BachelorPartyOutfitTypes mOutfitType;

                public DefinitionModified()
                {
                }

                public DefinitionModified(CustomBachelorPartyOutfit.BachelorPartyOutfitTypes outfitType)
                {
                    mOutfitType = outfitType;
                }

                public override void AddInteractions(InteractionObjectPair interaction, Sim actor, GameObject target, List<InteractionObjectPair> results)
                {
                    foreach (CustomBachelorPartyOutfit.BachelorPartyOutfitTypes outfitType in Enum.GetValues(typeof(CustomBachelorPartyOutfit.BachelorPartyOutfitTypes)))
                    {
                        results.Add(new InteractionObjectPair(new DefinitionModified(outfitType), target));
                    }
                }

                public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
                {
                    EditBachelorPartyOutfit editBachelorPartyOutfit = new EditBachelorPartyOutfit();
                    editBachelorPartyOutfit.SetOutfitType(mOutfitType);
                    editBachelorPartyOutfit.Init(ref parameters);
                    return editBachelorPartyOutfit;
                }

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return Common.Localize(actor.IsFemale, GetLocalizationKey(mOutfitType) + "InteractionName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Common.Localize(isFemale, GetLocalizationKey(mOutfitType) + "Path0"),
                        Common.Localize(isFemale, GetLocalizationKey(mOutfitType) + "Path1")
                    };
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return (actor == target && CustomOutfits.kShowSimMenu || target as Sim == null && CustomOutfits.kShowObjectMenu) && actor.SimDescription.YoungAdultOrAbove && actor.SimDescription.IsHuman && !actor.SimDescription.IsRobot && !isAutonomous;
                }
            }

            public override bool Run()
            {
                string outfitName = CustomBachelorPartyOutfit.GetBachelorPartyOutfitName(Actor, mOutfitType);
                if (!Actor.SimDescription.HasSpecialOutfit(outfitName))
                {
                    Actor.SimDescription.AddSpecialOutfit(CustomBachelorPartyOutfit.CreateBachelorPartyOutfit(Actor, mOutfitType), outfitName);
                }
                return EditSpecialOutfit(Actor, GetLocalizationKey(mOutfitType), outfitName);
            }
        }

        public class EditBeekeeperOutfit : CustomBeekeeperOutfit.EditBeekeeperOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditBeekeeperOutfit>
            {
                Definition mDefinitionBase = new Definition();

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return mDefinitionBase.GetInteractionName(actor, target, interaction);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return mDefinitionBase.GetPath(isFemale);
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return mDefinitionBase.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
                }
            }

            public override bool Run()
            {
                string outfitName = CustomBeekeeperOutfit.GetBeekeeperOutfitName(Actor);
                return EditSpecialOutfit(Actor, sLocalizationKey, outfitName, outfitName, ProductVersion.EP7);
            }
        }

        public class EditChefOutfit : CustomTeppanyakiChefOutfit.EditChefOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditChefOutfit>
            {
                Definition mDefinitionBase = new Definition();

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return mDefinitionBase.GetInteractionName(actor, target, interaction);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return mDefinitionBase.GetPath(isFemale);
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return mDefinitionBase.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
                }
            }

            public override bool Run()
            {
                return EditSpecialOutfit(Actor, sLocalizationKey, CustomTeppanyakiChefOutfit.kChefSpecialOutfitKey, CustomTeppanyakiChefOutfit.GetChefOutfitName(Actor), ProductVersion.BaseGame);
            }
        }

        public class EditChemistryLabOutfit : CustomChemistryLabOutfit.EditChemistryLabOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditChemistryLabOutfit>
            {
                Definition mDefinitionBase = new Definition();

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return mDefinitionBase.GetInteractionName(actor, target, interaction);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return mDefinitionBase.GetPath(isFemale);
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return mDefinitionBase.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
                }
            }

            public override bool Run()
            {
                return EditSpecialOutfit(Actor, sLocalizationKey, CustomChemistryLabOutfit.kChemistryLabSpecialOutfitKey, CustomChemistryLabOutfit.GetChemistryLabOutfitName(Actor), ProductVersion.EP4);
            }
        }

        public class EditGraduationOutfit : CustomGraduationOutfit.EditGraduationOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditGraduationOutfit>
            {
                public AcademicDegreeNames mAcademicDegreeName;

                public DefinitionModified()
                {
                }

                public DefinitionModified(AcademicDegreeNames academicDegreeName)
                {
                    mAcademicDegreeName = academicDegreeName;
                }

                public override void AddInteractions(InteractionObjectPair interaction, Sim actor, GameObject target, List<InteractionObjectPair> results)
                {
                    foreach (AcademicDegreeNames academicDegreeName in Enum.GetValues(typeof(AcademicDegreeNames)))
                    {
                        switch (academicDegreeName)
                        {
                            case AcademicDegreeNames.Undefined:
                                break;
                            case AcademicDegreeNames.MaxDegreeNames:
                                break;
                            default:
                                results.Add(new InteractionObjectPair(new DefinitionModified(academicDegreeName), target));
                                break;
                        }
                    }
                }

                public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
                {
                    EditGraduationOutfit editGraduationOutfit = new EditGraduationOutfit();
                    editGraduationOutfit.SetAcademicDegreeName(mAcademicDegreeName);
                    editGraduationOutfit.Init(ref parameters);
                    return editGraduationOutfit;
                }

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return Common.Localize(actor.IsFemale, GetLocalizationKey(mAcademicDegreeName) + "InteractionName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Common.Localize(isFemale, GetLocalizationKey(mAcademicDegreeName) + "Path0"),
                        Common.Localize(isFemale, GetLocalizationKey(mAcademicDegreeName) + "Path1")
                    };
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return (actor == target && CustomOutfits.kShowSimMenu || target as Sim == null && CustomOutfits.kShowObjectMenu) && actor.SimDescription.YoungAdultOrAbove && actor.SimDescription.IsHuman && !actor.SimDescription.IsRobot && !isAutonomous;
                }
            }

            public override bool Run()
            {
                string outfitName = CustomGraduationOutfit.GetGraduationOutfitName(Actor, mAcademicDegreeName);
                return EditSpecialOutfit(Actor, GetLocalizationKey(mAcademicDegreeName), outfitName, outfitName, ProductVersion.EP9);
            }
        }

        public class EditHighSchoolGraduationOutfit : CustomHighSchoolGraduationOutfit.EditHighSchoolGraduationOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditHighSchoolGraduationOutfit>
            {
                Definition mDefinitionBase = new Definition();

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return mDefinitionBase.GetInteractionName(actor, target, interaction);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return mDefinitionBase.GetPath(isFemale);
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return mDefinitionBase.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
                }
            }

            public override bool Run()
            {
                return EditSpecialOutfit(Actor, sLocalizationKey, CustomHighSchoolGraduationOutfit.kHighSchoolGraduationSpecialOutfitKey, CustomHighSchoolGraduationOutfit.GetHighSchoolGraduationOutfitName(Actor), ProductVersion.EP4);
            }
        }

        public class EditMassageTableOutfit : CustomMassageTableOutfit.EditMassageTableOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditMassageTableOutfit>
            {
                Definition mDefinitionBase = new Definition();

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return mDefinitionBase.GetInteractionName(actor, target, interaction);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return mDefinitionBase.GetPath(isFemale);
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return mDefinitionBase.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
                }
            }

            public override bool Run()
            {
                return EditSpecialOutfit(Actor, sLocalizationKey, CustomMassageTableOutfit.kMassageTableSpecialOutfitKey, CustomMassageTableOutfit.GetMassageTableOutfitName(Actor), ProductVersion.EP3, Actor.SimDescription.GetOutfit(OutfitCategories.Swimwear, 0));
            }
        }

        public class EditMechanicalBullOutfit : CustomMechanicalBullOutfit.EditMechanicalBullOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditMechanicalBullOutfit>
            {
                Definition mDefinitionBase = new Definition();

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return mDefinitionBase.GetInteractionName(actor, target, interaction);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return mDefinitionBase.GetPath(isFemale);
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return mDefinitionBase.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
                }
            }

            public override bool Run()
            {
                return EditSpecialOutfit(Actor, sLocalizationKey, CustomMechanicalBullOutfit.kMechanicalBullSpecialOutfitKey, CustomMechanicalBullOutfit.GetMechanicalBullOutfitName(Actor), ProductVersion.EP6);
            }
        }

        public class EditSaunaOutfit : CustomSaunaOutfit.EditSaunaOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditSaunaOutfit>
            {
                Definition mDefinitionBase = new Definition();

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return mDefinitionBase.GetInteractionName(actor, target, interaction);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return mDefinitionBase.GetPath(isFemale);
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return mDefinitionBase.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
                }
            }

            public override bool Run()
            {
                Actor.SimDescription.AddSpecialOutfit(new SimOutfit(Actor.SimDescription.GetOutfit(OutfitCategories.Swimwear, 0).Key), CustomSaunaOutfit.kSaunaSpecialOutfitKey);
                return EditSpecialOutfit(Actor, sLocalizationKey, CustomSaunaOutfit.kSaunaSpecialOutfitKey);
            }
        }

        public class EditSingedOutfit : CustomSingedOutfit.EditSingedOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditSingedOutfit>
            {
                Definition mDefinitionBase = new Definition();

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return mDefinitionBase.GetInteractionName(actor, target, interaction);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return mDefinitionBase.GetPath(isFemale);
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return mDefinitionBase.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
                }
            }

            public override bool Run()
            {
                if (!Actor.SimDescription.HasSpecialOutfit(CustomSingedOutfit.kSingedSpecialOutfitKey) && !string.IsNullOrEmpty(OutfitUtils.GetSingedOutfit(Actor)))
                {
                    Actor.SimDescription.AddSpecialOutfit(CustomSingedOutfit.CreateSingedOutfit(Actor), CustomSingedOutfit.kSingedSpecialOutfitKey);
                }
                return EditSpecialOutfit(Actor, sLocalizationKey, CustomSingedOutfit.kSingedSpecialOutfitKey);
            }
        }

        public class EditSkatingOutfit : CustomSkatingOutfit.EditSkatingOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditSkatingOutfit>
            {
                public CustomSkatingOutfit.SkatingTypes mSkatingType;

                public DefinitionModified()
                {
                }

                public DefinitionModified(CustomSkatingOutfit.SkatingTypes skatingType)
                {
                    mSkatingType = skatingType;
                }

                public override void AddInteractions(InteractionObjectPair interaction, Sim actor, GameObject target, List<InteractionObjectPair> results)
                {
                    foreach (CustomSkatingOutfit.SkatingTypes skatingType in Enum.GetValues(typeof(CustomSkatingOutfit.SkatingTypes)))
                    {
                        results.Add(new InteractionObjectPair(new DefinitionModified(skatingType), target));
                    }
                }

                public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
                {
                    EditSkatingOutfit editSkatingOutfit = new EditSkatingOutfit();
                    editSkatingOutfit.SetSkatingType(mSkatingType);
                    editSkatingOutfit.Init(ref parameters);
                    return editSkatingOutfit;
                }

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return Common.Localize(actor.IsFemale, GetLocalizationKey(mSkatingType) + "InteractionName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
                    {
                        Common.Localize(isFemale, GetLocalizationKey(mSkatingType) + "Path0"),
                        Common.Localize(isFemale, GetLocalizationKey(mSkatingType) + "Path1")
                    };
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return InteractionDefinitionUtilities.FromBool((CustomOutfits.kShowObjectMenu && SkatableTerrain.GetPondSkatingAreaAtPoint(parameters.Hit.mPoint) != null && mSkatingType == CustomSkatingOutfit.SkatingTypes.Ice && PondManager.ArePondsFrozen() || parameters.Target is ISkatableObject && mSkatingType == (((ISkatableObject)parameters.Target).IsIceRink ? CustomSkatingOutfit.SkatingTypes.Ice : CustomSkatingOutfit.SkatingTypes.Roller) || CustomOutfits.kShowSimMenu && parameters.Actor == parameters.Target) && parameters.Actor.SimDescription.ChildOrAbove && parameters.Actor.SimDescription.IsHuman && !parameters.Actor.SimDescription.IsRobot && !parameters.Autonomous);
                }
            }

            public override bool Run()
            {
                string outfitName = CustomSkatingOutfit.GetSkatingOutfitName(Actor, mSkatingType);
                if (!Actor.SimDescription.HasSpecialOutfit(outfitName))
                {
                    Actor.SimDescription.AddSpecialOutfit(CustomSkatingOutfit.CreateSkatingOutfit(Actor, mSkatingType), outfitName);
                }
                return EditSpecialOutfit(Actor, GetLocalizationKey(mSkatingType), outfitName);
            }
        }

        public class EditTowelOutfit : CustomTowelOutfit.EditTowelOutfit
        {
            public class DefinitionModified : ImmediateInteractionDefinition<Sim, GameObject, EditTowelOutfit>
            {
                Definition mDefinitionBase = new Definition();

                public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair interaction)
                {
                    return mDefinitionBase.GetInteractionName(actor, target, interaction);
                }

                public override string[] GetPath(bool isFemale)
                {
                    return mDefinitionBase.GetPath(isFemale);
                }

                public override bool Test(Sim actor, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return mDefinitionBase.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
                }

                public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return mDefinitionBase.Test(ref parameters, ref greyedOutTooltipCallback);
                }
            }

            public override bool Run()
            {
                return EditSpecialOutfit(Actor, sLocalizationKey, CustomTowelOutfit.kTowelSpecialOutfitKey, CustomTowelOutfit.GetTowelOutfitName(Actor), ProductVersion.EP3, Actor.SimDescription.GetOutfit(OutfitCategories.Swimwear, 0));
            }
        }

        public static bool EditSpecialOutfit(Sim actor, string localizationKey, string specialOutfitKey)
        {
            if (kIntegrateNRaasMasterController)
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
                new NRaas.MasterControllerSpace.Sims.Stylist().Perform(new NRaas.CommonSpace.Options.GameHitParameters<GameObject>(actor, actor, GameObjectHit.NoHit));
                casLogic.ShowUI += Common.OnShowUI;
                //Common.Notify(Common.Localize(actor.IsFemale, localizationKey + "Warning", actor.Name), simDescription, StyledNotification.NotificationStyle.kSystemMessage);
                while (GameStates.NextInWorldStateId != 0)
                {
                    NRaas.SpeedTrap.Sleep();
                }
                casLogic.ShowUI -= Common.OnShowUI;
                simDescription.AddSpecialOutfit(simDescription.GetOutfit(OutfitCategories.Everyday, 0), specialOutfitKey);
                simDescription.RemoveOutfit(OutfitCategories.Everyday, 0, true);
                actor.SwitchToOutfitWithoutSpin(previousOutfitCategory, previousOutfitIndex);
                if (!CASChangeReporter.Instance.CasCancelled)
                {
                    Common.Notify(Common.Localize(actor.IsFemale, localizationKey + "Feedback", actor.Name), simDescription, StyledNotification.NotificationStyle.kSystemMessage);
                }
                return true;
            }
            return Common.EditSpecialOutfit(actor, localizationKey, specialOutfitKey);
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

        static void OnPreLoad()
        {
            CustomAfterschoolActivityOutfit.EditAfterschoolActivityOutfit.Singleton = new EditAfterschoolActivityOutfit.DefinitionModified();
            CustomBachelorPartyOutfit.EditBachelorPartyOutfit.Singleton = new EditBachelorPartyOutfit.DefinitionModified();
            CustomBeekeeperOutfit.EditBeekeeperOutfit.Singleton = new EditBeekeeperOutfit.DefinitionModified();
            CustomChemistryLabOutfit.EditChemistryLabOutfit.Singleton = new EditChemistryLabOutfit.DefinitionModified();
            CustomGraduationOutfit.EditGraduationOutfit.Singleton = new EditGraduationOutfit.DefinitionModified();
            CustomHighSchoolGraduationOutfit.EditHighSchoolGraduationOutfit.Singleton = new EditHighSchoolGraduationOutfit.DefinitionModified();
            CustomMechanicalBullOutfit.EditMechanicalBullOutfit.Singleton = new EditMechanicalBullOutfit.DefinitionModified();
            CustomMassageTableOutfit.EditMassageTableOutfit.Singleton = new EditMassageTableOutfit.DefinitionModified();
            CustomSaunaOutfit.EditSaunaOutfit.Singleton = new EditSaunaOutfit.DefinitionModified();
            CustomSingedOutfit.EditSingedOutfit.Singleton = new EditSingedOutfit.DefinitionModified();
            CustomSkatingOutfit.EditSkatingOutfit.Singleton = new EditSkatingOutfit.DefinitionModified();
            CustomTeppanyakiChefOutfit.EditChefOutfit.Singleton = new EditChefOutfit.DefinitionModified();
            CustomTowelOutfit.EditTowelOutfit.Singleton = new EditTowelOutfit.DefinitionModified();
        }
    }
}
