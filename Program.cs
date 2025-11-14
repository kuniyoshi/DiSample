using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddTransient<IReadOnlyList<Team>>(static _ =>
[
    new(
        new("Altair Torte"),
        [
            new(new("Satoka"), new(100)),
            new(new("Io"), new(100)),
            new(new("Tsubame"), new(100)),
            new(new("Yumi"), new(100)),
            new(new("Mana"), new(100)),
        ]),
    new(
        new("Procyon Pudding"),
        [
            new(new("Sasa"), new(100)),
            new(new("Haruka"), new(100)),
            new(new("Amane"), new(100)),
            new(new("Itsumi"), new(100)),
            new(new("Mano"), new(100)),
        ]),
]);
services.AddTransient(static sp => new Game(sp.GetRequiredService<IReadOnlyList<Team>>()));

using var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();

var game = scope.ServiceProvider.GetRequiredService<Game>();

var hp = game.Teams.FirstOrDefault()?.Heroes.FirstOrDefault()?.Hp.Value;

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
    internal Name Name { get; }
    internal IReadOnlyList<Hero> Heroes { get; }

    internal Team(Name name, IReadOnlyList<Hero> heroes)
    {
        Name = name;
        Heroes = heroes;
    }
}

class Game
{
    internal IReadOnlyList<Team> Teams { get; }

    internal Game(IReadOnlyList<Team> teams)
    {
        Teams = teams;
    }
}
