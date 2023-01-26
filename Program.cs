using RepairSoftware;
using System.Text.Json;

const string filename = "Repairs.json";
var repairs = await ReadFileAsync();
Choice(repairs);

async Task<List<Repair>?> ReadFileAsync()
{
    try
    {
        string content = "[]";
        if (File.Exists(filename))
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                content = await reader.ReadToEndAsync();
            }
        }
        else
            await File.WriteAllTextAsync(filename, content);

        return JsonSerializer.Deserialize<List<Repair>>(content, new JsonSerializerOptions{ WriteIndented = true });
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while reading from the file: " + ex.Message);
        return new List<Repair>();
    }
}

void Choice(List<Repair> repairs)
{
    Console.Write(@"======================================
                MENU
======================================

1 - Ajouter une réparation
2 - Voir les réparations
3 - Quitter

======================================

Quel est votre choix ? ");

    string result = Console.ReadLine().ToLower();

    switch (result)
    {
        case "1":
            AddRepairAsync(repairs);
            Choice(repairs);
            break;
        case "2":
            SeeRepairChoice(repairs);
            break;
        case "3":
            break;
        default:
            Console.Clear();
            Choice(repairs);
            break;
    }
}

async Task AddRepairAsync(List<Repair> repairs)
{
    Console.Clear();
    Console.WriteLine(@"
======================================
           Ajout d'une carte          
======================================
");
    string typeCard = Validation.AskTheUser("Nom du produit: ", validationString: Validation.CheckValidationName).ToUpper();
    string repairer = Validation.AskTheUser("Trigramme réparateur: ", validationString: Validation.CheckValidationTrigram).ToUpper();
    string stringDate = Validation.AskTheUser("Date (\"jj/mm/aaaa\"): ", validationString: Validation.CheckValidationDate);
    if (string.IsNullOrWhiteSpace(stringDate))
        stringDate = DateTime.Now.ToString("dd/MM/yyyy");
    var repairDate = new DateTime();
    string description = Validation.AskTheUser("Description: ");
    int goodQuantity = int.Parse(Validation.AskTheUser("Qté carte bonnes: ", validationString: Validation.CheckValidationNumber));
    int badQuantity = int.Parse(Validation.AskTheUser("Qté carte en erreur: ", validationString: Validation.CheckValidationNumber));

    try
    {
        repairDate = DateTime.Parse(stringDate);
    }
    catch
    {
        stringDate = DateTime.Now.ToString("dd/MM/yyyy");
        repairDate = DateTime.Parse(stringDate);
    }

    if (string.IsNullOrEmpty(repairer))
        repairer = "AAA";
    if (string.IsNullOrEmpty(description))
        repairs.Add(new Repair(repairer, typeCard, goodQuantity, badQuantity, repairDate));
    else
        repairs.Add(new Repair(repairer, typeCard, goodQuantity, badQuantity, repairDate, description));

    Console.Beep();
    Console.Clear();

    await SaveFileAsync(repairs);
}

async Task SaveFileAsync(List<Repair> repairs)
{
    try
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            string json = JsonSerializer.Serialize(repairs, new JsonSerializerOptions{ WriteIndented = true });

            await writer.WriteAsync(json);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while writing to the file: " + ex.Message);
    }
}

void SeeRepairChoice(List<Repair> repairs)
{
    Console.Clear();

    Console.Write(@"=======================================
      Temporalité des réparations      
=======================================

1 - Hebdomadaires
2 - Mensuelles
3 - Annuelles
4 - Toutes
5 - Retour MENU

=======================================

Quel est votre choix ? ");

    string result = Console.ReadLine().ToLower();

    switch (result)
    {
        case "1":
            SeeRepair("1", repairs);
            break;
        case "2":
            SeeRepair("2", repairs);
            break;
        case "3":
            SeeRepair("3", repairs);
            break;
        case "4":

            SeeRepair("4", repairs);
            break;
        case "5":
            Console.Clear();
            Choice(repairs);
            break;
        default:
            Console.Clear();
            SeeRepairChoice(repairs);
            break;
    }
}

void SeeRepair(string choice, List<Repair> repairs)
{
    Console.Clear();

    if (repairs != null)
    {
        DateTime today = DateTime.Now;
        var newListRepair = new List<Repair>();
        switch (choice)
        {
            case "1":
                newListRepair = repairs.Where(x => x.RepairDate >= today.AddDays(-7)).ToList();
                break;
            case "2":
                newListRepair = repairs.Where(x => x.RepairDate >= today.AddMonths(-1)).ToList();
                break;
            case "3":
                newListRepair = repairs.Where(x => x.RepairDate >= today.AddMonths(-12)).ToList();
                break;
            case "4":
                newListRepair = repairs;
                break;
            default:
                Console.WriteLine("ERREUR: LE PROGRAMME N'EMPRUNTE PAS UN DES 4 CHEMINS POSSIBLE DABS \"TEMPORALITE DES REPARATIONS\"\n");
                break;
        }

        var finalListRepair = from repair in newListRepair
                               orderby (repair.GoodQuantity + repair.BadQuantity)
                               orderby repair.CardType
                               orderby repair.RepairDate
                               group repair by repair.RepairerName into newGroup
                               orderby newGroup.Key
                               select newGroup;

        Console.Write(@$"=======================================
         Liste des réparations      
=======================================

");
         
        foreach (var repairer in finalListRepair)
        {
            float averageGoodQuantity = (float)repairer.Select(x => x.GoodQuantity).Average();
            float averageBadQuantity = (float)repairer.Select(x => x.BadQuantity).Average();

            Other.ColorString($"  OPERATEUR: {repairer.Key}\n", ConsoleColor.Yellow);

            foreach (var repair in repairer)
            {
                Other.ColorString($"Carte: {repair.CardType}", ConsoleColor.DarkYellow);

                if (repair.Description != null)
                    Other.ColorString($"Description: {repair.Description}", ConsoleColor.DarkGray);

                GetGoodAndBadRepairPercent(repair.GoodQuantity, repair.BadQuantity);

                Other.ColorString($"Date: {repair.RepairDate.ToString("dd/MM/yyyy")}", ConsoleColor.Cyan);
                Other.ColorString(new String('-', 39), ConsoleColor.DarkGray);
            }
            Console.WriteLine();
            Other.ColorString($"Score {repairer.Key}: {(averageGoodQuantity * 100) / (averageGoodQuantity + averageBadQuantity):0.00}%", ConsoleColor.Yellow);
            Other.ColorString($"Quantitées réparées: {repairer.Select(x => x.GoodQuantity).Sum()}\n", ConsoleColor.Yellow);
            Console.WriteLine(new String('=', 39));
            Console.WriteLine();
        }
    }
    else
        Console.WriteLine("...La liste est vide...\n");

    Other.ColorString("...Presser une touche pour continuer...");
    Console.ReadLine();
    Console.Clear();
    Choice(repairs);
}

static void GetGoodAndBadRepairPercent(float goodQuantity, float badQuantity)
{
    Console.WriteLine($"Total: {goodQuantity + badQuantity} unitée(s)");
    Console.Write("Ok: ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write($"{goodQuantity} unitée(s) ");
    Console.ResetColor();
    Console.Write("(soit ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write($"{goodQuantity * 100 / (goodQuantity + badQuantity):0.##}%");
    Console.ResetColor();
    Console.WriteLine(")");
    Console.Write("Erreur: ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write($"{badQuantity} unitée(s)");
    Console.ResetColor();
    Console.Write(" (soit ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write($"{badQuantity * 100 / (goodQuantity + badQuantity):0.##}%");
    Console.ResetColor();
    Console.WriteLine(")");
}