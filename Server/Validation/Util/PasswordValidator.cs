using System.Linq;
using System.Text.RegularExpressions;

namespace Server.Validation.Util
{
    public enum PasswordComplexity
    {
        Blank = 0,
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5, 
        Impenetrable = 6
    }

    public class PasswordValidator
    {
        public static int PasswordComplexityScore(string password)
        {
            int score = 0;

            if (password == null)
                return score;

            if (password.Length > 4)
                score++;
            if (password.Length > 8)
                score++;
            if (password.Length > 12)
                score++;
            //Must contain a number
            if (password.Any(char.IsDigit))
                score++;
            //Must contain a capital letter
            if (password.Any(char.IsUpper) && password.Any(char.IsLower))
                score++;
            //Must contain a special character
            if (password.Any(char.IsPunctuation))
                score++;

            return score;
        }
    }
}