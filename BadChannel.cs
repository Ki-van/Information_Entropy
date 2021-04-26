using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LB_2_TIPIS
{
    class BadChannel
    {
        private enum TransProbsNames{
            ZeroToZero, 
            ZeroToOne, 
            OneToZero, 
            OneToOne
        };
        private struct Symbol
        {
            public string code;
            public double probability;
        };

        public Dictionary<int, int> sended = new Dictionary<int, int>(), retrieved = new Dictionary<int, int>();
        public double[,] NoiseEffect;

        private double[] symbolsProbs;
        private int symbolsCount;
        private Dictionary<TransProbsNames, double> TransProbs = new Dictionary<TransProbsNames, double>();
        private Symbol[] Symbols;

        
        public BadChannel(double[] symbolsProbs, int symbolsCount, double zeroToZero, double zeroToOne, double oneToZero, double oneToOne, int messageCount = 300)
        {
            
            this.symbolsProbs = symbolsProbs;
            this.symbolsCount = symbolsCount;
            this.TransProbs.Add(TransProbsNames.ZeroToZero, zeroToZero);
            this.TransProbs.Add(TransProbsNames.ZeroToOne, zeroToOne);
            this.TransProbs.Add(TransProbsNames.OneToZero, oneToZero);
            this.TransProbs.Add(TransProbsNames.OneToOne, oneToOne);
            
            StartChannel(messageCount);
        }

        private void StartChannel(int messageCount)
        {
            Symbols = new Symbol[symbolsCount];
            int codeLenght = (int)Math.Ceiling(Math.Log2(symbolsCount));

            for (int i = 0; i < Symbols.Length; i++)
            {
                Symbols[i].probability = symbolsProbs[i];
                Symbols[i].code = ToBinaryStr(i + 1, codeLenght);
            }

            NoiseEffect = new double[symbolsCount, (int)Math.Pow(2, codeLenght)];

            int message, noisedMessage;
            Random random = new Random();
            for (int i = 0; i < messageCount; i++)
            {
                message = GetNewMessage(random.NextDouble());
                if (sended.ContainsKey(message))
                    sended[message]++;
                else
                {
                    sended.Add(message, 1);
                }
                noisedMessage = ApplyNoiseToMessage(message);
                if (retrieved.ContainsKey(noisedMessage))
                    retrieved[noisedMessage]++;
                else
                {
                    retrieved.Add(noisedMessage, 1);
                }
                NoiseEffect[message, noisedMessage]++;
            }

            for (int i = 0; i < NoiseEffect.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < NoiseEffect.GetUpperBound(1) + 1; j++)
                {
                    NoiseEffect[i, j] = NoiseEffect[i, j] / retrieved[j];
                }
            }
        }

        private string ToBinaryStr(int a, int strLength)
        {
            string str = Convert.ToString(a, 2);
            string strAlign = "";
            for (int j = 0; j < strLength - str.Length; j++)
                strAlign += "0";
            str = strAlign + str;

            if (strLength < str.Length)
                str = str.Substring(str.Length - strLength, strLength);

            return str;
        }

        private int ApplyNoiseToMessage(int message)
        {
            String noisedMessage = "";
            Random random = new Random();
            double transProb;
            for(int i = 0; i < Symbols[message].code.Length; i++)
            {
                transProb = random.NextDouble();
                if(Symbols[message].code[i] == '0')
                {
                    if (transProb < TransProbs[TransProbsNames.ZeroToOne])
                        noisedMessage += "1";
                    else
                        noisedMessage += "0";
                } else
                {
                    if (transProb < TransProbs[TransProbsNames.OneToZero])
                        noisedMessage += "0";
                    else
                        noisedMessage += "1";
                }
            }

            return Convert.ToInt32(noisedMessage, 2);
        }

        private int GetNewMessage(double p)
        {
            double interval = 0;
            int i = 0;
            while (p >= interval && i < symbolsCount) 
            {
                interval += Symbols[i].probability;
                i++;
            }
            return i - 1;
        }
    }
}
