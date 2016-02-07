using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grimacizer7.Utils
{
    public static class Quotes
    {
        private static string[] quotes = 
        {
            "The greater the power, the more dangerous the abuse - by Edmund Burke",
            "Our patience will achieve more than our force - by Edmund Burke",
            "Genius is eternal patience - by Michelangelo",
            "Endurance is patience concentrated - by Thomas Carlyle",
            "The test of good manners is to be patient with the bad ones - by Solomon Ibn Gabirol",
            "A man who is a master of patience is master of everything else - by George Savile",
            "Have patience. All things are difficult before they become easy - by Saadi",
            "We could never learn to be brave and patient, if there were only joy in the worl - by Helen Keller",
            "Patience is the art of hoping - by Luc de Clapiers",
            "Beware the fury of a patient man - by John Dryden",
            "Patience and Diligence, like faith, remove mountains - by William Penn",
            "Hope is patience with the lamp lit - by Tertullian",
            "Adopt the pace of nature: her secret is patience - by Ralph Waldo Emerson",
            "The two most powerful warriors are patience and time - by Leo Tolstoy",
            "With love and patience, nothing is impossible - by Daisaku Ikeda",
            "The principle part of faith is patience - by George MacDonald",
            "The greatest power is often simple patience - by E. Joseph Cossman",
            "Anger dwells only in the bosom of fools - by Albert Einstein",
            "Whatever is begun in anger ends in shame- by Benjamin Franklin",
            "Get mad, then get over it - by Colin Powell",
            "Expressing anger is a form of public littering -by Willard Gaylin",
            "Silence is a source of great strength - by Lao Tzu"
        };

        public static string GetRandomQuote()
        {
            var random = new Random();
            return quotes[random.Next(quotes.Length)];
        }
    }
}
