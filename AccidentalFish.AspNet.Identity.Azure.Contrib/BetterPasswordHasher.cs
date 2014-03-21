using CryptoHelper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccidentalFish.AspNet.Identity.Azure.Contrib
{
    public class BetterPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return PasswordHash.CreateHash(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            PasswordVerificationResult result = PasswordVerificationResult.Failed;
            if (PasswordHash.ValidatePassword(providedPassword, hashedPassword))
            {
                result = PasswordVerificationResult.Success;
            }
            return result;
        }
    }
}
