using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace journalism_thingie
{
    class News
    {
        public String situationDescription;
        public Option[] options;
        /// <summary>
        /// The constructor receives the name of the file it will use to populate its fields.
        /// </summary>
        public News(String filename)
        {
            StreamReader file = new StreamReader(filename);
            String s = file.ReadLine();
            while (s.StartsWith("#"))
                s = file.ReadLine();
            situationDescription = s;
            s = file.ReadLine();
            while (s.StartsWith("#"))
                s = file.ReadLine();
            int n = Convert.ToInt32(s);//nr of options
            options = new Option[n];
            for (int i = 0; i < n; i++)
            {
                options[i] = new Option();
                s = file.ReadLine();
                while (s.StartsWith("#"))
                    s = file.ReadLine();
                options[i].description = s;
                s = file.ReadLine();
                while (s.StartsWith("#"))
                    s = file.ReadLine();
                options[i].newsArticle = s;
                s = file.ReadLine();
                while (s.StartsWith("#"))
                    s = file.ReadLine();
                String[] word = s.Split(';');
                options[i].nationalismFactor = Convert.ToDouble(word[0]);
                options[i].minorityRightsFactor = Convert.ToDouble(word[1]);
                options[i].isolationismFactor = Convert.ToDouble(word[2]);
                options[i].socialJusticeFactor = Convert.ToDouble(word[3]);
                options[i].ideologyFactor = Convert.ToDouble(word[4]);
            }
        }
    }
    class Option
    {
        public double nationalismFactor, minorityRightsFactor, isolationismFactor, socialJusticeFactor, ideologyFactor;
        public String description;//the short version you see on your notepad
        public String newsArticle;//the thing that gets read on the telly screen
    }
}
