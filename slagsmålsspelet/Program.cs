using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Dataflow;
using System.Data;
using System.Runtime.CompilerServices;


//floccinaucinihilipilification
bool playGame = true;
Console.WriteLine("write your name");
string? playerName = Console.ReadLine();


Console.WriteLine($"{playerName}, do you want to become a pokemon master? [Y/N]");
string? answer = Console.ReadLine().ToLower();
while (answer != "y" && answer != "n")
{
    Console.WriteLine("invalid input, try again");
    answer = Console.ReadLine();
}
if (answer == "y")
{
    Console.WriteLine("Alrgiht");
}
else
{
    System.Environment.Exit(0);
}




Move waterGun = new Move("Water Gun", "special", 40, 100, "water", 0);
Move rockSmash = new Move("Rock Smash", "phyiscal", 50, 90, "fighting", 0);
Move scratch = new Move("Scratch", "physical", 40, 100, "normal", 0);
Move tackle = new Move("Tackle", "physical", 40, 100, "normal", 0);
Move pound = new Move("Pound", "physical", 40, 100, "normal", 0);
Move ember = new Move("Ember", "special", 40, 100, "fire", 0);
Move quickAttack = new Move("Quick attack", "physical", 40, 100, "normal", 1);
Move extremeSpeed = new Move("Extreme speed", "physical", 80, 100, "normal", 2);
Move aquaJet = new Move("Aqua jet", "physical", 40, 100, "water", 1);
Move vineWhip = new Move("Vine whip", "physical", 40, 100, "grass", 0);
Move razorLeaf = new Move("Razor leaf", "physical", 55, 95, "grass", 0);
Move acidSpray = new Move("Acid spray", "special", 20, 100, "poison", 0, 100, 4, "target", 0.5f);
Move rapidSpin = new Move("Rapid spin", "physical", 50, 100, "normal", 0, 100, 5, "self", 0.5f);
Move hydroPump = new Move("Hydro pump", "special", 110, 80, "water", 0);
Move flameCharge = new Move("Flame charge", "physical", 50, 100, "fire", 0, 100, 5, "self", 0.5f);
Move aerialAce = new Move("Aerial ace", "physical", 60, 101, "flying", 0);
Move mudShot = new Move("Mud shot", "special", 55, 95, "ground", 0, 50, 5, "target", 0.5f);
Move energyBall = new Move("Energy ball", "special", 90, 100, "grass", 0, 10, 4, "target", 0.5f);
Move dragonAscent = new Move("Dragon ascent", "physical", 120, 100, "flying", 0);


Pokemon Treecko = new Pokemon("Treecko", 5, "grass", 40, 45, 35, 65, 55, 70, new List<Move> { pound, quickAttack, energyBall, vineWhip });
Pokemon torchic = new Pokemon("Torchic", 5, "fire", 45, 60, 40, 70, 50, 45, new List<Move> { ember, flameCharge, aerialAce, scratch });
Pokemon mudkip = new Pokemon("Mudkip", 5, "water", 70, 50, 50, 50, 50, 40, new List<Move> { waterGun, rockSmash, mudShot, aquaJet });
Pokemon bulbasaur = new Pokemon("Bulbasaur", 5, "grass", 45, 49, 49, 65, 65, 45, new List<Move> { acidSpray, tackle, razorLeaf, vineWhip });
Pokemon charmander = new Pokemon("Charmander", 5, "fire", 39, 52, 43, 60, 50, 65, new List<Move> { scratch, quickAttack, ember, tackle });
Pokemon squirtle = new Pokemon("Squirtle", 5, "water", 44, 48, 65, 50, 64, 43, new List<Move> { tackle, waterGun, rapidSpin, hydroPump });
Pokemon rayquaza = new Pokemon("Rayquaza", 70, "dragon", 105, 150, 90, 150, 90, 95, new List<Move> { extremeSpeed, dragonAscent }, "flying");


List<Pokemon> pokemons = new List<Pokemon> { bulbasaur, charmander, mudkip, squirtle, torchic, Treecko, rayquaza };


while (playGame)
{
    playGame = false;
    // this allows the user to choose a pokemon
    Console.WriteLine($"{playerName}, choose a pokemon:");


    for (int i = 0; i < pokemons.Count; i++)
    {
        Console.WriteLine($"pokemon {i + 1}:");
        pokemons[i].WriteInfo();
    }


    int pokemomIndex = -1;
    while (pokemomIndex < 0 || pokemomIndex >= pokemons.Count)
    {
        Console.Write("Enter the number of the pokemom: ");
        if (int.TryParse(Console.ReadLine(), out pokemomIndex))
        {
            pokemomIndex--;
            Console.WriteLine("");
            Console.ReadLine();
            if (pokemomIndex < 0 || pokemomIndex >= pokemons.Count)
            {
                Console.WriteLine("Invalid choice");
                Console.ReadLine();
            }
        }
        else
        {
            Console.WriteLine("Invalid input");
            Console.ReadLine();
        }
    }
    Pokemon playerPokemon = pokemons[pokemomIndex];
    pokemons.Remove(playerPokemon);


    // the ai chooses one of the remaining pokemon in the list
    Random rng = new Random();
    Pokemon aiPokemon = pokemons[rng.Next(0, pokemons.Count)];
    pokemons.Remove(aiPokemon);




    BattleLoop(playerPokemon, aiPokemon);


    Console.WriteLine("Write a to play again");
    string a = Console.ReadLine();
    if (a == "a") playGame = true;
    Console.ReadLine();
}


void AttackEachOther(Pokemon firstPokemon, Pokemon lastPokemon, Move firstMoveToUse, Move lastMoveToUse, List<Pokemon> battleList)
{


    firstMoveToUse.UseMove(firstPokemon, lastPokemon);
    //if the pokemon fainted of the attack we shall end the loop and remove it from the list
    if (!lastPokemon.fainted)
    {
        lastMoveToUse.UseMove(lastPokemon, firstPokemon);
    }
    else
    {
        battleList.Remove(lastPokemon);
        return;
    }
}




void BattleLoop(Pokemon playerPokemon, Pokemon aiPokemon)
{


    List<Pokemon> battleList = new List<Pokemon> { playerPokemon, aiPokemon };
    int turn = 0;


    while (!playerPokemon.fainted && !aiPokemon.fainted)
    {
        turn++;
        Console.WriteLine($"turn {turn}");


        Random rnd = new Random();
        Move playerMoveToUse = playerPokemon.SelectMove();
        Move AiMoveToUse = aiPokemon.moves[rnd.Next(0, aiPokemon.moves.Count)];


        //speed and priority check
        if (playerMoveToUse.priority == AiMoveToUse.priority)
        {
            if (playerPokemon.speed > aiPokemon.speed)
            {
                AttackEachOther(playerPokemon, aiPokemon, playerMoveToUse, AiMoveToUse, battleList);


            }
            //speedtie code
            else if (playerPokemon.speed == aiPokemon.speed)
            {
                Random rng = new Random();
                int speedTie = rng.Next(0, 2);
                if (speedTie == 0)
                {
                    AttackEachOther(playerPokemon, aiPokemon, playerMoveToUse, AiMoveToUse, battleList);
                }
                else
                {
                    AttackEachOther(aiPokemon, playerPokemon, AiMoveToUse, playerMoveToUse, battleList);
                }
            }
            //if the players speed is not greater or equal to the enemy speed than the ai must be faster
            else
            {
                AttackEachOther(aiPokemon, playerPokemon, AiMoveToUse, playerMoveToUse, battleList);
            }
        }
        else if (playerMoveToUse.priority > AiMoveToUse.priority)
        {
            AttackEachOther(playerPokemon, aiPokemon, playerMoveToUse, AiMoveToUse, battleList);
        }
        //if the ai priority is not equal to or smaller than player priority than it must be greater
        else
        {
            AttackEachOther(aiPokemon, playerPokemon, AiMoveToUse, playerMoveToUse, battleList);
        }
    }
    Console.WriteLine($"{battleList[0].name} wins!");
    Console.ReadLine();
    if (aiPokemon.fainted)
    {
        Console.WriteLine($"{playerName}, You are the pokemon master!!!!!!!111");
    }
    else
    {


        Console.WriteLine("You failed at becoming the pokemon master! :(");
    }
    Console.ReadLine();
    Console.ReadLine();
}




//--- Pokmemon class -----------------------------------------------------------------------------------------------------
class Pokemon
{
    public string name;
    public int level;
    public string type;
    public string? type2;
    public float hp, attack, defense, specialAttack, specialDefense, speed;
    public float maxHp;
    Random rng = new Random();
    int hpIV, attackIV, defenseIV, specialAttackIV, specialDefenseIV, speedIV;
    int effortValues = 252;
    public List<Move> moves = new List<Move>();


    public bool fainted = false;


    private int calculateHp(int baseHp, int hpIV, int level)
    {
        return (((2 * baseHp) + hpIV + (effortValues / 4)) * level / 100) + level + 10; ;
    }


    private int calculateStat(int baseStat, int statIV, int level)
    {
        return (((2 * baseStat) + statIV + (effortValues / 4)) * level / 100) + 5;
    }
    public void WriteInfo()
    {
        Console.WriteLine($"{this.name}");
        Console.WriteLine($"level: {this.level}");
        Console.WriteLine($"type: {this.type + this.type2}");
        Console.WriteLine($"hp: {this.hp}");
        Console.WriteLine($"attack: {this.attack}");
        Console.WriteLine($"defense: {this.defense}");
        Console.WriteLine($"special attack: {this.specialAttack}");
        Console.WriteLine($"special defense: {this.specialDefense}");
        Console.WriteLine($"speed: {this.speed}");
        Console.WriteLine($"moves:");
        foreach (Move move in moves)
        {
            Console.WriteLine($"- {move.name}");
        }
        Console.ReadLine();
    }


    public Move SelectMove()
    {
        Console.WriteLine($"{name}, choose a move:");


        for (int i = 0; i < moves.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {moves[i].GetInfo()}");
        }


        int moveIndex = -1;
        while (moveIndex < 0 || moveIndex >= moves.Count)
        {
            Console.Write("Enter the number of the move: ");
            if (int.TryParse(Console.ReadLine(), out moveIndex))
            {
                moveIndex--;
                Console.WriteLine("");
                Console.ReadLine();
                if (moveIndex < 0 || moveIndex >= moves.Count)
                {
                    Console.WriteLine("Invalid choice");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Invalid input");
                Console.ReadLine();
            }
        }
        return moves[moveIndex];
    }


    public void TakeDamage(float damage)
    {
        this.hp -= damage;


        Console.WriteLine($"{name} took {damage} damage!");
        Console.WriteLine($"remaining hp: {this.hp}");


        if (this.hp <= 0)
        {
            this.fainted = true;
            Console.WriteLine($"{name} fainted!");
            Console.ReadLine();
        }
    }


    public Pokemon(string name, int level, string type, int baseHp, int baseAttack, int baseDefense, int baseSpecialAttack, int baseSpecialDefense, int baseSpeed, List<Move> moveSet, string? type2 = null)
    {
        this.name = name;
        this.level = level;
        this.type = type;
        if (type2 != null)
        {
            this.type2 = type2;
        }


        this.hpIV = rng.Next(1, 32);
        this.attackIV = rng.Next(1, 32); ;
        this.defenseIV = rng.Next(1, 32);
        this.specialAttackIV = rng.Next(1, 32);
        this.specialDefenseIV = rng.Next(1, 32);
        this.speedIV = rng.Next(1, 32);


        this.hp = calculateHp(baseHp, hpIV, level);
        this.attack = calculateStat(baseAttack, attackIV, level);
        this.defense = calculateStat(baseDefense, defenseIV, level);
        this.specialAttack = calculateStat(baseSpecialAttack, specialAttackIV, level);
        this.specialDefense = calculateStat(baseSpecialDefense, specialDefenseIV, level);
        this.speed = calculateStat(baseSpeed, speedIV, level);
        this.moves.AddRange(moveSet);


    }
}










//--- moves class -------------------------------------------------------------------------------------------------------------------------------------<
class Move
{
    public string name;
    public string category;
    public int basePower;
    public int accuracy;
    public string type;
    public int priority;
    public int additionalEffectChance;
    public int additionalEffect;
    public string additionalEffectTarget;
    public float additionalEffectAmount;


    public string GetInfo()
    {
        return $"{name} ({category}, Type: {type}, Power: {basePower}, Accuracy: {accuracy}%)";
    }


    private float calculateDamage(Pokemon attacker, Pokemon defender)
    {
        Random rng = new Random();
        float damageRole = 0.85f + (float)rng.NextDouble() * 0.15f;


        float attackerStat = (category == "physical") ? attacker.attack : attacker.specialAttack;
        float defenderStat = (category == "physical") ? defender.defense : defender.specialDefense;


        if (defenderStat <= 0)
        {
            defenderStat = 1;
        }


        return MathF.Round(((2f * attacker.level + 2f) / 5f * basePower * (attackerStat / defenderStat) / 50f + 2f) * damageRole);
    }


    public void UseMove(Pokemon user, Pokemon target)
    {
        Console.WriteLine($"{user.name} used {name}!");
        Random rng = new Random();
        int hitChance = rng.Next(1, 101);
        if (hitChance > accuracy)
        {
            Console.WriteLine($"{user.name}'s {name} missed!");
            return;
        }
        float damage = calculateDamage(user, target);
        target.TakeDamage(damage);


        Random rng2 = new Random();
        int effectChance = rng2.Next(1, 101);
        if (effectChance < additionalEffectChance)
        {
            if (additionalEffectTarget == "self")
            {
                switch (additionalEffect)
                {
                    case 1:
                        user.attack = MathF.Round(user.attack * (1 + additionalEffectAmount));
                        Console.WriteLine($"{user.name}'s attack rose!");
                        break;
                    case 2:


                        user.defense = MathF.Round(user.defense * (1 + additionalEffectAmount));
                        Console.WriteLine($"{user.name}'s defense rose!");
                        break;
                    case 3:
                        user.specialAttack = MathF.Round(user.specialAttack * (1 + additionalEffectAmount));
                        Console.WriteLine($"{user.name}'s special attack rose!");
                        break;
                    case 4:
                        user.specialDefense = MathF.Round(user.specialDefense * (1 + additionalEffectAmount));
                        Console.WriteLine($"{user.name}'s special defense rose!");
                        break;
                    case 5:
                        user.speed = MathF.Round(user.speed * (1 + additionalEffectAmount));
                        Console.WriteLine($"{user.name}'s speed rose!");
                        break;
                }
            }
            else if (additionalEffectTarget == "target")
            {
                switch (additionalEffect)
                {
                    case 1:
                        target.attack = MathF.Round(user.attack * (1 - additionalEffectAmount));
                        Console.WriteLine($"{target.name}'s attack fell!");
                        break;
                    case 2:
                        target.defense = MathF.Round(user.defense * (1 - additionalEffectAmount));
                        Console.WriteLine($"{target.name}'s defense fell!");
                        break;
                    case 3:
                        target.specialAttack = MathF.Round(user.specialAttack * (1 - additionalEffectAmount));
                        Console.WriteLine($"{target.name}'s special attack fell!");
                        break;
                    case 4:
                        target.specialDefense = MathF.Round(user.specialDefense * (1 - additionalEffectAmount));
                        Console.WriteLine($"{target.name}'s special defense fell!");
                        break;
                    case 5:
                        target.speed = MathF.Round(user.speed * (1 - additionalEffectAmount));
                        Console.WriteLine($"{target.name}'s speed fell!");
                        break;
                }
            }
        }
        Console.ReadLine();
    }
    //constructor for moves without additional effects
    public Move(string name, string category, int basePower, int accuracy, string type, int priority)
    {
        this.name = name;
        this.category = category;
        this.basePower = basePower;
        this.accuracy = accuracy;
        this.type = type;
        this.priority = priority;
    }
    //constructor for moves with additional effects
    public Move(string name, string category, int basePower, int accuracy, string type, int priority, int additionalEffectChance, int additionalEffect, string additionalEffectTarget, float additionalEffectAmount)
    {
        this.name = name;
        this.category = category;
        this.basePower = basePower;
        this.accuracy = accuracy;
        this.type = type;
        this.priority = priority;
        this.additionalEffectChance = additionalEffectChance;
        this.additionalEffect = additionalEffect;
        this.additionalEffectTarget = additionalEffectTarget;
        this.additionalEffectAmount = additionalEffectAmount;
    }
}