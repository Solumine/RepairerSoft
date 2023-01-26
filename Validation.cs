namespace RepairSoftware;

internal class Validation
{
    public delegate string ValidationString(string str);

    public static string? AskTheUser(string message, ValidationString? validationString = null)
    {
        Console.Write($"{message} ");
        string? answer = Console.ReadLine();

        if (validationString != null)
        {
            string error = validationString(answer);
            if (error != null)
            {
                Console.WriteLine($"Erreur: {error}");
                return AskTheUser(message, validationString);
            }
        }
        return answer;
    }

    public static string? CheckValidationTrigram(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return null;

        if (str.Length != 3 || str.Any(char.IsDigit))
            return "Le trigramme doit être composé de 3 lettres: la 1ère lettre du prénom et les 2 premières lettre du nom)";

        return null;
    }

    public static string? CheckValidationName(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return "Le nom ne peut être vide";

        if (str.All(char.IsDigit))
            return "Le nom ne peut pas contenir que des chiffres";

        return null;
    }

    public static string? CheckValidationNumber(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return "Le nombre ne peut pas être vide";

        if (!str.All(char.IsDigit))
            return "La réponse doit être un chiffre ou un nombre";

        return null;
    }

    public static string? CheckValidationDate(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return null;
        if (str.Length != 10)
            return "Le format se doit dêtre comme suit: \"JJ/MM/AAAA\"";

        return null;
    }
}
