namespace HistoriasPaolin.Domain.Channels;

public static class ChannelPublicationPrivacy
{
    public const string Private = "private";
    public const string Unlisted = "unlisted";
    public const string Public = "public";

    public static readonly string[] Allowed = [Private, Unlisted, Public];
}
