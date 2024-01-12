using Cysharp.Threading.Tasks;
using Sheet;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GameUtility
{
    public static class GameUtility
    {
        public static readonly string PLAYER_TAG = "Player";
        static System.Text.StringBuilder builder = new System.Text.StringBuilder();
        static readonly string zero = "0";
        private static int globalSeed;
        public static string GetId(int id)
        {
            builder.Clear();
            builder.Append(id <= 9 ? zero + zero : (id <= 99 ? zero : string.Empty)).Append(id);
            return builder.ToString();
        }
        public static int GetSeed()
        {
            return globalSeed==0? UnityEngine.Random.seed:globalSeed;
        }
        public static void SetSeed(int seed)
        {
            globalSeed = seed;
        }
        public static List<RowData> ConvertSheetToList(object[,] data)
        {
            List<RowData> list = new List<RowData>();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                RowData row = new RowData();
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    string value = data[i, j].ToString();
                    row.list.Add(value);
                }
                list.Add(row);
            }
            return list;
        }
        static string tokens = " KMBtqQsSondUDT"; //Infinitely expandable (at least to the limit of double floating point values)
        public static string ShortenNumber(double count)
        {
            if (count < 1000) return count.ToString();
            for (float i = 1; true; i += 1)
            {
                double val = Mathf.Pow(1000, i);
                if (val > count)
                {
                    double result = count / Mathf.Pow(1000, i - 1);

                    return $"{((int)(result*100))/100f}{tokens[(int)i - 1]}".Trim();
                }
            }
        }
        //public static string ShortenNumber(int price)
        //{
        //    if (price < 1000) return price.ToString();
        //    string result;
        //    float p = (price / 1000f);
        //    p = ((int)(p * 10)) / 10f;
        //    result = p+"K";

        //    return result;
        //}
        public static int Pow(int a,int b)
        {
            int result=a;
            while (b >= 1)
            {
                result *= a;
                b--;
            }

            return result;

        }
        public static void ResetTransformation(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }
        public static async UniTask Shake(this Transform transform,float time,float power=1,float scale=0.1f, CancellationToken cancellationToken = default(CancellationToken),float defaultScale=1)
        {
            float t = time;
            Vector2 defaultPos = transform.localPosition;
            Vector2 pos = Vector2.zero;
            Vector3 s = transform.localScale;
            while (t >= 0)
            {
                if (transform == null) return;
                s.x = s.y = defaultScale + ((t * scale * power / time));
                transform.localScale = s;
                pos = defaultPos + UnityEngine.Random.insideUnitCircle * (t * 0.15f*power / time);
                transform.localPosition = pos;
                t -= Time.fixedUnscaledDeltaTime;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
            }
            s.Set(defaultScale, defaultScale, 1);
            transform.localScale = s;
        }
        public static async UniTask Shake(this RectTransform transform, float time, float power = 1,float scale=1f,CancellationToken cancellationToken=default(CancellationToken))
        {
            float t = time;
            Vector2 defaultPos = transform.anchoredPosition;
            Vector2 defaultScale = transform.localScale;
            Vector2 pos = Vector2.zero;
            Vector3 s = transform.localScale;
            while (t >= 0)
            {
                if (transform == null) return;
                s.x = s.y = 1 + ((t * 0.1f * scale / time));
                transform.localScale = s;
                pos = defaultPos + UnityEngine.Random.insideUnitCircle * (t * 1f * power / time);
                transform.anchoredPosition = pos;
                t -= Time.fixedUnscaledDeltaTime;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
            }
        }
        static int[] CalculateChange(int[] baseChances)
        {
            int[] changes = new int[baseChances.Length];
            for (int i = 0; i < changes.Length; i++)
            {
                changes[i] = baseChances[i] + (i > 0 ? changes[i - 1] : 0);
            }
            return changes;
        }
        public static int GetRandom(int[] chances)
        {
            chances = CalculateChange(chances);
          
            int rd = UnityEngine.Random.Range(0, chances[chances.Length - 1]);
            for (int i = 0; i < chances.Length; i++)
            {
                if (rd < chances[i])
                {
                    return i;
                }
            }
            return chances[chances.Length - 1];
        }

        public static void Log(object message)
        {
            Debug.Log(message.ToString());
        }
        public static void LogError(object message)
        {
            Debug.LogError(message.ToString());
        }
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message.ToString());
        }
    }
}