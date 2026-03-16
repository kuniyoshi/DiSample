using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var teamCsv =
    """
    Altair Torte, Satoka, 100
    Altair Torte, Io, 100
    Altair Torte, Tsubame, 100
    Altair Torte, Yumi, 100
    Altair Torte, Mana, 100
    Procyon Pudding, Sasa, 100
    Procyon Pudding, Haruka, 100
    Procyon Pudding, Amane, 100
    Procyon Pudding, Itsumi, 100
    Procyon Pudding, Mano, 100
    """;

var teams = CreateTeams(teamCsv);
var selectedTeamName = SelectTeamName([.. teams.Select(team => team.Name.Value)]);

services.AddSingleton<IReadOnlyList<Team>>(teams);
services.AddSingleton(selectedTeamName);
services.AddTransient<Team>(static sp =>
{
    var targetTeamName = sp.GetRequiredService<string>();
    var registeredTeams = sp.GetRequiredService<IReadOnlyList<Team>>();

    var team = registeredTeams.FirstOrDefault(t =>
        string.Equals(t.Name.Value, targetTeamName, StringComparison.OrdinalIgnoreCase));

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

static IReadOnlyList<Team> CreateTeams(string csv)
{
    var teams = new List<Team>();
    var teamHeroes = new Dictionary<string, List<Hero>>(StringComparer.OrdinalIgnoreCase);
    var teamNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    using var reader = new StringReader(csv);

    while (reader.ReadLine() is { } line)
    {
        var rawLine = line.Trim();

        if (string.IsNullOrWhiteSpace(rawLine))
        {
            continue;
        }

        var columns = rawLine.Split(',', StringSplitOptions.TrimEntries);

        if (columns.Length != 3)
        {
            throw new FormatException($"CSV の形式が不正です: '{rawLine}'");
        }

        var teamName = columns[0];
        var characterName = columns[1];

        if (!int.TryParse(columns[2], out var hp))
        {
            throw new FormatException($"hp は整数で指定してください: '{rawLine}'");
        }

        if (!teamHeroes.TryGetValue(teamName, out var heroes))
        {
            heroes = [];
            teamHeroes[teamName] = heroes;
            teamNames[teamName] = teamName;
        }

        heroes.Add(new Hero(new(characterName), new(hp)));
    }

    foreach (var entry in teamHeroes)
    {
        teams.Add(new Team(new(teamNames[entry.Key]), entry.Value));
    }

    return teams;
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

        var byName = teamNames.FirstOrDefault(name => string.Equals(name, input, StringComparison.OrdinalIgnoreCase));

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
