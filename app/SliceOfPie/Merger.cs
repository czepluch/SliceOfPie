using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SliceOfPie {
    class Merger {
        private Merger() {
        }

        private static Document Merge(String[] curArr, String[] oldArr) {
            int curLast = curArr.Length - 1;
            int oldLast = oldArr.Length - 1;
            int m = 0;

            int maxLen = curArr.Length > oldArr.Length ? curArr.Length : oldArr.Length;
            String[] merged = new String[maxLen];
            int o = 0, n = 0;

            //actual algorithm, 'steps' are according to the describtion from the assignment https://docs.google.com/viewer?url=https%3A%2F%2Fblog.itu.dk%2FBDSA-E2012%2Ffiles%2F2012%2F11%2Fslice-of-pie.pdf
            while (o < oldArr.Length && n < curArr.Length) {
                //true if the merge 'skipped ahead' or appended one array after the other in merged
                //this boolean serves ony to lessenthe degree of nested if - else if - else if statements 
                Boolean jumped = false;
                //Step 4
                if (o == oldLast) {
                    //copy the last [length of curArr] - n lines from curArr of merged
                    Array.Copy(curArr, n, merged, o, curArr.Length - n);
                    n = oldLast;
                    jumped = true;
                }

                //step 5
                if (n == curLast) {
                    o = oldLast;
                    jumped = true;
                }

                //step 6
                if (!jumped && curArr[curLast].Equals(oldArr[oldLast])) {
                    merged[oldLast] = oldArr[oldLast];
                    ++n;
                    ++o;
                }
                else if (!jumped) { //step 7
                    int t = -1;
                    //step 7.a
                    for (int i = n; i < curLast; i++) {
                        if (curArr[i].Equals(oldArr[o])) {
                            t = i;
                            break;
                        }
                    }
                    //step 7.b + 7.c
                    if (t == -1) {
                        ++o;
                    }
                    else {
                        Array.Copy(curArr, n, merged, n, t - n);
                        n = t + 1;
                    }
                }
            }

            Document ret = new Document { CurrentRevision = merged.ToString() };
            return ret;
        }

        public static void TestRun() {
            Document o = new Document {
                CurrentRevision = @"DRACULA 



CHAPTER I 

JONATHAN BARKER'S JOURNAL 
(Kept in shorthand.) 

3 May. Bistritz. Left Munich at 8:35 p. M., on ist May, ar- 
riving at Vienna early next morning; should have arrived at 
6:46, but train was an hour late. Buda-Pesth seems a wonderful 
place, from the glimpse which I got of it from the train and the 
little I could walk through the streets. I feared to go very far 
from the station, as we had arrived late and would start as near 
the correct time as possible. The impression I had was that we 
were leaving the West and entering the East; the most western 
of splendid bridges over the Danube, which is here of noble width 
and depth, took us among the traditions of Turkish rule. "
            };
            Document c = new Document();
            c.CurrentRevision = @"DRACULA 



CHAPTER I 

JONATHAN BARKER'S JOURNAL 
(Kept in shorthand.) 

3 May. Bistritz. Left Munich at 8:35 p. M., on ist May, ar- 
riving at Vienna early next morning; should have arrived at 
6:46, but train was an hour late. Buda-Pesth seems a wonderful 
place, from the glimpse which I got of it from the train and the 
little I could walk through the streets. I feared to go very far 
from the station, as we had arrived late and would start as near 
the correct time as possible. The impression I had was that we 
were leaving the West and entering the East; the most western 
of splendid bridges over the Danube, which is here of noble width 
and depth, took us among the traditions of Turkish rule. ";

            String[] arrC = c.CurrentRevision.Split('\n');
            String[] arrO = o.CurrentRevision.Split('\n');
            Document merged = Merger.Merge(arrC, arrO);
            String[] mergedArr = merged.CurrentRevision.Split('\n');
            for (int i = 0; i < mergedArr.Length; i++) {
                Console.WriteLine("[{0}] {1}", i, mergedArr[i]);
            }
        }
    }
}