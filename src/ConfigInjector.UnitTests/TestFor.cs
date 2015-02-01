using NUnit.Framework;

namespace ConfigInjector.UnitTests
{
    public abstract class TestFor<T>
    {
        protected abstract T Given();
        protected abstract void When();

        protected T Subject { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Subject = Given();
            When();
        }
    }
}