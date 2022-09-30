using System.Xml.Serialization;
using System;

namespace Books.Models
{
    public class LoginModel
    {
        public string Server { get; set; } = "https://localhost:8443";
        private static string ConfigPathname()
        {
            string folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Books");
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            return System.IO.Path.Combine(folder, "books-login.xml");
        }
        private static XmlSerializer CreateSerializer()
        {
            return new XmlSerializer(typeof(LoginModel));
        }
        public static LoginModel Load()
        {
            return Load(ConfigPathname());
        }
        public static LoginModel Load(string filename)
        {
            LoginModel? res = null;
            try
            {
                using System.IO.FileStream fs = new(filename, System.IO.FileMode.Open);
                var xs = CreateSerializer();
                res = xs.Deserialize(fs) as LoginModel;
            }
            catch
            {
            }
            res ??= new LoginModel();
            return res;
        }
        public void Save()
        {
            Save(ConfigPathname());
        }
        public void Save(string filename)
        {
            try
            {
                using System.IO.FileStream fs = new(filename, System.IO.FileMode.Create);
                var xs = CreateSerializer();
                xs.Serialize(fs, this);
            }
            catch
            {

            }
        }
    }
}
