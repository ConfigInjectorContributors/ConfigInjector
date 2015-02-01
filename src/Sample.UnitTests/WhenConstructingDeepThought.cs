using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Sample.ConfigSettings;

namespace Sample.UnitTests
{
    [TestClass]
    [TestFixture]
    public class WhenConstructingDeepThought
    {
        private const string _theQuestion = "What is the answer to life, the universe and everything?";
        private const int _theAnswer = 42;

        private DeepThought _deepThought;

        [TestInitialize]
        [SetUp]
        public void SetUp()
        {
            _deepThought = new DeepThought(new QuestionConfigurationSetting { Value = _theQuestion },
                                           new AnswerConfigurationSetting { Value = _theAnswer });
        }

        [TestMethod]
        [Test]
        public void TheQuestionShouldBeCorrect()
        {
            NUnit.Framework.Assert.AreEqual(_theQuestion, _deepThought.Question);
        }

        [TestMethod]
        [Test]
        public void TheAnswerShouldBeCorrect()
        {
            NUnit.Framework.Assert.AreEqual(_theAnswer, _deepThought.Answer);
        }
    }
}