namespace Core.Enums;

public enum State
{
    NEW,
    UNPUBLISHED,
    PUBLISHED,
    // TODO : Remove this state. You will save changes in a separate object which if not null means it is modified
    MODIFIED
}