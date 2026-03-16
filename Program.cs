using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var teams = CreateTeams();
var selectedTeamName = SelectTeamName([.. teams.Select(team => team.Name.Value)]);

services.AddSingleton<IReadOnlyList<Team>>(teams);
services.AddSingleton(selectedTeamName);
services.AddTransient<Team>(static sp =>
{
    var targetTeamName = sp.GetRequiredService<string>();
    var registeredTeams = sp.GetRequiredService<IReadOnlyList<Team>>();

    var team = registeredTeams.FirstOrDefault(
        t => string.Equals(t.Name.Value, targetTeamName, StringComparison.OrdinalIgnoreCase));

    if (team is null)
    {
        throw new InvalidOperationException($"Team '{targetTeamName}' は見つかりませんでした。");
    }

    return team;
});
services.AddTransient(static sp => new Game(sp.GetRequiredService<Team>()));

using var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();

var game = scope.ServiceProvider.GetRequiredService<Game>();

var hp = game.Team.Heroes.FirstOrDefault()?.Hp.Value;

Console.WriteLine(hp);

static IReadOnlyList<Team> CreateTeams()
{
    return
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
];
}

static string SelectTeamName(IReadOnlyList<string> teamNames)
{
    while (true)
    {
        Console.WriteLine("チームを選択してください:");

        for (var i = 0; i < teamNames.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {teamNames[i]}");
        }

        Console.Write("> ");
        var input = Console.ReadLine();

        if (int.TryParse(input, out var index) && index is >= 1 and <= int.MaxValue && index <= teamNames.Count)
        {
            return teamNames[index - 1];
        }

        var byName = teamNames.FirstOrDefault(
            name => string.Equals(name, input, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(byName))
        {
            return byName;
        }

        Console.WriteLine("無効な入力です。もう一度入力してください。");
    }
}

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
    internal Team Team { get; }

    internal Game(Team team)
    {
        Team = team;
    }
}
