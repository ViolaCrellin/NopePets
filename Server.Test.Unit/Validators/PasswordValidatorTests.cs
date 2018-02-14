using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework;
using Server.Validation.Util;

namespace Server.Test.Unit.Validators
{
    [TestFixture]
    public class PasswordValidatorTests
    {
        public static IEnumerable PasswordStrengthTestCases
        {
            get
            {
                yield return new TestCaseData(string.Empty, PasswordComplexity.Blank);
                yield return new TestCaseData("tom", PasswordComplexity.Blank);
                yield return new TestCaseData("Tom", PasswordComplexity.VeryWeak);
                yield return new TestCaseData("t.m", PasswordComplexity.VeryWeak);
                yield return new TestCaseData("t0m", PasswordComplexity.VeryWeak);
                yield return new TestCaseData("thomas", PasswordComplexity.VeryWeak);
                yield return new TestCaseData("th0mas", PasswordComplexity.Weak);
                yield return new TestCaseData("Thomas", PasswordComplexity.Weak);
                yield return new TestCaseData("T0m", PasswordComplexity.Weak);
                yield return new TestCaseData("thomasthomas", PasswordComplexity.Weak);
                yield return new TestCaseData("Thomasthomas", PasswordComplexity.Medium);
                yield return new TestCaseData("Th0mas", PasswordComplexity.Medium);
                yield return new TestCaseData("thomasthetankengine", PasswordComplexity.Medium);
                yield return new TestCaseData("Thomasth0mas", PasswordComplexity.Strong);
                yield return new TestCaseData("thom@sth0mas", PasswordComplexity.Strong);
                yield return new TestCaseData("Thomasthetankengine", PasswordComplexity.Strong);
                yield return new TestCaseData("Th0masthetankengine", PasswordComplexity.VeryStrong);
                yield return new TestCaseData("Th0masthom@s", PasswordComplexity.VeryStrong);
                yield return new TestCaseData("Th0masthetankengine", PasswordComplexity.VeryStrong);
                yield return new TestCaseData("Th0masthetankeng/ne", PasswordComplexity.Impenetrable);
                yield return new TestCaseData("Thom@sth0mas.", PasswordComplexity.Impenetrable);
                yield return new TestCaseData("dfsj34*^40fksd;JS", PasswordComplexity.Impenetrable);
            }
        }

        [TestCaseSource(nameof(PasswordStrengthTestCases))]
        public void Given_a_password_then_when_scored_returns_correct_PasswordComplexity_score(string password, PasswordComplexity expectedScore)
        {
            var resultScore = (PasswordComplexity) PasswordValidator.PasswordComplexityScore(password);

            Assert.AreEqual(expectedScore, resultScore);
        }

    }
}
