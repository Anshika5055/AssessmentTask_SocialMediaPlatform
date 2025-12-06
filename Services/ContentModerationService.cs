namespace Social_Media.Services;

public class ContentModerationService
{
    private readonly string[] bannedWords = {
        "monolith", "spaghettiCode", "goto", "hack", "architrixs",
        "quickAndDirty", "cowboy", "yo", "globalVariable", "recursiveHell",
        "backdoor", "hotfix", "leakyAbstraction", "mockup", "singleton",
        "silverBullet", "technicalDebt"
    };

    public bool ContainsBannedWords(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        string lowerContent = content.ToLower();

        foreach (var word in bannedWords)
        {
            if (lowerContent.Contains(word.ToLower()))
            {
                return true;
            }
        }

        return false;
    }
}

