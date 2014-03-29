using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace journalism_thingie
{
    class Citizen
    {
        internal double mediaTrust,//how much the citizen trusts your media concern [0,1]
        ideology,//0 - extreme right; 1 - extreme left
        lifeQuality,//0 - hobo; 1 - bill gates
        fanaticism,//0 - politically apathetic; 1 - party hardliner
        //happiness,//0 - miserable; 1 - in heaven
        //ISSUES
        nationalist,//wants to recover lost territory
        minorityRights,//wants to expand minority rights
        socialJustice,//wants to punish the upper class for its corruption
        isolationism;//wants nothing to do with Great Powers
        public const int POOR = 0, MIDDLE = 1, RICH = 2;//social class
        private static Random rand = new Random();

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
                ch = rand.Next(10);
                //it's quite unlikely for the poor to be politically moderate
                //0-3: rightist ideology
                //4-5: centrist ideology
                //6-9: leftist ideology
                if (ch < 4)
                    ideology = rand.NextDouble() * 0.35;
                else if (ch < 6)
                    ideology = 0.35 + (rand.NextDouble() * 0.3);
                else
                    ideology = 0.65 + (rand.NextDouble() * 0.35);
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
                    ideology = rand.NextDouble() * 0.3;
                else if (ch < 8)
                    ideology = 0.3 + (rand.NextDouble() * 0.4);
                else
                    ideology = 0.7 + (rand.NextDouble() * 0.3);
                lifeQuality = 0.3 + rand.NextDouble() * 0.45;
            }
            if (socialClass == RICH)
            {
                ch = rand.Next(10);
                //the rich are quite likely on the right, or, at the most, in the centre
                //0-4: rightist ideology
                //5-8: centrist ideology
                //9: leftist ideology
                if (ch < 4)
                    ideology = rand.NextDouble() * 0.35;
                else if (ch < 6)
                    ideology = 0.35 + (rand.NextDouble() * 0.35);
                else
                    ideology = 0.7 + (rand.NextDouble() * 0.3);
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
            //double miu = 0.5;
            //double var = 2.5;
            //fanaticism = Math.Pow(Math.E, -(Math.Pow(rand.NextDouble()-miu,2)/2*var*var))/(var*Math.Sqrt(2*Math.PI));
            mediaTrust = 1 - Math.Abs(ideology - mediaIdeology);
            nationalist = (1 - ideology) * fanaticism;//hardcore rightist believe in nationalism
            minorityRights = ideology * fanaticism;//hardcore leftists believe in minority rights
            ch = rand.Next(15);
            if (ch < 1)
                isolationism = rand.NextDouble() * 0.2;
            else if (ch < 14)
                isolationism = 0.2 + (rand.NextDouble() * 0.6);
            else
                isolationism = 0.8 + (rand.NextDouble() * 0.2);
            ch = rand.Next(15);
            if (ch < 1)
                socialJustice = rand.NextDouble() * 0.2;
            else if (ch < 14)
                socialJustice = 0.2 + (rand.NextDouble() * 0.6);
            else
                socialJustice = 0.8 + (rand.NextDouble() * 0.2);
        }

        /// <summary>
        /// Adjust citizen's awareness of issues and his trust in your network, based on the news you presented.
        /// Basically, there are two phases when reacting to a piece of news:
        /// Firstly, the citizen's trust in the network is affected, based on his ideology and fanaticism;
        /// if a rightist news is presented to a staunch leftist, his trust will lower considerably;
        /// however, existing trust is also taken into account; that is, the higher the trust, the faster it grows
        /// and the slower it drops; someone who trusts your network will give you the benefit of the doubt even when
        /// you present something he normally doesn't agree with.
        /// Secondly, the issues the citizen cares about will be modified by the news's respective factor;
        /// trust in your network decides the strength of the influence.
        /// </summary>
        public void reactToNews(double nationalismFactor, double minorityRightsFactor, double isolationismFactor,
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
        }
    }
}
