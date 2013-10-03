using Sample.ConfigSettings;

namespace Sample
{
    public class DeepThought
    {
        private readonly QuestionConfigurationSetting _question;
        private readonly AnswerConfigurationSetting _answer;

        public DeepThought(QuestionConfigurationSetting question, AnswerConfigurationSetting answer)
        {
            _question = question;
            _answer = answer;
        }

        public string Question
        {
            get { return _question; }
        }

        public int Answer
        {
            get { return _answer; }
        }
    }
}