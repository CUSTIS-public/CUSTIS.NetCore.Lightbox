using Moq;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Mocks
{
    /// <summary> Скелет для создания моков </summary>
    public class MockSkeleton<T> where T : class
    {
        /// <summary> Мок </summary>
        protected Mock<T> Mock { get; } = new Mock<T>();

        /// <summary> Объект </summary>
        public T Object => Mock.Object;

        /// <summary> Скелет для создания моков </summary>
        public MockSkeleton()
        {
            Reset();
        }

        /// <summary> Сбросить состояние мока </summary>
        public virtual void Reset()
        {
            Mock.Reset();
        }
    }
}