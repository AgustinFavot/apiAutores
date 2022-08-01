using apiVS.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace apiVS.Services
{
    public class HashService
    {
        public Hash createSemilla(string textplano) {

            var semilla = new byte[16];
            using (var random = RandomNumberGenerator.Create()) {
                random.GetBytes(semilla);            
            }
            return Hash(textplano, semilla);
        }

        public Hash Hash(string textplano, byte[] semilla)
        {
            var llave = KeyDerivation.Pbkdf2(password: textplano, salt: semilla, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 32);
            var hash = Convert.ToBase64String(llave);
            return new Hash() { hash = hash, semilla = llave };
        }
    }
}
