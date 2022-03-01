namespace StreetNameRegistry
{
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;

    public static class StringExtensions
    {
        public static string ToHash(this IHaveHashFields haveHashFields, params string[] extraValues)
        {
            const string hashSeparator = "þ";

            var valuesToHash = extraValues.Union(haveHashFields.GetHashFields());
            var value = string.Join(hashSeparator, valuesToHash);

            using SHA512 sha512Managed = new SHA512Managed();
            var hashedBytes = sha512Managed.ComputeHash(Encoding.UTF8.GetBytes(value));

            return GetStringFromHash(hashedBytes);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}
