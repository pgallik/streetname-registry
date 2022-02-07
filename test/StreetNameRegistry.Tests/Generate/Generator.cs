namespace StreetNameRegistry.Tests.Generate
{
    using System;

    [Obsolete("Do not use this class, try with autofixture and/or look at projection V2 tests")]
    public interface IGenerator<out T>
    {
        IGenerator<T1> Select<T1>(Func<T, T1> f);
        IGenerator<T1> Then<T1>(Func<T, IGenerator<T1>> f);
        T Generate(Random random);
    }

    [Obsolete("Do not use this class, try with autofixture and/or look at projection V2 tests")]
    public class Generator<T>:IGenerator<T>
    {
        private readonly Func<Random, T> _generate;

        public Generator(Func<Random, T> generate)
        {
            _generate = generate ?? throw new ArgumentNullException(nameof(generate));
        }

        public IGenerator<T1> Select<T1>(Func<T, T1> f)
        {
            if (f == null)
                throw new ArgumentNullException(nameof(f));

            return new Generator<T1>(r => f(_generate(r)));
        }

        public IGenerator<T1> Then<T1>(Func<T, IGenerator<T1>> f)
        {
            if (f == null)
                throw new ArgumentNullException(nameof(f));

            return new Generator<T1>(r => f(_generate(r)).Generate(r));
        }

        public T Generate(Random random)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            return _generate(random);
        }
    }
}
