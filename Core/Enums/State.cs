namespace Core.Enums;

public enum State
{
    New,
    Unpublished,
    Published,
    // TODO : Remove this state. You will save changes in a separate object which if not null means it is modified
    Modified
}