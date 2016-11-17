using System;
using System.IO;
using System.Reflection;

namespace MtarTool.Core.Utility
{
    public static class NameResolver
    {
        static string[] dictionary = File.ReadAllLines(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\mtar_dictionary.txt");
        static string[] hashDictionary = HashDictionary(dictionary);

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

            if (hash.Substring(4)[0] == '0')
            {
                return hash.Substring(5);
            } //if ends

            return prefix + hash.Substring(4);
        } //method GetHashFromULong ends

        public static string GetHashFromString(string text)
        {
            const string ASSETS_CONST = "/Assets/";
            string toHash = "";

            if(text.Contains(ASSETS_CONST))
            {
                toHash = text.Substring(ASSETS_CONST.Length);
            } //if ends
            else
            {
                toHash = text;
            } //else ends

             return GetStrCode32(text).ToString("x");
        } //method getNameHash ends

        public static string TryFindName(string text)
        {
            for(int i = 0; i < hashDictionary.Length; i++)
            {

                if (text == hashDictionary[i])
                {
                    Console.WriteLine(hashDictionary[i]);
                    return dictionary[i];
                } //if ends
            } //for ends

            Console.WriteLine(text);
            return text;
        } //method TryFindName ends

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
            for (int i = 0; i < dictionary.Length; i++)
            {
                dictionary[i] = GetHashFromString(dictionary[i]);
            } //for ends

            return dictionary;
        } //method HashDictionary ends
    } //class NameResolver ends
}
