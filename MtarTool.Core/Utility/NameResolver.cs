using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MtarTool.Core.Utility
{
    public static class NameResolver
    {
        static string[] dictionary = File.ReadAllLines(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\mtar_dictionary.txt");
        static string[] hashDictionary = HashDictionary(dictionary);

        static List<string> outputList = new List<string>(0);

        const string ASSETS_CONST = "/Assets/";

        public static string GetExtension(ulong hash)
        {
            ulong hashExtension = hash >> 51;

            switch(hashExtension)
            {
                case 8074: return "gani";
            } //switch ends

            return "Unknown!";
        } //method GetExtension ends

        public static string GetHashFromULong(ulong ul)
        {
            string hash = ul.ToString("x");

            string prefix = "";

            switch (hash.Substring(2, 2))
            {
                case "51":
                    prefix = "1";
                    break;
                case "52":
                    prefix = "2";
                    break;
                case "53":
                    prefix = "3";
                    break;
            } //switch ends

            if (hash.Substring(3)[0] == '0' && hash.Substring(4)[0] == '0')
            {
                return hash.Substring(5);
            } //if ends

            return prefix + hash.Substring(4);
        } //method GetHashFromULong ends

        public static string GetHashFromString(string text)
        {
            string toHash = text;

            if(text.Contains(ASSETS_CONST))
            {
                toHash = text.Substring(ASSETS_CONST.Length);
            } //if ends

             return GetStrCode32(toHash).ToString("x");
        } //method getNameHash ends

        public static string TryFindName(string text)
        {
            for(int i = 0; i < hashDictionary.Length; i++)
            {
                if (text == hashDictionary[i])
                {
                    Console.WriteLine(dictionary[i]);
                    outputList.Add(hashDictionary[i] + " = " + dictionary[i]);
                    return dictionary[i];
                } //if ends
            } //for ends

            Console.WriteLine(text);
            return text;
        } //method TryFindName ends

        public static ulong GetHashFromName(string text)
        {
            string ganiPath = Path.GetDirectoryName(text);
            string ganiName = Path.GetFileName(text);

            if(char.IsDigit(ganiName[0]) && ganiName[4] == '_')
            {
                string[] strings = ganiName.Split('_');

                ganiName = "";
                
                for(int i = 1; i < strings.Length; i++)
                {
                    ganiName += strings[i];
                } //for ends

                text = ganiPath + ganiName;
            } //if ends

            if (text.Contains(ASSETS_CONST))
            {
                text = GetHashFromString(text);
            } //if ends

            string outputText = "";
            ulong outputULong = 0x0;

            text = text.Replace(".gani", "");

            while (text.Length < 13)
            {
                text = "0" + text;
            } //while ends

            switch(text[0])
            {
                case '0': outputText = "FC50" + text.Substring(1);
                    break;
                case '1': outputText = "FC51" + text.Substring(1);
                    break;
                case '2': outputText = "FC52" + text.Substring(1);
                    break;
                case '3': outputText = "FC53" + text.Substring(1);
                    break;
            } //switch ends

            outputULong = Convert.ToUInt64(outputText, 16);

            Console.WriteLine(outputText);

            return outputULong;
        } //method GetHashFromName ends

        public static void WriteOutputList()
        {
            File.WriteAllLines(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\hashed_names.txt", outputList);
        } //method WriteOutputList ends

        private static ulong GetStrCode32(string text)
        {
            const ulong seed0 = 0x9ae16a3b2f90404f;
            byte[] seed1Bytes = new byte[sizeof(ulong)];
            for (int i = text.Length - 1, j = 0; i >= 0 && j < sizeof(ulong); i--, j++)
            {
                seed1Bytes[j] = Convert.ToByte(text[i]);
            }
            ulong seed1 = BitConverter.ToUInt64(seed1Bytes, 0);
            ulong maskedHash = CityHash.CityHash.CityHash64WithSeeds(text, seed0, seed1) & 0x3FFFFFFFFFFFF;
            return maskedHash;
        } //method GetStrCode32 ends

        private static string[] HashDictionary(string[] dictionary)
        {
            string[] hashDictionary = new string[dictionary.Length];

            for (int i = 0; i < dictionary.Length; i++)
            {
                hashDictionary[i] = GetHashFromString(dictionary[i]);
            } //for ends

            return hashDictionary;
        } //method HashDictionary ends
    } //class NameResolver ends
}
