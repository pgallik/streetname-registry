namespace StreetNameRegistry.Tests.Generate
{
    using System;
    using System.Collections.Generic;
    using NodaTime;

    [Obsolete("Do not use this class, try with autofixture and/or look at projection V2 tests")]
    public static class Produce
    {
        // Use this flag to investigate possible problems with multiple execution of IEnumerable (for example, IQueryables being executed multiple times)
        private const bool UseDeferredEnumerableExecution = false;

        private const string Digits = "0123456789";
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string SpecialChars = "/-.,";
        private const string SpaceChars = " ";

        private const int ManyMaxLimit = 25;

        public static Generator<string> AlphaNumericString(int minLength = 1, int maxLength = 25)
        {
            return new Generator<string>(r =>
            {
                var alphaNumericCharacters = string.Concat(Digits, LowercaseChars, UppercaseChars);
                var length = r.Next(minLength, maxLength);
                var chars = new char[length];
                for (var i = 0; i < length; i++)
                {
                    var charIndex = r.Next(alphaNumericCharacters.Length);
                    chars[i] = alphaNumericCharacters[charIndex];
                }

                return new string(chars);
            });
        }

        public static Generator<string> AlphaNumericString(int length)
        {
            return new Generator<string>(r =>
            {
                var alphaNumericCharacters = string.Concat(Digits, LowercaseChars, UppercaseChars);
                var chars = new char[length];
                for (var i = 0; i < length; i++)
                {
                    var charIndex = r.Next(alphaNumericCharacters.Length);
                    chars[i] = alphaNumericCharacters[charIndex];
                }

                return new string(chars);
            });
        }

        public static Generator<string> UpperCaseString(int minLength = 1, int maxLength = 25)
        {
            return new Generator<string>(r =>
            {
                var length = r.Next(minLength, maxLength);
                var chars = new char[length];
                for (var i = 0; i < length; i++)
                {
                    var charIndex = r.Next(UppercaseChars.Length);
                    chars[i] = UppercaseChars[charIndex];
                }

                return new string(chars);
            });
        }

        public static Generator<string> UpperCaseString(int length)
        {
            return new Generator<string>(r =>
            {
                var chars = new char[length];
                for (var i = 0; i < length; i++)
                {
                    var charIndex = r.Next(UppercaseChars.Length);
                    chars[i] = UppercaseChars[charIndex];
                }

                return new string(chars);
            });
        }

        public static Generator<string> LowerCaseString(int minLength = 1, int maxLength = 25)
        {
            return new Generator<string>(r =>
            {
                var length = r.Next(minLength, maxLength);
                var chars = new char[length];
                for (var i = 0; i < length; i++)
                {
                    var charIndex = r.Next(LowercaseChars.Length);
                    chars[i] = LowercaseChars[charIndex];
                }

                return new string(chars);
            });
        }

        public static Generator<string> LowerCaseString(int length)
        {
            return new Generator<string>(r =>
            {
                var chars = new char[length];
                for (var i = 0; i < length; i++)
                {
                    var charIndex = r.Next(LowercaseChars.Length);
                    chars[i] = LowercaseChars[charIndex];
                }

                return new string(chars);
            });
        }

        public static Generator<string> NumericString(int length)
        {
            return new Generator<string>(r =>
            {
                var chars = new char[length];
                for (var i = 0; i < length; i++)
                {
                    var charIndex = r.Next(Digits.Length);
                    chars[i] = Digits[charIndex];
                }

                return new string(chars);
            });
        }

        public static Generator<Guid> Guid()
        {
            return new Generator<Guid>(r => System.Guid.NewGuid());
        }

        public static Generator<DateTime> Date()
        {
            return new Generator<DateTime>(r => DateTime.Now);
        }

        public static Generator<LocalDateTime> LocalDateTime()
        {
            return new Generator<LocalDateTime>(r => NodaTime.LocalDateTime.FromDateTime(DateTime.Now));
        }

        public static Generator<Instant> Instant()
        {
            return new Generator<Instant>(r => NodaTime.Instant.FromDateTimeOffset(DateTimeOffset.Now));
        }

        public static Generator<int> Integer(int minValue = 0, int maxValue = int.MaxValue)
        {
            return new Generator<int>(r => r.Next(minValue, maxValue));
        }

        public static Generator<IEnumerable<T>> Many<T>(Generator<T> generator)
        {
            return new Generator<IEnumerable<T>>(r =>
            {
                var length = r.Next(2, ManyMaxLimit);

                return Exactly(length, generator).Generate(r);

                //return ForLoop(length, () => generator.Generate(r));

                //var elements = new List<T>();
                //for (int i = 0; i < length; i++)
                //    elements.Add(generator.Generate(r));
                //return elements;
            });
        }

        public static Generator<IEnumerable<T>> Exactly<T>(int length, Generator<T> generator)
        {
            return new Generator<IEnumerable<T>>(r =>
            {
                if (UseDeferredEnumerableExecution)
                    return ForLoop(length, () => generator.Generate(r));

                var elements = new List<T>();
                for (var i = 0; i < length; i++)
                    elements.Add(generator.Generate(r));
                return elements;
            });
        }

        public static Generator<IEnumerable<T>> One<T>(Generator<T> generator)
        {
            return Exactly(1, generator);
        }

        public static Generator<IEnumerable<T>> EmptyList<T>()
        {
            return new Generator<IEnumerable<T>>(r => new T[] { });
        }

        private static IEnumerable<T> ForLoop<T>(int count, Func<T> func)
        {
            //this can be used to detect multiple enumerations
            //Mocking.LogAction($"Enumeration of IEnumerable<{typeof(T).Name}>");
            for (var i = 0; i < count; i++)
                yield return func();
        }
    }
}
