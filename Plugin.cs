using BepInEx;
using HarmonyLib;
using CoinJumper.Patches;

namespace CoinJumper;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.eternalUnion.pluginConfigurator", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    public static float BounceMultiplierValue = 1f;
    public static bool SoundOnValue = true;
    public static int MaxBouncesValue = 0; // 0 = unlimited
    private static readonly Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        Harmony.PatchAll(typeof(CoinPatch));
        if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.eternalUnion.pluginConfigurator"))
            ConfigSetup.Init();
    }
}

internal static class ConfigSetup
{
    internal static void Init()
    {
        // why, just to work without plugin config
        var config = PluginConfig.API.PluginConfigurator.Create(MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_GUID);
        var bounce = new PluginConfig.API.Fields.FloatSliderField(config.rootPanel, "Bouncy", "bouncy", new(0, 2), 1, 1);
        var sound = new PluginConfig.API.Fields.BoolField(config.rootPanel, "Sound", "sound_on", true);
        var maxBounces = new PluginConfig.API.Fields.IntField(config.rootPanel, "Max Bounces (0 = unlimited)", "max_bounces", 0);
        bounce.onValueChange += e => Plugin.BounceMultiplierValue = e.newValue;
        sound.onValueChange += e => Plugin.SoundOnValue = e.value;
        maxBounces.onValueChange += e => Plugin.MaxBouncesValue = e.value;
        Plugin.BounceMultiplierValue = bounce.value;
        Plugin.SoundOnValue = sound.value;
        Plugin.MaxBouncesValue = maxBounces.value;
    }
}
