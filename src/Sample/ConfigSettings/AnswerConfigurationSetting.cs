using ConfigInjector;

namespace Sample.ConfigSettings
{
    public class AnswerConfigurationSetting : ConfigurationSetting<int>
    {
        protected override System.Collections.Generic.IEnumerable<string> ValidationErrors(int value)
        {
            if (value < 42) yield return "Too low!";
            if (value > 42) yield return "Too high!";
        }
    }
}