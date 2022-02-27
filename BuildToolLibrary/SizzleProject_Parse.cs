using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BuildToolLibrary
{
    public partial class SizzleProject
    {
        public static SizzleProject ParseProjectConfig(string ProjectPath)
        {
            using(StreamReader reader = new StreamReader(ProjectPath))
            {
                string JsonData = reader.ReadToEnd();
                var proj = JsonConvert.DeserializeObject<SizzleProject>(JsonData);
                return proj;
            }

            throw new Exception("Failed to create project!");
        }

        public void CloseProjectConfig()
        {
            string FinalPath = Path.Combine(ProjectPath, Common.ProjectFileName);
            using (StreamWriter writer = new StreamWriter(FinalPath))
            {
                string SerializeData = JsonConvert.SerializeObject(this);
                writer.WriteLine(SerializeData);
                writer.Close();
            }
        }

        public static SizzleProject CreateDefaultProject(string FilePath, string ProjectName)
        {
            SizzleProject project = new SizzleProject();
            project._projectName = ProjectName;
            project._projectFilePath = FilePath;

            return project;
        }

    }
}
