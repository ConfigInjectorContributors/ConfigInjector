using NUnit.Framework;
using Sample.Configuration;

namespace Sample.UnitTests
{
    [TestFixture]
    public class WhenConstructingDeepThought
    {
        private const string _theQuestion = "What is the answer to life, the universe and everything?";
        private const int _theAnswer = 42;

        private DeepThought _deepThought;

        [SetUp]
        public void SetUp()
        {
            _deepThought = new DeepThought(new QuestionConfigurationSetting {Value = _theQuestion},
                                           new AnswerConfigurationSetting {Value = _theAnswer});
        }

        [Test]
        public void TheQuestionShouldBeCorrect()
        {
            Assert.AreEqual(_theQuestion, _deepThought.Question);
        }

        [Test]
        public void TheAnswerShouldBeCorrect()
        {
            Assert.AreEqual(_theAnswer, _deepThought.Answer);
        }
    }
}