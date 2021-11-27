using CUSTIS.NetCore.Lightbox.UnitTests.Common;
using CUSTIS.NetCore.Lightbox.Utils;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Core
{
    public class TypeLoaderTests
    {
        [Test]
        public void GetType_CorrectType_TypeRetrieved()
        {
            //Arrange
            var loader = new TypeLoader();

            //Act
            var type = loader.RetrieveType(typeof(Dto).FullName!);

            //Assert
            Assert.That(type, Is.EqualTo(typeof(Dto)));
        }

        [Test]
        public void GetType_IncorrectType_ExceptionThrown()
        {
            //Arrange
            var loader = new TypeLoader();

            //Act & Assert
            Assert.That(() => loader.RetrieveType("tro-lo-lo"),
                        Throws.Exception.With.Message.EqualTo("Type tro-lo-lo doesn't exist in the current app domain"));
        }
    }
}