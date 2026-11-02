using SGStudios.Random.Generator;

namespace Framework.Tickets;

public static class GeneratorExtensions
{
    public static T Select<T>(this IDeterministicGenerator generator, T[] choices)
    {
        return choices[generator.GetRandomNumber((ulong)choices.Length)];
    }
}
