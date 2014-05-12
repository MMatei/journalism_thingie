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
        private static Random rand = new Random();
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
                options[i].nationalismFactor = Convert.ToDouble(word[0], Game.cultureInfo);
                options[i].minorityRightsFactor = Convert.ToDouble(word[1], Game.cultureInfo);
                options[i].isolationismFactor = Convert.ToDouble(word[2], Game.cultureInfo);
                options[i].socialJusticeFactor = Convert.ToDouble(word[3], Game.cultureInfo);
                options[i].ideologyFactor = Convert.ToDouble(word[4], Game.cultureInfo);
                options[i].aggressiveness = Convert.ToDouble(word[5], Game.cultureInfo);
                options[i].evidenceLvl = Convert.ToDouble(word[6], Game.cultureInfo);
                options[i].evidenceLvl *= rand.NextDouble();
                options[i].description += " (we have ";
                if (options[i].evidenceLvl < 0.33)
                    options[i].description += "weak ";
                else if (options[i].evidenceLvl < 0.66)
                    options[i].description += "some ";
                else
                    options[i].description += "strong ";
                options[i].description += "evidence to back this news)";
            }
        }
    }
    class Option
    {
        public double nationalismFactor, minorityRightsFactor, isolationismFactor, socialJusticeFactor, ideologyFactor,
            aggressiveness, evidenceLvl;
        public String description;//the short version you see on your notepad
        public String newsArticle;//the thing that gets read on the telly screen
    }
}
