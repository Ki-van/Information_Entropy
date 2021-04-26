using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LB_2_TIPIS
{
    class PosteriorEntropy
    {
        BadChannel badChannel;

        public double[] sourceProbs, recieverProbs;
        public double SourceEntropy, PosteriorChannelEntropy, ReceiverEntropy, UsefulInformation, Speed, EntropyOfNoise;
        
        public PosteriorEntropy(BadChannel badChannel)
        {
            this.badChannel = badChannel;
            CalcPosteriorSourceEntropy();
            CalcRecieverEntropy();
            CalcPosteriorChannelEntropy();
            CalcUsefulInformation();
            CalcSpeed();
            CalcEntropyOfNoise();
        }
        void CalcEntropyOfNoise()
        {
            EntropyOfNoise = ReceiverEntropy - UsefulInformation;
        }
        private void CalcSpeed()
        {
            Speed = UsefulInformation / 0.1;
        }
        private void CalcUsefulInformation()
        {
            UsefulInformation = ReceiverEntropy - PosteriorChannelEntropy;
        }
        void CalcRecieverEntropy()
        {
            double H = 0;
            recieverProbs = new double[badChannel.retrieved.Count];
            double n = 0;

            foreach (var c in badChannel.retrieved)
                n += c.Value;

            for (int i = 0; i < badChannel.retrieved.Count; i++)
                recieverProbs[i] = badChannel.retrieved[i] / n;

            foreach (var p in recieverProbs)
                H += -p * Math.Log2(p);
            ReceiverEntropy = H;
        }

        void CalcPosteriorChannelEntropy()
        {
            double H = 0, Huvi;

            for (int i = 0; i < badChannel.NoiseEffect.GetUpperBound(0) + 1; i++)
            {
                Huvi = 0;
                for (int j = 0; j < badChannel.NoiseEffect.GetUpperBound(1) + 1; j++)
                {
                    if (badChannel.NoiseEffect[i, j] == 0)
                        continue;
                    Huvi += -badChannel.NoiseEffect[i, j] * Math.Log2(badChannel.NoiseEffect[i, j] );
                }
                H += recieverProbs[i] * Huvi;
            }


            PosteriorChannelEntropy = H;
        }

        void CalcPosteriorSourceEntropy()
        {
            sourceProbs = new double[badChannel.sended.Count];
            double H = 0;
            double n = 0;

            foreach (var c in badChannel.sended)
                n += c.Value;

            for (int i = 0; i < badChannel.sended.Count; i++)
                sourceProbs[i] = badChannel.sended[i] / n;

            foreach (var p in sourceProbs)
                H += -p * Math.Log2(p);

            SourceEntropy = H;
        }
    }
}
