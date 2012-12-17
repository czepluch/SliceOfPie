using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SliceOfPie;
using System.Diagnostics;

namespace SliceOfPie {
    public static class Merger {
        private static Document Merge(String[] curArr, String[] oldArr) {
            int curLast = curArr.Length - 1;
            int oldLast = oldArr.Length - 1;

            int maxLen = curArr.Length > oldArr.Length ? curArr.Length : oldArr.Length;
            String[] merged = new String[maxLen];
            int o = 0, n = 0, m = 0;

            //actual algorithm, 'steps' are according to the describtion from the assignment https://docs.google.com/viewer?url=https%3A%2F%2Fblog.itu.dk%2FBDSA-E2012%2Ffiles%2F2012%2F11%2Fslice-of-pie.pdf
            while (o <= oldLast || n <= curLast) {
                System.Diagnostics.Debug.WriteLine("step");
                //Step 4
                if (o == oldLast + 1 && n <= curLast) {
                    System.Diagnostics.Debug.WriteLine("step 4");
                    //copy the last [length of curArr] - n lines from curArr of merged
                    Array.Copy(curArr, n, merged, m, (curLast - n) + 1);
                    n = curLast + 1;
                } else if (n == curLast + 1) { //step 5
                    System.Diagnostics.Debug.WriteLine("step 5");
                    o = oldLast + 1;
                } else if (curArr[n].Equals(oldArr[o])) { //step 6
                    System.Diagnostics.Debug.WriteLine("step 6");
                    merged[m++] = oldArr[o++];
                    ++n;
                } else if (!curArr[n].Equals(oldArr[o])) { //step 7
                    System.Diagnostics.Debug.WriteLine("step 7");
                    int t = -1;
                    //step 7.a
                    for (int i = n + 1; i <= curLast; i++) {
                        if (curArr[i].Equals(oldArr[o])) {
                            t = i;
                            break;
                        }
                    }
                    //step 7.b + 7.c
                    if (t == -1) {
                        System.Diagnostics.Debug.WriteLine("step 7.b");
                        ++o;
                    } else {
                        System.Diagnostics.Debug.WriteLine("step 7.c");
                        Array.Copy(curArr, n, merged, m, (t - n) + 1);
                        n = t + 1;
                        m = n;
                        ++o; //not necessary, but it lets us skip a round (without this, the next run would end in step 7.b)
                    }
                }
                PrintArray(merged);
            }
            Document ret = new Document();
            for (int i = 0; i < merged.Length; i++) {
                if (i == merged.Length - 1) {
                    ret.CurrentRevision += merged[i];
                } else {
                    ret.CurrentRevision += merged[i] + "\n";
                }
            }
            return ret;
        }

        #region comments
        /// <summary>
        /// Produces a merge of current and old. Lines from current will take precedence.
        /// </summary>
        /// <param name="current">"newest" Document</param>
        /// <param name="old">"oldest" Document</param>
        /// <returns>Merged document</returns>
        #endregion
        public static Document Merge(Document current, Document old) {
            //Null checks
            if (current == null || current.CurrentRevision == null) {
                if (old == null || old.CurrentRevision == null) return null;
                return old;
            } 
            if (old == null || old.CurrentRevision == null) {
                return current;
            }

            String[] arrC = current.CurrentRevision.Split('\n');
            String[] arrO = old.CurrentRevision.Split('\n');
            return Merge(arrC, arrO);
        }

        #region comments
        /// <summary>
        /// Produces a merge of current and old. Lines from current will take precedence.
        /// </summary>
        /// <param name="current">"newest" Document</param>
        /// <param name="old">"oldest" Document</param>
        /// <returns>Merged document</returns>
        #endregion
        public static String Merge(String current, String old) {
            //Null checks
            if (current == null && old == null) return null;
            if (current == null) return old;
            if (old == null) return current;

            String[] arrC = current.Split('\n');
            String[] arrO = old.Split('\n');
            return Merge(arrC, arrO).CurrentRevision;
        }

        public static void PrintDoc(Document d) {
            PrintArray(d.CurrentRevision.Split('\n'));
        }

        //Only runs during debug runs
        [Conditional("DEBUG")] private static void PrintArray(String[] arr) {
            for (int i = 0; i < arr.Length; i++) {
                System.Diagnostics.Debug.WriteLine("[{0}] {1}", i, arr[i]);
            }
        }
    }
}
