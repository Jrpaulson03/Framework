﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.HL7.Models
{
    public static class MessageHelper
    {
        public static List<string> SplitString(string strStringToSplit, string splitBy, StringSplitOptions splitOptions = StringSplitOptions.None)
        {
            return strStringToSplit.Split(new string[] { splitBy }, splitOptions).ToList();
        }

        public static List<string> SplitString(string strStringToSplit, char chSplitBy, StringSplitOptions splitOptions = StringSplitOptions.None)
        {
            return strStringToSplit.Split(new char[] { chSplitBy }, splitOptions).ToList();
        }
        
        public static List<string> SplitString(string strStringToSplit, char[] chSplitBy, StringSplitOptions splitOptions = StringSplitOptions.None)
        {
            return strStringToSplit.Split(chSplitBy, splitOptions).ToList();
        }

        public static string LongDateWithFractionOfSecond(DateTime dt)
        {
            return dt.ToString("yyyyMMddHHmmss.FFFF");
        }
    }
}
