public static class FileChecker
{
    public static float ConvertBytesToMegabytes(long bytes)
    {
        return bytes / 1024f / 1024f;
    }

    public static float ConvertBytesToGigabytes(long bytes)
    {
        return bytes / 1024f / 1024f / 1024f;
    }

    public static bool CheckIfImage(string fileType)
    {
        string[] imageTypes = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp"];
        foreach (var type in imageTypes)
        {
            if (fileType.Contains(type))
            {
                return true;
            }
        }

        return false;
    }

    public static bool CheckIfVideo(string fileType)
    {
        string[] videoTypes = [".mp4", ".mov", ".avi", ".mkv", ".wmv", ".flv", ".webm"];
        foreach (var type in videoTypes)
        {
            if (fileType.Contains(type))
            {
                return true;
            }
        }

        return false;
    }

    public static bool CheckIfAudio(string fileType)
    {
        string[] audioTypes = [".mp3", ".wav", ".flac", ".ogg", ".m4a", ".wma", ".aac"];
        foreach (var type in audioTypes)
        {
            if (fileType.Contains(type))
            {
                return true;
            }
        }

        return false;
    }

    public static bool CheckIfDocument(string fileType)
    {
        string[] documentTypes = [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt"];
        foreach (var type in documentTypes)
        {
            if (fileType.Contains(type))
            {
                return true;
            }
        }

        return false;
    }
}