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
        /// Each piece of news may discuss certain issues (nationalism, minority rights, globalization, social justice)
        /// from an ideological standpoint. If an issue has a factor of 0, it is not being discussed.
        /// Each discussed issue will first influence the citizen's trust in the network and then it will influence
        /// his opinions. This influence is determined by:
        /// -> the citizen's existing opinion (he will lose trust if he doesn't hear what he likes;
        /// trust is harder to gain when low and harder to lose when high)
        /// -> the aggressiveness of the news (aggressive news has a bigger impact on opinion, BUT it will also
        /// alienate those who don't implicitly trust you)
        /// aggressiveness is between 0 (rational) and 1 (aggressive)
        /// -> the evidence presented (lack of evidence hurts the trust of every non-fan,
        /// while plentiful evidence may convince even the most ardent haters;
        /// rational approach depends far more on evidence to be successful)
        /// evidenceLvl is between 0 and 1
        /// To conclude:
        /// 1. we modify trust based on news factor vs citizen opinion, modified by aggressive and evidence
        /// 2. we modify opinions based on news factor, modified by aggressive and evidence
        /// </summary>
        public void reactToNews(double nationalismFactor, double minorityRightsFactor, double isolationismFactor,
            double socialJusticeFactor, double ideologyFactor, double aggressiveness, double evidenceLvl)
        {
            mediaTrust += 0.1;//temporary modifier to ensure that even haters can be influenced
            // remember it's mediaTrust * ....
            double f = 0;//trust gain factor        |
            double lf = 0;//trust loss factor       | => computed using aggressiveness and evidenceLvl
            double opf = 0;//opinion change factor  |
            if (evidenceLvl > 0.5)
            {//if I have lots of evidence, those with high difference will gain trust instead of losing
                //rational news - more trust, but greater dependence on evidence, less opinion change
                if (aggressiveness < 0.1) { f = 0.03 * evidenceLvl; lf = f; opf = 0.01 * evidenceLvl; }
                else if (aggressiveness < 0.3) { f = 0.02 * evidenceLvl; lf = f; opf = 0.015 * evidenceLvl; }
                else if (aggressiveness < 0.5) { f = 0.01 * evidenceLvl; lf = f; opf = 0.02 * evidenceLvl; }
                //aggressive news - less trust gain, more opinion change
                else if (aggressiveness < 0.7) { f = 0.01 * evidenceLvl; lf = f/2; opf = 0.03 * evidenceLvl; }
                else if (aggressiveness < 0.9) { f = 0.01 * evidenceLvl; lf = f/2; opf = 0.04 * evidenceLvl; }
                else  { f = 0.01 * evidenceLvl; lf = f/2; opf = 0.05 * evidenceLvl; }
            }
            else
            {
                if (aggressiveness < 0.1) { f = 0.03 * evidenceLvl; lf = 0.01 * (1 - evidenceLvl); opf = 0.01 * evidenceLvl; }
                else if (aggressiveness < 0.3) { f = 0.02 * evidenceLvl; lf = 0.015 * (1 - evidenceLvl); opf = 0.015 * evidenceLvl; }
                else if (aggressiveness < 0.5) { f = 0.01 * evidenceLvl; lf = 0.02 * (1 - evidenceLvl); opf = 0.02 * evidenceLvl; }
                else if (aggressiveness < 0.7) { f = 0.01 * evidenceLvl; lf = 0.03 * (1 - evidenceLvl); opf = 0.03 * evidenceLvl; }
                else if (aggressiveness < 0.9) { f = 0.01 * evidenceLvl; lf = 0.04 * (1 - evidenceLvl); opf = 0.04 * evidenceLvl; }
                else { f = 0.01 * evidenceLvl; lf = 0.05 * (1 - evidenceLvl); opf = 0.05 * evidenceLvl; }
            }

            if (evidenceLvl > 0.5)
            {//if I have lots of evidence, those with high difference will gain trust instead of losing
                if (nationalismFactor != 0)
                {
                    double nationalismDifference = Math.Abs(nationalist - nationalismFactor);
                    if (nationalismDifference < 0.1) mediaTrust += mediaTrust * fanaticism * f * 2;
                    else if (nationalismDifference < 0.2) mediaTrust += mediaTrust * fanaticism * f;
                    else if (nationalismDifference < 0.4) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.5;
                    else if (nationalismDifference < 0.6) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.25;
                    else if (nationalismDifference < 0.8) mediaTrust += mediaTrust * fanaticism * lf * 0.01;
                    else mediaTrust += 0;
                    //I used lf instead of f in order to differentiate between aggressive (where lf < f in this case) and rational
                    //also (1-fanaticism) to illustrate resistance  to being convinced
                }

                if (isolationismFactor != 0)
                {
                    double isolationismDifference = Math.Abs(isolationism - isolationismFactor);
                    if (isolationismDifference < 0.1) mediaTrust += mediaTrust * fanaticism * f * 2;
                    else if (isolationismDifference < 0.2) mediaTrust += mediaTrust * fanaticism * f;
                    else if (isolationismDifference < 0.4) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.5;
                    else if (isolationismDifference < 0.6) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.25;
                    else if (isolationismDifference < 0.8) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.01;
                    else mediaTrust += 0;
                }

                if (minorityRightsFactor != 0)
                {
                    double minorityRightsDifference = Math.Abs(minorityRights - minorityRightsFactor);
                    if (minorityRightsDifference < 0.1) mediaTrust += mediaTrust * fanaticism * f * 2;
                    else if (minorityRightsDifference < 0.2) mediaTrust += mediaTrust * fanaticism * f;
                    else if (minorityRightsDifference < 0.4) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.5;
                    else if (minorityRightsDifference < 0.6) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.25;
                    else if (minorityRightsDifference < 0.8) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.01;
                    else mediaTrust += 0;
                }

                if (socialJusticeFactor != 0)
                {
                    double socialJusticeDifference = Math.Abs(socialJustice - socialJusticeFactor);
                    if (socialJusticeDifference < 0.1) mediaTrust += mediaTrust * fanaticism * f * 2;
                    else if (socialJusticeDifference < 0.2) mediaTrust += mediaTrust * fanaticism * f;
                    else if (socialJusticeDifference < 0.4) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.5;
                    else if (socialJusticeDifference < 0.6) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.25;
                    else if (socialJusticeDifference < 0.8) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.01;
                    else mediaTrust += 0;
                }

                double ideologyDifference = Math.Abs(ideology - ideologyFactor);
                if (ideologyDifference < 0.1) mediaTrust += mediaTrust * fanaticism * f * 2;
                else if (ideologyDifference < 0.2) mediaTrust += mediaTrust * fanaticism * f;
                else if (ideologyDifference < 0.4) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.5;
                else if (ideologyDifference < 0.6) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.25;
                else if (ideologyDifference < 0.8) mediaTrust += mediaTrust * (1 - fanaticism) * lf * 0.01;
                else mediaTrust += 0;
            }
            else
            {
                if (nationalismFactor != 0)
                {
                    double nationalismDifference = Math.Abs(nationalist - nationalismFactor);
                    if (nationalismDifference < 0.1) mediaTrust += mediaTrust * fanaticism * f * 2;
                    else if (nationalismDifference < 0.2) mediaTrust += mediaTrust * fanaticism * f;
                    else if (nationalismDifference < 0.4) mediaTrust += 0;
                    else if (nationalismDifference < 0.6) mediaTrust -= (1 - mediaTrust) * fanaticism * lf;
                    else if (nationalismDifference < 0.8) mediaTrust -= (1 - mediaTrust) * fanaticism * lf * 2;
                    else mediaTrust -= (1 - mediaTrust) * fanaticism * lf * 3;
                }

                if (isolationismFactor != 0)
                {
                    double isolationismDifference = Math.Abs(isolationism - isolationismFactor);
                    if (isolationismDifference < 0.1) mediaTrust += mediaTrust * fanaticism * f * 2;
                    else if (isolationismDifference < 0.2) mediaTrust += mediaTrust * fanaticism * f;
                    else if (isolationismDifference < 0.4) mediaTrust += 0;
                    else if (isolationismDifference < 0.6) mediaTrust -= (1 - mediaTrust) * fanaticism * lf;
                    else if (isolationismDifference < 0.8) mediaTrust -= (1 - mediaTrust) * fanaticism * lf * 2;
                    else mediaTrust -= (1 - mediaTrust) * fanaticism * lf * 3;
                }

                if (minorityRightsFactor != 0)
                {
                    double minorityRightsDifference = Math.Abs(minorityRights - minorityRightsFactor);
                    if (minorityRightsDifference < 0.1) mediaTrust += mediaTrust * fanaticism * f * 2;
                    else if (minorityRightsDifference < 0.2) mediaTrust += mediaTrust * fanaticism * f;
                    else if (minorityRightsDifference < 0.4) mediaTrust += 0;
                    else if (minorityRightsDifference < 0.6) mediaTrust -= (1 - mediaTrust) * fanaticism * lf;
                    else if (minorityRightsDifference < 0.8) mediaTrust -= (1 - mediaTrust) * fanaticism * lf * 2;
                    else mediaTrust -= (1 - mediaTrust) * fanaticism * lf * 3;
                }

                if (socialJusticeFactor != 0)
                {
                    double socialJusticeDifference = Math.Abs(socialJustice - socialJusticeFactor);
                    if (socialJusticeDifference < 0.1) mediaTrust += mediaTrust * fanaticism * f * 2;
                    else if (socialJusticeDifference < 0.2) mediaTrust += mediaTrust * fanaticism * f;
                    else if (socialJusticeDifference < 0.4) mediaTrust += 0;
                    else if (socialJusticeDifference < 0.6) mediaTrust -= (1 - mediaTrust) * fanaticism * lf;
                    else if (socialJusticeDifference < 0.8) mediaTrust -= (1 - mediaTrust) * fanaticism * lf * 2;
                    else mediaTrust -= (1 - mediaTrust) * fanaticism * lf * 3;
                }

                double ideologyDifference = Math.Abs(ideology - ideologyFactor);
                if (ideologyDifference < 0.1) mediaTrust += mediaTrust * fanaticism * f * 2;
                else if (ideologyDifference < 0.2) mediaTrust += mediaTrust * fanaticism * f;
                else if (ideologyDifference < 0.4) mediaTrust += 0;
                else if (ideologyDifference < 0.6) mediaTrust -= (1 - mediaTrust) * fanaticism * lf;
                else if (ideologyDifference < 0.8) mediaTrust -= (1 - mediaTrust) * fanaticism * lf * 2;
                else mediaTrust -= (1 - mediaTrust) * fanaticism * lf * 3;
            }
            mediaTrust -= 0.1;//mediaTrust is restored to its previous condition
            if (mediaTrust < 0) mediaTrust = 0;//stop trust from derailing towards -inf
            nationalist += nationalismFactor * mediaTrust * opf;
            isolationism += isolationismFactor * mediaTrust * opf;
            minorityRights += minorityRightsFactor * mediaTrust * opf;
            socialJustice += socialJusticeFactor * mediaTrust * opf;
            ideology += ideologyFactor * mediaTrust * opf * 0.5;//ideology should be harder to change :-?
        }

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

        /// <summary>
        /// Static method that randomly generates the 100 citizens that form our little focus group.
        /// </summary>
        public static Citizen[] generatePopulation()
        {
            Citizen[] population = new Citizen[100];
            int i;
            uint nrmin = 0, nrmed = 0, nrmax = 0;
            Console.WriteLine("#POOR#");
            for (i = 0; i < 40; i++)
            {
                population[i] = new Citizen(Citizen.POOR, 0);
                Console.WriteLine(population[i].ideology + " " + population[i].fanaticism);
                if (population[i].fanaticism < 0.1) nrmin++;
                if (population[i].fanaticism > 0.45 && population[i].fanaticism < 0.55) nrmed++;
                if (population[i].fanaticism > 0.9) nrmax++;
            }
            Console.WriteLine("#MIDDLE#");
            for (i = 40; i < 90; i++)
            {
                population[i] = new Citizen(Citizen.MIDDLE, 0);
                Console.WriteLine(population[i].ideology + " " + population[i].fanaticism);
                if (population[i].fanaticism < 0.1) nrmin++;
                if (population[i].fanaticism > 0.45 && population[i].fanaticism < 0.55) nrmed++;
                if (population[i].fanaticism > 0.9) nrmax++;
            }
            Console.WriteLine("#RICH#");
            for (i = 90; i < 100; i++)
            {
                population[i] = new Citizen(Citizen.RICH, 0);
                Console.WriteLine(population[i].ideology + " " + population[i].fanaticism);
                if (population[i].fanaticism < 0.1) nrmin++;
                if (population[i].fanaticism > 0.45 && population[i].fanaticism < 0.55) nrmed++;
                if (population[i].fanaticism > 0.9) nrmax++;
            }
            Console.WriteLine("###\nOverly apathetic: " + nrmin);
            Console.WriteLine("Middle ground: " + nrmed);
            Console.WriteLine("Overly fanatic: " + nrmax);
            return population;
        }
    }
}
