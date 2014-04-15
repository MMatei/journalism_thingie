using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace journalism_thingie
{
    class Citizen
    {
        internal double mediaTrust,//how much the citizen trusts your media concern [0,1]
        ideology,//-1 - extreme left; 0 - liberal; 1 - extreme right
        lifeQuality,//0 - hobo; 1 - bill gates
        fanaticism,//0 - politically apathetic; 1 - party hardliner
        //happiness,//0 - miserable; 1 - in heaven
        //ISSUES -1 - anti, 0 - don't care, 1 - pro
        nationalist,//wants to recover lost territory
        minorityRights,//wants to expand minority rights
        socialJustice,//wants to punish the upper class for its corruption
        isolationism;//wants nothing to do with Great Powers
        public const int POOR = 0, MIDDLE = 1, RICH = 2;//social class
        private static Random rand = new Random();
        public const int IDEOLOGY = 0, NATIONALISM = 1, MINORITY = 2, SOCIAL_JUSTICE = 3, ISOLATIONISM = 4;

        /// <summary>
        /// Randomized constructor for the Citizen.
        /// But we can't make it completely random - so we take into consideration one factor: social class.
        /// The natural predisposition of each class should then provide an adequate distribution.
        /// The behaviour of your network so far is summed up in mediaIdeology -> it will influence starting trust.
        /// </summary>
        public Citizen(int socialClass, float mediaIdeology)
        {
            int ch;
            if (socialClass == POOR)
            {
                ch = rand.Next(11);
                //it's quite unlikely for the poor to be politically moderate
                //0-3: rightist ideology
                //4-6: centrist ideology
                //7-10: leftist ideology
                if (ch < 4)
                    ideology = 0.4 + (rand.NextDouble() * 0.6);
                else if (ch < 7)
                    ideology = -0.4 + (rand.NextDouble() * 0.8);
                else
                    ideology = -0.4 - (rand.NextDouble() * 0.6);
                lifeQuality = rand.NextDouble() * 0.3;
            }
            if (socialClass == MIDDLE)
            {
                ch = rand.Next(10);
                //it's quite likely for the bourgeoisie to be politically moderate
                //0-1: rightist ideology
                //2-7: centrist ideology
                //8-9: leftist ideology
                if (ch < 2)
                    ideology = 0.5 + (rand.NextDouble() * 0.5);
                else if (ch < 8)
                    ideology = -0.5 + (rand.NextDouble() * 1.0);
                else
                    ideology = -0.5 - (rand.NextDouble() * 0.5);
                lifeQuality = 0.3 + rand.NextDouble() * 0.45;
            }
            if (socialClass == RICH)
            {
                ch = rand.Next(10);
                //the rich are quite likely on the right, or, at the most, in the centre
                //0-4: rightist ideology
                //5-8: centrist ideology
                //9: leftist ideology
                if (ch < 5)
                    ideology = 0.4 + (rand.NextDouble() * 0.6);
                else if (ch < 9)
                    ideology = -0.5 + (rand.NextDouble() * 0.9);
                else
                    ideology = -0.5 - (rand.NextDouble() * 0.5);
                lifeQuality = 0.75 + rand.NextDouble() * 0.25;
            }
            ch = rand.Next(15);
            //"normal" distribution
            if (ch < 1)
                fanaticism = rand.NextDouble() * 0.2;
            else if (ch < 14)
                fanaticism = 0.2 + (rand.NextDouble() * 0.6);
            else
                fanaticism = 0.8 + (rand.NextDouble() * 0.2);
            
            mediaTrust = 1 - Math.Abs(ideology - mediaIdeology)/2;
            nationalist = ideology * fanaticism;//hardcore rightist believe in nationalism
            minorityRights = -ideology * fanaticism;//hardcore leftists believe in minority rights
            ch = rand.Next(15);
            if (ch < 1)
                isolationism = -0.5 - (rand.NextDouble() * 0.5);
            else if (ch < 14)
                isolationism = -0.5 + (rand.NextDouble() * 1.0);
            else
                isolationism = 0.5 + (rand.NextDouble() * 0.5);
            ch = rand.Next(15);
            if (ch < 1)
                socialJustice = -0.5 - (rand.NextDouble() * 0.5);
            else if (ch < 14)
                socialJustice = -0.5 + (rand.NextDouble() * 1.0);
            else
                socialJustice = 0.5 + (rand.NextDouble() * 0.5);
        }

        /// <summary>
        /// Adjust citizen's awareness of issues and his trust in your network, based on the news you presented.
        /// dominantFactor identifies which of ideology, nationalism etc will influence trust
        /// factor is the value around which the citizen's respective factor must be in order for him to trust this news
        /// the modifiers will influence the citizen's issues depending on his trust in the network
        /// Basically, there are two phases when reacting to a piece of news:
        /// 1. determine trust modification; the more trust a citizen has the easier it is to gain more and
        /// the harder it is to lose it (people are usually conservative)
        /// 2. affect the citizen's issue based on news modifiers and trust
        /// Altogether, the way I've designed this function ensures that you can convert citizens to your
        /// point of view by presenting news they like (which gains their trust, without significantly lowering
        /// the trust of your loyal viewers)
        /// Of course, you need to be careful not to make extreme assertions when trying to appeal to a broader audience
        /// that will lead to an undesireable shift in your fanbase's issues
        /// </summary>
        public void reactToNews(int dominantFactor, double factor, double ideologyModif, double nationalismModif,
            double minorityRightsModif, double isolationismModif, double socialJusticeModif)
        {
            double factorDifference;//the difference between the citizen's beliefs and the news's presentation
            switch (dominantFactor)
            {
                case IDEOLOGY: factorDifference = Math.Abs(ideology - factor);
                    break;
                case NATIONALISM: factorDifference = Math.Abs(nationalist - factor);
                    break;
                case MINORITY: factorDifference = Math.Abs(minorityRights - factor);
                    break;
                case ISOLATIONISM: factorDifference = Math.Abs(isolationism - factor);
                    break;
                case SOCIAL_JUSTICE: factorDifference = Math.Abs(socialJustice - factor);
                    break;
            }
        }

        /*public void reactToNews(double nationalismFactor, double minorityRightsFactor, double isolationismFactor,
            double socialJusticeFactor, double ideologyFactor)
        {
             double nationalismDifference = Math.Abs(nationalist - nationalismFactor);
             if(nationalismDifference<0.05) mediaTrust += mediaTrust * fanaticism * 0.02;
                else if(nationalismDifference<0.1) mediaTrust += mediaTrust * fanaticism * 0.01;
                else if(nationalismDifference<0.25) mediaTrust += 0;
                else if(nationalismDifference<0.5) mediaTrust -= (1 -mediaTrust) * fanaticism * 0.02;
                else if (nationalismDifference < 0.75) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.05;
                else if (nationalismDifference < 1) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.1;
             
             double isolationismDifference = Math.Abs(isolationism - isolationismFactor);
             if(isolationismDifference<0.05) mediaTrust += mediaTrust * fanaticism * 0.02;
                else if(isolationismDifference<0.1) mediaTrust += mediaTrust * fanaticism * 0.01;
                else if(isolationismDifference<0.25) mediaTrust += 0;
                else if (isolationismDifference < 0.5) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.02;
                else if (isolationismDifference < 0.75) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.05;
                else if (isolationismDifference < 1) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.1;
             
             double socialJusticeDifference = Math.Abs(socialJustice - socialJusticeFactor);
             if(socialJusticeDifference<0.05) mediaTrust += mediaTrust * fanaticism * 0.02;
                else if(socialJusticeDifference<0.1) mediaTrust += mediaTrust * fanaticism * 0.01;
                else if(socialJusticeDifference<0.25) mediaTrust += 0;
                else if (socialJusticeDifference < 0.5) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.02;
                else if (socialJusticeDifference < 0.75) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.05;
                else if (socialJusticeDifference < 1) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.1;
             
             double ideologyDifference = Math.Abs(ideology - ideologyFactor);
             if(ideologyDifference<0.05) mediaTrust += mediaTrust * fanaticism * 0.02;
                else if(ideologyDifference<0.1) mediaTrust += mediaTrust * fanaticism * 0.01;
                else if(ideologyDifference<0.25) mediaTrust += 0;
                else if (ideologyDifference < 0.5) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.02;
                else if (ideologyDifference < 0.75) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.05;
                else if (ideologyDifference < 1) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.1;
             
             
             double minorityRightsDifference = Math.Abs(minorityRights - minorityRightsFactor);
             if(minorityRightsDifference<0.05) mediaTrust += mediaTrust * fanaticism * 0.02;
                else if(minorityRightsDifference<0.1) mediaTrust += mediaTrust * fanaticism * 0.01;
                else if(minorityRightsDifference<0.25) mediaTrust += 0;
                else if (minorityRightsDifference < 0.5) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.02;
                else if (minorityRightsDifference < 0.75) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.05;
                else if (minorityRightsDifference < 1) mediaTrust -= (1 - mediaTrust) * fanaticism * 0.1;
        }*/

        /// <summary>
        /// Compute the citizen's political preference. The Option with the most votes gives us the ending.
        /// </summary>
        /// <returns>The code of the chosen option. The code is option_id * 3 + party_id, where options are:
        /// irredentism, forget lost territories, suppress minority, give rights to minority,
        /// reject globalization, embrace globalization, social revolution, social reform
        /// </returns>
        public int vote()
        {
            double max = 0;
            int maxID = 0;
            if (-nationalist > max)
            {
                max = -nationalist;
                maxID = 0;
            }
            if (nationalist > max)
            {
                max = nationalist;
                maxID = 1;
            }
            if (-minorityRights > max)
            {
                max = -minorityRights;
                maxID = 2;
            }
            if (minorityRights > max)
            {
                max = minorityRights;
                maxID = 3;
            }
            if (-isolationism > max)
            {
                max = -isolationism;
                maxID = 4;
            }
            if (isolationism > max)
            {
                max = isolationism;
                maxID = 5;
            }
            if (-socialJustice > max)
            {
                max = -socialJustice;
                maxID = 6;
            }
            if (socialJustice > max)
            {
                max = socialJustice;
                maxID = 7;
            }
            maxID *= 3;
            if (ideology < -0.5)
                maxID += 0;
            else if (ideology < 0.5)
                maxID++;
            else
                maxID += 2;
            return maxID;
        }
    }
}
