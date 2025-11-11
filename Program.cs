var heroes = new List<Hero>
{
    new(new Hp(100))
};

var game = new Game(new Team(heroes));

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

class Hero
{
    internal Hp Hp { get; }

    internal Hero(Hp hp)
    {
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
