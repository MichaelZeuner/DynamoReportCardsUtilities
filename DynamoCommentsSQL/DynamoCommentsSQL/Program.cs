using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoCommentsSQL
{
    class Program
    {
        struct CommentInfo
        {
            public string type;
            public string comment;
            public List<int> levels;
        }

        static void Main(string[] args)
        {
            List<CommentInfo> comments = new List<CommentInfo>();
            string[] fileEntries = Directory.GetFiles(@"C:\Users\mzeuner\Google Drive\Contract Jobs\Dynamo\Report Card\Comments\Women's");
            foreach (string fileName in fileEntries)
            {
                if(fileName.Contains("output")) { continue; }
                int levelId = int.Parse(fileName.Substring(fileName.LastIndexOf("\\") + 1).Split('.')[0]);
                
                string line;
                StreamReader file = new StreamReader(fileName);
                while ((line = file.ReadLine()) != null)
                {
                    string[] components = line.Split(',');
                    for(int i=1; i<components.Length; i++)
                    {
                        if(components[i].Contains("\""))
                        {
                            Console.WriteLine("Likely an issue in file: " + levelId);
                            Console.Read();
                        }
                        if(components[i] != "")
                        {
                            bool added = false;
                            for(int j=0; j<comments.Count; j++)
                            {
                                if(comments[j].type == components[0] && comments[j].comment == components[i])
                                {
                                    added = true;
                                    comments[j].levels.Add(levelId);
                                    break;
                                }
                            }
                            if(added == false)
                            {
                                CommentInfo comment = new CommentInfo();
                                comment.comment = components[i];
                                comment.type = components[0];
                                comment.levels = new List<int>();
                                comment.levels.Add(levelId);
                                comments.Add(comment);
                            }
                        } 
                    }
                }

                file.Close();

            }
            using (StreamWriter file =
            new StreamWriter(@"C:\Users\mzeuner\Google Drive\Contract Jobs\Dynamo\Report Card\Comments\Women's\output.csv"))
            {
                foreach (CommentInfo comment in comments)
                {
                    file.Write(comment.type + "," + comment.comment);
                    foreach (int num in comment.levels)
                    {
                        file.Write("," + num);
                    }
                    file.Write("\n");
                }
            }

            using (StreamWriter fileMain =
            new StreamWriter(@"C:\Users\mzeuner\Google Drive\Contract Jobs\Dynamo\Report Card\Comments\Women's\outputMain.txt"))
            {
                foreach (CommentInfo comment in comments)
                {
                    fileMain.Write("INSERT INTO comments (type, comment) VALUES (\"" + comment.type + "\", \"" + comment.comment + "\");\n");
                    fileMain.Write("SELECT @id:= LAST_INSERT_ID();\n");
                    foreach (int num in comment.levels)
                    {
                        fileMain.Write("INSERT INTO comment_levels(comments_id, levels_id) VALUES (@id,"+num+");\n");
                    }
                }
            }
        }
        
    }
}
