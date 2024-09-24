using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Dataflow;
//floccinaucinihilipilification 
Console.WriteLine("skriv ditt namn");
string? namn = Console.ReadLine();
while (namn.Length > 9)
{
    Console.WriteLine("ditt namn är för långt");
    namn = Console.ReadLine();
}

Move waterGun = new Move("Water Gun", "special", 40, 100, "water", 0);
Move rockSmash = new Move("Rock Smash", "phyiscal", 50, 90, "fighting", 0);
Move scratch = new Move("Scratch", "physical", 40, 100, "normal", 0);

Pokemon mudkip = new Pokemon("Mudkip", 50, "water", 70, 50, 50, 50, 50, 40, new List<Move> { waterGun, rockSmash });
mudkip.writeInfo();
Pokemon charmander = new Pokemon("Charmander", 50, "fire", 39, 52, 43, 60, 50, 65, new List<Move> { scratch });
charmander.writeInfo();

battleLoop(mudkip, charmander);


void battleLoop(Pokemon playerPokemon, Pokemon AiPokemon)
{
    List<Pokemon> battleList = new List<Pokemon> { playerPokemon, AiPokemon };
    int turn = 0;
    while (!playerPokemon.fainted && !AiPokemon.fainted)
    {
        turn++;
        Console.WriteLine($"turn {turn}");


        // choose moves script here 
        Move playerMoveToUse = playerPokemon.selectMove();
        Move AiMoveToUse = scratch;





        //speed och priority check
        if (playerMoveToUse.priority == AiMoveToUse.priority)
        {
            if (playerPokemon.speed > AiPokemon.speed)
            {
                playerMoveToUse.useMove(playerPokemon, AiPokemon);
                if (!AiPokemon.fainted)
                {
                    AiMoveToUse.useMove(AiPokemon, playerPokemon);
                }
                else
                {
                    battleList.Remove(AiPokemon);
                    return;
                }

            }
            //speedtie code
            else if (playerPokemon.speed == AiPokemon.speed)
            {
                Random rng = new Random();
                int speedTie = rng.Next(0, 2);
                if (speedTie == 0)
                {
                    playerMoveToUse.useMove(playerPokemon, AiPokemon);
                    if (!AiPokemon.fainted)
                    {
                        AiMoveToUse.useMove(AiPokemon, playerPokemon);
                    }
                    else
                    {
                        battleList.Remove(AiPokemon);
                        return;
                    }
                }
                else
                {
                    AiMoveToUse.useMove(AiPokemon, playerPokemon);
                    if (!playerPokemon.fainted)
                    {
                        playerMoveToUse.useMove(playerPokemon, AiPokemon);
                    }
                    else
                    {
                        battleList.Remove(playerPokemon);
                        return;
                    }
                }
            }
            //if the players speed is not greater or equal to the enemy speed than the ai must be faster
            else
            {
                AiMoveToUse.useMove(AiPokemon, playerPokemon);
                if (!playerPokemon.fainted)
                {
                    playerMoveToUse.useMove(playerPokemon, AiPokemon);
                }
                else
                {
                    battleList.Remove(playerPokemon);
                    return;
                }
            }
        }
    }
    Console.WriteLine($"{battleList} wins!");
    Console.ReadLine();
}








//--- Pokmemon class -----------------------------------------------------------------------------------------------------
class Pokemon
{
    public string name;
    public int level; //1 till 100
    public string type;
    public string? type2; //vet inte hur jag ska göra det här än
    public float hp, attack, defense, specialAttack, specialDefense, speed;
    public float maxHp;
    Random rng = new Random();
    int hpIV, attackIV, defenseIV, specialAttackIV, specialDefenseIV, speedIV;
    //EVs om jag orkar lägga till det, sätt till 0 eller 252 annars
    int effortValues = 0;

    public bool fainted = false;
    public List<Move> moves = new List<Move>();
    public List<Move> learnableMoves = new List<Move>();
    public string statusCondition;

    //räknar ut hp
    private int calculateHp(int baseHp, int hpIV, int level)
    {
        return (((2 * baseHp) + hpIV + (effortValues / 4)) * level / 100) + level + 10; ;
    }
    //räknar ut alla andra stats, flurp
    private int calculateStat(int baseStat, int statIV, int level)
    {
        return (((2 * baseStat) + statIV + (effortValues / 4)) * level / 100) + 5;
    }
    public void writeInfo()
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

    public Move selectMove()
    {
        Console.WriteLine($"{name}, choose a move:");
        // for (int i = 0; i < moves.Count; i++)
        foreach (Move move in moves)
        {
            Console.WriteLine();
        }

        int moveIndex = -1;
        while (moveIndex < 0 || moveIndex >= moves.Count)
        {
            Console.Write("Enter the number of the move: ");
            if (int.TryParse(Console.ReadLine(), out moveIndex))
            {
                moveIndex--; // Convert to 0-based index
                if (moveIndex < 0 || moveIndex >= moves.Count)
                {
                    Console.WriteLine("Invalid choice, try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input, enter a number.");
            }
        }

        return moves[moveIndex];
    }

    public void takeDamage(float damage)
    {
        this.hp -= damage;

        Console.WriteLine($"{name} took {damage} damage!");
        //show remainning damage, bara för devs
        Console.WriteLine($"remaining hp: {this.hp}");
        Console.ReadLine();

        if (this.hp <= 0)
        {
            this.fainted = true;
            Console.WriteLine($"{name} fainted!");
            Console.ReadLine();

        }
    }



    // hur faan ska man göra det här
    public void handleStatusConditions()
    {

        if (statusCondition == "paralysed")
        {
            this.speed = MathF.Round(this.speed / 2);
        }
        if (statusCondition == "poisoined")
        {
            hp -= (float)(maxHp * 0.25);
        }
    }
    public void statChanges()
    {

    }



    //construcotr för pokemon med bara en typ, gör en till senare för pokemon med 2 typer senare
    public Pokemon(string name, int level, string type, int baseHp, int baseAttack, int baseDefense, int baseSpecialAttack, int baseSpecialDefense, int baseSpeed, List<Move> moveSet = null, string? type2 = null)
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

        maxHp = MathF.Round(this.hp);

        this.moves.AddRange(moveSet);

    }
}








//--- moves class -------------------------------------------------------------------------------------------------------------------------------------<
class Move
{
    public string name;
    public string category; //physical, special eller kanske status
    public int basePower;
    public int accuracy; // 1 - 100, sätt till 101 för garanterat träff
    public string type; // får se om jag hinner med detta
    public int priority; // 5 till -6, trick room är minus 7 om jag lägger till det
    public int? additionalEffectChance; //1 - 100, kan vara null om attacken inte har någon effekt
    public string? additionalEffect;

    //flurpigt värre
    public void writeInfo()
    {

        Console.WriteLine($"{this.name}");
        Console.WriteLine($"Category: {this.category}");
        Console.WriteLine($"Base Power: {this.basePower}");
        Console.WriteLine($"Accuracy: {this.accuracy}");
        Console.WriteLine($"Type: {this.type}");
        Console.WriteLine($"Priority: {this.priority}");
        if (this.additionalEffectChance != null)
        {
            Console.WriteLine($"Additional effect chance: {this.additionalEffectChance}");
        }
        if (this.additionalEffect != null)
        {
            Console.WriteLine($"Additional effect chance: {this.additionalEffect}");
        }

        Console.ReadLine();
    }

    public string getInfo(string name, string category, int basePower, int accuracy, string type, int? priority = null, int? additionalEffectChance = null, string? additionalEffect = null)
    {

        Console.WriteLine($"{this.name}");
        Console.WriteLine($"Category: {this.category}");
        Console.WriteLine($"Base Power: {this.basePower}");
        Console.WriteLine($"Accuracy: {this.accuracy}");
        Console.WriteLine($"Type: {this.type}");
        Console.WriteLine($"Priority: {this.priority}");
        if (this.additionalEffectChance != null)
        {
            Console.WriteLine($"Additional effect chance: {this.additionalEffectChance}");
        }
        if (this.additionalEffect != null)
        {
            additionalEffect = this.additionalEffect;
        }
        else
        {
            additionalEffect = "none";
        }

        Console.ReadLine();

        return $"{name}-{category}-{type}(Power:{basePower} Accuracy:{accuracy} Additional effect: {additionalEffect})";
    }

    private float calculateDamage(Pokemon attacker, Pokemon defender)
    {
        Random rng = new Random();
        float damageRole = 0.85f + (float)rng.NextDouble() * 0.15f;

        //kod för om jag bestämmer mig för att bara physical och special moves
        float attackerStat = (category == "physical") ? attacker.attack : attacker.specialAttack;
        float defenderStat = (category == "physical") ? defender.defense : defender.specialDefense;

        //Kod för om jag bestämmer mig för att status moves
        /*float attackerStat;
        float defenderStat;
        if (category == "physical")
        {
            attackerStat = attacker.attack;
            defenderStat = defender.defense;
        }
        else if (category == "special")
        {
            attackerStat = attacker.specialAttack;
            defenderStat = defender.specialDefense;
        else {
            attackerStat = 0;
            defenderStat = defender.specialDefense;
        }
        }*/

        //allt ska rundas ner för att vara pokemon accurate
        //          ^^^ metroid reference?!?!?!?!?!!!!!11111
        return MathF.Round(((2f * attacker.level + 2) / 5f * basePower * (attackerStat / defenderStat) / 50f + 2f) * damageRole);
    }

    public void useMove(Pokemon user, Pokemon target)
    {
        Console.WriteLine($"{user.name} used {name}!");
        Random rng = new Random();
        int hitChance = rng.Next(1, 101);
        if (hitChance > accuracy)
        {
            Console.WriteLine($"{user.name}'s {name} missed!");
            Console.ReadLine();
            return;
        }
        float damage = calculateDamage(user, target);
        target.takeDamage(damage);
    }


    //för moves som bara gör damage
    public Move(string name, string category, int basePower, int accuracy, string type, int priority, int? additionalEffectChance = null, string? additionalEffect = null)
    {
        this.name = name;
        this.category = category;
        this.basePower = basePower;
        this.accuracy = accuracy;
        this.type = type;
        this.priority = priority;
        if (additionalEffectChance != null)
        {
            this.additionalEffectChance = additionalEffectChance;
        }
        if (additionalEffect != null)
        {
            this.additionalEffect = additionalEffect;
        }
    }
}

/*static bool hasGun;

if(hasGun){    
return false;
}
*/