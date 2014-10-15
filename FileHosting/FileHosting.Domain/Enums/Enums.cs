namespace FileHosting.Domain.Enums
{
    public enum ViewModelsMessageType
    {
        Default = 0,
        Error = 1,
        Success = 2
    }

    public enum FileActionsType
    {
        Change = 1,
        Delete = 0
    }

    public enum FileDetailsType
    {
        Guest = 0,
        Owner = 1
    }
}
