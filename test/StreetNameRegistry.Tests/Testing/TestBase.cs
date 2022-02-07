namespace StreetNameRegistry.Tests.Testing
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Generate;
    using Xunit.Abstractions;

    [Obsolete("Do not use this class, try with autofixture and/or look at projection V2 tests")]
    public abstract class TestBase
    {
        private readonly TestScope _scope;

        protected readonly Random Random = new Random();

        protected TestBase(ITestOutputHelper output) => _scope = new TestScope(output);

        protected void Log(string message) => _scope.Log(message);

        protected void LogAct(string message) => Log($"{"ACT".PadRight(10)}{message}");
        protected void LogArrange(string message) => Log($"{"ARRANGE".PadRight(10)}{message}");

        protected T Arrange<T>(IGenerator<T> generator) => generator.Generate(Random);
    }

    internal class TestScope
    {
        private static readonly ConcurrentDictionary<string, AsyncLocal<ITestOutputHelper>> State = new ConcurrentDictionary<string, AsyncLocal<ITestOutputHelper>>();

        public static Action<string> LogAction =>
            message => Log(OutputHelper, message);

        public TestScope(ITestOutputHelper output) =>
            State.GetOrAdd("", _ => new AsyncLocal<ITestOutputHelper>()).Value = output;

        public void Log(string message) =>
            Log(OutputHelper, message);

        private static void Log(ITestOutputHelper output, string message) =>
            output?.WriteLine(message);

        private static ITestOutputHelper OutputHelper =>
            State.TryGetValue("", out var data) ? data.Value : null;
    }
}
