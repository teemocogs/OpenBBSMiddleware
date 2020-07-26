using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BILib
{
    /**
     * 文章編號與檔案名稱轉換工具<br />
     * 將符合此格式的 [a-zA-Z-_]{8} 文章編號轉換為檔案名稱 或是 URL<br />
     * 範例:<br />
     * 1MVYyFDv -> M.1451110159.A.379 -> https://www.ptt.cc/bbs/Gossiping/M.1451110159.A.379.html<br /><br />
     * 或是將符合 [M|G].[unsigned_longeger].A.[HEX{3}] 格式的檔案名稱轉換為文章編號<br />
     * 若為 URL 則將轉換為 {@link AidBean}, 其中包含看板名與文章編號<br />
     * 範例:<br />
     * https://www.ptt.cc/bbs/Gossiping/M.1451110159.A.379.html -> M.1451110159.A.379 -> 1MVYyFDv
     */
    public class AidConverter
    {

        private static String DOMAIN_URL = "https://www.ptt.cc/bbs/";
        private static String FILE_EXT = ".html";

        private static String aidTable = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";
        private static SortedDictionary<Char, long> table = tableInitializer();

        /**
         * 建立文章編號字元 Map, 方便取得對應數值
         * @return
         *  文章編號字元 Map
         */
        private static SortedDictionary<Char, long> tableInitializer()
        {
            SortedDictionary<Char, long> table = new SortedDictionary<char, long>();
            long index = 0;
            long size = aidTable.Length;

            for (int i = 0; i < size; i++)
            {
                table.Add(aidTable[i], index);
                index++;
            }

            return table;
        }

        /**
         * 將檔案名稱轉換為˙數字型態的文章序號
         * @param fn    檔案名稱
         * @return
         *  數字型態的文章序號, 若檔案名稱格式不符將回傳 0<br />
         *  轉換後的文章編號將符合 [M|G].[unsigned_longeger].A.[HEX{3}]<br />
         *  範例: M.1451100858.A.71E
         */
        private static long fn2aidu(String fn)
        {
            long aidu = 0;
            long type = 0;
            long v1 = 0;
            long v2 = 0;

            if (fn == null) return 0;

            List<String> fnList = fn.Split('.').ToList();

            if (fnList.Count != 4) return 0;

            String typeString = fnList[0];
            String v1String = fnList[1];
            String v2String = fnList[3];

            if (!fnList[2].Equals("A")) return 0;
            if (string.IsNullOrEmpty(v1String) || v1String.Length != 10) return 0;

            switch (typeString)
            {
                case "M":
                    type = 0;
                    break;
                case "G":
                    type = 1;
                    break;
                default:
                    return 0;
            }

            v1 = long.Parse(v1String);
            v2 = Convert.ToInt32(v2String, 16);//long.Parse(v2String, System.Globalization.NumberStyles.AllowParentheses);
            //long number = Int64.Parse(v2String, System.Globalization.NumberStyles.AllowParentheses);
            aidu = ((type & 0xf) << 44) | ((v1 & 0xffffffffL) << 12) | (v2 & 0xfff);

            return aidu;
        }

        /**
         * 將數字型態的文章序號轉換為字串型態的文章編號
         * @param aidu  數字型態之文章序號
         * @return
         *  轉換後的文章編號將符合 [a-zA-Z-_]{8}<br />
         *  範例: 1MVWgwSU
         */
        private static String aidu2aidc(long aidu)
        {
            long size = table.Count;
            //List<long, Char> inverseTable = table.Reverse().ToList();

            StringBuilder stringBuffer = new StringBuilder();

            while (stringBuffer.Length < 8)
            {
                int v = (int)(aidu % size);
                //if( !inverseTable.containsKey( v ) ) return null;
                var keyValue = table.Where(x => x.Value == v).ToList();
                if (keyValue == null) return null;
                stringBuffer.Insert(0, keyValue.First().Key);

                aidu = aidu / size;
            }

            return stringBuffer.ToString();
        }

        /**
         * 將文章編號轉換為數字型態的文章序號
         * @param aid   文章編號
         * @return
         *  數字型態的文章序號
         */
        private static long aidc2aidu(String aid)
        {
            //char[] aidChars = aid.toCharArray();
            long aidu = 0;

            foreach (char aidChar in aid)
            {
                if (aidChar == '@') break;
                if (!table.ContainsKey(aidChar))
                    return 0;

                long v = table[aidChar];

                aidu = aidu << 6;
                aidu = aidu | (v & 0x3f);
            }

            return aidu;
        }

        /**
         * 將文章序號(數字型態)轉換為檔案名稱
         * @param aidu  文章序號(數字型態)
         * @return
         *  轉換後的檔案名稱, 格式將符合 [M|G].[unsigned_longeger].A.[HEX{3}]<br />
         *  最後的16進位表示法若未滿3個字將以0從左邊開始補齊<br />
         *  範例: M.1451100858.A.71E
         */
        private static String aidu2fn(long aidu)
        {
            long type = ((aidu >> 44) & 0xf);
            long v1 = ((aidu >> 12) & 0xffffffffL);
            long v2 = (aidu & 0xfff);

            // casting to unsigned
            //        v1 = v1 & 0xffffffffL;  Long.toHexString
            String hex = v2.ToString("X").ToUpper();

            return ((type == 0) ? "M" : "G") + "." + v1 + ".A." + hex.PadLeft(3, '0');
        }

        /**
         * 將文章編號轉換為檔案名稱
         * @param aid   文章編號
         * @return
         *  轉換後的檔案名稱, 格式將符合 [M|G].[unsigned_longeger].A.[HEX{3}]<br />
         *  最後的16進位表示法若未滿3個字將以0從左邊開始補齊<br />
         *  範例: M.1451100858.A.71E
         */
        public static String aidToFileName(String aid)
        {
            return aidu2fn(aidc2aidu(aid));
        }

        /**
         * 將文章編號轉換為 WEB 版 URL
         * @param boardTitle    文章所屬看板名稱
         * @param aid           文章編號
         * @return
         *  WEB 版的完整 URL
         */
        public static String aidToUrl(String boardTitle, String aid)
        {
            if (string.IsNullOrEmpty(boardTitle) || string.IsNullOrEmpty(aid)) return "";

            return DOMAIN_URL + boardTitle + "/" + aidToFileName(aid) + FILE_EXT;
        }

        /**
         * 將檔案名稱(也就是 URL 的最後一段 不包含副檔名)轉換為文章編號
         * @param fileName  檔案名稱
         * @return
         *  轉換後的文章編號, 若檔案名稱格式不符則將回傳 null
         */
        public static String fileNameToAid(String fileName)
        {
            return aidu2aidc(fn2aidu(fileName));
        }

        /**
         * 將 URL 轉換為 AID 物件
         * @param url   PTT WEB 版的 URL
         * @return
         *  物件內包含 文章編號 與 看板名
         * @see AidBean
         */
        public static string urlToAid(String url)
        {
            List<String> urlList = url.Split('/').ToList();

            if (urlList.Count < 4) return null;

            String boardTitle = urlList[2];
            String fileName = urlList[3].Replace(".html", "").Replace(".htm", "");
            String aid = fileNameToAid(fileName);

            return aid;
        }

    }

}
