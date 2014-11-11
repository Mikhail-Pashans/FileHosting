namespace FileHosting.Domain.Enums
{
    public enum ViewModelsMessageType
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5
    }

    public enum FileBrowsingPermission
    {
        AllUsers = 0,
        RegisteredUsers = 1,
        SpecificUsers = 2
    }

    public enum SubscribeActionType
    {
        Subscribe = 0,
        Unsubscribe = 1
    }

    public enum EmailType
    {
        FileDeleted = 0,
        FileChanged = 1,
        UserPasswordChanged = 2
    }

    public enum SearchFilesType
    {
        ByName = 0,
        ByCategory = 1,
        ByTag = 2
    }
}