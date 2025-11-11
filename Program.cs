using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddTransient<IReadOnlyList<Hero>>(_ => new List<Hero>
{
    new(new("Satoka"), new(100)),
    new(new("Io"), new(100)),
    new(new("Tsubame"), new(100)),
    new(new("Yumi"), new(100)),
    new(new("Mana"), new(100)),
});
services.AddTransient(sp => new Team(sp.GetRequiredService<IReadOnlyList<Hero>>()));
services.AddTransient(sp => new Game(sp.GetRequiredService<Team>()));

using var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();

var game = scope.ServiceProvider.GetRequiredService<Game>();

var hp = game.Team.Heroes.FirstOrDefault()?.Hp.Value;

Console.WriteLine(hp);

class Hp
{
    internal int Value { get; }

    internal Hp(int value)
    {
        Value = value;
    }
}

class Name
{
    internal string Value { get; }

    internal Name(string value)
    {
        Value = value;
    }
}

class Hero
{
    internal Hp Hp { get; }
    internal Name Name { get; }

    internal Hero(Name name, Hp hp)
    {
        Name = name;
        Hp = hp;
    }
}

class Team
{
    internal IReadOnlyList<Hero> Heroes { get; }

    internal Team(IReadOnlyList<Hero> heroes)
    {
        Heroes = heroes;
    }
}

class Game
{
    internal Team Team { get; }

    internal Game(Team team)
    {
        Team = team;
    }
}
