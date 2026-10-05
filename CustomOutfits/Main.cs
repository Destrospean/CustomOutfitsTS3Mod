namespace Destrospean.CustomOutfits
{
    public class Main
    {
        [Sims3.SimIFace.Tunable]
        protected static bool kInstantiator;

        static Main()
        {
            if (System.Array.Exists(System.AppDomain.CurrentDomain.GetAssemblies(), x => x.GetName().Name == "NRaasMasterController"))
            {
                Sims3.Gameplay.Destrospean.Utils.NRaasMasterControllerIntegration.Init();
            }
        }
    }
}
