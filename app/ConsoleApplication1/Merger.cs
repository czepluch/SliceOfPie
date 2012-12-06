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

            int maxLen = curArr.Length > oldArr.Length ? curArr.Length : oldArr.Length;
            String[] merged = new String[maxLen];
            int o = 0, n = 0, m = 0;

            //actual algorithm, 'steps' are according to the describtion from the assignment https://docs.google.com/viewer?url=https%3A%2F%2Fblog.itu.dk%2FBDSA-E2012%2Ffiles%2F2012%2F11%2Fslice-of-pie.pdf
            while (o <= oldLast || n <= curLast) {
                Console.WriteLine("step");
                //Step 4
                if (o == oldLast+1 && n <= curLast) {
                    Console.WriteLine("step 4");
                    //copy the last [length of curArr] - n lines from curArr of merged
                    Array.Copy(curArr, n, merged, m, (curLast - n)+1);
                    n = curLast+1;
                } else if (n == curLast+1) { //step 5
                    Console.WriteLine("step 5");
                    o = oldLast+1;
                } else if (curArr[n].Equals(oldArr[o])) { //step 6
                    Console.WriteLine("step 6");
                    merged[m++] = oldArr[o++];
                    ++n;
                } else if (!curArr[n].Equals(oldArr[o])) { //step 7
                    Console.WriteLine("step 7");
                    int t = -1;
                    //step 7.a
                    for (int i = n+1; i <= curLast; i++) {
                        if (curArr[i].Equals(oldArr[o])) {
                            t = i;
                            break;
                        }
                    }
                    //step 7.b + 7.c
                    if (t == -1) {
                        Console.WriteLine("step 7.b");
                        ++o;
                    } else {
                        Console.WriteLine("step 7.c");
                        Array.Copy(curArr, n, merged, m, (t - n)+1);
                        n = t + 1;
                        m = n;
                        ++o; //not necessary, but it lets us skip a round (without this, the next run would end in step 7.b)
                    }
                }
                PrintArray(merged);
            }
            Document ret = new Document();
            for (int i=0; i<merged.Length;i++) {
                if (i == merged.Length - 1) {
                    ret.CurrentRevision += merged[i];
                } else {
                    ret.CurrentRevision += merged[i] + "\n";
                }
            }
            return ret;
        }
        /// <summary>
        /// Used as a demonstration only.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(String[] args) {
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
            //PrintArray(arrC);
            //PrintArray(arrO);

            Document merged = Merger.Merge(arrC, arrO);
            String[] mergedArr = merged.CurrentRevision.Split('\n');
            Console.WriteLine("Merge result:");
            PrintArray(mergedArr);
            
            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static void PrintArray(String[] arr) {
            for (int i = 0; i < arr.Length; i++) {
                Console.WriteLine("[{0}] {1}", i, arr[i]);
            }
        }
    }
}