namespace BooksApi.Data
{
    public static class ImageStorage
    {
        public static string Folder { get; }
        static ImageStorage()
        {
            Folder = Path.Combine(BooksDbContext.DbFolder, "images");
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
        }
        static string GetPath(long id)
        {
            return Path.Combine(Folder, $"{id}.dat");
        }
        public static byte[] Get(long id)
        {
            string path = GetPath(id);
            if (File.Exists(path))
            {
                try
                {
                    return File.ReadAllBytes(path);
                }
                catch
                {
                }
            }
            return Array.Empty<byte>();
        }
        public static void Put(long id, byte[]? data)
        {
            try
            {
                string path = GetPath(id);
                if (data != null && data.Length > 0)
                {
                    File.WriteAllBytes(path, data);
                }
                else
                {
                    File.Delete(path);
                }
            }
            catch
            {
            }
        }
        public static void Delete(long id)
        {
            try
            {
                File.Delete(GetPath(id));
            }
            catch
            {
            }
        }
    }
}
