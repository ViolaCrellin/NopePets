using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Server.Configuration;
using Server.TestingShared;
using Server.Util;

namespace Server.Test.Unit.Util
{
    [TestFixture]
    public class EncrypterTests
    {
        [TestCase("Hello")]
        [TestCase("")]
        [TestCase("123ljkdfs99")]
        [TestCase("程序设计员在用电脑")]
        [TestCase("المبرمجة إستخدمت الحاسوب")]
        [TestCase("プログラマーがコンピュータを使う。")]
        [TestCase("¯\\_(ツ)_/¯")]
        public void Given_any_string_when_encrypted_it_can_be_decrypted(string input)
        {
            var sut = new Encrypter(TestData.TestConfiguration);

            var encrypted = sut.Encrypt(input);
            Assert.AreNotEqual(encrypted, input);

            var decrypted = sut.Decrypt(encrypted);
            Assert.AreEqual(decrypted, input);
        }

        [Test]
        public void Given_a_null_value_then_when_encrypted_or_decrypted_no_exception_is_thrown_and_null_is_returned()
        {
            var sut = new Encrypter(TestData.TestConfiguration);

            var encrypted = sut.Encrypt(null);
            Assert.DoesNotThrow(() => sut.Encrypt(null));
            Assert.IsNull(encrypted);

            var decrypted = sut.Decrypt(encrypted);
            Assert.DoesNotThrow(() => sut.Decrypt(encrypted));
            Assert.IsNull(decrypted);
        }

    }
}
