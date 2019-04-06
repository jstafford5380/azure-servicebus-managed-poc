namespace AzureServiceBusPoc.Lib.Core
{
    public enum ChannelType
    {
        Unspecified,
        Queue,
        Topic
    }

    public enum MessageIntent
    {
        Unspecified,
        Command,
        Event
    }
}