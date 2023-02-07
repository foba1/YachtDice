#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("iji7mIq3vLOQPPI8Tbe7u7u/urn3Oo9n+JMZaxlNNDYYYeYWlmAQzeLnHPMF3vcBWjyuZQCHH6jK43xiNZb+fYmF2uiS97At+aN3C3JWrxjTqKsx6RPyrl8cC3wbwnLSa+u6xopcVGQaUKVvHlhad1ws/6BOd6/Dgj4jALGffl0WpQrtUoMcP4OGSwuThSsyr95Wkp4O7jd7GMVDrV6errv7h9YkBNN9ZWquPm8DHuxxCwKpGNZqJT4dnTbe3k/831CpzNnhOdYKNfZLXGjJSh/XI7s5N1mqmY2Efji7tbqKOLuwuDi7u7oxE8TQ1XwQVaC+8avNr60hDvXvEzC3Bcgm340MHp7rjYhpG+B4Ow22NHHMWB2DnMqQJtrofZrxN7i5u7q7");
        private static int[] order = new int[] { 0,7,2,10,12,12,13,11,12,10,13,11,12,13,14 };
        private static int key = 186;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
