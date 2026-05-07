public abstract class Hero
{
    public abstract string GetDescription();
    public abstract int GetAttack();
    public abstract int GetDefense();
}

public class Warrior : Hero
{
    public override string GetDescription() => "Воїн";
    public override int GetAttack() => 15;
    public override int GetDefense() => 12;
}

public class Mage : Hero
{
    public override string GetDescription() => "Маг";
    public override int GetAttack() => 20;
    public override int GetDefense() => 5;
}

public class Paladin : Hero
{
    public override string GetDescription() => "Паладин";
    public override int GetAttack() => 12;
    public override int GetDefense() => 15;
}

public abstract class InventoryDecorator : Hero
{
    protected Hero _hero;

    public InventoryDecorator(Hero hero)
    {
        _hero = hero;
    }

    public override string GetDescription() => _hero.GetDescription();
    public override int GetAttack() => _hero.GetAttack();
    public override int GetDefense() => _hero.GetDefense();
}

public class Sword : InventoryDecorator
{
    public Sword(Hero hero) : base(hero) { }

    public override string GetDescription() => _hero.GetDescription() + " + Сталевий меч";
    public override int GetAttack() => _hero.GetAttack() + 10;
    public override int GetDefense() => _hero.GetDefense();
}

public class Armor : InventoryDecorator
{
    public Armor(Hero hero) : base(hero) { }

    public override string GetDescription() => _hero.GetDescription() + " + Важка броня";
    public override int GetAttack() => _hero.GetAttack();
    public override int GetDefense() => _hero.GetDefense() + 15;
}

public class Artifact : InventoryDecorator
{
    public Artifact(Hero hero) : base(hero) { }

    public override string GetDescription() => _hero.GetDescription() + " + Амулет сили";
    public override int GetAttack() => _hero.GetAttack() + 5;
    public override int GetDefense() => _hero.GetDefense() + 5;
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Hero hero1 = new Mage();
        PrintHeroInfo("Базовий персонаж", hero1);

        hero1 = new Artifact(hero1);
        PrintHeroInfo("Після додавання артефакту", hero1);

        hero1 = new Sword(hero1);
        PrintHeroInfo("Після додавання меча", hero1);

        Hero armoredWarrior = new Paladin();
        armoredWarrior = new Armor(armoredWarrior);
        armoredWarrior = new Sword(armoredWarrior);
        armoredWarrior = new Sword(armoredWarrior);

        PrintHeroInfo("Воїн-танк (Броня + 2 Мечі)", armoredWarrior);

        Console.ReadKey();
    }

    static void PrintHeroInfo(string title, Hero hero)
    {
        Console.WriteLine($"{title}");
        Console.WriteLine($"Опис: {hero.GetDescription()}");
        Console.WriteLine($"Атака: {hero.GetAttack()} | Захист: {hero.GetDefense()}");
        Console.WriteLine();
    }
}