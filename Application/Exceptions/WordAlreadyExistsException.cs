namespace Application.Exceptions;

public class WordAlreadyExistsException(string word) 
    : Exception($"The word '{word}' already exists in the dictionary.");
