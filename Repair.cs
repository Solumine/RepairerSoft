namespace RepairSoftware;

internal class Repair
{
    public string RepairerName { get; set; }
    public string CardType { get; set; }
    public int GoodQuantity { get; set; }
    public int BadQuantity { get; set; }
    public DateTime RepairDate { get; set; }
    public string? Description { get; set; }
    public float SuccessRate
    {
        get
        {
            return (float)GoodQuantity * 100 / (GoodQuantity + BadQuantity);
        }
    }

    public Repair(string repairerName, string cardType, int goodQuantity, int badQuantity, DateTime repairDate, string? description = null)
    {
        RepairerName = repairerName;
        CardType = cardType;
        GoodQuantity = goodQuantity;
        BadQuantity = badQuantity;
        RepairDate = repairDate;
        Description = description;
    }
}
